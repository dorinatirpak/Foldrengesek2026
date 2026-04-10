# Konyvtari Nyilvantarto Rendszer (LibrarySystem)

## Szoftvertechnologia beadando feladat – 2. merfoldk

Egyetemi konyvtari adminisztracios rendszer, amely lehetove teszi a konyvek, tagok es kolcsonzesek kezeleset. Az alkalmazas ASP.NET Core MVC architekturara epul, SQLite adatbazist hasznal, es cookie-alapu hitelesitest biztosit.

---

## Tartalomjegyzek

1. [Projekt attekintes](#projekt-attekintes)
2. [Architektura](#architektura)
3. [Hasznalati esetek](#hasznalati-esetek)
4. [Adatmodell](#adatmodell)
5. [Technologiak](#technologiak)
6. [Telepites es futtatas](#telepites-es-futtatas)
7. [Demo bejelentkezesi adatok](#demo-bejelentkezesi-adatok)
8. [Projekt struktura](#projekt-struktura)
9. [Fejlesztesi terv](#fejlesztesi-terv)
10. [Kepernyokepek](#kepernyokepek)

---

## Projekt attekintes

A rendszer egy egyetemi konyvtar munkafolyamatait tamogatja:

- **Konyvnyilvantartas**: Konyvek felvetele, keresese, listazasa
- **Tagkezeles**: Kulonbozo tipusu tagok (hallgato, oktato, kulso, egyeb) kulonbozo jogosultsagokkal
- **Kolcsonzes**: Konyvek kolcsonzese tagoknak, hatarido szamitas automatikusan a tag tipusa alapjan
- **Hitelesites**: Konyvtaros bejelentkezes jelszovedett rendszerbe, max. 3 probalkozas utani zarolas

### 2. merfoldk allapot

| Funkcio | Allapot |
|---------|---------|
| Modellek es adatbazis | 100% kesz |
| Bejelentkezes / Kilepes | Mukodo |
| Iranyitopult (Dashboard) | Mukodo |
| Konyvek listazasa + kereses | Mukodo |
| Uj konyv felvetele | Mukodo |
| Kolcsonzes rogzitese | Mukodo |
| Tagok listazasa | Mukodo |
| Konyv szerkesztese | Tervezve (3. merfoldk) |
| Konyv torlese | Tervezve (3. merfoldk) |
| Konyv reszletek | Tervezve (3. merfoldk) |
| Tag felvetele | Tervezve (3. merfoldk) |
| Tag szerkesztese | Tervezve (3. merfoldk) |
| Tag torlese | Tervezve (3. merfoldk) |
| Tag kereses | Tervezve (3. merfoldk) |
| Kolcsonzes visszavetele | Tervezve (3. merfoldk) |
| Osszes kolcsonzes lista | Tervezve (3. merfoldk) |
| Tag kolcsonzesei | Tervezve (3. merfoldk) |

---

## Architektura

Az alkalmazas haromretegu MVC (Model-View-Controller) architekturara epul:

```
┌─────────────────────────────────────────┐
│              Views (Razor)              │
│  Login, Dashboard, Books, Members,     │
│  Loans nezetek                         │
├─────────────────────────────────────────┤
│            Controllers                  │
│  AccountController, HomeController,    │
│  BooksController, MembersController,   │
│  LoansController                       │
├─────────────────────────────────────────┤
│          Services (DataService)         │
│  Uzleti logika: Auth, CRUD, validacio  │
├─────────────────────────────────────────┤
│        Data (LibraryDbContext)          │
│  Entity Framework Core + SQLite        │
│  Seed adatok, kapcsolatok, korlatok    │
├─────────────────────────────────────────┤
│              Models                     │
│  Book, Loan, LibraryMember (abstract), │
│  StudentMember, ProfessorMember,       │
│  ExternalMember, OtherMember,          │
│  Librarian, ViewModels                 │
└─────────────────────────────────────────┘
```

### Reteg leiras

- **Models**: Teljes erteku domain modellek oroklodessel (LibraryMember -> 4 altipus). Minden tag tipusnak sajat kolcsonzesi limitje es idotartama van. A `ViewModels.cs` tartalmazza a nezet-specifikus adatmodelleket.

- **Data (LibraryDbContext)**: Entity Framework Core kontextus SQLite adatbazissal. TPH (Table Per Hierarchy) diskriminator strategia a tagok oroklodesere. Tartalmazza a seed adatokat a demo bemutatohoz.

- **Services (DataService)**: Kozponti uzleti logika reteg. SHA-256 jelszo haseles, kolcsonzesi limit ellenorzes, peldanyszam nyomonkovetes, keresesi funkciok.

- **Controllers**: MVC controllerek `[Authorize]` attributummal vedett vegspontokkal. A demo verzioban a meg nem implementalt funkciok "kovetkezo verzioban elerheto" uzenetet adnak.

- **Views (Razor)**: Sotet temas, reszponziv felhasznaloi felulet. Modern CSS dizajn valtozo rendszerrel.

---

## Hasznalati esetek

### Mukodo hasznalati esetek (2. merfoldk)

1. **Bejelentkezes**: A konyvtaros felhasznalonev es jelszo megadasaval lep be. 3 sikertelen probalkozas utan a rendszer zarolodik.

2. **Kilepes**: A bejelentkezett konyvtaros kilep a rendszerbol, visszairanyitva a bejelentkezesi oldalra.

3. **Iranyitopult megtekintese**: A fooldal osszefoglalot mutat: konyvek szama, tagok szama, aktiv kolcsonzesek, kesesben levo kolcsonzesek, legutobbi kolcsonzesek tablazata.

4. **Konyvek listazasa es keresese**: A konyvek listajanak megtekintese tablazatos formaban. Kereses cim, szerzo, ISBN vagy azonosito alapjan.

5. **Uj konyv felvetele**: Konyv adatainak megadasa urlapon (szerzo, cim, kiado, ev, kiadas, ISBN, peldanyszam, kolcsonozheto-e). Ha azonos ISBN-nel mar letezik konyv, a peldanyszam novelodik.

6. **Tagok listazasa**: Konyvtari tagok listajanak megtekintese a tipusukkal, kolcsonzesi limitjeikkel es idotartamaikkal.

7. **Kolcsonzes rogzitese**: Konyv es tag kivalasztasa, datum megadasa. A rendszer automatikusan szamolja a lejarati hataridet a tag tipusa alapjan. Ellenorzi a szabad peldanyokat es a tag kolcsonzesi limitet.

### Tervezett hasznalati esetek (3. merfoldk)

8. Konyv szerkesztese
9. Konyv torlese (egy peldany vagy osszes)
10. Konyv reszleteinek megtekintese
11. Uj tag felvetele
12. Tag szerkesztese
13. Tag torlese (soft delete)
14. Tag keresese
15. Konyv visszavetele (keses szamitas)
16. Osszes kolcsonzes listazasa
17. Tag kolcsonzeseinek megtekintese

---

## Adatmodell

### Book (Konyv)

| Mezo | Tipus | Leiras |
|------|-------|--------|
| Id | int | Elsodleges kulcs |
| Author | string (max 200) | Szerzo neve |
| Title | string (max 300) | Konyv cime |
| Publisher | string (max 200) | Kiado |
| Year | int | Kiadasi ev |
| Edition | string (max 50) | Kiadas |
| ISBN | string (max 20) | ISBN szam |
| IsLoanable | bool | Kolcsonozheto-e |
| IsDeleted | bool | Soft delete jelzo |
| CopyCount | int | Peldanyok szama |

### LibraryMember (Konyvtari tag) - absztrakt

| Mezo | Tipus | Leiras |
|------|-------|--------|
| Id | int | Elsodleges kulcs |
| Name | string (max 200) | Tag neve |
| Address | string (max 300) | Lakcim |
| Contact | string (max 200) | Elerhetoseg |
| IsDeleted | bool | Soft delete jelzo |
| MemberType | enum | Tag tipusa (diskriminator) |
| MaxBooks | int (szamitott) | Max kolcsonozheto konyvek szama |
| LoanDays | int (szamitott) | Kolcsonzesi idotartam napokban |

#### Tag tipusok

| Tipus | MaxBooks | LoanDays | Leiras |
|-------|----------|----------|--------|
| StudentMember | 5 | 60 | Egyetemi hallgato |
| ProfessorMember | Korlatlan | 365 | Egyetemi oktato |
| ExternalMember | 4 | 30 | Mas egyetem polgara |
| OtherMember | 2 | 14 | Egyeb |

### Loan (Kolcsonzes)

| Mezo | Tipus | Leiras |
|------|-------|--------|
| Id | int | Elsodleges kulcs |
| BookId | int | Konyv FK |
| MemberId | int | Tag FK |
| LoanDate | DateTime | Kolcsonzes datuma |
| DueDate | DateTime | Lejarati hatarido |
| ReturnDate | DateTime? | Visszahozas datuma (null = aktiv) |
| IsReturned | bool (szamitott) | Visszahoztak-e |
| IsOverdue | bool (szamitott) | Kesesben van-e |
| OverdueDays | int (szamitott) | Keses napokban |

### Librarian (Konyvtaros)

| Mezo | Tipus | Leiras |
|------|-------|--------|
| Id | int | Elsodleges kulcs |
| Username | string (max 100, egyedi) | Felhasznalonev |
| PasswordHash | string | SHA-256 jelszo hash |
| FullName | string (max 200) | Teljes nev |

### ER Diagram

```
┌──────────────┐     ┌──────────────┐     ┌──────────────────┐
│   Librarian  │     │     Book     │     │  LibraryMember   │
├──────────────┤     ├──────────────┤     ├──────────────────┤
│ Id (PK)      │     │ Id (PK)      │     │ Id (PK)          │
│ Username     │     │ Author       │     │ Name             │
│ PasswordHash │     │ Title        │     │ Address          │
│ FullName     │     │ Publisher    │     │ Contact          │
└──────────────┘     │ Year         │     │ MemberType (disc)│
                     │ Edition      │     │ IsDeleted        │
                     │ ISBN         │     └────────┬─────────┘
                     │ IsLoanable   │              │
                     │ IsDeleted    │     ┌────────┴─────────┐
                     │ CopyCount    │     │  StudentMember   │
                     └──────┬───────┘     │  ProfessorMember │
                            │             │  ExternalMember  │
                            │             │  OtherMember     │
                     ┌──────┴───────┐     └────────┬─────────┘
                     │    Loan      │              │
                     ├──────────────┤              │
                     │ Id (PK)      │              │
                     │ BookId (FK)──┘              │
                     │ MemberId (FK)───────────────┘
                     │ LoanDate     │
                     │ DueDate      │
                     │ ReturnDate   │
                     └──────────────┘
```

---

## Technologiak

| Technologia | Verzio | Celjuk |
|-------------|--------|--------|
| .NET | 8.0 | Futtatokornyezet |
| ASP.NET Core MVC | 8.0 | Web framework |
| Entity Framework Core | 8.0.11 | ORM / adatbazis kezeles |
| SQLite | - | Egyszeru, fajl-alapu adatbazis |
| Razor Views | - | Szerver-oldali HTML rendereles |
| CSS3 (Custom) | - | Egyedi sotet temas dizajn |

---

## Telepites es futtatas

### Elofeltetel

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Lepesek

```bash
# 1. Repository klonozasa
git clone https://github.com/dorinatirpak/Foldrengesek2026.git
cd Foldrengesek2026/LibrarySystem

# 2. Fuggosegek telepitese es alkalmazas futtatasa
dotnet restore
dotnet run
```

Az alkalmazas alapertelmezetten a `http://localhost:5000` cimen erheto el.

> Az adatbazis automatikusan letrejon az elso indulaskor a seed adatokkal egyutt.

---

## Demo bejelentkezesi adatok

| Mezo | Ertek |
|------|-------|
| Felhasznalonev | `admin` |
| Jelszo | `admin123` |

> 3 sikertelen bejelentkezesi kiserlet utan a rendszer zarodik.

### Demo adatok

Az alkalmazas eloretoltott demo adatokkal indul:

**Konyvek (7 db):**
- The Art of Computer Programming (Knuth) - 3 peldany
- Clean Code (Martin) - 2 peldany
- Design Patterns (Gamma et al.) - 1 peldany
- Introduction to Algorithms (Cormen) - 4 peldany
- Modern Operating Systems (Tanenbaum) - 2 peldany
- Database System Concepts (Silberschatz) - 1 peldany (nem kolcsonozheto)
- Refactoring (Fowler) - 2 peldany

**Tagok (5 fo):**
- Kovacs Anna (hallgato)
- Dr. Nagy Peter (oktato)
- Toth Gabor (kulso egyetem)
- Szabo Petra (hallgato)
- Kiss Zoltan (egyeb)

**Kolcsonzesek (5 db):** Eloretoltott aktiv es lezart kolcsonzesek a demo bemutatohoz.

---

## Projekt struktura

```
LibrarySystem/
├── Controllers/
│   ├── AccountController.cs    # Bejelentkezes, kilepes, zarolas
│   ├── BooksController.cs      # Konyv CRUD muveletek
│   ├── HomeController.cs       # Iranyitopult
│   ├── LoansController.cs      # Kolcsonzes muveletek
│   └── MembersController.cs    # Tagkezeles
├── Data/
│   └── LibraryDbContext.cs     # EF Core kontextus + seed adatok
├── Models/
│   ├── Book.cs                 # Konyv entitas
│   ├── Librarian.cs            # Konyvtaros entitas
│   ├── LibraryMember.cs        # Absztrakt tag + 4 altipus
│   ├── Loan.cs                 # Kolcsonzes entitas
│   └── ViewModels.cs           # Nezet modellek
├── Services/
│   └── DataService.cs          # Kozponti uzleti logika
├── Views/
│   ├── Account/
│   │   ├── Login.cshtml        # Bejelentkezesi oldal
│   │   └── Lockout.cshtml      # Zarolt allapot oldal
│   ├── Books/
│   │   ├── Index.cshtml        # Konyvek listazasa + kereses
│   │   └── Create.cshtml       # Uj konyv felvetele urlap
│   ├── Home/
│   │   └── Index.cshtml        # Iranyitopult / Dashboard
│   ├── Loans/
│   │   └── Create.cshtml       # Kolcsonzes rogzitese urlap
│   ├── Members/
│   │   └── Index.cshtml        # Tagok listazasa
│   └── Shared/
│       └── _Layout.cshtml      # Kozos elrendezes (navbar, alertek)
├── wwwroot/
│   ├── css/site.css            # Egyedi CSS (sotet tema)
│   └── js/site.js              # Kliensoldali JavaScript
├── Program.cs                  # Alkalmazas belepesi pont
├── appsettings.json            # Konfiguracio
├── LibrarySystem.csproj        # Projekt fajl
└── LibrarySystem.sln           # Solution fajl
```

---

## Fejlesztesi terv

### 2. merfoldk (jelenlegi) - Prototipus 1
- [x] Teljes adatmodell es adatbazis reteg
- [x] Bejelentkezes/kilepes/zarolas
- [x] Iranyitopult statisztikakkal
- [x] Konyvek listazasa es keresese
- [x] Uj konyv felvetele
- [x] Tagok listazasa
- [x] Kolcsonzes rogzitese
- [x] Sotet temas, reszponziv UI
- [x] Demo/seed adatok

### 3. merfoldk (kovetkezo) - Prototipus 2
- [ ] Konyv szerkesztese
- [ ] Konyv torlese (peldanyonkent vagy teljes)
- [ ] Konyv reszletek oldal
- [ ] Uj tag felvetele
- [ ] Tag szerkesztese
- [ ] Tag torlese
- [ ] Tag keresese
- [ ] Konyv visszavetele (keses szamitas)
- [ ] Osszes kolcsonzes listazasa
- [ ] Tag kolcsonzeseinek megtekintese

### 4. merfoldk - Vegleges verzio
- [ ] Kodminoseg javitas (Clean Code)
- [ ] Teszteles
- [ ] Vegleges dokumentacio

---

## Kepernyokepek

A demo verzio az alabbi oldalakat tartalmazza:

1. **Bejelentkezesi oldal** - Sotet temas login felulelet demo hinttel
2. **Iranyitopult** - Statisztikai kartyak, legutobbi kolcsonzesek, gyors muveletek
3. **Konyvek lista** - Tablazatos megjeleniteskeresesi lehetoseggel
4. **Uj konyv** - Urlap a konyv adatainak megadasahoz
5. **Tagok lista** - Tagok tablazata tipussal es limitekkel
6. **Kolcsonzes** - Konyv es tag kivalasztasa, elonezet a lejarati hatarilovel

---

## Szerzok

- Tirpak Dorina

## Licenc

Egyetemi projekt - Szoftvertechnologia tanterv
