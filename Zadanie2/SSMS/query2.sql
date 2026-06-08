SELECT typ_umowy,
    CAST(COUNT(*) AS DECIMAL(10,2)) / COUNT(DISTINCT firma_id) AS avg_liczba_pracownikow,
    CAST( AVG(pensja) AS DECIMAL(10,2) ) AS avg_pensja
FROM dbo.zatrudnienie
GROUP BY typ_umowy;