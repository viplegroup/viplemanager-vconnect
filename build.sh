#!/bin/bash

echo "Viple Management - Script de build"
echo "Application créée par Viple SAS"
echo "===================================="
echo

# Vérifier si .NET SDK est installé
if ! command -v dotnet &> /dev/null; then
    echo "ERREUR: .NET SDK n'est pas installé ou n'est pas dans votre PATH"
    echo "Veuillez télécharger et installer .NET SDK depuis https://dotnet.microsoft.com/download"
    exit 1
fi

echo "Création des répertoires nécessaires..."
mkdir -p builds/release

echo
echo "Compilation du projet en mode Release..."
dotnet build --configuration Release

if [ $? -ne 0 ]; then
    echo
    echo "ERREUR: La compilation a échoué"
    exit 1
fi

echo
echo "Création du package déployable..."
dotnet publish --configuration Release --output builds/release

if [ $? -ne 0 ]; then
    echo
    echo "ERREUR: La publication a échoué"
    exit 1
fi

echo
echo "Copie des fichiers de ressources..."
mkdir -p builds/release/viple_ressources/images
mkdir -p builds/release/viple_ressources/languages
mkdir -p builds/release/viple_res2
mkdir -p builds/release/vipledata

cp -r viple_ressources/* builds/release/viple_ressources/ 2>/dev/null || true
cp -r viple_res2/* builds/release/viple_res2/ 2>/dev/null || true

echo
echo "Création du fichier de version..."
echo "Viple Management System" > builds/release/version.txt
echo "Version: 1.0.0" >> builds/release/version.txt
echo "Date de compilation: $(date)" >> builds/release/version.txt
echo "(c) 2025 Viple SAS. Tous droits réservés." >> builds/release/version.txt

echo
echo "Build terminée avec succès !"
echo "Les fichiers se trouvent dans le dossier \"builds/release\""
echo

exit 0