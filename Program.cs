// Viple FilesVersion - Program 1.0.3 - Date 26/06/2025
// Application créée par Viple SAS

using System;
using System.IO;
using System.Windows.Forms;
using VipleManagement.Core;
using VipleManagement.Forms;
using VipleManagement.Resources;

namespace VipleManagement
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Créer les dossiers requis
            CreateRequiredDirectories();
            
            // Initialiser les ressources
            InitializeResources();
            
            // Afficher l'écran de connexion
            LoginForm loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK && AuthenticationManager.IsLoggedIn)
            {
                // Si l'authentification est réussie, afficher la fenêtre principale
                Application.Run(new MainForm());
            }
        }

        private static void CreateRequiredDirectories()
        {
            string[] directories = new string[]
            {
                "viple_ressources",
                "viple_ressources/images",
                "viple_ressources/languages",
                "viple_res2",
                "viple_res2/fonts",
                "viple_res2/themes",
                "vipledata"
            };

            foreach (string dir in directories)
            {
                Directory.CreateDirectory(dir);
            }
        }
        
        private static void InitializeResources()
        {
            try
            {
                // Installer le répertoire de polices
                ResourceLoader.SetupFontDirectory();
                
                // Extraire les polices incorporées
                FontResources.ExtractAllFonts();
                
                // Charger les polices disponibles
                ResourceLoader.LoadFonts();
                
                LogManager.LogAction("Ressources initialisées");
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de l'initialisation des ressources: {ex.Message}", ex);
            }
        }
    }
}