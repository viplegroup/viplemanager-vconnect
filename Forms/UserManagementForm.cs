// Viple FilesVersion - UserManagementForm 1.0.1 - Date 26/06/2025 02:00
// Application créée par Viple SAS

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using VipleManagement.Core;
using VipleManagement.Models;
using VipleManagement.Services;

namespace VipleManagement.Forms
{
    /// <summary>
    /// Formulaire de gestion des utilisateurs
    /// </summary>
    public class UserManagementForm : Form
    {
        private UserManager userManager;
        private DataGridView dgvUsers;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnResetPassword;
        private Button btnViewActions;
        private ComboBox cmbRoleFilter;
        private TextBox txtSearch;
        
        /// <summary>
        /// Constructeur
        /// </summary>
        public UserManagementForm()
        {
            userManager = new UserManager();
            
            InitializeComponent();
            LoadUsers();
            
            // S'abonner aux événements du gestionnaire d'utilisateurs
            userManager.UserAdded += (s, u) => LoadUsers();
            userManager.UserUpdated += (s, u) => LoadUsers();
            userManager.UserDeleted += (s, id) => LoadUsers();
        }
        
        /// <summary>
        /// Initialiser les composants du formulaire
        /// </summary>
        private void InitializeComponent()
        {
            // Configurer le formulaire
            this.Text = "Viple - Gestion des utilisateurs";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            
            // Panel de filtres et de recherche
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(37, 37, 38)
            };
            
            Label lblRole = new Label
            {
                Text = "Filtrer par rôle:",
                Location = new Point(20, 20),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };
            
            cmbRoleFilter = new ComboBox
            {
                Location = new Point(120, 19),
                Size = new Size(150, 25),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRoleFilter.Items.Add("Tous les rôles");
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                cmbRoleFilter.Items.Add(role);
            }
            cmbRoleFilter.SelectedIndex = 0;
            cmbRoleFilter.SelectedIndexChanged += (s, e) => LoadUsers();
            
            Label lblSearch = new Label
            {
                Text = "Rechercher:",
                Location = new Point(290, 20),
                Size = new Size(80, 20),
                ForeColor = Color.White
            };
            
            txtSearch = new TextBox
            {
                Location = new Point(370, 19),
                Size = new Size(200, 25),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += (s, e) => LoadUsers();
            
            topPanel.Controls.AddRange(new Control[] { lblRole, cmbRoleFilter, lblSearch, txtSearch });
            
            // Grille des utilisateurs
            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(30, 30, 30),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsers.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(45, 45, 48);
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvUsers.ColumnHeadersHeight = 30;
            dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            
            dgvUsers.DefaultCellStyle.BackColor = Color.FromArgb(37, 37, 38);
            dgvUsers.DefaultCellStyle.ForeColor = Color.White;
            dgvUsers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 122, 204);
            dgvUsers.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvUsers.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            
            dgvUsers.Columns.Add("Id", "ID");
            dgvUsers.Columns.Add("Username", "Nom d'utilisateur");
            dgvUsers.Columns.Add("FullName", "Nom complet");
            dgvUsers.Columns.Add("Email", "Email");
            dgvUsers.Columns.Add("Role", "Rôle");
            dgvUsers.Columns.Add("IsActive", "Actif");
            dgvUsers.Columns.Add("LastLogin", "Dernière connexion");
            
            dgvUsers.Columns["Id"].Visible = false;
            dgvUsers.Columns["Username"].Width = 150;
            dgvUsers.Columns["FullName"].Width = 200;
            dgvUsers.Columns["Email"].Width = 200;
            dgvUsers.Columns["Role"].Width = 100;
            dgvUsers.Columns["IsActive"].Width = 80;
            dgvUsers.Columns["LastLogin"].Width = 150;
            
            dgvUsers.CellDoubleClick += DgvUsers_CellDoubleClick;
            dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;
            
            // Panel des boutons
            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(37, 37, 38)
            };
            
            btnAdd = new Button
            {
                Text = "Ajouter",
                Location = new Point(20, 15),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnAdd.Click += BtnAdd_Click;
            
            btnEdit = new Button
            {
                Text = "Modifier",
                Location = new Point(130, 15),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Enabled = false
            };
            btnEdit.Click += BtnEdit_Click;
            
            btnDelete = new Button
            {
                Text = "Supprimer",
                Location = new Point(240, 15),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(204, 51, 51),
                ForeColor = Color.White,
                Enabled = false
            };
            btnDelete.Click += BtnDelete_Click;
            
            btnResetPassword = new Button
            {
                Text = "Réinitialiser mot de passe",
                Location = new Point(350, 15),
                Size = new Size(180, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Enabled = false
            };
            btnResetPassword.Click += BtnResetPassword_Click;
            
            btnViewActions = new Button
            {
                Text = "Historique des actions",
                Location = new Point(540, 15),
                Size = new Size(150, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Enabled = false
            };
            btnViewActions.Click += BtnViewActions_Click;
            
            bottomPanel.Controls.AddRange(new Control[] 
            { 
                btnAdd, btnEdit, btnDelete, btnResetPassword, btnViewActions 
            });
            
            // Ajouter les contrôles au formulaire
            this.Controls.AddRange(new Control[] { topPanel, dgvUsers, bottomPanel });
        }
        
        /// <summary>
        /// Charger les utilisateurs dans la grille
        /// </summary>
        private void LoadUsers()
        {
            dgvUsers.Rows.Clear();
            
            List<User> users = userManager.GetAllUsers();
            
            // Filtrer par rôle
            if (cmbRoleFilter.SelectedIndex > 0)
            {
                UserRole selectedRole = (UserRole)cmbRoleFilter.SelectedItem;
                users = users.FindAll(u => u.Role == selectedRole);
            }
            
            // Filtrer par recherche
            string searchText = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                users = users.FindAll(u => 
                    u.Username.ToLower().Contains(searchText) ||
                    u.FullName.ToLower().Contains(searchText) ||
                    u.Email.ToLower().Contains(searchText)
                );
            }
            
            // Ajouter les utilisateurs à la grille
            foreach (User user in users)
            {
                int rowIndex = dgvUsers.Rows.Add(
                    user.Id,
                    user.Username,
                    user.FullName,
                    user.Email,
                    user.Role.ToString(),
                    user.IsActive ? "Oui" : "Non",
                    user.LastLogin == DateTime.MinValue ? "Jamais" : user.LastLogin.ToString("dd/MM/yyyy HH:mm:ss")
                );
                
                // Colorer la ligne si l'utilisateur est inactif
                if (!user.IsActive)
                {
                    dgvUsers.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                }
            }
            
            // Désactiver les boutons
            UpdateButtonStates();
        }
        
        /// <summary>
        /// Mettre à jour l'état des boutons en fonction de la sélection
        /// </summary>
        private void UpdateButtonStates()
        {
            bool userSelected = dgvUsers.SelectedRows.Count > 0;
            btnEdit.Enabled = userSelected;
            btnDelete.Enabled = userSelected;
            btnResetPassword.Enabled = userSelected;
            btnViewActions.Enabled = userSelected;
        }
        
        /// <summary>
        /// Gérer le double-clic sur une cellule
        /// </summary>
        private void DgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                EditSelectedUser();
            }
        }
        
        /// <summary>
        /// Gérer le changement de sélection
        /// </summary>
        private void DgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }
        
        /// <summary>
        /// Gérer le clic sur le bouton Ajouter
        /// </summary>
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Vérifier les droits
            if (!AuthenticationManager.CanManageUsers())
            {
                MessageBox.Show(
                    "Vous n'avez pas les droits nécessaires pour ajouter un utilisateur.",
                    "Accès refusé",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            
            // Ouvrir le formulaire d'ajout
            using (UserEditForm form = new UserEditForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // L'utilisateur a été ajouté, recharger la liste
                    LoadUsers();
                }
            }
        }
        
        /// <summary>
        /// Gérer le clic sur le bouton Modifier
        /// </summary>
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            EditSelectedUser();
        }
        
        /// <summary>
        /// Modifier l'utilisateur sélectionné
        /// </summary>
        private void EditSelectedUser()
        {
            // Vérifier les droits
            if (!AuthenticationManager.CanManageUsers())
            {
                MessageBox.Show(
                    "Vous n'avez pas les droits nécessaires pour modifier un utilisateur.",
                    "Accès refusé",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            
            // Vérifier qu'un utilisateur est sélectionné
            if (dgvUsers.SelectedRows.Count == 0)
                return;
                
            string userId = dgvUsers.SelectedRows[0].Cells["Id"].Value.ToString();
            User user = userManager.GetUserById(userId);
            
            if (user != null)
            {
                // Ouvrir le formulaire d'édition
                using (UserEditForm form = new UserEditForm(user))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // L'utilisateur a été modifié, recharger la liste
                        LoadUsers();
                    }
                }
            }
        }
        
        /// <summary>
        /// Gérer le clic sur le bouton Supprimer
        /// </summary>
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            // Vérifier les droits
            if (!AuthenticationManager.CanManageUsers())
            {
                MessageBox.Show(
                    "Vous n'avez pas les droits nécessaires pour supprimer un utilisateur.",
                    "Accès refusé",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            
            // Vérifier qu'un utilisateur est sélectionné
            if (dgvUsers.SelectedRows.Count == 0)
                return;
                
            string userId = dgvUsers.SelectedRows[0].Cells["Id"].Value.ToString();
            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();
            
            // Vérifier qu'il ne s'agit pas de l'utilisateur courant
            if (userId == AuthenticationManager.CurrentUser?.Id)
            {
                MessageBox.Show(
                    "Vous ne pouvez pas supprimer votre propre compte.",
                    "Action impossible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            
            // Demander confirmation
            DialogResult result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer l'utilisateur '{username}' ?\n\nCette action est irréversible.",
                "Confirmation de suppression",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            
            if (result == DialogResult.Yes)
            {
                // Supprimer l'utilisateur
                if (userManager.DeleteUser(userId))
                {
                    // Utilisateur supprimé, recharger la liste
                    LoadUsers();
                }
                else
                {
                    MessageBox.Show(
                        "Une erreur est survenue lors de la suppression de l'utilisateur.\n\nConsultez les logs pour plus d'informations.",
                        "Erreur",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
        
        /// <summary>
        /// Gérer le clic sur le bouton Réinitialiser mot de passe
        /// </summary>
        private void BtnResetPassword_Click(object sender, EventArgs e)
        {
            // Vérifier les droits
            if (!AuthenticationManager.CanManageUsers())
            {
                MessageBox.Show(
                    "Vous n'avez pas les droits nécessaires pour réinitialiser un mot de passe.",
                    "Accès refusé",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            
            // Vérifier qu'un utilisateur est sélectionné
            if (dgvUsers.SelectedRows.Count == 0)
                return;
                
            string userId = dgvUsers.SelectedRows[0].Cells["Id"].Value.ToString();
            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();
            
            // Demander confirmation
            DialogResult result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir réinitialiser le mot de passe de l'utilisateur '{username}' ?\n\nUn nouveau mot de passe sera généré automatiquement.",
                "Confirmation de réinitialisation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            
            if (result == DialogResult.Yes)
            {
                // Réinitialiser le mot de passe
                string newPassword = userManager.ResetPassword(userId);
                
                if (newPassword != null)
                {
                    // Afficher le nouveau mot de passe
                    MessageBox.Show(
                        $"Le mot de passe de l'utilisateur '{username}' a été réinitialisé avec succès.\n\nNouveau mot de passe: {newPassword}\n\nVeuillez communiquer ce nouveau mot de passe à l'utilisateur de façon sécurisée.",
                        "Mot de passe réinitialisé",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Une erreur est survenue lors de la réinitialisation du mot de passe.\n\nConsultez les logs pour plus d'informations.",
                        "Erreur",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
        
        /// <summary>
        /// Gérer le clic sur le bouton Historique des actions
        /// </summary>
        private void BtnViewActions_Click(object sender, EventArgs e)
        {
            // Vérifier qu'un utilisateur est sélectionné
            if (dgvUsers.SelectedRows.Count == 0)
                return;
                
            string userId = dgvUsers.SelectedRows[0].Cells["Id"].Value.ToString();
            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();
            
            // Afficher l'historique des actions
            using (UserActionsForm form = new UserActionsForm(userId, username))
            {
                form.ShowDialog();
            }
        }
    }
}