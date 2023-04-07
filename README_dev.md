# Tutorial - architektura projektu i pisanie testów

## TL;DR

Zmiany:

- wszystkie metody pomocnicze nieużywające bazy danych w UsersRepository i VideoRepository przeniesione do UsersManager i VideoManager. Powód: po staremu trzeba było mockować też je w testach, co jest totalnie bez sensu bo de facto trzeba było je w testach pisać od nowa. **Zatem w Repozytoriach mogą być tylko metody korzystające z bazy danych.**
- MappingProfile rozbity na UsersProfile i VideoProfile. Czeka nas jeszcze dużo mapowania i bez sensu robić wszystko w jednej klasie.

## Architektura projektu

Zaimplementowana architektura jest zgodna z [Repository Pattern](https://code-maze.com/net-core-web-development-part4/).

Solucja składa się z 5 projektów:

- Contracts - zawiera interfejsy repozytoriów i ich wrapera
- Entities - zawiera klasy używane w wymianie danych z bazą i frontem. Zawiera foldery:
  - _Data_ z klasami DTO
  - DatabaseUtils z klasą zawierająca info o strukturze naszej bazy danych
  - Enums
  - Models - klasy modelowe, identyczne jak struktury przechowywane w bazie danych
- Repository - zawiera:

  - implementację wszystkich interfejsów z projektu Contracts
  - folder Managers - folder z klasami, które zawierają metody pomocnicze dla kontrolerów niekorzystające z bazy danych

- MojeWidelo_WebApi - projekt główny zawierący:

  - kontrolery w folderze Controllers
  - klasę rozszerzającą interfejs IServiceCollection - tutaj znajduje się jest głównie konfiguracja i wstrzykiwanie zależności, które potem łatwo wywołać w Program.cs i zachować tam porządek i czytelność:

    ![image](https://user-images.githubusercontent.com/102852926/230637556-7f4bdbb7-c81a-41a8-99f9-bbec0411b432.png)

  - Filters - folder zawierający filtry, czyli klasy implementujące dwie metody z interfejsu IActionFilter: OnActionExecuting i OnActionExecuted. Zaaplikowanie filtru odbywa się poprzez dodanie dekoratora `[ServiceFilter(typeof(ObjectIdValidationFilter))]` do metody, my używamy tego w metodach endpointowych w kontrolerach. OnActionExecuting wywoła się przed wykonaniem faktycznego ciała metody, a OnActionExecuting po jej wykonaniu. OnActionExecuting może też od razu zwrócić kod błędu (np. gdy id jest w niepoprawnym formacie - ObjectIdValidationFilter.cs) i wtedy metoda udekorowana nie wywołuje się.

    Po utworzeniu nowego filtru trzeba go wstrzyknąć w metodzie `ConfigureFilters` w `ServiceExtensions.cs`.

  - Mapper - folder zawierający mappery, w których konstruktorze trzeba wskazać jakie klasy mają się automatycznie mapować, można wskazać konkretne zachowanie dla konkretnego pola, ignorować niektóre, itp.

- Projekt z testami MojeWidelo_WebApi.UnitTests (szczegółowe omówienie niżej).

<br/>

## Dodawanie nowego kontrolera

Jeden kontroler powinien być odpowiedzialny za jedno repozytorium (ale ma wstrzyknięty RepositoryWrapper, by w szczególnych przypadkach móc wykonywać operacje na kilku repozytoriach jeśli logika tego wymaga).

Jedno repozytorium przeprowadza operacje na jednej kolekcji w bazie danych. Nie powinien mieć żadnych metod, które nie korzystają z kolekcji w bazie danych - utrudnia to testowanie.

Każdy kontroller ma też swojego managera, który zawiera metody pomocnicze, które nie potrzebują dostępu do bazy danych.

A więc by dodać np. VideoController wykonać trzeba było następujące kroki:

1. W pliku appsettings.json w projekcie głównym dodać nazwę kolekcji przechowującej metadane filmów:

   ![image](https://user-images.githubusercontent.com/102852926/230640454-c25ac6a4-c849-4010-b890-01d678a2c421.png)

2. Do `IDatabaseSettings` i `DatabaseSettings` dodać pole o nazwie takiej samej jak klucz dodany w poprzednim kroku:

   ![image](https://user-images.githubusercontent.com/102852926/230640742-aee76448-72d7-4b9d-9f95-5075b0f54858.png)

   Po co? W `ServiceExtensions.cs` mamy metodę, która pobiera konfigurację z appsettings.json i wstrzykuje w `DatabaseSettings`. Potem nazwy tych kolekcji są podawane w konstruktorach do repozytoriów.

3. W Entities -> Models dodać klasę `VideoMetadata` z polami które chcemy trzymać w bazie dziedziczącą po MongoDocumentBase

   ![image](https://user-images.githubusercontent.com/102852926/230641002-6d357d7f-71f0-4034-819a-ce76e7f174b9.png)

4. W projekcie Contracts dodać interfejs `IVideoRepository` dziedziczący po `IRepositoryBase<VideoMetadata>`, który ma już property ze swoją kolekcją, kolekcją do przechowywania plików (GridFS) i zadeklarowane podstawowe operacje na kolekcjach.

   ![image](https://user-images.githubusercontent.com/102852926/230641741-e821a260-c36a-4419-b79a-faf0c9191a11.png)

5. W projekcie Repository dodać implementację powyższego interfejsu

   ![image](https://user-images.githubusercontent.com/102852926/230642005-66594d6e-ca8f-4778-b5f2-9772061b73c0.png)

6. W `IRepositoryWrapper` i `RepositoryWrapper` dodać uchwyt do `IVideoRepository`.
7. Wstrzyknąć zależność w metodzie `ConfigureRepository` w `ServiceExtensions.cs`:

   ![image](https://user-images.githubusercontent.com/102852926/230642542-e75e8f63-07c1-4b47-9abc-08850ffe933d.png)

8. Stworzyć managera, być może na razie pustego.

9. Dopiero teraz tworzymy nowy kontroler, zmieniamy jego klasę bazową na naszą customową `BaseController`. Są tam metody do wyciągania usera lub jego id z tokenu.
   Do konstruktora, oprócz bazowych wymaganych parametrów dodać managera z punktu 8. i utworzymy prywatne pole do trzymania tego managera.

   ![image](https://user-images.githubusercontent.com/102852926/230681901-5d5fba48-cfd7-4c78-a11e-5b96a5a1bf4c.png)

   Następnie wstrzyknąć managera w projekcie `Repository` poprzez dodanie `AddScoped<XYZManager>()`:

   ![image](https://user-images.githubusercontent.com/102852926/230682031-86611560-4eca-4922-837f-5194af82072c.png)

Mam nadzieję, że o niczym nie zapomniałem. W razie co są już 2 w pełni funkcjonalne kontrolery, na których można się wzorować

<br/>

## Pisanie testów

**_TO DO_**
