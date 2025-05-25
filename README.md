# Informacijski sustavi

## Naziv projekta
- Sustav upravljanja ugostiteljskim objektom

## Kratica:
- SUUO

## Autori:
- Ivan Pavelić
- Ante Volarević

## Upute za pokretanje "Sustav upravljanja ugostiteljskim objektom" aplikacije
----------------------------------------------------------

1. Projekt se može klonirati s pomoću naredbe:  
   `git clone https://github.com/AnteVol/SUUO.git`  
   (Projekt je također predan i u .zip podmapi `Database`)

2. Naredbom `cd SUUO/DZ3` pozicionirajte se u root same programske aplikacije.

3. U datotekama:
   - `appsettings.json`
   - `appsettings.Development.json`  
   Potrebno je promijeniti (ili dodati novi) `ConnectionString` kako biste se mogli povezati s bazom podataka.  
   *(Bazu podataka se može pronaći u mapi `Database` koju je prethodno potrebno importati kroz SSMS kako biste imali spremne podatke.)*

4. Ako imate razvojnu okolinu za .NET, dovoljno je samo pokrenuti `"SUUO_DZ3: http"` konfiguraciju.  
   *(Ako prethodno nemate .NET okruženje, prvo izvršite `dotnet restore`, a zatim `dotnet run`)*

5. Ako ste sve uspješno napravili, otvorit će vam se web stranica na:  
   `http://localhost:5020/`
