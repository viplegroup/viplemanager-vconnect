// Viple FilesVersion - ResourceLoader 1.0.3 - Date 26/06/2025
// Application créée par Viple SAS

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using VipleManagement.Resources;

namespace VipleManagement.Core
{
    public static class ResourceLoader
    {
        private static readonly PrivateFontCollection privateFontCollection = new PrivateFontCollection();
        private static readonly string FontsDirectory = Path.Combine("viple_res2", "fonts");
        private static bool fontsLoaded = false;
        
        // Liste des polices chargées
        public static Font VipleHeader { get; private set; }
        public static Font VipleTitle { get; private set; }
        public static Font VipleRegular { get; private set; }
        public static Font VipleBold { get; private set; }
        public static Font VipleLight { get; private set; }
        public static Font VipleMonospace { get; private set; }
        
        static ResourceLoader()
        {
            // Charger les polices par défaut au cas où le chargement personnalisé échoue
            VipleHeader = new Font("Segoe UI", 16, FontStyle.Bold);
            VipleTitle = new Font("Segoe UI", 12, FontStyle.Bold);
            VipleRegular = new Font("Segoe UI", 9, FontStyle.Regular);
            VipleBold = new Font("Segoe UI", 9, FontStyle.Bold);
            VipleLight = new Font("Segoe UI", 9, FontStyle.Regular);
            VipleMonospace = new Font("Consolas", 9, FontStyle.Regular);
        }
        
        public static void LoadFonts()
        {
            if (fontsLoaded) return;
            
            try
            {
                // Extraire les polices incorporées
                FontResources.ExtractAllFonts();
                
                if (Directory.Exists(FontsDirectory))
                {
                    // Charger toutes les polices dans le répertoire
                    string[] fontFiles = Directory.GetFiles(FontsDirectory, "*.ttf");
                    foreach (string fontFile in fontFiles)
                    {
                        try
                        {
                            privateFontCollection.AddFontFile(fontFile);
                            LogManager.LogAction($"Police chargée: {Path.GetFileName(fontFile)}");
                        }
                        catch (Exception ex)
                        {
                            LogManager.LogError($"Erreur lors du chargement de la police {Path.GetFileName(fontFile)}: {ex.Message}", ex);
                        }
                    }
                    
                    // Assigner les polices spécifiques
                    AssignFonts();
                    fontsLoaded = true;
                    
                    LogManager.LogAction("Polices personnalisées chargées avec succès");
                }
                else
                {
                    LogManager.LogError("Répertoire de polices non trouvé");
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors du chargement des polices: {ex.Message}", ex);
            }
        }
        
        private static void AssignFonts()
        {
            try
            {
                // Rechercher et assigner les polices
                for (int i = 0; i < privateFontCollection.Families.Length; i++)
                {
                    FontFamily family = privateFontCollection.Families[i];
                    string familyName = family.Name.ToLower();
                    
                    if (familyName.Contains("roboto") && familyName.Contains("bold"))
                    {
                        VipleHeader = new Font(family, 16, FontStyle.Bold);
                        VipleTitle = new Font(family, 12, FontStyle.Bold);
                        VipleBold = new Font(family, 9, FontStyle.Bold);
                    }
                    else if (familyName.Contains("roboto") && (familyName.Contains("light") || familyName.Contains("thin")))
                    {
                        VipleLight = new Font(family, 9, FontStyle.Regular);
                    }
                    else if (familyName.Contains("roboto") && familyName.Contains("regular"))
                    {
                        VipleRegular = new Font(family, 9, FontStyle.Regular);
                    }
                    else if (familyName.Contains("opensans") && familyName.Contains("regular"))
                    {
                        VipleRegular = new Font(family, 9, FontStyle.Regular);
                    }
                    else if ((familyName.Contains("jetbrains") || 
                             familyName.Contains("firacode") || 
                             familyName.Contains("hack")) && 
                             !familyName.Contains("bold"))
                    {
                        VipleMonospace = new Font(family, 9, FontStyle.Regular);
                    }
                }
                
                LogManager.LogAction("Polices spécifiques assignées");
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de l'assignation des polices: {ex.Message}", ex);
            }
        }
        
        public static void SetupFontDirectory()
        {
            try
            {
                // Créer le répertoire de polices s'il n'existe pas
                Directory.CreateDirectory(FontsDirectory);
                LogManager.LogAction("Répertoire de polices créé ou vérifié");
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la création du répertoire de polices: {ex.Message}", ex);
            }
        }
    }
}