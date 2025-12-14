# Secure Text Adventure

## Groep CAAN
- [Charafeddine Bukraa](https://github.com/sharaftien)
- [Arthur Öksüz](https://github.com/Arthur-Oksuz)
- [Amer Ahmady](https://github.com/amerahmady)
- [Nicolas Herrera Santibanez](https://github.com/Lucraison)

---

## Project Overzicht

In dit project hebben we onze **Mini Text Adventure** uitgebreid met **security-functionaliteit**.
Het spel bestaat uit een **console-applicatie** en een **Minimal API** die samen zorgen voor gameplay en beveiliging.

De speler kan het spel **enkel starten na succesvolle authenticatie**.
Bepaalde kamers zijn **versleuteld** en kunnen enkel geopend worden met een combinatie van:
- een **keyshare** via de API
- een **passphrase** gevonden in het spel

---

## Projectstructuur

TextAdventure.sln
├── TextAdventure
│ ├── Program.cs Console interface & spelstart
│ ├── Gameplay Spelverloop en regels
│ ├── Rooms Kamers en wereldstructuur
│ └── Security Login, tokengebruik, decryptie
│
├── TextAdventure.Api
│ ├── Program.cs Minimal API setup
│ ├── Controllers Authenticatie & sleutelbeheer
│ ├── Models Users, tokens, requests
│ ├── Services Hashing, JWT, encryptie
│ └── Security Security-configuratie
│
└── TextAdventure.Tests
├── UnitTests
└── IntegrationTests

---

## Security

### Authenticatie & Autorisatie
- Gebruikers registreren en loggen in via de **Minimal API**
- Wachtwoorden worden **gehasht met SHA-256**
- Bij succesvolle login wordt een **JWT-token** gegenereerd
- De console-app gebruikt dit token voor beveiligde API-calls
- Ongeldige of verlopen tokens worden geweigerd

### Encryptie
- Minstens twee kamers zijn **versleuteld**
- Encryptie gebeurt met **X.509 / CMS**
- Voor toegang tot deze kamers zijn nodig:
  - een **keyshare** van de API
  - een **passphrase** uit het spel

### Secure Coding
- Inputvalidatie op console- en API-niveau
- Geen gevoelige data in logs
- Scheiding van verantwoordelijkheden
- Tokens en sleutels worden gecontroleerd op geldigheid

---

## Spel Starten

### Vereisten
- .NET SDK 7.0 of hoger
- Visual Studio

### 1. API starten
1. Open `TextAdventure.sln`
2. Zet **TextAdventure.Api** als startup project
3. Start de applicatie  
   → De API draait lokaal op `https://localhost:xxxx`

### 2. Console game starten
1. Zet **TextAdventure** als startup project
2. Start de applicatie
3. Log in met een geldige gebruiker
4. Het spel start automatisch na succesvolle login

---

## Hoe speel je het spel?

### Beschikbare commando’s
- `help` → toont alle beschikbare commando’s
- `look` → beschrijft de huidige kamer, items en uitgangen
- `inventory` → toont je inventaris
- `go n | e | s | w` → beweeg naar een andere kamer
- `take <item>` → neem een item op
- `fight` → vecht met het monster
- `quit` → stop het spel

### Belangrijke spelregels
- **Zonder zwaard** → `fight` = **dood**
- **Zonder sleutel** → bepaalde deuren blijven gesloten
- **Weglopen van het monster** = **dood**
- Sommige kamers zijn **dodelijk**
- Versleutelde kamers vereisen correcte decryptie

---

## Tests

Het project bevat tests voor:
- gameplay-regels
- security-logica
- integratie tussen console en API

Tests kunnen uitgevoerd worden via **Test Explorer** in Visual Studio.

---

## Conclusie

Dit project combineert:
- een werkende **text adventure**
- veilige **authenticatie met hashing en JWT**
- **encryptie en sleutelbeheer**
- duidelijke scheiding tussen gameplay en security

Het spel is enkel speelbaar binnen een **beveiligde context**, conform de opdrachtvereisten.
