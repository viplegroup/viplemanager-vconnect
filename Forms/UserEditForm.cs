// Viple FilesVersion - UserEditForm 1.0.0 - Date 26/06/2025 02:00
// Application créée par Viple SAS

using System;
using System.Drawing;
using System.Windows.Forms;
using VipleManagement.Core;
using VipleManagement.Models;
using VipleManagement.Services;

namespace VipleManagement.Forms
{
    /// <summary>
    /// Formulaire d'édition d'un utilisateur
    /// </summary>
    public class UserEditForm : Form
    {
        private User user;
        private UserManager userManager;
        private bool isNewUser;
        
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private ComboBox cmbRole;
        private CheckBox chkActive;
        private Button btnSave;
        private Button btnCancel;
        private Label lblPasswordRequirement;
        
        /// <summary>
        /// Constructeur pour nouvel utilisateur
        /// </summary>
        public UserEditForm()
        {
            this.user = new User();
            this.userManager = new UserManager();
            this.isNewUser = true;
            
            InitializeComponent();
        }
        
        /// <summary>
        /// Constructeur pour modifier un utilisateur existant
        /// </summary>
        public UserEditForm(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "L'utilisateur ne peut pas être null");
                
            this.user = new User
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive,
                CreationDate = user.CreationDate,
                LastLogin = user.LastLogin
                // Ne pas copier le mot de passe
            };
            
            this.userManager = new UserManager();
            this.isNewUser = false;
            
            InitializeComponent();
        }
        
        /// <summary>
        /// Initialiser les composants du formulaire
        /// </summary>
        private void InitializeComponent()
        {
            // Configurer le formulaire
            this.Text = isNewUser ? "Viple - Nouvel utilisateur" : $"Viple - Modifier {user.Username}";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            
            // Titre
            Label lblTitle = new Label
            {
                Text = isNewUser ? "Ajouter un utilisateur" : "Modifier un utilisateur",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(460, 30),
                Location = new Point(20, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            // Nom d'utilisateur
            Label lblUsername = new Label
            {
                Text = "Nom d'utilisateur *",
                Size = new Size(150, 20),
                Location = new Point(20, 70),
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            txtUsername = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(170, 70),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9)
            };
            txtUsername.TextChanged += ValidateForm;
            
            // Mot de passe
            Label lblPassword = new Label
            {
                Text = isNewUser ? "Mot de passe *" : "Nouveau mot de passe",
                Size = new Size(150, 20),
                Location = new Point(20, 110),
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            txtPassword = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(170, 110),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9),
                UseSystemPasswordChar = true
            };
            txtPassword.TextChanged += ValidateForm;
            
            // Confirmer mot de passe
            Label lblConfirmPassword = new Label
            {
                Text = "Confirmer mot de passe",
                Size = new Size(150, 20),
                Location = new Point(20, 150),
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            txtConfirmPassword = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(170, 150),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9),
                UseSystemPasswordChar = true
            };
            txtConfirmPassword.TextChanged += ValidateForm;
            
            // Message pour le mot de passe
            lblPasswordRequirement = new Label
            {
                Text = isNewUser ? "Le mot de passe est obligatoire" : "Laissez vide pour ne pas modifier",
                Size = new Size(300, 20),
                Location = new Point(170, 180),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Silver
            };
            
            // Nom complet
            Label lblFullName = new Label
            {
                Text = "Nom complet *",
                Size = new Size(150, 20),
                Location = new Point(20, 210),
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            txtFullName = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(170, 210),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9)
            };
            txtFullName.TextChanged += ValidateForm;
            
            // Email
            Label lblEmail = new Label
            {
                Text = "Email *",
                Size = new Size(150, 20),
                Location = new Point(20, 250),
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            txtEmail = new TextBox
            {
                Size = new Size(300, 25),
                Location = new Point(170, 250),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9)
            };
            txtEmail.TextChanged += ValidateForm;
            
            // Rôle
            Label lblRole = new Label
            {
                Text = "Rôle *",
                Size = new Size(150, 20),
                Location = new Point(20, 290),
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            cmbRole = new ComboBox
            {
                Size = new Size(300, 25),
                Location = new Point(170, 290),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                cmbRole.Items.Add(role);
            }
            cmbRole.SelectedIndex = 0;
            
            // Actif
            chkActive = new CheckBox
            {
                Text = "Compte actif",
                Size = new Size(150, 20),
                Location = new Point(170, 330),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                Checked = true
            };
            
            // Boutons
            btnSave = new Button
            {
                Text = "Enregistrer",
                Size = new Size(100, 30),
                Location = new Point(270, 390),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Enabled = false
            };
            btnSave.Click += BtnSave_Click;
            
            btnCancel = new Button
            {
                Text = "Annuler",
                Size = new Size(100, 30),
                Location = new Point(380, 390),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.Click += (s, e) => this.Close();
            
            // Ajouter les contrôles au formulaire
            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                lblConfirmPassword, txtConfirmPassword,
                lblPasswordRequirement,
                lblFullName, txtFullName,
                lblEmail, txtEmail,
                lblRole, cmbRole,
                chkActive,
                btnSave, btnCancel
            });
            
            // Remplir le formulaire avec les données de l'utilisateur existant
            if (!isNewUser)
            {
                txtUsername.Text = user.Username;
                txtUsername.Enabled = false; // Ne pas autoriser la modification du nom d'utilisateur
                txtFullName.Text = user.FullName;
                txtEmail.Text = user.Email;
                cmbRole.SelectedItem = user.Role;
                chkActive.Checked = user.IsActive;
            }
            
            // Configurer l'acceptation des touches de raccourci
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
            
            // Valider le formulaire initial
            ValidateForm(null, null);
        }
        
        /// <summary>
        /// Valider les données du formulaire
        /// </summary>
        private void ValidateForm(object sender, EventArgs e)
        {
            bool isValid = true;
            
            // Vérifier le nom d'utilisateur
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                isValid = false;
            }
            
            // Vérifier le mot de passe pour un nouvel utilisateur
            if (isNewUser && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                isValid = false;
            }
            
            // Vérifier que les mots de passe correspondent
            if (!string.IsNullOrWhiteSpace(txtPassword.Text) && txtPassword.Text != txtConfirmPassword.Text)
            {
                isValid = false;
            }
            
            // Vérifier le nom complet
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                isValid = false;
            }
            
            // Vérifier l'email
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !IsValidEmail(txtEmail.Text))
            {
                isValid = false;
            }
            
            // Activer/désactiver le bouton d'enregistrement
            btnSave.Enabled = isValid;
        }
        
        /// <summary>
        /// Vérifier si une adresse email est valide
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Gérer le clic sur le bouton d'enregistrement
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Mettre à jour les propriétés de l'utilisateur
                user.Username = txtUsername.Text;
                user.FullName = txtFullName.Text;
                user.Email = txtEmail.Text;
                user.Role = (UserRole)cmbRole.SelectedItem;
                user.IsActive = chkActive.Checked;
                
                bool success;
                
                if (isNewUser)
                {
                    // Ajouter un nouvel utilisateur
                    success = userManager.AddUser(user, txtPassword.Text);
                    if (success)
                    {
                        LogManager.LogAction($"Utilisateur '{user.Username}' créé avec succès");
                    }
                }
                else
                {
                    // Modifier un utilisateur existant
                    success = userManager.UpdateUser(user, txtPassword.Text.Length > 0 ? txtPassword.Text : null);
                    if (success)
                    {
                        LogManager.LogAction($"Utilisateur '{user.Username}' mis à jour avec succès");
                    }
                }
                
                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        $"Une erreur est survenue lors de l'{(isNewUser ? "ajout" : "édition")} de l'utilisateur.",
                        "Erreur",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Une erreur est survenue: {ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                LogManager.LogError($"Erreur lors de l'{(isNewUser ? "ajout" : "édition")} d'un utilisateur", ex);
            }
        }
    }
}