BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Tag" (
	"ID"	INTEGER NOT NULL UNIQUE,
	"Text"	TEXT NOT NULL CHECK(length("Text") > 0) UNIQUE COLLATE NOCASE,
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "Category" (
	"ID"	INTEGER NOT NULL UNIQUE,
	"Title"	TEXT NOT NULL DEFAULT 'Category' CHECK(length("Title") > 0) COLLATE NOCASE,
	"Parent"	INTEGER DEFAULT NULL CHECK("Parent" != "ID"),
	PRIMARY KEY("ID" AUTOINCREMENT),
	UNIQUE("Parent","Title"),
	FOREIGN KEY("Parent") REFERENCES "Category"("ID") ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE IF NOT EXISTS "Document" (
	"CategoryID"	INTEGER,
	"SectionID"	INTEGER NOT NULL UNIQUE,
	PRIMARY KEY("SectionID"),
	FOREIGN KEY("CategoryID") REFERENCES "Category"("ID") ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY("SectionID") REFERENCES "Section"("ID") ON UPDATE CASCADE ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "Section" (
	"ID"	INTEGER NOT NULL UNIQUE,
	"Title"	TEXT NOT NULL DEFAULT 'Title' CHECK(length("Title") > 0) COLLATE NOCASE,
	"Content"	TEXT NOT NULL DEFAULT '',
	"Parent"	INTEGER CHECK("ID" != "Parent"),
	"OrderIndex"	INTEGER NOT NULL DEFAULT 0,
	PRIMARY KEY("ID" AUTOINCREMENT),
	UNIQUE("Parent","OrderIndex"),
	UNIQUE("Parent","Title"),
	FOREIGN KEY("Parent") REFERENCES "Section"("ID") ON UPDATE CASCADE ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "DocumentsTags" (
	"DocumentID"	INTEGER NOT NULL,
	"TagID"	INTEGER NOT NULL,
	PRIMARY KEY("DocumentID","TagID"),
	UNIQUE("DocumentID","TagID"),
	FOREIGN KEY("DocumentID") REFERENCES "Document"("SectionID") ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY("TagID") REFERENCES "Tag"("ID") ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TRIGGER propagate_document_deletion AFTER DELETE ON Document
BEGIN
	DELETE FROM Section WHERE Section.ID=OLD.SectionID;
END;
CREATE TRIGGER tags_cleaner AFTER DELETE ON DocumentsTags
WHEN NOT EXISTS (SELECT * FROM DocumentsTags WHERE TagID=OLD.TagID)
BEGIN
	DELETE FROM Tag WHERE ID=OLD.TagID;
END;
CREATE TRIGGER document_create_title_duplication_guard BEFORE INSERT ON Document
WHEN EXISTS (
	SELECT *
	FROM (Document INNER JOIN Section ON Document.SectionID=Section.ID)
	WHERE Document.CategoryID=NEW.CategoryID AND Section.Title IN
		(SELECT Title FROM Section WHERE Section.ID=NEW.SectionID)
)
BEGIN
	SELECT RAISE(ABORT, 'Duplicate title in category');
END;
CREATE TRIGGER document_move_title_duplication_guard BEFORE UPDATE ON Document
WHEN EXISTS (
	SELECT *
	FROM (Document INNER JOIN Section ON Document.SectionID=Section.ID)
	WHERE Document.CategoryID=NEW.CategoryID AND Section.Title IN
		(SELECT Title FROM Section WHERE Section.ID=NEW.SectionID)
)
BEGIN
	SELECT RAISE(ABORT, 'Duplicate title in category');
END;
CREATE TRIGGER document_rename_title_duplication_guard BEFORE UPDATE ON Section
WHEN NEW.Parent IS NULL AND EXISTS (
	SELECT *
	FROM (Document INNER JOIN Section ON Document.SectionID=Section.ID)
	WHERE NEW.Title=Section.Title AND Document.CategoryID IN
		(SELECT CategoryID FROM Document WHERE Document.SectionID=NEW.ID)
)
BEGIN
	SELECT RAISE(ABORT, 'Duplicate title in category');
END;
COMMIT;
