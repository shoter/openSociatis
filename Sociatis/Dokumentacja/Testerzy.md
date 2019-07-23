# Sociatis 0.2
### Wersja bardzo (bardzo) wczesnego dostępu

Po pierwsze na wstępie. Chciałbym wam z góry podziękować za czas poświęcony na testowanie mojej gry. Mam nadzieje że dzięki temu uda nam się stworzyć produkt który zadowoli wasze potrzeby jak i wymagania innych graczy. Jeszcze raz dziękuje.

Tutaj zostaną przedstawione najważniejsze koncepcje które aktualnie działają w grze.

Na panelu startowym gry będziecie mieć wyświetlony zestaw przycisków debugowych. Jest on dość ważny ze względu na to że gra działa w systemie dziennym a nie widze potrzeby uruchamiania harmonogragramu aby zmieniać dzień raz dziennie. Sami go sobie zmienicie za pomocą tego panelu. Dodatkowo możecie się tam uleczyć i dodać do ekwipunku dodatkowy przedmiot. Najpierw wybieracie przedmiot, potem jego jakość i na końcu wpisujecie jego ilość i sobie go pobieracie.

Tutaj ważna uwaga. Niektóre przedmioty nie mogą mieć quality innego niż 1. Do takich przedmiotów zaliczają się:

- Żelazo
- Drewno
- Pszenica
- Liście herbaty
- Olej
- Papier

## Ważne pojęcia
Entity - tak będe nazywał wszystkie grywalne obiekty w grze którymi mogą zarządzać gracze. Są to :

- Obywatele (citizen) - postać gracza
- Firma (company)
- Gazeta (newspaper)
- Organizacja (organisation)
- Państwo (country)
- Partia (party)

### Zaimplementowane funkcjonalności
##### System firm
Możemy założyć własną firmę. Mamy ich 3 rodzaje:
- Firmy wydobywające/tworzące surowce (Żelazo, zboże etc.).
- Firmy przetwarzające surowce/produkty na gotowe produkty (Większość firm).
- Sklepy skupujące od przetwórców towary aby potem sprzedawać je na rynku krajowym/lokalnym.

Aby firma produkujące surowce mogła efektywnie działać musi być postawiona w regionie który posiada surowce. Aktualnie nie ma nigdzie informacji o tym jakie surowce gdzie są więc możecie po prostu dodawać sobie surowiec poprzez menu debugu. Pojawi się to na dniach.

Firma przetwarzająca/sklep aby kupić cokolwiek potrzebuje paliwa w celu przetransportowania surowców do swoich magazynów. Od razu po utworzeniu firmy dostajemy (a przynajmniej powinniśmy :P) jakąś startową ilość paliwa aby móc rozkręcić firme. (W przyszłości jak będziemy mogli kupić paliwo bez posiadania własnego paliwa w firmie. Ilośc zużyta przy transporcie zostanie odjęta od transportu).

Wszystkie firmy mogą sprzedawać swój towar za pośrednictwem dwóch dróg :
- Wystawić oferte na swojej stronie firmy. 
- Wystawić oferte na rynku danego kraju (Może być płatne, zależy od tego co konkres ustali.)

Jeśli oferta zostanie wystawiona na rynku danego kraju to wtedy będziemy mogli kupować towar takiej firmy z każdego miejsca w kraju w którym ten towar jest sprzedawany. W przeciwnym razie musimy być w tym samym regionie co firma (Dotyczy tylko sklepów, w przypadku wymiany między firmami nie ma takiego obowiązku).

Wielkość produkcji każdego pracownika w firmie zależy od :
- jakości towaru który produkuje. Im większa jakość tym dłużej produkujemy dany towar
- (Tylko firmy surowcowe) Jakości złoża surowców
- Rodzaju surowca (na razie tylko papier jest produkowany 3.5 razy szybciej)
- Umiejętności pracownika
- Zdrowia pracownika
- Stanu zaopatrzenia danego regionu (o tym później!)

Każda firma może zatrudnić pracownika na krzywy ryj lub za pomocą umowy.
- Zatrudnienie na krzywy ryj polega na tym że tworzymy normalną oferte pracy. Możemy takiemu pracownikowi dowolnie zmieniać jego wynagrodzenie w czasie pracy u nas, jednakże on może w każdej chwili odejść.
- Zatrudnienie za pomocą kontraktu. Określamy minimalną ilość punktów życia jaką musi posiadać pracownik aby mógł pracować. Nie może on pracować mająć mniejsza ilość życia. Dodatkowo określamy minimalną wypłate jaką możemy pracownikowi ustalić. Jego wypłate możemy zmienić w każdej chwili, jednakże nie może być ona mniejsza niż ta co była na kontrakcie.
Pracownik nie może odejść w każdej chwili ani być zwlniony w każdej chwili. Pracownik może odejść jeśli pracodawca danego dnia nie wypłaci mu pieniędzy za prace lub nie będzie miał wystarczającej ilości surowców aby ten mógł kontynuować prace. Pracodawca może zwolnić pracownika jeśli następnego dnia jeśli pracownik nie pracował dzień wcześniej

W firmie możemy określić domyślną ilość życia potrzebną do pracy dla pracowników na krzywy ryj. Jeśli nie będa mięc tej minimalnej ilości życia to nie mogą pracować. 

Sklepy aby sprzedwać muszą zatrudniać pracownikó którzy produkują 'moc sprzedaży'. 1 moc sprzedaży pozwala sprzedać 1 produkt. Każdy sklep ma na początek dnia 10 mocy sprzedaży.

##### Trening
Każdy obywatel może raz dziennie trenować na polu treningowym w celu zwiększenia swojej siły.
Po roku grania obywatel winien mieć około 8 siły. Żeby zdobyć następny punkt siły musi poświęcić około 500 dni.
Chyba jeszcze nie zaimplementowałem dobrze tej formuły. Jest napisana ale nie wykorzystana. Będzie poprawione

Zależność siły od ilości dni spędzonych na treningu

![](http://i.imgur.com/ndnff1l.png)

##### Wiadomości

Każde entity może wysłać i odbierać wiadomości od innego entity. Wiadomości posegregowane są w wątki do których możemy zaprosić większa ilość uczestników.
Nie ma jak na razie żadnego widecznego powiadomienia o nowych wiadomościach dopóty nie wejdziemy do widoku z wiadomościami.

##### Alerty (warningi)

Wyświetlają informacje tekstowe o tym co się z nami ostatnio działo (szef nas wyrąbał z roboty etc.). 
Nie ma żadnych widocznych powiadomień o nieodczytanych alertach.

##### System zaopatrzenia (supply)

KAŻDY region ma tzw poziom zaopatrzenia. Jego poziom determinuje skuteczność takich akcji jak :
- Produkcja towarów
- Obrażenia zadawane w walce przez obrońców (nie zaimplementowane jeszcze)

Prezydent danego kraju może zwiększać dofinansowanie danych regionów swojego kraju na rzecz zwiększenia poziomu zaopatrzenia.
Ma 4 poziomy dofinansowania w którym każdy pobiera inną ilość złota każdego dnia ze skarbca krajowego.
Poziomy dofinansowania :
- Brak. Przy takim stanie poziom zaopatrzenia będzie spadał do wartości 0.
- Małe. Rośnie do ~1
- Średnie. Rośnie do ~2.8
- Duże. Rośnie do wartości maksymalnej 5.

Po wygranej przegranej bitwie dochodzi do uskodzenia poziomu zaopatrzenia danego regionu.

##### Wojny

Można już toczyć wojny. Aktualnie po wypowiedzeniu wojny dochodzi do wezwania wszystkich aktualnych sojuszników do walki w danej wojnie.
Każdy taki sojusznik bierze tzw niebezpośredni udział w wojnie. Jego żołnierze mogą brać udział w walkach ale nie może rozpoczynać walk lub być atakowanym.
(Oczywiście można mu wypowiedzieć wojne aby nie czuł się taki niebezpośredni :P)
Tylko 2 główne państwa w wojnie mogą atakować swoje regiony.

Czy to rozwiązanie idealne/dobre? Nie wiem. Musze nad tym pomyśleć.

Wojny i bitwy wypowiada prezydent. Do wypowiedzenia wojny potrzebuje złota.

Wojny powodują utrudnienia w przepływie obywateli dla obywateli krajów które uczestniczą w wojnie. Koszt ruchu dla takich osobników jest zwiększony

##### Bitwy

W bitwach mogą brać udział wszyscy członkowie państw bezpośrednich/pośrednich biorących udział w wojnie.
Ilość obrażen jakie zadadzą zależy od:
- jakości broni którą użyją
- Odległości od walki (lepiej podróżować do miejsca walki!)
- Wartości zaopatrzenia (nie zaimplementowane)

##### Liczenie odległości (dotyczy bitew/podróży/handlu)

Wszystko jest liczone automatycznie. Wybierana jest najkrótsza możliwa droga.

Podczas liczenia odległości dla bitwy przejścia między regionami gdzie na przejściu jest chociaż jeden wrogi region dochodzi do policzenia kary za bycie w wojnie.
Przechodzenie przez regiony państw uczestniczących pośrednio w wojnie ma mniejsze kary. 

Podczas liczenia odległości podróży nie liczone są żadne kary.

Podczas obliczania drogi dla handlu omijane są regiony z embargiem nałożonym na dany kraj (wypowiadanie embarg nie jest zaimplementowane)

Na ostateczną odległość pokonaną na danym przejściu między regionami ma wpływ poziom infrastrktury między tymi dwoma regionami. Tym większy tym lepiej. 
Aktualnie nie ma możliwości zmiany stopnia infrastruktury. W bazie danych są wstawione sztywne wartości


##### Partie
Politykują. Co jakiś czas są wybory na prezydenta partii i do kongresu.
Prezydenta partii wybierają klubowicze danej partii.
Wybory kongresowe są rozstrzygane za pomocą głosów obywateli.


##### Kongres
Tam politycy którzy dostali się do kongresu ustalają prawo.

##### Prezydent kraju
Wybierany co jakiś czas (czas ustalany przez kongres). Aktualnie może wypowiadać wojny i ustalać poziom dofinansowania zaopatrzenia.

##### Gazety
Organizacja/obywatel może założyc gazete i pisać w niej wiadomości. Oprócz tego można zaprosić do gazety dziennikarzy i określić im prawa wewnątrz gazety. Tacy zaproszeni ludzie są w stanie zarządzać gazetą i pisać w niej wiadomości.
Gazeta może mieć płatne artykuły. W takim przypadku właściciel ustala cene jednej sztuki. Aby móc sprzedać 1 sztuke gazety należy mieć 1 sztuke papieru.

##### Organizacje
One wymagaja doszlifowania. Działają to na pewno. Nie moge za dużo o nich napisać bo nie mogę się zdecydować na koncepcje co z nimi zrobić.
Ogólnie to są byty za pomocą których możemy zarządzać firmą/gazetą w większa ilość osób.


#### Embarga

Zostały wprowadzone w wersji 0.2.2.5

Pozwalają na całkowitą blokade ruchu handlowego między danymi państwami. Powoduje to omijanie państw podczas obliczania kosztów podróży w wymianach handlowych firm. Skutkiem takiego embarga może być drastyczny wzrost zaopotrzebowania na paliwo w handlu.

Każde embargo może zostać zainicjowane przez prezydenta. Embarga mają swój dzienny koszt utrzymania.

##### Mapa
Jest w bardzo biednej wersji :).

![](http://i.imgur.com/3g6lewG.png)

Liczby między regionami oznaczają ilość kilometrów między nimi. Pamiętajmy że ostateczna odległość może być zależna od wojny/infrastruktury.

### Interfejs
jest brzydki bo testowy. Doskonale o tym wiem :)

### Monetary Market
Nareszcie został zaimplementowany. Nie wiem dokładnie jak działą Forex więc model działania oparłem bardziej o GPW z tym że na GPW handluje się akcjami a tutaj musiałem przełożyć to na waluty.

Można tworzyć oferte sprzedaży/kupna danej waluty i jeśli po stworzeniu oferty jest inna oferta na podstawie której może nastapić wymiana to do takowej wymiany dochodzi na rynku i jest aktualizowany aktualny kurs waluty.

Jest pobierany od tego podatek.
Każde państwo może wprowadzić procent wartości jaki pobiera z MM oraz minimalny podatek jaki trzeba zapłacić jeśli obliczony procent ceny jest mniejszy od podanej wartości minimalnej.


### Nowe wersje.

Jeśli pojawi się jakaś nowa wersja której schemat bazy danych jest niezgodny ze schematem starej wersi to nastąpi restart.
Nie ma co się bawić w dodawaniu pojedynczych kolumn do bazy etc. I tak jest to wersja mocno testowa :).


Jeszcze najważniejsze. Za pomocą <a href="http://testx.sociatis.net/Account/Register">http://testx.sociatis.net/Account/Register</a> możecie się zarejestrować.<br/>
Za pomocą <a href="http://testx.sociatis.net/Account/Login">http://testx.sociatis.net/Account/Login</a>  możecie się zalogować.<br/>
Wasze hasła są zaszyfrowane. Jak je zgubicie i chcecie dostęp do swojego konta to moge je co najwyżej ustawić na dane hasło. To i tak wersja testowa więc nie sądze aby taka potrzeba zaistniała.


Email nie jest weryfikowany w żaden sposób. Najlepiej podać coś w stylu a@b.c

Pozdrawiam,

*Admin*