# Smart Rockets

Tento projekt je vizuální diskrétní simulace v C# a WPF, která demonstruje schopnosti genetického algoritmu. Cílem programu je naučit populaci virtuálních "raket" najít optimální cestu ze startovního bodu do cíle, a to při vyhýbání se překážkám. Rakety se v každé generaci učí ze svých předchozích pokusů pomocí evolučních principů (selekce, křížení, mutace).

## Popis programu
Program simuluje chování hejna raket ve 2D prostoru. Každá raketa má vlastní "DNA", která se skládá z posloupnosti vektorů síly. Tyto vektory určují pohyb rakety v jednotlivých krocích (ticích) simulace. 

Základní herní smyčka je navázána na překreslování okna (`CompositionTarget.Rendering`). Simulace probíhá po určený počet ticků (životnost rakety). Pokud raketa narazí do překážky nebo okraje obrazovky, umírá. Pokud dosáhne cíle, zastaví se a zapíše si úspěch. Po uplynutí životnosti se celá populace vyhodnotí, vytvoří se nová (lepší) generace a proces se opakuje. Průběh úspěšnosti se automaticky vizualizuje v grafu přímo v uživatelském rozhraní.

## Dekompozice projektu
Projekt je logicky rozdělen do několika jmenných prostorů a tříd oddělujících UI od aplikační logiky:

* **`UI (MainWindow.xaml.cs)`**: Řeší hlavní smyčku (`GameLoop`), aktualizaci vykreslování, dynamické generování grafu úspěšnosti a export dat.
* **`Core.Entities`**: 
    * `Rocket`: Reprezentuje fyzikální objekt (pozice, rychlost, zrychlení) a uchovává si odkaz na svou DNA a aktuální stav (zda žije, zda došla do cíle a jaká je její fitness).
    * `Obstacle`: Definuje překážky ve světě pomocí hraničního obdélníku (`Rect`).
* **`Core.Genetics`**:
    * `DNA`: Zapouzdřuje genetickou informaci (pole vektorů). Obsahuje logiku pro uniformní křížení a mutaci.
    * `Population`: Spravuje kolekci raket. Zajišťuje ohodnocení (evaluate) a tvorbu nové generace pomocí selekce a elitismu.
* **`Core.Simulation`**:
    * `World`: Definuje prostředí (rozměry, cíl, seznam překážek) a poskytuje metody pro detekci kolizí.
* **`Core`**:
    * `Config`: Statická třída uchovávající globální hyperparametry simulace (velikost populace, míra mutace, bonusy/penalizace).

## Datové struktury
Pro reprezentaci logiky využívá projekt následující klíčové datové struktury:
* **Vestavěné WPF struktury (`Vector`, `Point`, `Rect`)**: Použity pro veškeré výpočty fyziky a detekci kolizí ve 2D prostoru.
* **Pole (`Array`)**: Vektory v třídě `DNA` jsou uloženy v poli (`Vector[]`), protože velikost genů je fixní a odpovídá předem dané životnosti (`Lifespan`). Přístup přes index v každém ticku je tak maximálně efektivní.
* **Seznamy (`List<T>`)**: Použity pro dynamičtější kolekce, například `List<Rocket>` pro populaci, `List<Obstacle>` pro překážky ve světě a `List<int>` v hlavním okně pro ukládání historie počtu úspěšných raket pro vykreslení grafu a export.

## Genetický algoritmus a jeho úpravy
Evoluce v programu funguje na základě následujících mechanismů:

1. **Kódování (DNA)**: Jednotlivcem je raketa, jejíž chromozom (DNA) je posloupnost 2D vektorů. Každý vektor reprezentuje akceleraci v jednom diskrétním časovém kroku.
2. **Fitness funkce**: Kvalita každé rakety je hodnocena podle převrácené hodnoty druhé mocniny vzdálenosti od cíle:
   `Fitness = 1 / (distance^2)`
   *Úpravy a penalizace:* Program obsahuje dodatečné úpravy fitness skóre, aby lépe usměrňoval evoluci. Pokud raketa dosáhne cíle, její fitness se násobí bonusem (`TargetBonus = 5.0`). Naopak, pokud raketa narazí do překážky a zemře, její skóre se výrazně sníží penalizací (`CrashPenalty = 0.2`).
3. **Selekce**: Rodiče pro novou generaci jsou vybíráni metodou ruletového kola (Roulette Wheel Selection) – pravděpodobnost výběru je přímo úměrná fitness rakety. Je implementován **elitismus**, což znamená, že nejlepší raketa z předchozí generace automaticky a bez jakýchkoliv změn přechází do nové generace, čímž se zabraňuje ztrátě nejlepšího nalezeného řešení.
4. **Křížení (Crossover)**: Využívá se *uniformní křížení*. Na rozdíl od jednobodového křížení se pro každý tick iteruje přes pole genů a s 50% pravděpodobností se vybere gen (vektor) od prvního, nebo od druhého rodiče.
5. **Mutace**: S pravděpodobností danou parametrem `MutationRate` (standardně 1 %) je vektor v sekvenci nahrazen zcela novým náhodným vektorem. Tím se udržuje diverzita populace.

## Návod k použití

1. **Spuštění**: Program nevyžaduje žádné složité nastavování. Stačí aplikaci zkompilovat a spustit soubor MainWindow.xaml.cs. Simulace pak běží automaticky.
2. **Sledování evoluce**: V hlavním okně můžete sledovat, jak se rakety snaží obletět překážku. V horní části okna se zobrazuje aktuální číslo generace, počet zbývajících ticků a aktuální počet raket v cíli.
3. **Graf úspěšnosti**: V levém panelu se s každou další generací překresluje zelený graf, který ukazuje vývoj úspěšnosti populace v čase.
4. **Export dat**: V levém panelu se nachází tlačítko "Exportovat statistiky". Po jeho stisknutí se otevře dialogové okno pro uložení `.txt` souboru. Tento soubor obsahuje všechny nastavené hyperparametry simulace a detailní log historie generací, což se dá použít pro následnou analýzu dat.
