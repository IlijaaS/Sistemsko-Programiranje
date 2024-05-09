# Sistemsko-Programiranje
Projekti iz predmeta Sistemsko Programiranje, skolska godina 2023/2024

## Projekat 1
**Zadatak 21:**
Kreirati Web server koji klijentu omogućava prikaz vrednosti zagađenja vazduha uz pomoć IQ
Air API-a. Pretraga se može vršiti pomoću filtera koji se definišu u okviru query-a. Vrednosti
zagađenja vazduha se vraćaju kao odgovor (pretragu vršiti po gradu). Svi zahtevi serveru se šalju
preko browser-a korišćenjem GET metode. Ukoliko navedene vrednosti zagađenja ne postoje,
prikazati grešku klijentu.
Način funkcionisanja IQ Air API-a je moguće proučiti na sledećem linku: https://apidocs.iqair.com/?version=latest
Primer poziva serveru:
http://api.airvisual.com/v2/city?city=Los%20Angeles&state=California&country=USA&key={{YOUR_API_KEY}}

**Uputstvo:**
- Pokrenuti projekat
- Testirati njegov rad na sledeci nacin:
  - http://localhost:8080/?city={Grad}
- Podrzava samo gradove iz Srbije, jer je za poziv svakog grada na svetu potrebno uneti i ime drzave kao i ime regije u kojoj se grad nalazi, pa zbog lakseg testiranja izbacili smo potrebu za svim ovim parametrima iz query-a
