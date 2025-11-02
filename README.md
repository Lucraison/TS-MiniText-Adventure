# Mini Text Adventure: Project Documentatie

**Groep CAAN**  
- [Charafeddine Bukraa](https://github.com/sharaftien)  
- [Arthur Öksüz](https://github.com/Arthur-Oksuz)  
- [Amer Ahmady](https://github.com/amerahmady)  
- [Nicolas Herrera Santibanez](https://github.com/Lucraison)  


---

## Project Overzicht
We hebben in dit project een volledig **Mini Text Adventure** spel gemaakt volgens de opdracht.\
Het spel bevat zes kamers, twee items, een monster, en spelregels rond beweging, inventory, vechten, en winnen en verliezen.

Het project is is opgedeeld in duidelijke lagen: **Domain**, **Gameplay**, **Program**.\
We passen de **SOLID-principes** toe voor onderhoudbaarheid en testbaarheid.  

Alle vereisten zijn aanwezig:  
- **Wereld** met 6 kamers en verbindingen  
- **Items** oppakken en gebruiken  
- **Sleutel** om deur te openen  
- **Monster** dat alleen met zwaard verslagen kan worden  
- **Dodelijke kamer** (west)  
- Volledige **commando-ondersteuning** (`help`, `look`, `inventory`, `go`, `take`, `fight`, `quit`)  
- **Testing** (Unit Testing, Integration Testing, Behaviour Driven Testing)

---

## Bestandstructuur & Functionaliteit

```
Solution 'TextAdventure'
├── TextAdventure
│   ├── Domain
│   │   ├── Direction.cs          Enum + TryParse voor N/E/S/W (richting parsing)
│   │   ├── Inventory.cs          Beheert spelerinventaris (add/remove/contains/describe)
│   │   ├── Item.cs               Item met ID, naam, beschrijving (onveranderlijk)
│   │   └── Room.cs               Kamer met naam, beschrijving, items, exits, speciale vlaggen
│   ├── Gameplay
│   │   ├── GameSetup.cs          Bouwt de volledige wereld op (kamers, items, verbindingen)
│   │   ├── Input.cs              Parse-t console-input naar (command, argument)
│   │   └── World.cs              Gameplay logica: beweging, vechten, win/dead, regels
│   └── Program.cs                Console interface: input, commando's, output
│
└── TextAdventure.Tests
    ├── Features
    │   ├── Monster.feature       Gherkin scenario's voor monsterinteracties
    │   └── WinPath.feature       Gherkin scenario's voor win path
    ├── BddRunner.cs              Given/When/Then runner voor BDD-tests
    ├── InventoryTests.cs         Unit tests: Add, Remove, Contains
    ├── RoomTests.cs              Unit tests: Take item uit kamer
    ├── MonsterFeatureTests.cs    BDD: alle monster-scenario's
    ├── WinPathFeatureTests.cs    BDD: win path
    └── WorldRulesTests.cs        Integration tests: beweging, sleutel, dood, fight
```

> **Alle klassen uit de opdracht zijn geïmplementeerd**: Item, Inventory, Room, World (Rooms), Direction, GameSetup, Program.  
> **Alle commando’s werken**: help, look, inventory, go n|e|s|w, take <id>, fight, quit.  
> **Alle spelregels zijn correct**: dodelijke kamer, sleutel vereist, monster alleen met zwaard, vluchten = dood.

---

## Testaanpak

We hebben een **drievoudige teststrategie** toegepast:

### 1. Unit Testing (xUnit)
- **InventoryTests.cs**: Testen van **Add**, **Remove**, **Contains**
- **RoomTests.cs**: Testen van **Take** verwijdert item correct
- Geïsoleerde, snelle tests zonder afhankelijkheden

### 2. Integratietesten
- **WorldRulesTests.cs**: Testen van complexe regels:
  - West = direct dood
  - North zonder sleutel = geblokkeerd
  - Fight zonder zwaard = dood
  - Fight met zwaard = monster verslagen
  - Vluchten van monster = dood
- Gebruiken volledige **GameSetup.BuildWorld()** voor realistisch gedrag

### 3. Behavior-Driven Testing
- **Monster.feature** & **WinPath.feature**: Leesbare scenario’s
- **BddRunner.cs**: Verwerkt **Given**, **When**, **Then**, **And**
- End-to-end validation van spelverloop

---

## Spel Starten

1. Open **TextAdventure.sln**
2. Bouw de solution (**Ctrl+Shift+B**)  
3. Run het **TextAdventure** project (**F5**)  
4. Typ commando’s in de console:  
   - **help** → lijst met commando’s  
   - **look** → kamer, items, uitgangen  
   - **go n** / **take key** / **fight** etc.  

5. Tests run je via **Test Explorer** om werking te checken.

