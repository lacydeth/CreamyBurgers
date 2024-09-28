# CreamyBurgers dokumentáció

## A programról
A projekt célja egy asztali alkalmazás, ahol a felhasználók regisztrálhatnak,
beléphetnek, és ételeket rendelhetnek egy étterem felületén keresztül. Az
adminisztrátor felületen kezelhetők az ételek és azok alapanyagai, beleértve a
raktárkészlet nyomon követését is.
## Csapatunk
### Nagy Kristóf Róbert
> Az alkalmazás megjelenésért, teszteléséért, tervezéséért felelt.
### Bánkuti László
> Adatbázis tervezés, lekérdezések, rendelés leadás, profil adatok módosítása.
### Szabó Tamás
> Adatbázis tervezés, alapanyagnyilvántartás, bejelentkezési rendszer megvalósítása.
## Program működése
### Szerepkörök
- Felhasználó: Regisztrációt követően hozzáférést kap az alkalmazáshoz, megtekintheti a termékeket, rendelést adhat le, rendelési előzményt nézhet és módosíthatja a profiljának adatai.
- Adminisztrátor: Megtekintheti a leadott rendeléseket, ellenőrizheti az alapanyagkészletet, vehet fel hozzá újat vagy módosíthatja a meglévőket.
### Rendelés menete
1. A + és - gombokkal adhat hozzá vagy törölhet a kosárból a felhasználó.
2. A termékek kiválasztását követően a `Fizetés` gombra kattinva leadhatja rendelését.
3. Sikeres rendelésről az alkalmazás felugró üzenettel tájékoztat (sikertelen lehet, ha nincs elegendő alapanyag), illetve a `Rendeléseim` menüpont alatt megtekintheti azt.
## Fiókkezelés
### Regisztráció
![regpage](https://github.com/user-attachments/assets/6913723c-9677-4711-84f6-124c7b76d347)

Új fiók regisztrációja:
- Felhasználónév: Nem tartalmazhat szóközt, nem lehet üres mező.
- Jelszó: Legalább 6 karakter hosszúnak kell lenni, nem lehet üres mező. MD5 titkosítással tároljuk adatbázisunkban.
- Email: Érvényes emailcímet kell megadni (@-ot tartalmaz), nem lehet üres mező.
### Bejelentkezés
![loginpage](https://github.com/user-attachments/assets/baf03f78-6024-4ca9-bdde-0e24964c454c)

Bejelentkezés már meglévő fiókkal:
- A megfelelő felhasználónév, jelszó kombinációval tud bejelentkezni, ezt követően a rendelési ablak jelenik meg.

## Felhasználóknak
### Rendelés leadása
![rendelesleadas](https://github.com/user-attachments/assets/8b9d0155-4f5e-4e57-b0b3-3b1dfb9a4306)
### Profiladatok módosítása
![profiladat](https://github.com/user-attachments/assets/25e08543-d74b-4d58-bc7a-5d045c7b9fc1)
### Leadott rendelések megtekintése
![rendeles](https://github.com/user-attachments/assets/eacdfff0-0edb-4c8a-95b2-6231d13343da)

## Adminisztrátoroknak
### Rendelési előzmények megtekintése
![leadottrendekesek](https://github.com/user-attachments/assets/cb22aee0-1b29-4a67-ab89-fffab61684cb)

Megtekintheti az összes leadott rendelést:
- Rendelés azon. (orderId): Egyedi azonosító a rendelés számára.
- Felhasználó azon. (userId): A megrendelő beazonosítására egyedi érték.
- Rendelés dátuma (orderDate): A rendelés leadásának dátuma.
- Összeg (totalAmount): A rendelés végösszege.
### Alapanyagnyilvántartás megtekintése
![alapanyagnyilvantartas](https://github.com/user-attachments/assets/6afa0e84-5ea6-4e66-a043-1853937f8c27)

Megtekintheti milyen alapanyagból mennyi van, illetve újakat is adhat hozzá vagy meglévőt módosíthat.
## Fejlesztői dokumentáció
### Verziókezelés
![github](https://github.com/user-attachments/assets/a1606ede-2233-4929-816f-9f4af275e9ee)

A verziókezeléshez Git-et használtunk az előírásnak megfelelően, egy branchbe a main-be dolgoztunk és azt mergeltük minden push-nál ha conflict volt. 
### Kommunikáció a csapattagok között
Órákon, illetve Discord hívásban folyamatos kommunikáció mellett dolgoztunk.
### Futtatás
A program futtatásához a `Database` mappában található Creamyburgers.db fájlt a projekt `..\CreamyBurgers\CreamyBurgers\bin\Debug\net8.0-windows` mappájában kell elhelyezni az adatbázis kapcsolat megvalósításához.

- Felhasználó fiók: felhasz: laci jelsz: Asd123
- Admin fiók: felhasz: Strof jelsz: 0

![db](https://github.com/user-attachments/assets/e6b01bec-50ec-4e6c-9395-c827ac3d928b)

## Adatbázis
![dbdiagram](https://github.com/user-attachments/assets/55e54c1d-fe61-42d9-82be-4e8b63c6bf8a)
### Adatbázis szerkesztése
![sqlite](https://github.com/user-attachments/assets/491e3221-640f-439d-8dec-8e640d0ea495)
- Mivel SqLite package-t használtunk a hordozhatóság miatt, ezért egy .db fájl szükséges a futtatáshoz. Ennek szerkesztése a `DB Browser for SqLite` alkalmazázással történt, ahol láthatjuk a táblák szerkezetét és azok recordjait is.
