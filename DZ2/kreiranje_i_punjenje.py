import pyodbc
from datetime import datetime

conn_str = (
    "DRIVER={SQL Server};"
    "SERVER=localhost;"
    "DATABASE=ImeTvojeBaze;" 
    "Trusted_Connection=yes;"
)

conn = pyodbc.connect(conn_str)
cursor = conn.cursor()

cursor.execute("""
IF OBJECT_ID('stavkaNarudzbe', 'U') IS NOT NULL DROP TABLE stavkaNarudzbe;
IF OBJECT_ID('narudzba', 'U') IS NOT NULL DROP TABLE narudzba;
IF OBJECT_ID('rezervacija', 'U') IS NOT NULL DROP TABLE rezervacija;
IF OBJECT_ID('zaposlenik', 'U') IS NOT NULL DROP TABLE zaposlenik;
IF OBJECT_ID('pozicija', 'U') IS NOT NULL DROP TABLE pozicija;
IF OBJECT_ID('stol', 'U') IS NOT NULL DROP TABLE stol;
IF OBJECT_ID('stavkeJelovnika', 'U') IS NOT NULL DROP TABLE stavkeJelovnika;
IF OBJECT_ID('statusNarudzbe', 'U') IS NOT NULL DROP TABLE statusNarudzbe;
IF OBJECT_ID('statusStavkeNarudzbe', 'U') IS NOT NULL DROP TABLE statusStavkeNarudzbe;
IF OBJECT_ID('namirnica', 'U') IS NOT NULL DROP TABLE namirnica;
""")
conn.commit()

# Create fresh tables
table_creation_sql = """
CREATE TABLE pozicija (
    pozicijaId INT PRIMARY KEY IDENTITY(1,1),
    nazivPozicije VARCHAR(30) NOT NULL
);

CREATE TABLE zaposlenik (
    zaposlenikId INT PRIMARY KEY IDENTITY(1,1),
    pozicijaId INT,
    ime VARCHAR(15) NOT NULL,
    prezime VARCHAR(30) NOT NULL,
    zaporka VARCHAR(100) NOT NULL,
    datumZaposljavanja DATETIME NOT NULL,
    FOREIGN KEY (pozicijaId) REFERENCES pozicija(pozicijaId)
);

CREATE TABLE stol (
    stolId INT PRIMARY KEY IDENTITY(1,1),
    kapacitet INT NOT NULL
);

CREATE TABLE rezervacija (
    rezervacijaId INT PRIMARY KEY IDENTITY(1,1),
    stolId INT,
    prezime VARCHAR(30) NOT NULL,
    vrijeme DATETIME NOT NULL,
    brojOsoba INT NOT NULL,
    FOREIGN KEY (stolId) REFERENCES stol(stolId)
);

CREATE TABLE stavkeJelovnika (
    stavkaJelovnikaId INT PRIMARY KEY IDENTITY(1,1),
    naziv VARCHAR(30) NOT NULL,
    cijena MONEY NOT NULL
);

CREATE TABLE statusNarudzbe (
    statusNarudzbeId INT PRIMARY KEY IDENTITY(1,1),
    naziv VARCHAR(30) NOT NULL
);

CREATE TABLE statusStavkeNarudzbe (
    statusStavkeNarudzbeId INT PRIMARY KEY IDENTITY(1,1),
    naziv VARCHAR(30) NOT NULL
);

CREATE TABLE narudzba (
    narudzbaId INT PRIMARY KEY IDENTITY(1,1),
    zaprZaposlenikId INT NOT NULL,
    naplZaposlenikId INT NOT NULL,
    stolId INT NOT NULL,
    statusId INT NOT NULL,
    vrijemeNarudzbe DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (zaprZaposlenikId) REFERENCES zaposlenik(zaposlenikId),
    FOREIGN KEY (naplZaposlenikId) REFERENCES zaposlenik(zaposlenikId),
    FOREIGN KEY (stolId) REFERENCES stol(stolId),
    FOREIGN KEY (statusId) REFERENCES statusNarudzbe(statusNarudzbeId)
);

CREATE TABLE stavkaNarudzbe (
    stavkaNarudzbeId INT PRIMARY KEY IDENTITY(1,1),
    narudzbaId INT NOT NULL,
    stavkaJelovnikaId INT NOT NULL,
    statusId INT NOT NULL,
    kolicina INT DEFAULT 1,
    FOREIGN KEY (narudzbaId) REFERENCES narudzba(narudzbaId),
    FOREIGN KEY (stavkaJelovnikaId) REFERENCES stavkeJelovnika(stavkaJelovnikaId),
    FOREIGN KEY (statusId) REFERENCES statusStavkeNarudzbe(statusStavkeNarudzbeId)
);

CREATE TABLE namirnica (
    namirnicaId INT PRIMARY KEY IDENTITY(1,1),
    nazivNamirnice VARCHAR(50) NOT NULL,
    dostupnaKolicina FLOAT NOT NULL
);
"""
cursor.execute(table_creation_sql)
conn.commit()
print("Tablice uspješno kreirane!")


for naziv in ['Konobar', 'Kuhar', 'Menadžer']:
    cursor.execute("INSERT INTO pozicija (nazivPozicije) VALUES (?)", naziv)

zaposlenici = [
    (1, 'Miron', 'Ivić', 'lozinka123e', datetime(2023, 5, 10)),
    (1, 'Ivan', 'Pavelić', 'lozinka3123', datetime(2020, 7, 15)),
    (1, 'Anton', 'Kvesić', 'lozink4a123', datetime(2025, 1, 3)),
    (2, 'Ana', 'Anić', 'kuharica456', datetime(2022, 3, 22)),
    (2, 'Ante', 'Volarević', 'kuhar23', datetime(2016, 12, 5)),
    (3, 'Marko', 'Marić', 'admin789', datetime(2021, 11, 5)),
]
for zap in zaposlenici:
    cursor.execute("""
        INSERT INTO zaposlenik (pozicijaId, ime, prezime, zaporka, datumZaposljavanja) 
        VALUES (?, ?, ?, ?, ?)""", zap)

for kapacitet in [2, 4, 6, 8]:
    cursor.execute("INSERT INTO stol (kapacitet) VALUES (?)", kapacitet)

rezervacije.extend([
    (1, 'Marić', datetime(2025, 4, 9, 19, 0), 2),
    (2, 'Novak', datetime(2025, 4, 9, 20, 30), 5),
    (2, 'Perić', datetime(2025, 4, 10, 18, 0), 3),
    (4, 'Jurić', datetime(2025, 4, 10, 19, 30), 6),
    (3, 'Knežević', datetime(2025, 4, 11, 18, 45), 4),
    (1, 'Vuković', datetime(2025, 4, 11, 20, 15), 2),
    (3, 'Kovačić', datetime(2025, 4, 12, 19, 0), 7),
    (2, 'Blažević', datetime(2025, 4, 12, 20, 0), 4),
    (3, 'Matić', datetime(2025, 4, 13, 18, 30), 5),
    (4, 'Pavić', datetime(2025, 4, 13, 19, 45), 3)
])

for r in rezervacije:
    cursor.execute("""
        INSERT INTO rezervacija (stolId, prezime, vrijeme, brojOsoba)
        VALUES (?, ?, ?, ?)""", r)

jelovnik.extend([
    ('Lasagne Bolognese', 12.5),
    ('Rižoto s morskim plodovima', 13.0),
    ('Biftek s umakom od gljiva', 18.5),
    ('Pileći file sa žara', 10.5),
    ('Miješana salata', 4.5),
    ('Tiramisu', 5.0),
    ('Panna cotta', 4.5),
    ('Prirodni sok 0.3L', 3.0),
    ('Pivo 0.5L', 3.5),
    ('Vino 0.75L', 15.0)
])

for naziv, cijena in jelovnik:
    cursor.execute("INSERT INTO stavkeJelovnika (naziv, cijena) VALUES (?, ?)", naziv, cijena)

for s in ['U pripremi', 'Posluženo', 'Naplaćeno']:
    cursor.execute("INSERT INTO statusNarudzbe (naziv) VALUES (?)", s)

for s in ['Novo', 'U pripremi', 'Gotovo']:
    cursor.execute("INSERT INTO statusStavkeNarudzbe (naziv) VALUES (?)", s)

narudzbe = [
    (1, 3, 1, 1),
    (1, 3, 2, 2),
    (2, 2, 1, 1),
    (3, 1, 3, 1),
    (3, 1, 4, 2),
    (1, 2, 2, 2),
    (2, 3, 3, 1),
    (3, 2, 4, 1)
]
for n in narudzbe:
    cursor.execute("""
        INSERT INTO narudzba (zaprZaposlenikId, naplZaposlenikId, stolId, statusId)
        VALUES (?, ?, ?, ?)""", n)

stavke = [
    (1, 1, 1, 1),
    (1, 3, 1, 2),
    (2, 2, 2, 1),
    (2, 4, 2, 1),
    (3, 1, 3, 1),
    (3, 2, 3, 1),
    (4, 1, 4, 2),
    (4, 2, 4, 1),
    (5, 1, 5, 1),
    (5, 2, 5, 1),
    (6, 1, 6, 1),
    (6, 2, 6, 1)
]
for s in stavke:
    cursor.execute("""
        INSERT INTO stavkaNarudzbe (narudzbaId, stavkaJelovnikaId, statusId, kolicina)
        VALUES (?, ?, ?, ?)""", s)

namirnice.extend([
    ('Maslinovo ulje', 12.0),
    ('Brašno', 4.5),
    ('Panceta', 8.0),
    ('Jaja', 3.5),
    ('Parmezan', 7.0),
    ('Bosiljak', 2.5),
    ('Češnjak', 3.0),
    ('Luk', 2.0),
    ('Gljive', 6.5),
    ('Vrhnje za kuhanje', 4.0)
])
for naziv, kol in namirnice:
    cursor.execute("INSERT INTO namirnica (nazivNamirnice, dostupnaKolicina) VALUES (?, ?)", naziv, kol)

conn.commit()
print("Successfuly!")
conn.close()
