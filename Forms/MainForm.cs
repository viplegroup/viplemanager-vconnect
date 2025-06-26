// Viple FilesVersion - MainForm 1.0.1 - Date 26/06/2025 02:10
// Application créée par Viple SAS

using System;
using System.Drawing;
using System.Windows.Forms;
using VipleManagement.Core;
using VipleManagement.Forms.Services;
using VipleManagement.Models;

namespace VipleManagement.Forms
{
    /// <summary>
    /// Formulaire principal de l'application
    /// </summary>
    public class MainForm : Form
    {
        private Panel sideMenu;
        private Panel contentPanel;
        private Label lblUserInfo;
        private Form activeForm;
        
        /// <summary>
        /// Constructeur
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            DisplayUserInfo();
        }
        
        /// <summary>
        /// Initialiser les composants du formulaire
        /// </summary>
        private void InitializeComponent()
        {
            // Configurer le formulaire principal
            this.Text = "Viple Management";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            
            // Créer le menu latéral
            sideMenu = new Panel
            {
                Width = 250,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(37, 37, 38)
            };
            
            // Logo
            PictureBox picLogo = new PictureBox
            {
                Size = new Size(200, 100),
                Location = new Point(25, 20),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            
            try
            {
                string logoPath = "viple_ressources/images/viple_logo.png";
                if (System.IO.File.Exists(logoPath))
                {
                    picLogo.Image = Image.FromFile(logoPath);
                }
            }
            catch
            {
                // Ignorer les erreurs de chargement d'image
            }
            
            // Informations sur l'utilisateur connecté
            lblUserInfo = new Label
            {
                Location = new Point(10, 130),
                Size = new Size(230, 40),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.LightGray
            };
            
            // Boutons du menu
            Button btnDashboard = CreateMenuButton("Tableau de bord", 0);
            btnDashboard.Click += (s, e) => OpenForm(new DashboardForm());
            
            Button btnClients = CreateMenuButton("Clients", 1);
            btnClients.Click += (s, e) => OpenForm(new ClientsListForm());
            
            Button btnServices = CreateMenuButton("État des services", 2);
            btnServices.Click += (s, e) => OpenForm(new ServicesStatusForm());
            
            Button btnCatalog = CreateMenuButton("Catalogue de services", 3);
            btnCatalog.Click += (s, e) => OpenForm(new ServiceCatalogForm());
            
            Button btnProducts = CreateMenuButton("Produits", 4);
            btnProducts.Click += (s, e) => OpenForm(new ProductsForm());
            
            Button btnUsers = CreateMenuButton("Utilisateurs", 5);
            btnUsers.Click += (s, e) => {
                if (AuthenticationManager.CurrentUser.Role == UserRole.Administrator)
                {
                    OpenForm(new UserManagementForm());
                }
                else
                {
                    MessageBox.Show(
                        "Vous n'avez pas les droits nécessaires pour accéder à cette fonctionnalité.",
                        "Accès refusé",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            };
            
            Button btnLinks = CreateMenuButton("Liens utiles", 6);
            btnLinks.Click += (s, e) => OpenForm(new LinksForm());
            
            Button btnSettings = CreateMenuButton("Paramètres", 7);
            btnSettings.Click += (s, e) => OpenForm(new SettingsForm());
            
            Button btnLogout = CreateMenuButton("Déconnexion", 8);
            btnLogout.BackColor = Color.FromArgb(204, 51, 51);
            btnLogout.Click += (s, e) => Logout();
            
            // Ajouter les contrôles au menu
            sideMenu.Controls.AddRange(new Control[] {
                picLogo, lblUserInfo,
                btnDashboard, btnClients, btnServices,
                btnCatalog, btnProducts, btnUsers,
                btnLinks, btnSettings, btnLogout
            });
            
            // Créer le panel de contenu
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 48)
            };
            
            // Ajouter les panels au formulaire
            this.Controls.AddRange(new Control[] { contentPanel, sideMenu });
            
            // Ouvrir le tableau de bord par défaut
            OpenForm(new DashboardForm());
        }
        
        /// <summary>
        /// Créer un bouton pour le menu
        /// </summary>
        private Button CreateMenuButton(string text, int index)
        {
            Button button = new Button
            {
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(250, 45),
                Location = new Point(0, 180 + index * 50),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                Image = null,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Padding = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };
            
            return button;
        }
        
        /// <summary>
        /// Ouvrir un formulaire dans le panel de contenu
        /// </summary>
        private void OpenForm(Form form)
        {
            if (activeForm != null)
            {
                activeForm.Close();
            }
            
            activeForm = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            
            contentPanel.Controls.Add(form);
            contentPanel.Tag = form;
            
            form.Show();
            form.BringToFront();
        }
        
        /// <summary>
        /// Afficher les informations de l'utilisateur connecté
        /// </summary>
        private void DisplayUserInfo()
        {
            if (AuthenticationManager.CurrentUser != null)
            {
                lblUserInfo.Text = $"Connecté en tant que:\n{AuthenticationManager.CurrentUser.FullName}\n{AuthenticationManager.CurrentUser.Role}";
            }
        }
        
        /// <summary>
        /// Se déconnecter de l'application
        /// </summary>
        private void Logout()
        {
            DialogResult result = MessageBox.Show(
                "Êtes-vous sûr de vouloir vous déconnecter ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            
            if (result == DialogResult.Yes)
            {
                // Déconnecter l'utilisateur
                AuthenticationManager.Logout();
                
                // Fermer le formulaire principal
                this.Close();
                
                // Afficher le formulaire de connexion
                LoginForm loginForm = new LoginForm();
                if (loginForm.ShowDialog() == DialogResult.OK && AuthenticationManager.IsLoggedIn)
                {
                    // L'utilisateur s'est reconnecté
                    Application.Run(new MainForm());
                }
                else
                {
                    // L'application va se fermer
                }
            }
        }
    }
    
    /// <summary>
    /// Formulaire de tableau de bord (temporaire, à remplacer)
    /// </summary>
    public class DashboardForm : Form
    {
        public DashboardForm()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(45, 45, 48);
            
            Label lblTitle = new Label
            {
                Text = "Tableau de bord",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(400, 40),
                Location = new Point(20, 20)
            };
            
            Label lblWelcome = new Label
            {
                Text = $"Bienvenue, {AuthenticationManager.CurrentUser?.FullName}",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.LightGray,
                Size = new Size(400, 30),
                Location = new Point(20, 70)
            };
            
            this.Controls.AddRange(new Control[] { lblTitle, lblWelcome });
        }
    }
    
    /// <summary>
    /// Formulaire de paramètres (temporaire, à remplacer)
    /// </summary>
    public class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(45, 45, 48);
            
            Label lblTitle = new Label
            {
                Text = "Paramètres",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(400, 40),
                Location = new Point(20, 20)
            };
            
            this.Controls.Add(lblTitle);
        }
    }
    
    /// <summary>
    /// Formulaire de gestion des produits (temporaire, à remplacer)
    /// </summary>
    public class ProductsForm : Form
    {
        public ProductsForm()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(45, 45, 48);
            
            Label lblTitle = new Label
            {
                Text = "Gestion des produits",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(400, 40),
                Location = new Point(20, 20)
            };
            
            this.Controls.Add(lblTitle);
        }
    }
}