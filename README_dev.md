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

  - Data z klasami DTO
  - Utils z klasą zawierająca info o strukturze naszej bazy danych i zmiennych
  - Enums
  - Models - klasy modelowe, identyczne jak struktury przechowywane w bazie danych

- Repository - zawiera:

  - implementację wszystkich interfejsów z projektu Contracts
  - folder Managers - folder z klasami, które zawierają metody pomocnicze dla kontrolerów niekorzystające z bazy danych

- MojeWidelo_WebApi - projekt główny zawierący:

  - kontrolery w folderze Controllers
  - klasę rozszerzającą interfejs IServiceCollection - tutaj znajduje się głównie konfiguracja i wstrzykiwanie zależności. Metody rozszerzające potem łatwo wywołać w Program.cs zachowując tam porządek i czytelność:

    ![image](https://user-images.githubusercontent.com/102852926/230637556-7f4bdbb7-c81a-41a8-99f9-bbec0411b432.png)

  - Filters - folder zawierający filtry, czyli klasy implementujące dwie metody z interfejsu IActionFilter: OnActionExecuting i OnActionExecuted. Zaaplikowanie filtru odbywa się poprzez dodanie dekoratora `[ServiceFilter(typeof(ObjectIdValidationFilter))]` do metody, my używamy tego w metodach endpointowych w kontrolerach. OnActionExecuting wywoła się przed wykonaniem faktycznego ciała metody, a OnActionExecuted po jej wykonaniu. OnActionExecuting może też od razu zwrócić kod błędu (np. gdy id jest w niepoprawnym formacie - ObjectIdValidationFilter.cs) i wtedy metoda udekorowana nie wywołuje się.

    Po utworzeniu nowego filtru trzeba go wstrzyknąć w metodzie `ConfigureFilters` w `ServiceExtensions.cs`.

  - Mapper - folder zawierający mappery, w których konstruktorze trzeba wskazać jakie klasy mają się automatycznie mapować, można wskazać konkretne zachowanie dla konkretnego pola, ignorować niektóre, itp.

- Projekt z testami MojeWidelo_WebApi.UnitTests (szczegółowe omówienie niżej).

<br/>

## Dodawanie nowego kontrolera

Jeden kontroler powinien być odpowiedzialny za jedno repozytorium (ale ma wstrzyknięty RepositoryWrapper, by w szczególnych przypadkach móc wykonywać operacje na kilku repozytoriach jeśli logika tego wymaga).

Jedno repozytorium przeprowadza operacje na jednej kolekcji w bazie danych. Nie powinno mieć żadnych metod, które nie korzystają z kolekcji w bazie danych - utrudnia to testowanie.

Każdy kontroler ma też swojego managera, który zawiera metody pomocnicze, które nie potrzebują dostępu do bazy danych.

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

6. W `IRepositoryWrapper` i `RepositoryWrapper` dodać property `IVideoRepository`.
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

## Testy jednostkowe

**UWAGA: od teraz każdy pisze testy do swojego kodu na bieżąco!!!**

### Przydatne linki

- [Testy ASP.NET](https://code-maze.com/asp-net-core-testing/)
- [Testowanie Repository Pattern](https://code-maze.com/testing-repository-pattern-entity-framework/)

### Mockowanie

W folderze `Tests` znajduje się projekt do unit testów, potem może będą projekty z innymi testami.

W unit testach będziemy mockować wszystkie `Repository`, bo w trakcie testów nie mamy połączenia z bazą danych, działamy na jakiejś statycznej kolekcji elementów.
Korzystamy z nugeta `Moq`.

W folderze `Mocks` znajdują się zamockowane:

- Repozytoria - na przykładzie `UsersRepository`

  W `MockIRepositoryBase<T, U>` mamy zamockowane wszystkie operacje CRUD, które prawdopodobnie będą takie same dla wszystkich mocków repozytoriów. Dlatego IUserRepository dziedziczy po `MockIRepositoryBase<IUsersRepository, User>`.

  ![image](https://user-images.githubusercontent.com/102852926/231672350-0004c434-3dae-4e90-879e-1d1f7788e4bd.png)

  `GetMock()` tworzy nam zmockowaną instancję `IUsersRepository`.

  ![image](https://user-images.githubusercontent.com/102852926/231671333-d31a480f-9f54-402e-a90d-4de26215102b.png)

  Najpierw tworzymy statyczną kolekcję która będzie imitować kolekcję z bazy danych - wklepane z palca jakieś poprawne dane.

  Następnie trzeba zdefiniować zachowanie dla każdego pola i każdej metody mockowanej klasy za pomocą `Setup`. Najpierw bierzemy mocka bazowego, potem domockowujemy do niego metody z interfejsu `IUsersRepository`, w tym przypadku jest tylko jedna.

  ![image](https://user-images.githubusercontent.com/102852926/231672177-a6bc5e24-fafb-4bea-954c-094c8704fef4.png)

  Czyli tak naprawdę na nowo piszemy uproszczoną wersję oryginalnej metody, która tylko z grubsza imituje zachowanie metod korzystających z bazy danych. Testy te zatem nie sprawdzą czy te metody oryginalne działają bo to nie jest zadaniem unit testów. Więc np. `Update` zwróci od razu obiekt który chcemy update'ować.

  Syntax:

  - `It.IsAny<T>()` to wskazanie że parametrem funkcji jest zmienna typu `T`.
  - `Callback` - do mockowania metod nic nie zwracających, np. `Delete`. Wsadzamy tu wyrażenie lambda, które będzie się wykonywać przy każdym wywołaniu funkcji wskazanej w `Setup()`.
  - `Returns` - do metod synchronicznych, i tak samo jak wyżej wsadzamy tu wyrażenie lambda...
  - `ReturnsAsync` - do metod asynchronicznych, tak samo jak wyżej wsadzamy tu wyrażenie lambda...

  Na sam koniec `GetMock()` zwracamy całego mocka, będziemy go używać w zamockowanym RepositoryWrapperze.

- RepositoryWrapper - tutaj wykorzystujemy to co zrobiliśmy wyżej i wskazujemy, że pole UsersRepository zwraca zamockowaną instancję (`mock.Object` zwraca instancję zamockowanego obiektu).

  ![image](https://user-images.githubusercontent.com/102852926/230717965-e5a1215d-0f31-4915-bd42-2652134a56ab.png)

- MockUser - to jest zamockowany user wykorzystywany tylko do tego, by z `HttpContext` móc wyciągać Claimsy usera, tak jak to robimy normalnie za pomocą tokena. Są to statyczne pola, pewnie dałoby się tego usera zamockować jakoś ładniej, jeśli ktoś ma na to pomysł to z chęcią wysłucham.

  ![image](https://user-images.githubusercontent.com/102852926/230718133-f66f2433-a25b-49b6-ae56-0cf9698b9f31.png)

### Pisanie testów

Testy są uporządkowane w folderach ze względu na typ obiektu jaki testujemy, controllers osobno, managers osobno, repositories osobno.

Można stosować dwa (a może istnieje więcej, nie wiem) sposoby pisania testów:

- używając dekoratora `[Fact]`

  Test nie przyjmuje żadnych parametrów, dane przygotowywujemy ręcznie (tzn. żeby powtórzyć ten sam test dla różnych danych trzeba go skopiować i pozmieniać dane).
  W teście przygotowujemy dane, wywołujemy metodę, którą testujemy i na koniec sprawdzamy to co chcemy uzyskać za pomocą `Assert`. Test przechodzi gdy wszystkie Asserty przejdą.

  ![image](https://user-images.githubusercontent.com/102852926/230719643-0a366fe7-838c-4fcf-bd6f-b1e05a056406.png)

- używając dekoratorów `[Theory]` i `[InlineData] `

  Test przyjmuje parametry wejściowe, które wskazujemy w parametrach `[InlineData]`. Czyli umieszczając przed testem `[InlineData]` 2 razy napisaliśmy tak naprawdę 2 testy. Poniżej na przykład widać, że fajnie jest używać tego `[InlineData]` bo testujemy funkcję przyjmującą stringa, więc można go łatwo wrzucić do `[InlineData]` i nie kopiować kodu.

  ![image](https://user-images.githubusercontent.com/102852926/230718704-7965a2c2-155d-4aaf-aad6-4a6748460b3f.png)

1. Testowanie managerów:

   - tworzenie managera zwykłym konstruktorem (przypomnienie - nie mockowaliśmy go wcześniej, bo mockujemy tylko klasy i metody korzystające z bazy danych)
   - przygotowanie danych
   - wywołanie metody z managera, którą testujemy
   - sprawdzenie Assertami

     ![image](https://user-images.githubusercontent.com/102852926/230718997-b8c9a856-13bc-49c6-8b03-f915ad28cbc6.png)

2. Testowanie repozytoriów:

   - to samo co wyżej tylko `Repository` bierzemy z mocka

     ![image](https://user-images.githubusercontent.com/102852926/230719061-67732f29-9f60-4645-85d2-9ea434ea909d.png)

3. Testowanie kontrolerów - tutaj jest więcej logiki, więc stworzyłem bazową klasę `BaseControllerTests<T>`, gdzie T jest kontrolerem, który chcemy testować. Trzeba pamiętać, by przy dodawaniu nowego profilu do mapowanie dodać go też tutaj w `GetMapper()`.
   `GetControllerContext()` zwraca nam context z zapisanymi Claimsami usera, by móc je odczytać tak jak byśmy odczytywali je z tokena.

   ![image](https://user-images.githubusercontent.com/102852926/230719220-0ebfabfb-8ea0-4125-ae24-695a9e3ff09b.png)

   W klasie pochodnej, w której będziemy pisać testy trzeba zaimplementować metodę `GetController()`, w której przygotowujemy wszystko co jest potrzebne w teście, żeby nie powtarzać 10 linijek kodu za każdym razem.

   ![image](https://user-images.githubusercontent.com/102852926/230719462-bdcc7f13-1394-4912-80ec-b4494a8b3b8e.png)

   Samo pisanie testów dla kontrolera analogiczne do tego co wyżej.
   
## Opisywanie kodów odpowiedzi HTML

Wszędzie (gdzie się da (1)) do zwracania odpowiedzi w kontrolerze używamy:
	
	return StatusCode(StatusCodes.StatusXXX, zwracany_obiekt);
	
Jeżeli kontroler, zgodnie z dokumentacją, zwraca jakiś obiekt DTO, obiekt do zwrotu wstawiamy pod zwracany_obiekt.
Jeżeli kontroler, zgodnie z dokumentacją, zwraca jedynie kod odpowiedzi, pod zwracany_obiekt wstawiamy string podsumowujący wykonaną procedurę. Opis ten powinien być w języku Polskim.

(1) - Są przypadki, gdy nie trzeba wprost specyfikować zwracanego kodu odpowiedzi (np. podczas streamingu video). Takie rozwiązania są w pełni poprawne.
