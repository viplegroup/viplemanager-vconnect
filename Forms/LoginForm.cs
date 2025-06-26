// Viple FilesVersion - LoginForm 1.0.0 - Date 26/06/2025 01:50
// Application créée par Viple SAS

using System;
using System.Drawing;
using System.Windows.Forms;
using VipleManagement.Core;

namespace VipleManagement.Forms
{
    /// <summary>
    /// Formulaire de connexion à l'application
    /// </summary>
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnCancel;
        private Label lblError;
        private PictureBox picLogo;
        
        /// <summary>
        /// Constructeur
        /// </summary>
        public LoginForm()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Initialiser les composants du formulaire
        /// </summary>
        private void InitializeComponent()
        {
            // Configurer le formulaire
            this.Text = "Viple - Authentification";
            this.Size = new Size(400, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            this.AcceptButton = btnLogin;
            this.CancelButton = btnCancel;
            
            // Logo
            picLogo = new PictureBox
            {
                Size = new Size(200, 200),
                Location = new Point(100, 20),
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
            
            // Titre
            Label lblTitle = new Label
            {
                Text = "VIPLE MANAGEMENT",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(380, 30),
                Location = new Point(10, 230),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            
            // Sous-titre
            Label lblSubTitle = new Label
            {
                Text = "Système de gestion des services",
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(380, 20),
                Location = new Point(10, 260),
                ForeColor = Color.Silver,
                BackColor = Color.Transparent
            };
            
            // Nom d'utilisateur
            Label lblUsername = new Label
            {
                Text = "Nom d'utilisateur",
                Size = new Size(120, 20),
                Location = new Point(50, 300),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.White
            };
            
            txtUsername = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(50, 320),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };
            
            // Mot de passe
            Label lblPassword = new Label
            {
                Text = "Mot de passe",
                Size = new Size(120, 20),
                Location = new Point(50, 350),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.White
            };
            
            txtPassword = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(50, 370),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true
            };
            
            // Message d'erreur
            lblError = new Label
            {
                Text = "",
                ForeColor = Color.Red,
                Size = new Size(300, 40),
                Location = new Point(50, 400),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            
            // Bouton de connexion
            btnLogin = new Button
            {
                Text = "Se connecter",
                Size = new Size(140, 35),
                Location = new Point(50, 445),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnLogin.Click += BtnLogin_Click;
            
            // Bouton d'annulation
            btnCancel = new Button
            {
                Text = "Annuler",
                Size = new Size(140, 35),
                Location = new Point(210, 445),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.Click += BtnCancel_Click;
            
            // Ajouter les contrôles au formulaire
            this.Controls.AddRange(new Control[]
            {
                picLogo, lblTitle, lblSubTitle,
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                lblError,
                btnLogin, btnCancel
            });
        }
        
        /// <summary>
        /// Gérer le clic sur le bouton de connexion
        /// </summary>
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // Cacher le message d'erreur
            lblError.Visible = false;
            
            // Vérifier si les champs sont remplis
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblError.Text = "Veuillez remplir tous les champs";
                lblError.Visible = true;
                return;
            }
            
            // Désactiver les contrôles pendant l'authentification
            SetControlsEnabled(false);
            
            try
            {
                // Tenter l'authentification
                if (AuthenticationManager.Login(txtUsername.Text, txtPassword.Text))
                {
                    // Authentification réussie
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // Échec de l'authentification
                    lblError.Text = "Nom d'utilisateur ou mot de passe incorrect";
                    lblError.Visible = true;
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = $"Erreur: {ex.Message}";
                lblError.Visible = true;
                LogManager.LogError($"Erreur de connexion: {ex.Message}", ex);
            }
            finally
            {
                // Réactiver les contrôles
                SetControlsEnabled(true);
            }
        }
        
        /// <summary>
        /// Gérer le clic sur le bouton d'annulation
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        /// <summary>
        /// Activer/désactiver les contrôles
        /// </summary>
        private void SetControlsEnabled(bool enabled)
        {
            txtUsername.Enabled = enabled;
            txtPassword.Enabled = enabled;
            btnLogin.Enabled = enabled;
            btnCancel.Enabled = enabled;
        }
    }
}