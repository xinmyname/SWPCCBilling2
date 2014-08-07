CREATE TABLE Version
(
    Version INTEGER
);

INSERT INTO Version (Version) VALUES (1);

CREATE TABLE Family
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name TEXT,
    StreetAddress TEXT,
    City TEXT,
    State TEXT,
    Zip TEXT,
    DueDay INTEGER,
    NumChildren INTEGER,
    BillableDays INTEGER,
    Disposition INTEGER,
    IsGraduating INTEGER,
    CheckSHA256 TEXT,
    Joined DATE,
    Departed DATE
);

CREATE TABLE Parent
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    FamilyId INTEGER,
    Name TEXT,
    Email TEXT
);

CREATE TABLE Fee
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name TEXT,
    Type TEXT,
    Category TEXT,
    Amount NUMERIC
);

CREATE TABLE Discount
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    FamilyId INTEGER,
    FeeId INTEGER,
    Percent NUMERIC,
    IsFinancialAid INTEGER
);

CREATE TABLE Ledger
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    FamilyId INTEGER,
    Date DATE,
    FeeId INTEGER,
    PaymentId INTEGER,
    UnitPrice NUMERIC,
    Quantity INTEGER,
    Amount NUMERIC,
    Notes TEXT
);

CREATE TABLE Payment
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    FamilyId INTEGER,
    CheckNum TEXT,
    Amount NUMERIC,
    Received DATE,
    Deposited DATE
);
