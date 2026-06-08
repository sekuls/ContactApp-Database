--ZnajdŸ imiê i nazwisko osoby posiadaj¹cej najwiêksz¹ liczbê wnucz¹t p³ci ¿eñskiej.

-- temp tabela. wyci¹gam tylko pary ojciec->dziecko matka ->dziecko i ³¹czymy (union)
WITH dzieci AS (
    SELECT ojciec_id AS rodzic_id, osoba_id AS dziecko_id 
    FROM dbo.osoba WHERE ojciec_id IS NOT NULL
    UNION 
    SELECT matka_id, osoba_id 
    FROM dbo.osoba WHERE matka_id IS NOT NULL
)

SELECT TOP 1 WITH TIES dziadek.imie, dziadek.nazwisko, COUNT(wnuczka.osoba_id) AS ile_wnuczat_k
FROM dzieci pokolenie1
JOIN dzieci pokolenie2   ON pokolenie1.dziecko_id = pokolenie2.rodzic_id -- szukamu dzieci, które s¹ ju¿ rodzicami
JOIN dbo.osoba wnuczka   ON pokolenie2.dziecko_id = wnuczka.osoba_id AND wnuczka.plec = 'K'-- szukamu wnuczek
JOIN dbo.osoba dziadek   ON pokolenie1.rodzic_id  = dziadek.osoba_id 
GROUP BY dziadek.osoba_id, dziadek.imie, dziadek.nazwisko
ORDER BY 
    ile_wnuczat_k DESC;
GO