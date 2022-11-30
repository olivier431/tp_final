DROP TABLE IF EXISTS PlayList_Musics, PlayLists, Musics, Users;
DROP TRIGGER IF EXISTS DEFAULT_INDEX;
DROP FUNCTION IF EXISTS EDIT_INDEX;



CREATE TABLE Users (
	id 		INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	username 	VARCHAR(20) NOT NULL UNIQUE CHECK( LENGTH(username) >= 3 ),
	password 	VARCHAR(64) NOT NULL
);
INSERT INTO Users VALUES (1, 'Public', SHA2('123', 256));

CREATE TABLE Musics (
	id 		INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	duration 	TIME NOT NULL,
	title 		VARCHAR(20) NOT NULL UNIQUE CHECK( LENGTH(title) >= 3 ),
	`group` 	VARCHAR(20) NOT NULL CHECK( LENGTH(`group`) >= 3 ),
	UNIQUE 		(title, `group`)
);

CREATE TABLE PlayLists (
	id 		INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	user_id 	INT UNSIGNED NOT NULL DEFAULT 1 REFERENCES PlayLists(id) ON DELETE CASCADE,
	name 		VARCHAR(20) NOT NULL UNIQUE CHECK( LENGTH(name) >= 3 ),
	public 		BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE PlayList_Musics (
	id 		INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	playList_id 	INT UNSIGNED NOT NULL REFERENCES PlayLists(id) ON DELETE CASCADE,
	music_id 	INT UNSIGNED NOT NULL REFERENCES Musics(id) ON DELETE CASCADE,
	`index` 	INT UNSIGNED NOT NULL DEFAULT 0,
	UNIQUE 		(playList_id, music_id)
);


CREATE TRIGGER DEFAULT_INDEX BEFORE INSERT ON PlayList_Musics FOR EACH ROW BEGIN
	SELECT COUNT(id) INTO @INDEX FROM PlayList_Musics WHERE playList_id = NEW.playList_id;
    
	IF(NEW.`index` NOT BETWEEN 1 AND @INDEX) THEN SET NEW.`index` = @INDEX+1; END IF;
END;


CREATE FUNCTION EDIT_INDEX(PlayListId INT, OldIndex INT, NewIndex INT) RETURNS BOOLEAN BEGIN
	SELECT COUNT(id) INTO @INDEX FROM PlayList_Musics WHERE playList_id = PlayListId;
    
	IF((OldIndex BETWEEN 1 AND @INDEX) AND (NewIndex BETWEEN 1 AND @INDEX)) THEN
		UPDATE PlayList_Musics SET `index` = 0 WHERE playList_id = PlayListId AND `index` = OldIndex;
		UPDATE PlayList_Musics SET `index` = `index`-1 WHERE playList_id = PlayListId AND OldIndex < `index`;
		UPDATE PlayList_Musics SET `index` = `index`+1 WHERE playList_id = PlayListId AND NewIndex <= `index`;
		UPDATE PlayList_Musics SET `index` = NewIndex WHERE playList_id = PlayListId AND `index` = 0;
		RETURN TRUE;
	ELSE 
		RETURN FALSE;
	END IF;
END;
