@echo off
echo Viple Management - Script de build
echo Application créée par Viple SAS
echo ====================================
echo.

REM Vérifier si .NET SDK est installé
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERREUR: .NET SDK n'est pas installé ou n'est pas dans votre PATH
    echo Veuillez télécharger et installer .NET SDK depuis https://dotnet.microsoft.com/download
    exit /b 1
)

echo Création des répertoires nécessaires...
if not exist "builds" mkdir builds
if not exist "builds\release" mkdir builds\release

echo.
echo Compilation du projet en mode Release...
dotnet build --configuration Release

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERREUR: La compilation a échoué
    exit /b 1
)

echo.
echo Création du package déployable...
dotnet publish --configuration Release --output builds\release

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERREUR: La publication a échoué
    exit /b 1
)

echo.
echo Copie des fichiers de ressources...
if not exist "builds\release\viple_ressources" mkdir builds\release\viple_ressources
if not exist "builds\release\viple_ressources\images" mkdir builds\release\viple_ressources\images
if not exist "builds\release\viple_ressources\languages" mkdir builds\release\viple_ressources\languages
if not exist "builds\release\viple_res2" mkdir builds\release\viple_res2
if not exist "builds\release\vipledata" mkdir builds\release\vipledata

xcopy /E /I /Y viple_ressources builds\release\viple_ressources
xcopy /E /I /Y viple_res2 builds\release\viple_res2

echo.
echo Création du fichier de version...
echo Viple Management System > builds\release\version.txt
echo Version: 1.0.0 >> builds\release\version.txt
echo Date de compilation: %date% %time% >> builds\release\version.txt
echo (c) 2025 Viple SAS. Tous droits réservés. >> builds\release\version.txt

echo.
echo Build terminée avec succès !
echo Les fichiers se trouvent dans le dossier "builds\release"
echo.

exit /b 0