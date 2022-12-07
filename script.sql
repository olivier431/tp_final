DROP TABLE IF EXISTS playlists_tunes, tunes, stats_playlists, playlists, users;


# ------------------------ CREATE TABLE ------------------------
CREATE TABLE IF NOT EXISTS users (
	id			INT PRIMARY KEY AUTO_INCREMENT,
	isAdmin			BOOLEAN NOT NULL DEFAULT FALSE,
	username		VARCHAR(45) NOT NULL UNIQUE CHECK(LENGTH(username) >= 3),
	pwd			VARCHAR(64) NOT NULL,
	email			VARCHAR(60) CHECK(email REGEXP '^[\\w-]*+@([\\w-]+[.])+[\\w-]{2,4}$'), 
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
	length		INT NOT NULL DEFAULT 0,
	isAlbum		BOOLEAN #NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS tunes (
	id		INT PRIMARY KEY AUTO_INCREMENT,
	user_id		INT NOT NULL DEFAULT 1 REFERENCES users(id),
	album_id	INT NOT NULL DEFAULT 1 REFERENCES playlists(id),
	album_ord	INT NOT NULL DEFAULT 0,
	title		VARCHAR(100) NOT NULL CHECK(LENGTH(title) > 0),
	artist		VARCHAR(100) NOT NULL CHECK(LENGTH(artist) > 0),
	genre		VARCHAR(45),
	filepath	VARCHAR(255),
	length		INT,
	year		INT,
	UNIQUE		(title, artist),
	UNIQUE		(album_id, album_ord)
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
CREATE OR REPLACE FUNCTION edit_playlist_ord(pl_id INT, OLDord INT, NEWord INT) RETURNS BOOLEAN
BEGIN
	SELECT `count`, isAlbum INTO @MAX, @isAlbum FROM stats_playlists WHERE playlist_id = pl_id;

	IF(@isAlbum is NULL OR OLDord NOT BETWEEN 1 AND @MAX OR OLDord NOT BETWEEN 1 AND @MAX) THEN 
		RETURN FALSE; 
	END IF;

	IF(@isAlbum) THEN
		UPDATE tunes SET album_ord = 0 
		WHERE album_id = pl_id AND album_ord = OLDord;

		UPDATE tunes SET album_ord = album_ord-1 
		WHERE album_id = pl_id AND OLDord < album_ord 
		ORDER BY album_ord ASC;

		UPDATE tunes SET album_ord = album_ord+1 
		WHERE album_id = pl_id AND NEWord <= album_ord 
		ORDER BY album_ord DESC;

		UPDATE tunes SET album_ord = NEWord 
		WHERE album_id = pl_id AND album_ord = 0;
	ELSE
		UPDATE playlists_tunes SET ord = 0 
		WHERE playlist_id = pl_id AND ord = OLDord;

		UPDATE playlists_tunes SET ord = ord-1 
		WHERE playlist_id = pl_id AND OLDord < ord 
		ORDER BY ord ASC;

		UPDATE playlists_tunes SET ord = ord+1 
		WHERE playlist_id = pl_id AND NEWord <= ord 
		ORDER BY ord DESC;

		UPDATE playlists_tunes SET ord = NEWord 
		WHERE playlist_id = pl_id AND ord = 0;
	END IF;

	RETURN TRUE;
END;

CREATE OR REPLACE FUNCTION get_admin_id() RETURNS INT 
BEGIN
	RETURN (SELECT id FROM users WHERE isAdmin = true LIMIT 1);
END;

CREATE OR REPLACE FUNCTION get_unknown_album_id() RETURNS INT 
BEGIN
	RETURN (SELECT id FROM playlists WHERE title = 'Unknown Album' LIMIT 1);
END;



# ------------------------ PROCEDURE ------------------------
CREATE OR REPLACE PROCEDURE refresh_playlist(IN pl_id INT) 
BEGIN
	SELECT 
		MAX(GREATEST(ord, album_ord)), 
		COUNT(tunes.id), 
		IFNULL(SUM(length), 0), 
		album_id = pl_id 
	INTO @MAX, @COUNT, @LENGTH, @isAlbum 
	FROM playlists_tunes 
	RIGHT JOIN tunes ON tune_id = tunes.id 
	WHERE pl_id IN(album_id, playlist_id);

	IF(@MAX != @COUNT) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'ord ERROR';
	END IF;

	INSERT INTO stats_playlists VALUES(pl_id, @COUNT, @LENGTH, @isAlbum) 
	ON DUPLICATE KEY UPDATE `count` = @COUNT, length = @LENGTH, isAlbum = @isAlbum;
END;

CREATE OR REPLACE PROCEDURE shuffle(IN pl_id INT) 
BEGIN
	SELECT `count` INTO @MAX FROM stats_playlists WHERE playlist_id = pl_id;
	FOR i IN 1..@MAX DO
		IF(!edit_playlist_ord(pl_id, i, FLOOR(1+RAND()*@MAX))) THEN
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
CREATE OR REPLACE TRIGGER playlists_DEL_VAL BEFORE DELETE ON playlists FOR EACH ROW 
BEGIN
	SET @unknown_album_id = get_unknown_album_id();

	IF(@unknown_album_id = OLD.id) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'can not delete Unknown Album';
	END IF;
	
	UPDATE tunes SET album_id = @unknown_album_id WHERE album_id = OLD.id;
END;

CREATE OR REPLACE TRIGGER playlists_INS_REF AFTER INSERT ON playlists FOR EACH ROW 
BEGIN
	CALL refresh_playlist(NEW.id);
END;


# ------------------------ tunes TRIGGER ------------------------
CREATE OR REPLACE TRIGGER tunes_INS_VAL BEFORE INSERT ON tunes FOR EACH ROW 
BEGIN
	IF(NEW.album_id IN(SELECT DISTINCT playlist_id FROM playlists_tunes)) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "a Playlist can't be an Album";
	END IF;

	IF(NEW.user_id = 1) THEN
		SET NEW.user_id = get_admin_id();
	END IF;

	SET NEW.album_ord = (SELECT `count`+1 FROM stats_playlists WHERE playlist_id = NEW.album_id);
END;
CREATE OR REPLACE TRIGGER tunes_UPD_VAL BEFORE UPDATE ON tunes FOR EACH ROW# FOLLOWS playlists_DEL_VAL
BEGIN
	IF(NEW.album_id IN(SELECT DISTINCT playlist_id FROM playlists_tunes)) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "a Playlist can't be an Album";
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
CREATE OR REPLACE TRIGGER playlists_tunes_INS_VAL BEFORE INSERT ON playlists_tunes FOR EACH ROW 
BEGIN
	IF(NEW.playlist_id IN(SELECT DISTINCT album_id FROM tunes)) THEN
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = "an Album can't be a Playlist";
	END IF;
    
	SET NEW.ord = (SELECT `count`+1 FROM stats_playlists WHERE playlist_id = NEW.playlist_id);
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
	(TRUE, 'Felix',		SHA2('OVERkumoko4116',	256), 'carife2002@gmail.com'), 
	(TRUE, 'Olivier',	SHA2('123456789',		256), 'email@email.com'), 
	(TRUE, 'Samuel',	SHA2('12345678',		256), '1@1.com'); 
#	(TRUE, 'Keven',		SHA2('741852963',			256), 'kev@email.com');

INSERT INTO playlists(title) VALUES ('Unknown Album');
