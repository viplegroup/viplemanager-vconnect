@echo off
echo Préparation des polices pour Viple Manager...
echo.

:: Créer les dossiers nécessaires
mkdir viple_res2\fonts 2>nul

echo Structure de dossiers créée avec succès.
echo.
echo Pour utiliser les polices dans l'application :
echo 1. Placez manuellement vos fichiers TTF ou OTF dans le dossier viple_res2\fonts
echo 2. L'application les chargera automatiquement au démarrage
echo.
echo Polices recommandées :
echo - Roboto (Regular, Bold, Light)
echo - Open Sans (Regular, Bold)
echo - Fira Code (pour le texte monospace)
echo - JetBrains Mono (pour le texte de code)
echo.
echo Mise à jour du fichier journal...

:: Enregistrer l'action dans le fichier de journal
echo - Fichier : download_fonts.bat exécuté le %date% à %time% >> elioslogs-files.txt

echo.
echo Appuyez sur une touche pour quitter...
pause > nul