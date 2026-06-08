DROP TABLE IF EXISTS dbo.zatrudnienie;
DROP TABLE IF EXISTS dbo.firma;
DROP TABLE IF EXISTS dbo.osoba;
GO

CREATE TABLE dbo.osoba(
    osoba_id INT PRIMARY KEY,
    imie VARCHAR(100) NOT NULL,
    nazwisko VARCHAR(100) NOT NULL,
    data_urodzenia DATE NOT NULL,
    plec CHAR(1) NOT NULL,
    zarobki DECIMAL(10,2),
    matka_id INT REFERENCES dbo.osoba(osoba_id),
    ojciec_id INT REFERENCES dbo.osoba(osoba_id),
    malzonek_id INT REFERENCES dbo.osoba(osoba_id),

    CONSTRAINT check_data
        CHECK (data_urodzenia > '1900-12-31')
);
GO

CREATE TABLE dbo.firma(
    firma_id INT PRIMARY KEY,
    nazwa VARCHAR(200) NOT NULL,
    prezes_id INT NOT NULL REFERENCES dbo.osoba(osoba_id)
);
GO

CREATE TABLE dbo.zatrudnienie(
    zatrudnienie_id INT PRIMARY KEY,
    osoba_id INT NOT NULL REFERENCES dbo.osoba(osoba_id),
    firma_id INT NOT NULL REFERENCES dbo.firma(firma_id),
    typ_umowy VARCHAR(20) NOT NULL,
    pensja DECIMAL(10,2)
);
GO