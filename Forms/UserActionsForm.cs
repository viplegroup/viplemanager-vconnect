// Viple FilesVersion - UserActionsForm 1.0.0 - Date 26/06/2025 02:00
// Application créée par Viple SAS

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using VipleManagement.Models;
using VipleManagement.Services;

namespace VipleManagement.Forms
{
    /// <summary>
    /// Formulaire affichant l'historique des actions d'un utilisateur
    /// </summary>
    public class UserActionsForm : Form
    {
        private string userId;
        private string username;
        private UserManager userManager;
        private DataGridView dgvActions;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnApplyFilter;
        private Button btnReset;
        
        /// <summary>
        /// Constructeur
        /// </summary>
        public UserActionsForm(string userId, string username)
        {
            this.userId = userId;
            this.username = username;
            this.userManager = new UserManager();
            
            InitializeComponent();
            LoadActions();
        }
        
        /// <summary>
        /// Initialiser les composants du formulaire
        /// </summary>
        private void InitializeComponent()
        {
            // Configurer le formulaire
            this.Text = $"Viple - Historique des actions de {username}";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            
            // Panel de filtres
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(37, 37, 38)
            };
            
            Label lblStartDate = new Label
            {
                Text = "Date début:",
                Location = new Point(20, 20),
                Size = new Size(80, 20),
                ForeColor = Color.White
            };
            
            dtpStartDate = new DateTimePicker
            {
                Location = new Point(100, 19),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddMonths(-1)
            };
            
            Label lblEndDate = new Label
            {
                Text = "Date fin:",
                Location = new Point(270, 20),
                Size = new Size(70, 20),
                ForeColor = Color.White
            };
            
            dtpEndDate = new DateTimePicker
            {
                Location = new Point(340, 19),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            
            btnApplyFilter = new Button
            {
                Text = "Appliquer",
                Location = new Point(510, 15),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnApplyFilter.Click += (s, e) => LoadActions();
            
            btnReset = new Button
            {
                Text = "Réinitialiser",
                Location = new Point(620, 15),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White
            };
            btnReset.Click += (s, e) => 
            {
                dtpStartDate.Value = DateTime.Now.AddMonths(-1);
                dtpEndDate.Value = DateTime.Now;
                LoadActions();
            };
            
            topPanel.Controls.AddRange(new Control[] 
            { 
                lblStartDate, dtpStartDate, 
                lblEndDate, dtpEndDate,
                btnApplyFilter, btnReset
            });
            
            // Grille des actions
            dgvActions = new DataGridView
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
            
            dgvActions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvActions.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvActions.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(45, 45, 48);
            dgvActions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvActions.ColumnHeadersHeight = 30;
            dgvActions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            
            dgvActions.DefaultCellStyle.BackColor = Color.FromArgb(37, 37, 38);
            dgvActions.DefaultCellStyle.ForeColor = Color.White;
            dgvActions.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 122, 204);
            dgvActions.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvActions.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            
            dgvActions.Columns.Add("Timestamp", "Date et heure");
            dgvActions.Columns.Add("ActionType", "Type d'action");
            dgvActions.Columns.Add("Description", "Description");
            dgvActions.Columns.Add("EntityType", "Type d'entité");
            dgvActions.Columns.Add("EntityId", "ID d'entité");
            dgvActions.Columns.Add("IpAddress", "Adresse IP");
            
            dgvActions.Columns["Timestamp"].Width = 150;
            dgvActions.Columns["ActionType"].Width = 120;
            dgvActions.Columns["Description"].Width = 250;
            dgvActions.Columns["EntityType"].Width = 100;
            dgvActions.Columns["EntityId"].Width = 80;
            dgvActions.Columns["IpAddress"].Width = 100;
            
            // Ajouter les contrôles au formulaire
            this.Controls.AddRange(new Control[] { topPanel, dgvActions });
            
            // Bouton de fermeture
            Button btnClose = new Button
            {
                Text = "Fermer",
                DialogResult = DialogResult.Cancel,
                Location = new Point(660, 520),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White
            };
            btnClose.Click += (s, e) => this.Close();
            
            this.Controls.Add(btnClose);
            this.CancelButton = btnClose;
        }
        
        /// <summary>
        /// Charger les actions de l'utilisateur
        /// </summary>
        private void LoadActions()
        {
            dgvActions.Rows.Clear();
            
            // Récupérer les actions de l'utilisateur
            List<UserAction> actions = userManager.GetUserActions(userId);
            
            // Filtrer par date
            DateTime startDate = dtpStartDate.Value.Date;
            DateTime endDate = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1); // Fin de journée
            
            actions = actions.FindAll(a => a.Timestamp >= startDate && a.Timestamp <= endDate);
            
            // Ajouter les actions à la grille
            foreach (UserAction action in actions)
            {
                int rowIndex = dgvActions.Rows.Add(
                    action.Timestamp.ToString("dd/MM/yyyy HH:mm:ss"),
                    action.ActionType,
                    action.Description,
                    action.EntityType,
                    action.EntityId,
                    action.IpAddress
                );
                
                // Colorer la ligne selon le type d'action
                if (action.ActionType.Contains("ERROR") || action.ActionType.Contains("FAIL"))
                {
                    dgvActions.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(80, 30, 30);
                }
                else if (action.ActionType.Contains("LOGIN") || action.ActionType.Contains("LOGOUT"))
                {
                    dgvActions.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(30, 60, 80);
                }
            }
            
            // Afficher un message si aucune action
            if (dgvActions.Rows.Count == 0)
            {
                dgvActions.Rows.Add("Aucune action trouvée dans la période sélectionnée", "", "", "", "", "");
                dgvActions.Rows[0].DefaultCellStyle.ForeColor = Color.Gray;
            }
        }
    }
}