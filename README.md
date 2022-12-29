# Aplikacja frontend dla hurtowni danych MS Analysis Services

Jest to aplikacja, z graficznym interfejsem użytkownika, wywołująca zapytania MDX do hurtowni danych zbudowanej w technologii Microsoft SQL Server Analysis Services. Wyniki zapytania prezentowane są w tabeli.

Wszystkie zapytania kryją się pod odpowiednimi przyciskami. Użytkownik może też napisać swoje zapytanie i je wywołać.

W folderze **CompiledApplicationAndExampleConfiguration** znajduje się skomilowana aplikacja z przykładowymi plikiami, które zawierają konfigurację połączenia do hurtowni danych.

W folderze **database_backups** znajdują się niezbędne pliki bazy danych do zaimportowania na serwerze SQL Server:
1. **Northwind.bak** - plik relacyjnej bazy danych SQL, do zaimportowania na serwerze **Database Engine**
2. **ProjektNaHurtownieNorthwind.abf** - plik hurtowni danych, do zaimportowania na serwerze **Analysis Services (Multidimensional)**
