import pyodbc
from datetime import datetime, timedelta
import random

conn = pyodbc.connect('''
    DRIVER=SQL SERVER;
    SERVER=ANTE\\SQL2019;
    DATABASE=SUUO;
    TrustServerCertificate=yes;
''')
cursor = conn.cursor()

cursor.execute("""           
    CREATE TABLE pozicija (
    pozicijaId INT PRIMARY KEY IDENTITY(1,1),
    nazivPozicije VARCHAR(30) NOT NULL
);

CREATE TABLE zaposlenik (
    zaposlenikId INT PRIMARY KEY IDENTITY(1,1),
    pozicijaId INT,
    ime VARCHAR(15) NOT NULL,
    prezime VARCHAR(30) NOT NULL,
    email VARCHAR(100) NULL,
    zaporka VARCHAR(100) NOT NULL,
    datumZaposljavanja DATETIME NOT NULL,
    aktivan BIT DEFAULT 1,
    FOREIGN KEY (pozicijaId) REFERENCES pozicija(pozicijaId)
);

CREATE TABLE stol (
    stolId INT PRIMARY KEY IDENTITY(1,1),
    brojStola INT NOT NULL,
    kapacitet INT NOT NULL,
);

CREATE TABLE rezervacija (
    rezervacijaId INT PRIMARY KEY IDENTITY(1,1),
    stolId INT,
    ime VARCHAR(15) NOT NULL,
    prezime VARCHAR(30) NOT NULL,
    kontaktTelefon VARCHAR(20) NULL,
    email VARCHAR(100) NULL,
    vrijeme DATETIME NOT NULL,
    brojOsoba INT NOT NULL,
    napomena VARCHAR(255) NULL,
    FOREIGN KEY (stolId) REFERENCES stol(stolId)
);

CREATE TABLE kategorijaJelovnika (
    kategorijaId INT PRIMARY KEY IDENTITY(1,1),
    nazivKategorije VARCHAR(30) NOT NULL,
    redoslijed INT NULL
);

CREATE TABLE stavkaJelovnika (
    stavkaJelovnikaId INT PRIMARY KEY IDENTITY(1,1),
    kategorijaId INT NOT NULL,
    naziv VARCHAR(50) NOT NULL,
    opis VARCHAR(255) NULL,
    cijena MONEY NOT NULL,
    dostupno BIT DEFAULT 1,
    FOREIGN KEY (kategorijaId) REFERENCES kategorijaJelovnika(kategorijaId)
);

CREATE TABLE namirnica (
    namirnicaId INT PRIMARY KEY IDENTITY(1,1),
    nazivNamirnice VARCHAR(50) NOT NULL,
    jedinicaMjere VARCHAR(20) NOT NULL,
    dostupnaKolicina FLOAT NOT NULL,
    minimalnaKolicina FLOAT NULL
);

CREATE TABLE recept (
    receptId INT PRIMARY KEY IDENTITY(1,1),
    stavkaJelovnikaId INT NOT NULL,
    namirnicaId INT NOT NULL,
    kolicina FLOAT NOT NULL,
    FOREIGN KEY (stavkaJelovnikaId) REFERENCES stavkaJelovnika(stavkaJelovnikaId),
    FOREIGN KEY (namirnicaId) REFERENCES namirnica(namirnicaId)
);

CREATE TABLE trenutniStatus (
    statusId INT PRIMARY KEY IDENTITY(1,1),
    naziv VARCHAR(30) NOT NULL
);

CREATE TABLE narudzba (
    narudzbaId INT PRIMARY KEY IDENTITY(1,1),
    zaprZaposlenikId INT NOT NULL,
    naplZaposlenikId INT NULL, 
    stolId INT NOT NULL,
    statusId INT NOT NULL,
    vrijemeNarudzbe DATETIME DEFAULT GETDATE(),
    vrijemeNaplate DATETIME NULL,
    ukupnaCijena MONEY NULL,
    napomena VARCHAR(255) NULL,
    FOREIGN KEY (zaprZaposlenikId) REFERENCES zaposlenik(zaposlenikId),
    FOREIGN KEY (naplZaposlenikId) REFERENCES zaposlenik(zaposlenikId),
    FOREIGN KEY (stolId) REFERENCES stol(stolId),
    FOREIGN KEY (statusId) REFERENCES trenutniStatus(statusId)
);

CREATE TABLE stavkaNarudzbe (
    stavkaNarudzbeId INT PRIMARY KEY IDENTITY(1,1),
    narudzbaId INT NOT NULL,
    stavkaJelovnikaId INT NOT NULL,
    statusId INT NOT NULL,
    kolicina INT DEFAULT 1,
    cijenaPriNarudzbi MONEY NOT NULL,
    napomena VARCHAR(255) NULL,
    FOREIGN KEY (narudzbaId) REFERENCES narudzba(narudzbaId),
    FOREIGN KEY (stavkaJelovnikaId) REFERENCES stavkaJelovnika(stavkaJelovnikaId),
    FOREIGN KEY (statusId) REFERENCES trenutniStatus(statusId)
);

CREATE TABLE nabavaNamirnica (
    nabavaId INT PRIMARY KEY IDENTITY(1,1),
    zaposlenikId INT NOT NULL,
    datumNabave DATETIME NOT NULL,
    ukupniTrosak MONEY NULL,
    FOREIGN KEY (zaposlenikId) REFERENCES zaposlenik(zaposlenikId)
);

CREATE TABLE stavkaNabave (
    stavkaNabaveId INT PRIMARY KEY IDENTITY(1,1),
    nabavaId INT NOT NULL,
    namirnicaId INT NOT NULL,
    kolicina FLOAT NOT NULL,
    cijena MONEY NOT NULL,
    FOREIGN KEY (nabavaId) REFERENCES nabavaNamirnica(nabavaId),
    FOREIGN KEY (namirnicaId) REFERENCES namirnica(namirnicaId)
);

""")

cursor.execute("INSERT INTO pozicija (nazivPozicije) VALUES ('Vlasnik'), ('Konobar'), ('Kuhar'), ('Menadžer')")
cursor.execute("""INSERT INTO trenutniStatus (naziv) VALUES ('Nova'), ('U pripremi'), ('Gotova'), ('Poslužena'), ('Naplaćena'), ('Stornirana')""")
cursor.execute("""INSERT INTO kategorijaJelovnika (nazivKategorije) VALUES ('Predjela'), ('Glavna jela'), ('Deserti'), ('Bezalkoholna pića'), ('Alkoholna pića')""")

imena = ['Ivan', 'Marko', 'Petra', 'Ana', 'Luka', 'Katarina', 'Josip', 'Maja']
prezimena = ['Horvat', 'Kovač', 'Babić', 'Marić', 'Jurić', 'Novak', 'Knežević', 'Perić']

for _ in range(20):
    ime = random.choice(imena)
    prezime = random.choice(prezimena)
    email = f"{ime.lower()}.{prezime.lower()}@restoran.hr"
    pozicija_id = random.randint(1, 4)
    datum = datetime.now() - timedelta(days=random.randint(0, 1000))
    cursor.execute(
        "INSERT INTO zaposlenik (pozicijaId, ime, prezime, email, zaporka, datumZaposljavanja) VALUES (?, ?, ?, ?, ?, ?)",
        pozicija_id, ime, prezime, email, 'tajna123', datum
    )

for broj in range(1, 21):
    kapacitet = random.choice([2, 4, 6])
    cursor.execute("INSERT INTO stol (brojStola, kapacitet) VALUES (?, ?)", broj, kapacitet)

for _ in range(30):
    ime = random.choice(imena)
    prezime = random.choice(prezimena)
    vrijeme = datetime.now() + timedelta(days=random.randint(0, 15), hours=random.randint(12, 21))
    broj_osoba = random.randint(1, 6)
    stol_id = random.randint(1, 20)
    cursor.execute(
        "INSERT INTO rezervacija (stolId, ime, prezime, kontaktTelefon, email, vrijeme, brojOsoba, napomena) VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
        stol_id, ime, prezime, '0912345678', f"{ime.lower()}.{prezime.lower()}@gmail.com",
        vrijeme, broj_osoba, 'Bez napomena'
    )

namirnice = [
    ('Piletina', 'kg'), ('Junetina', 'kg'), ('Krumpir', 'kg'), ('Luk', 'kg'),
    ('Šećer', 'kg'), ('Jaja', 'kom'), ('Mlijeko', 'l'), ('Brašno', 'kg'),
    ('Vino', 'l'), ('Pivo', 'l'), ('Mineralna voda', 'l')
]

for naziv, jedinica in namirnice:
    dostupna = round(random.uniform(5, 50), 2)
    min_kol = round(random.uniform(2, 10), 2)
    cursor.execute(
        "INSERT INTO namirnica (nazivNamirnice, jedinicaMjere, dostupnaKolicina, minimalnaKolicina) VALUES (?, ?, ?, ?)",
        naziv, jedinica, dostupna, min_kol
    )

jela = [
    ('Juha od rajčice', 'Predjela', 4.5),
    ('Goveđa juha', 'Predjela', 5.0),
    ('Bečki odrezak', 'Glavna jela', 10.0),
    ('Ćevapi u lepinji', 'Glavna jela', 8.5),
    ('Palačinke s čokoladom', 'Deserti', 4.0),
    ('Sladoled', 'Deserti', 3.0),
    ('Coca-Cola 0.5L', 'Bezalkoholna pića', 2.5),
    ('Sok od naranče', 'Bezalkoholna pića', 2.5),
    ('Pivo točeno 0.5L', 'Alkoholna pića', 3.5),
    ('Crno vino 0.2L', 'Alkoholna pića', 4.0)
]

kategorije_map = {
    'Predjela': 1,
    'Glavna jela': 2,
    'Deserti': 3,
    'Bezalkoholna pića': 4,
    'Alkoholna pića': 5
}

for naziv, kat, cijena in jela:
    opis = f"{naziv} - opis!"
    kat_id = kategorije_map[kat]
    cursor.execute(
        "INSERT INTO stavkaJelovnika (kategorijaId, naziv, opis, cijena) VALUES (?, ?, ?, ?)",
        kat_id, naziv, opis, cijena
    )

for stavka_id in range(1, len(jela) + 1):
    for _ in range(random.randint(1, 3)):
        namirnica_id = random.randint(1, len(namirnice))
        kolicina = round(random.uniform(0.1, 1.0), 2)
        cursor.execute(
            "INSERT INTO recept (stavkaJelovnikaId, namirnicaId, kolicina) VALUES (?, ?, ?)",
            stavka_id, namirnica_id, kolicina
        )

for i in range(20):
    stol_id = random.randint(1, 20)
    status_id = random.randint(1, 6)
    zapr_zap = random.randint(1, 20)
    vrijeme = datetime.now() - timedelta(hours=random.randint(0, 48))
    cursor.execute(
        "INSERT INTO narudzba (zaprZaposlenikId, stolId, statusId, vrijemeNarudzbe) VALUES (?, ?, ?, ?)",
        zapr_zap, stol_id, status_id, vrijeme
    )

    for _ in range(random.randint(1, 5)):
        stavka_id = random.randint(1, len(jela))
        kolicina = random.randint(1, 3)
        cijena = jela[stavka_id - 1][2]
        cursor.execute(
            "INSERT INTO stavkaNarudzbe (narudzbaId, stavkaJelovnikaId, statusId, kolicina, cijenaPriNarudzbi) VALUES (?, ?, ?, ?, ?)",
            i+1, stavka_id, status_id, kolicina, cijena
        )

for i in range(10):
    zaposlenik_id = random.randint(1, 20)
    datum = datetime.now() - timedelta(days=random.randint(0, 30))
    cursor.execute(
        "INSERT INTO nabavaNamirnica (zaposlenikId, datumNabave) VALUES (?, ?)",
        zaposlenik_id, datum
    )

    for _ in range(random.randint(1, 5)):
        namirnica_id = random.randint(1, len(namirnice))
        kolicina = round(random.uniform(1, 10), 2)
        cijena = round(random.uniform(1, 20), 2)
        cursor.execute(
            "INSERT INTO stavkaNabave (nabavaId, namirnicaId, kolicina, cijena) VALUES (?, ?, ?, ?)",
            i+1, namirnica_id, kolicina, cijena
        )

conn.commit()
cursor.close()
conn.close()
