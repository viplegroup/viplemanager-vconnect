#!/bin/bash
# Script de configuration pour le projet Viple Management

# Créer la structure des dossiers
mkdir -p Forms/Clients Forms/Services Forms/Links Core Models Services viple_ressources/languages viple_ressources/images vipledata

# Créer le fichier elioslogs-files.txt 
echo "# Viple Files Log - Créé par Viple SAS" > elioslogs-files.txt
echo "# Historique des modifications et créations de fichiers" >> elioslogs-files.txt
echo "" >> elioslogs-files.txt
echo "#Fichiers créés" >> elioslogs-files.txt
echo "- Fichier : setup.sh créé le 25/06/2025 à $(date +%H:%M)" >> elioslogs-files.txt

# Restaurer les packages NuGet
dotnet restore

# Message de complétion
echo -e "\n\033[32mInstallation terminée!\033[0m"
echo -e "Pour lancer l'application, exécutez: \033[33mdotnet run\033[0m"