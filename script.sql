DROP TABLE IF EXISTS playlists_tunes, tunes, stats_playlists, playlists, users;


# ------------------------ CREATE TABLE ------------------------
CREATE TABLE IF NOT EXISTS users (
	id		INT PRIMARY KEY AUTO_INCREMENT,
	isAdmin		BOOLEAN NOT NULL DEFAULT FALSE,
	username	VARCHAR(45) NOT NULL UNIQUE CHECK(LENGTH(username) >= 3),
	pwd		VARCHAR(64) NOT NULL,
	email		VARCHAR(60) CHECK(email REGEXP '^[\\w-]*+@([\\w-]+[.])+[\\w-]{2,4}$'), 
	lastConnection	DATETIME
);

CREATE TABLE IF NOT EXISTS playlists (
	id		INT PRIMARY KEY AUTO_INCREMENT,
	user_id		INT NOT NULL DEFAULT 1 REFERENCES users(id),
	isPublic	BOOLEAN NOT NULL DEFAULT FALSE,
	title		VARCHAR(100) NOT NULL,
	artist		VARCHAR(100),
	genre		VARCHAR(45),
	album_cover	LONGTEXT,
	year		INT
);

CREATE TABLE IF NOT EXISTS stats_playlists (
	playlist_id	INT KEY REFERENCES playlists(id) ON DELETE CASCADE,
	`count`		INT NOT NULL DEFAULT 0,
	length		INT NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS tunes (
	id		INT PRIMARY KEY AUTO_INCREMENT,
	user_id		INT NOT NULL DEFAULT 1 REFERENCES users(id),
	album_id	INT NOT NULL DEFAULT 1 REFERENCES playlists(id),
	title		VARCHAR(100) NOT NULL CHECK(LENGTH(title) > 0),
	artist		VARCHAR(100) NOT NULL CHECK(LENGTH(artist) > 0),
	genre		VARCHAR(45),
	filepath	VARCHAR(255),
	length		INT,
	year		INT,
	UNIQUE		(title, artist)
);

CREATE TABLE IF NOT EXISTS playlists_tunes (
	id		INT PRIMARY KEY AUTO_INCREMENT,
	playlist_id	INT NOT NULL REFERENCES playlists(id) ON DELETE CASCADE,
	tune_id		INT NOT NULL REFERENCES tunes(id) ON DELETE CASCADE,
	ord		INT NOT NULL DEFAULT 0,
	UNIQUE		(playlist_id, tune_id),
	UNIQUE		(playlist_id, ord)
);



# ------------------------ FUNCTION ------------------------
CREATE OR REPLACE FUNCTION MAXord(pl_id INT) RETURNS INT 
BEGIN
	RETURN (SELECT IFNULL(MAX(ord), 0) FROM playlists_tunes WHERE playlist_id = pl_id);
END;

CREATE OR REPLACE FUNCTION edit_playlist_ord(pl_id INT, OLDord INT, NEWord INT) RETURNS BOOLEAN
BEGIN
	SELECT MAXord(pl_id) INTO @MAX;
	IF(OLDord NOT BETWEEN 1 AND @MAX OR OLDord NOT BETWEEN 1 AND @MAX) THEN RETURN FALSE; 
	END IF;

	UPDATE playlists_tunes SET ord = 0 
		WHERE playlist_id = pl_id AND ord = OLDord;
	UPDATE playlists_tunes SET ord = ord-1 
		WHERE playlist_id = pl_id AND OLDord < ord ORDER BY ord ASC;
	UPDATE playlists_tunes SET ord = ord+1 
		WHERE playlist_id = pl_id AND NEWord <= ord ORDER BY ord DESC;
	UPDATE playlists_tunes SET ord = NEWord 
		WHERE playlist_id = pl_id AND ord = 0;
	RETURN TRUE;
END;

CREATE OR REPLACE FUNCTION get_admin_id() RETURNS INT 
BEGIN
	RETURN (SELECT id FROM users WHERE isAdmin = true LIMIT 1);
    
#	IF(@admin_id IS NULL) THEN #create if null
#		INSERT INTO users(isAdmin, username, pwd) 
#		VALUES(TRUE, 'dmin', SHA2('admin', 256)) 
#		RETURNING @admin_id = id;
#	END IF;
END;

CREATE OR REPLACE FUNCTION get_UnAlb_id() RETURNS INT 
BEGIN
	RETURN (SELECT id FROM playlists WHERE title = 'Unknown Album' LIMIT 1);
    
#	IF(@UnAlb_id IS NULL) THEN #create if null
#		INSERT INTO playlists(title) VALUES('Unknown Album') 
#		RETURNING @UnAlb_id = id;
#	END IF;
END;



# ------------------------ PROCEDURE ------------------------
CREATE OR REPLACE PROCEDURE refresh_playlist(IN pl_id INT) 
BEGIN
	SELECT COUNT(id), SUM(length) INTO @COUNT, @LENGTH FROM tunes WHERE album_id = pl_id;

	IF(@COUNT = 0) THEN
		SELECT COUNT(id), IFNULL(
			SUM((SELECT length FROM tunes WHERE tunes.id = tune_id)), 0)
		INTO @COUNT, @LENGTH FROM playlists_tunes WHERE playlist_id = pl_id;
	END IF;

	INSERT INTO stats_playlists VALUES(pl_id, @COUNT, @LENGTH) 
	ON DUPLICATE KEY UPDATE `count` = @COUNT, length = @LENGTH;
END;

CREATE OR REPLACE PROCEDURE shuffle(IN pl_id INT) 
BEGIN
	SET @MAX = MAXord(pl_id);
	FOR i IN 1..@MAX DO
		IF(edit_playlist_ord(pl_id, i, FLOOR(1+RAND()*@MAX)) IS FALSE) THEN
			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'RAND ERROR';
		END IF;
	END FOR;
END;


# ------------------------ users TRIGGER ------------------------
CREATE OR REPLACE TRIGGER users_DEL_VAL BEFORE DELETE ON users FOR EACH ROW 
BEGIN
	IF(SELECT isAdmin FROM users WHERE id = OLD.id) THEN
		IF((SELECT COUNT(id) FROM users WHERE isAdmin) <= 1) THEN
			SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'can not delete the last admin';
		END IF;
	END IF;
	
	UPDATE playlists SET user_id = get_admin_id() WHERE user_id = OLD.id;
	UPDATE tunes SET user_id = get_admin_id() WHERE user_id = OLD.id;
END;


# ------------------------ playlists TRIGGER ------------------------
CREATE OR REPLACE TRIGGER playlists_INS BEFORE INSERT ON playlists FOR EACH ROW 
BEGIN
	IF(NEW.user_id = 1) THEN
		SET NEW.user_id = get_admin_id();
	END IF;
END;
CREATE OR REPLACE TRIGGER playlists_INS_REF AFTER INSERT ON playlists FOR EACH ROW 
BEGIN
	CALL refresh_playlist(NEW.id);
END;
CREATE OR REPLACE TRIGGER playlists_DEL_VAL BEFORE DELETE ON playlists FOR EACH ROW 
BEGIN
	SET @UnAlb_id = get_UnAlb_id();

	IF(@UnAlb_id = OLD.id) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'can not delete Unknown Album';
	END IF;
	
	UPDATE tunes SET album_id = @UnAlb_id WHERE album_id = OLD.id;
END;




# ------------------------ tunes TRIGGER ------------------------
CREATE OR REPLACE TRIGGER tunes_INS BEFORE INSERT ON tunes FOR EACH ROW 
BEGIN
	IF(NEW.user_id = 1) THEN
		SET NEW.user_id = get_admin_id();
	END IF;
END;
CREATE OR REPLACE TRIGGER tunes_INS_REF AFTER INSERT ON tunes FOR EACH ROW 
BEGIN
	CALL refresh_playlist(NEW.album_id);
END;
CREATE OR REPLACE TRIGGER tunes_UPD_REF AFTER UPDATE ON tunes FOR EACH ROW# FOLLOWS playlists_DEL_VAL
BEGIN
	CALL refresh_playlist(NEW.album_id);
	CALL refresh_playlist(OLD.album_id);
END;
CREATE OR REPLACE TRIGGER tunes_DEL_REF AFTER DELETE ON tunes FOR EACH ROW 
BEGIN
	CALL refresh_playlist(OLD.album_id);
END;


# ------------------------ playlists_tunes TRIGGER ------------------------
CREATE OR REPLACE TRIGGER default_order BEFORE INSERT ON playlists_tunes FOR EACH ROW 
BEGIN
	SET NEW.ord = MAXord(NEW.playlist_id)+1;
END;
CREATE OR REPLACE TRIGGER playlists_tunes_INS_REF AFTER INSERT ON playlists_tunes FOR EACH ROW 
BEGIN
	CALL refresh_playlist(NEW.playlist_id);
END;
CREATE OR REPLACE TRIGGER playlists_tunes_UPD_REF AFTER UPDATE ON playlists_tunes FOR EACH ROW 
BEGIN
	CALL refresh_playlist(NEW.playlist_id);
	CALL refresh_playlist(OLD.playlist_id);
END;
CREATE OR REPLACE TRIGGER playlists_tunes_DEL_REF AFTER DELETE ON playlists_tunes FOR EACH ROW 
BEGIN
	CALL refresh_playlist(OLD.playlist_id);
END;



# ------------------------ Admin ------------------------
INSERT INTO users(isAdmin, username, pwd, email) VALUES 
	(TRUE, 'admin',		SHA2('admin',			256), NULL), 
	(TRUE, 'Felix',		SHA2('OVERkumoko4116',		256), 'carife2002@gmail.com'), 
	(TRUE, 'Olivier',	SHA2('123456789',		256), 'email@email.com'), 
	(TRUE, 'Samuel',	SHA2('12345678',		256), '1@1.com'); 
#	(TRUE, 'nom',		SHA2('pwd',				256), 'email');

INSERT INTO playlists(title) VALUES ('Unknown Album');
