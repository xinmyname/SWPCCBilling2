CREATE TABLE Version
(
    Version INTEGER
);

INSERT INTO Version (Version) VALUES (7);

CREATE TABLE Family
(
    Name TEXT NOT NULL PRIMARY KEY,
    StreetAddress TEXT,
    City TEXT,
    State TEXT,
    Zip TEXT,
    DueDay INTEGER,
    NumChildren INTEGER,
    BillableDays INTEGER,
    IsNew INTEGER,
    IsGraduating INTEGER,
    CheckSHA256 TEXT,
    Joined DATE,
    Departed DATE
);

CREATE TABLE Parent
(
    FamilyName TEXT,
    Name TEXT,
    Email TEXT
);

CREATE TABLE MICR
(
    CheckSHA256 TEXT NOT NULL,
    FamilyName TEXT
);

CREATE TABLE Fee
(
    Code TEXT NOT NULL PRIMARY KEY,
    Description TEXT,
    Type TEXT,
    Category TEXT,
    Amount NUMERIC
);

CREATE TABLE Discount
(
    FamilyName TEXT,
    FeeCode TEXT,
    Percent NUMERIC,
    IsFinancialAid INTEGER
);

CREATE TABLE Payment
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    FamilyName TEXT,
    CheckNum TEXT,
    Amount NUMERIC,
    Received DATE,
    InvoiceId INTEGER,
    DepositId INTEGER
);

CREATE TABLE Deposit
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Date DATE,
    Amount NUMERIC
);

CREATE TABLE Invoice 
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    FamilyName TEXT,
    Due DATE,
    Sent DATE,
    Opened DATE,
    Closed DATE
);

CREATE TABLE InvoiceLine
(
    InvoiceId INTEGER,
    FeeCode TEXT,
    Quantity INTEGER,
    UnitPrice NUMERIC,
    Notes TEXT
);

CREATE TABLE LedgerLine
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    FamilyName TEXT,
    Date DATE,
    InvoiceId INTEGER,
    PaymentId INTEGER,
    FeeCode TEXT,
    Quantity INTEGER,
    UnitPrice NUMERIC,
    Notes TEXT
);

