Yaroslav Furmanov
Marat Ishakov

1. Rozbudowa modelu domenowego

Aplikacja została rozszerzona o obsługę kontaktów typu Organization.

Dodano możliwość:

dodawania organizacji,
edycji organizacji,
usuwania organizacji,
wyszukiwania organizacji.
2. Powiązanie kontaktów typu Person z organizacją

Dodano relację umożliwiającą przypisanie kontaktu typu Person do organizacji.

Organizacja może posiadać wielu członków (Members), natomiast kontakt typu Person może należeć do jednej organizacji.

3. Operacje CRUD

Rozszerzono aplikację o pełną obsługę operacji CRUD dla:

Person
dodawanie,
edycja,
usuwanie,
wyszukiwanie.
Organization
dodawanie,
edycja,
usuwanie,
wyszukiwanie.
4. Wyszukiwanie kontaktów

Dodano możliwość wyszukiwania kontaktów według różnych kryteriów, m.in.:

domeny adresu e-mail,
przynależności do organizacji,
nazwy firmy,
fragmentu imienia lub nazwiska.
5. ValueObject PESEL

Zdefiniowano klasę typu ValueObject reprezentującą numer PESEL.

Klasa odpowiada za:

walidację numeru PESEL,
poprawne odczytywanie daty urodzenia,
określanie płci,
wyznaczanie cyfry kontrolnej.

Klasa została zastosowana jako typ właściwości we wszystkich encjach posiadających numer PESEL.

6. Mapowanie PESEL w bazie danych

Dodano odpowiednie klasy konwersji umożliwiające:

mapowanie łańcucha znaków z bazy danych na obiekt klasy PESEL,
mapowanie obiektu PESEL na łańcuch znaków zapisywany w bazie danych.

Konwersja została skonfigurowana w Entity Framework Core.

7. Testy jednostkowe

Dodano testy jednostkowe dla klasy PESEL.

Testy sprawdzają:

poprawne zakodowanie daty urodzenia,
poprawne wyznaczanie cyfry kontrolnej,
poprawny odczyt płci.
8. Podsumowanie

Zaimplementowano:

obsługę kontaktów typu Organization,
relacje pomiędzy Person i Organization,
operacje CRUD,
wyszukiwanie kontaktów według różnych kryteriów,
klasę ValueObject dla numeru PESEL,
konwersję PESEL w Entity Framework Core,
testy jednostkowe klasy PESEL.
