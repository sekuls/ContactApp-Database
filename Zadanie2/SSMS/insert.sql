
INSERT INTO dbo.osoba (osoba_id, imie, nazwisko, data_urodzenia, plec, zarobki)
VALUES
--dziadkowie
(1, 'Adam', 'Stoch','1940-03-15', 'M', 3000.00),
(2, 'Maria', 'Stoch','1942-07-22', 'K', 2500.00),
(3, 'Jan','Kubica','1938-11-01', 'M', 4000.00),
(4, 'Anna','Kubica','1941-05-30', 'K', 3500.00),
--doroœli
(5,'Piotr','Stoch','1965-08-10', 'M', 5000.00),
(6,'Ewa', 'Stoch','1967-02-14', 'K', 4500.00),
(7,'Tomasz','Kubica', '1963-09-25','M', 6000.00),
(8,'Karolina','Kubica','1969-12-03', 'K', 3200.00),
(9,'Marek','Kubale','1960-01-20', 'M', 7000.00),
(10,'Zofia','Kubale','1962-06-11', 'K', 3800.00),
--dzieci
(11,'Katarzyna','Stoch','1990-04-05', 'K', 2800.00),
(12,'Micha³', 'Stoch', '1992-09-17', 'M', 3100.00),
(13,'Agnieszka','Kubica', '1988-07-23', 'K', 2600.00),
(14, 'Bartosz','Kubica','1991-11-30', 'M', 2900.00),
(15,'Natalia', 'Kubale','1993-03-08', 'K', 2700.00),
(16,'£ukasz','Kubale','1995-06-19', 'M', 2400.00),
(17,'Monika','Dziubich','1989-10-12', 'K', 3300.00),
(18, 'Rafa³','Dziubich','1987-02-28', 'M', 3600.00);
GO



-- ustawianie ma³¿eñstw miêdzy osobami 3 pokoleñ
-- pokolenie 1
UPDATE dbo.osoba SET malzonek_id = 2  WHERE osoba_id = 1;
UPDATE dbo.osoba SET malzonek_id = 1  WHERE osoba_id = 2;
UPDATE dbo.osoba SET malzonek_id = 4  WHERE osoba_id = 3;
UPDATE dbo.osoba SET malzonek_id = 3  WHERE osoba_id = 4;
-- pokolenie 2
UPDATE dbo.osoba SET malzonek_id = 6  WHERE osoba_id = 5;
UPDATE dbo.osoba SET malzonek_id = 5  WHERE osoba_id = 6;
UPDATE dbo.osoba SET malzonek_id = 8  WHERE osoba_id = 7;
UPDATE dbo.osoba SET malzonek_id = 7  WHERE osoba_id = 8;
UPDATE dbo.osoba SET malzonek_id = 10 WHERE osoba_id = 9;
UPDATE dbo.osoba SET malzonek_id = 9  WHERE osoba_id = 10;
--pokolenie 3
UPDATE dbo.osoba SET malzonek_id = 18 WHERE osoba_id = 17;
UPDATE dbo.osoba SET malzonek_id = 17 WHERE osoba_id = 18;
GO

-- relacja rodzic-dziecko
--pokolenie 1-2
UPDATE dbo.osoba SET ojciec_id = 1, matka_id = 2 WHERE osoba_id = 5;  --piotrus
UPDATE dbo.osoba SET ojciec_id = 3, matka_id = 4 WHERE osoba_id = 6;  --ewa
UPDATE dbo.osoba SET ojciec_id = 3, matka_id = 4 WHERE osoba_id = 7;  --tomek
UPDATE dbo.osoba SET ojciec_id = 1, matka_id = 2 WHERE osoba_id = 8;  -- karolina

-- pokolenie 2-3
UPDATE dbo.osoba SET ojciec_id = 5, matka_id = 6 WHERE osoba_id = 11; -- kasia
UPDATE dbo.osoba SET ojciec_id = 5, matka_id = 6 WHERE osoba_id = 12; -- michu
UPDATE dbo.osoba SET ojciec_id = 7, matka_id = 8 WHERE osoba_id = 13; -- aga
UPDATE dbo.osoba SET ojciec_id = 7, matka_id = 8 WHERE osoba_id = 14; -- bartus
UPDATE dbo.osoba SET ojciec_id = 9, matka_id = 10 WHERE osoba_id = 15;-- natalia
UPDATE dbo.osoba SET ojciec_id = 9, matka_id = 10 WHERE osoba_id = 16;-- ³uki
GO

INSERT INTO dbo.firma (firma_id, nazwa, prezes_id)
VALUES
 (1,'biedronka',5),
 (2,'lidl',7),
 (3,'zabka',9);
GO

INSERT INTO dbo.zatrudnienie (zatrudnienie_id, osoba_id, firma_id, typ_umowy, pensja)
VALUES
 (1,11,1,'o_prace', 2800.00),
 (2,12,1,'zlecenie',2000.00),
 (3,13,1,'zlecenie',1800.00),
 (4,14,2,'o_prace', 2900.00),
 (5,15,2,'o_prace', 2700.00),
 (6,16,2,'zlecenie',1500.00),
 (7,17,3,'o_prace', 3300.00),
 (8,18,3,'o_prace', 3600.00),
 (9,5,1,'o_prace',5000.00),
 (10,7,2,'o_prace', 6000.00),
 (11,9,3,'o_prace', 7000.00),
 (12,12,2,'zlecenie',1200.00),  -- michu 2 firmy
 (13,13,3,'zlecenie',1000.00);  -- aga tez
GO