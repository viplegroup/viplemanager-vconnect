@echo off
echo Configuration du projet Viple Management...

REM Créer la structure des dossiers
mkdir Forms\Clients Forms\Services Forms\Links Core Models Services viple_ressources\languages viple_ressources\images vipledata

REM Créer le fichier elioslogs-files.txt
echo # Viple Files Log - Créé par Viple SAS > elioslogs-files.txt
echo # Historique des modifications et créations de fichiers >> elioslogs-files.txt
echo. >> elioslogs-files.txt
echo #Fichiers créés >> elioslogs-files.txt
echo - Fichier : setup.bat créé le 25/06/2025 à %time% >> elioslogs-files.txt

REM Restaurer les packages NuGet
dotnet restore

REM Message de complétion
echo.
echo Installation terminée!
echo Pour lancer l'application, exécutez: dotnet run
pause