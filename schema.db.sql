BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Tag" (
	"ID"	INTEGER NOT NULL UNIQUE,
	"Text"	TEXT NOT NULL CHECK(length("Text") > 0) UNIQUE COLLATE NOCASE,
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "DocumentsTags" (
	"DocumentID"	INTEGER NOT NULL,
	"TagID"	INTEGER NOT NULL,
	PRIMARY KEY("DocumentID","TagID"),
	FOREIGN KEY("TagID") REFERENCES "Tag"("ID") ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY("DocumentID") REFERENCES "Document"("ID") ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE IF NOT EXISTS "Document" (
	"ID"	INTEGER NOT NULL UNIQUE,
	"Title"	TEXT NOT NULL DEFAULT 'Document' CHECK(length("Title") > 0) COLLATE NOCASE,
	"Content"	TEXT NOT NULL DEFAULT '',
	"Parent"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT),
	UNIQUE("Parent","Title"),
	FOREIGN KEY("Parent") REFERENCES "Category"("ID") ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE IF NOT EXISTS "Section" (
	"ID"	INTEGER NOT NULL UNIQUE,
	"Title"	TEXT NOT NULL DEFAULT 'Title' CHECK(length("Title") > 0) COLLATE NOCASE,
	"Content"	TEXT NOT NULL DEFAULT '',
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "Subsections" (
	"SectionID"	INTEGER NOT NULL DEFAULT 0,
	"SubsectionID"	INTEGER NOT NULL DEFAULT 0 UNIQUE,
	"OrderIndex"	INTEGER NOT NULL DEFAULT 0,
	"ID"	INTEGER NOT NULL UNIQUE,
	PRIMARY KEY("ID" AUTOINCREMENT),
	UNIQUE("SectionID","OrderIndex"),
	FOREIGN KEY("SectionID") REFERENCES "Section"("ID") ON UPDATE CASCADE ON DELETE NO ACTION,
	FOREIGN KEY("SubsectionID") REFERENCES "Section"("ID") ON UPDATE CASCADE ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "DocumentsSections" (
	"DocumentID"	INTEGER NOT NULL DEFAULT 0,
	"SectionID"	INTEGER NOT NULL DEFAULT 0 UNIQUE,
	"OrderIndex"	INTEGER DEFAULT 0,
	"ID"	INTEGER NOT NULL UNIQUE,
	PRIMARY KEY("ID" AUTOINCREMENT),
	UNIQUE("DocumentID","OrderIndex"),
	FOREIGN KEY("DocumentID") REFERENCES "Document"("ID") ON UPDATE CASCADE,
	FOREIGN KEY("SectionID") REFERENCES "Section"("ID") ON UPDATE CASCADE ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "Category" (
	"ID"	INTEGER NOT NULL UNIQUE,
	"Title"	TEXT NOT NULL DEFAULT 'Category' CHECK(length("Title") > 0) COLLATE NOCASE,
	"Parent"	INTEGER DEFAULT NULL CHECK("Parent" != "ID"),
	PRIMARY KEY("ID" AUTOINCREMENT),
	UNIQUE("Parent","Title"),
	FOREIGN KEY("Parent") REFERENCES "Category"("ID") ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TRIGGER section_title_insert_duplication_guard BEFORE INSERT ON DocumentsSections
WHEN EXISTS (
	SELECT * FROM (Section INNER JOIN DocumentsSections ON Section.ID=DocumentsSections.SectionID)
	WHERE DocumentsSections.DocumentID=NEW.DocumentID AND
		Section.Title IN (SELECT Section.Title FROM Section WHERE Section.ID=NEW.SectionID)
)
BEGIN
	SELECT RAISE(ABORT, 'Duplicate Title of section');
END;
CREATE TRIGGER section_title_update_duplication_guard BEFORE UPDATE ON Section
WHEN EXISTS (
	SELECT * FROM (Section INNER JOIN DocumentsSections ON Section.ID=DocumentsSections.SectionID)
	WHERE Section.Title=NEW.Title AND DocumentsSections.DocumentID IN
		(SELECT DocumentID FROM DocumentsSections WHERE SectionID=NEW.ID)
)
BEGIN
	SELECT RAISE(ABORT, 'Duplicate Title of section');
END;
CREATE TRIGGER subsection_title_insert_duplication_guard BEFORE INSERT ON Subsections
WHEN EXISTS (
	SELECT * FROM (Section INNER JOIN Subsections ON Section.ID=Subsections.SectionID)
	WHERE Subsections.SectionID=NEW.SectionID AND
		Section.Title IN (SELECT Section.Title FROM Section WHERE Section.ID=NEW.SectionID)
)
BEGIN
	SELECT RAISE(ABORT, 'Duplicate Title of section');
END;
CREATE TRIGGER subsection_title_update_duplication_guard BEFORE UPDATE ON Section
WHEN EXISTS (
	SELECT * FROM (Section INNER JOIN Subsections ON Section.ID=Subsections.SectionID)
	WHERE Section.Title=NEW.Title AND Subsections.SectionID IN
		(SELECT SectionID FROM Subsections WHERE SubsectionID=NEW.ID)
)
BEGIN
	SELECT RAISE(ABORT, 'Duplicate Title of section');
END;
CREATE TRIGGER tags_cleanup AFTER DELETE ON DocumentsTags
WHEN NOT EXISTS (SELECT * FROM DocumentsTags WHERE TagID=OLD.TagID)
BEGIN
	DELETE FROM Tag WHERE ID=OLD.TagID;
END;
COMMIT;
