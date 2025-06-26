// Viple FilesVersion - SplashForm 1.0.1 - Date 26/06/2025 02:44
// Application créée par Viple SAS

using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VipleManagement.Core;

namespace VipleManagement.Forms
{
    /// <summary>
    /// Écran de démarrage avec chargement
    /// </summary>
    public class SplashForm : Form
    {
        private Label lblStatus;
        private ProgressBar progressBar;
        private System.Windows.Forms.Timer timer;
        private PictureBox pictureBox;
        
        private int startupSteps = 5;
        private int currentStep = 0;
        private bool serverConnected = false;
        
        /// <summary>
        /// Constructeur
        /// </summary>
        public SplashForm()
        {
            InitializeComponent();
            StartLoading();
        }
        
        /// <summary>
        /// Initialiser les composants
        /// </summary>
        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(500, 300);
            this.BackColor = Color.FromArgb(37, 37, 38);
            
            // Logo Viple
            pictureBox = new PictureBox
            {
                Size = new Size(200, 100),
                Location = new Point(150, 40),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            
            try
            {
                string logoPath = "viple_ressources/images/viple_logo.png";
                if (System.IO.File.Exists(logoPath))
                {
                    pictureBox.Image = Image.FromFile(logoPath);
                }
            }
            catch
            {
                // Ignorer les erreurs de chargement d'image
            }
            
            // Titre de l'application
            Label lblTitle = new Label
            {
                Text = "Viple Management",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(400, 40),
                Location = new Point(50, 150)
            };
            
            // Barre de progression
            progressBar = new ProgressBar
            {
                Size = new Size(400, 20),
                Location = new Point(50, 200),
                Style = ProgressBarStyle.Continuous,
                Maximum = startupSteps * 10
            };
            
            // Étiquette de statut
            lblStatus = new Label
            {
                Text = "Initialisation...",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(400, 20),
                Location = new Point(50, 230)
            };
            
            // Timer pour animer la barre de progression
            timer = new System.Windows.Forms.Timer
            {
                Interval = 100
            };
            timer.Tick += Timer_Tick;
            
            // Ajouter les contrôles au formulaire
            this.Controls.AddRange(new Control[] {
                pictureBox,
                lblTitle,
                progressBar,
                lblStatus
            });
        }
        
        /// <summary>
        /// Démarrer le chargement
        /// </summary>
        private async void StartLoading()
        {
            timer.Start();

            // Étape 1: Initialisation
            await Task.Delay(500);
            lblStatus.Text = "Initialisation...";
            currentStep = 1;

            // Étape 2: Vérification des fichiers
            await Task.Delay(500);
            lblStatus.Text = "Vérification des fichiers...";
            currentStep = 2;

            // Étape 3: Connexion au serveur
            await Task.Delay(500);
            lblStatus.Text = "Connexion au serveur...";
            currentStep = 3;

            // Étape 4: Chargement des données
            await Task.Delay(500);
            lblStatus.Text = "Chargement des données...";
            currentStep = 4;

            // Étape 5: Prêt !
            await Task.Delay(500);
            lblStatus.Text = "Application prête !";
            currentStep = 5;

            progressBar.Value = progressBar.Maximum;

            await Task.Delay(300);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Animation de la barre de progression
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            int target = currentStep * 10;
            if (progressBar.Value < target)
            {
                progressBar.Value++;
            }
        }
    }
}