WITH malzonkowie AS (
    SELECT osoba_id, malzonek_id
    FROM dbo.osoba
    WHERE malzonek_id IS NOT NULL
),

dzieci AS (
    SELECT ojciec_id AS rodzic_id, osoba_id AS dziecko_id
    FROM dbo.osoba WHERE ojciec_id IS NOT NULL
    UNION
    SELECT matka_id,  osoba_id
    FROM dbo.osoba WHERE matka_id  IS NOT NULL
),

rodzice AS (
    SELECT osoba_id AS dziecko_id, ojciec_id AS rodzic_id
    FROM dbo.osoba WHERE ojciec_id IS NOT NULL
    UNION
    SELECT osoba_id, matka_id
    FROM dbo.osoba WHERE matka_id IS NOT NULL
),

-- ³¹czymy rodziny w zbiory. tworzymy kombinacje g³owa rodziny -> cz³onek rodziny
rodzinka AS (

    -- osoba x
    SELECT osoba_id AS glowa, osoba_id AS czlonek
    FROM dbo.osoba

    UNION
    -- ma³¿onek osoby x
    SELECT o.osoba_id, malzonkowie.malzonek_id
    FROM dbo.osoba o
    JOIN malzonkowie ON malzonkowie.osoba_id = o.osoba_id
    UNION

    -- dzieci osoby x
    SELECT o.osoba_id, dzieci.dziecko_id
    FROM dbo.osoba o
    JOIN dzieci ON dzieci.rodzic_id = o.osoba_id

    UNION
    -- ma³¿onkowie dzieci osobby x
    SELECT o.osoba_id, malzonkowie.malzonek_id
    FROM dbo.osoba o
    JOIN dzieci ON dzieci.rodzic_id = o.osoba_id
    JOIN malzonkowie ON malzonkowie.osoba_id= dzieci.dziecko_id

    UNION
    -- rodzice osoby x
    SELECT o.osoba_id, rodzice.rodzic_id
    FROM dbo.osoba o
    JOIN rodzice ON rodzice.dziecko_id = o.osoba_id

    UNION
    -- ma³¿onkowie rodziców osoby x
    SELECT o.osoba_id, malzonkowie.malzonek_id
    FROM dbo.osoba o
    JOIN rodzice ON rodzice.dziecko_id = o.osoba_id
    JOIN malzonkowie ON malzonkowie.osoba_id = rodzice.rodzic_id
),

-- suma zarobków rodziny ka¿dej g³owy
suma_rodziny AS (
    SELECT rodzinka.glowa, SUM(dbo.osoba.zarobki) AS suma_zarobkow
    FROM rodzinka
    JOIN dbo.osoba ON dbo.osoba.osoba_id = rodzinka.czlonek
    GROUP BY rodzinka.glowa
),

-- min suma spoœród wszystkich rodzin
min_suma AS (
    SELECT MIN(suma_zarobkow) AS minimum
    FROM suma_rodziny
)

-- dowolna osoba z najbiednieszej rodziny
SELECT TOP 1 o.imie, o.nazwisko, suma_rodziny.suma_zarobkow
FROM suma_rodziny
JOIN min_suma ON suma_rodziny.suma_zarobkow = min_suma.minimum
JOIN dbo.osoba o ON o.osoba_id = suma_rodziny.glowa;
GO