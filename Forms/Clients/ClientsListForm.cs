// Viple FilesVersion - ClientsListForm 1.0.1 - Date 26/06/2025
// Application créée par Viple SAS

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VipleManagement.Core;
using VipleManagement.Models;
using VipleManagement.Services;

namespace VipleManagement.Forms.Clients
{
    public partial class ClientsListForm : Form
    {
        private ClientManager clientManager;
        private DataGridView dgvClients;
        private TextBox txtSearch;
        private ComboBox cmbFilterType;
        private ComboBox cmbFilterPayment;
        private Button btnAddClient;
        private Button btnEditClient;
        private Button btnDeleteClient;
        private Button btnRefresh;
        private Button btnContact;
        private Button btnScheduleFollowUp;
        private Label lblTotalClients;

        public ClientsListForm()
        {
            clientManager = new ClientManager();
            InitializeComponent();
            SetupUI();
            LoadClients();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1100, 700);
            this.Text = "Viple - Gestion des clients";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
        }

        private void SetupUI()
        {
            // Panel supérieur pour les filtres et recherche
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(37, 37, 38)
            };

            Label lblSearch = new Label
            {
                Text = "Recherche:",
                Location = new Point(15, 20),
                AutoSize = true,
                ForeColor = Color.White
            };

            txtSearch = new TextBox
            {
                Location = new Point(90, 17),
                Size = new Size(200, 23),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // Filtres
            Label lblFilterType = new Label
            {
                Text = "Type:",
                Location = new Point(310, 20),
                AutoSize = true,
                ForeColor = Color.White
            };

            cmbFilterType = new ComboBox
            {
                Location = new Point(350, 17),
                Size = new Size(130, 23),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterType.Items.Add("Tous");
            foreach (ClientType type in Enum.GetValues(typeof(ClientType)))
            {
                cmbFilterType.Items.Add(type);
            }
            cmbFilterType.SelectedIndex = 0;
            cmbFilterType.SelectedIndexChanged += CmbFilterType_SelectedIndexChanged;

            Label lblFilterPayment = new Label
            {
                Text = "Paiement:",
                Location = new Point(500, 20),
                AutoSize = true,
                ForeColor = Color.White
            };

            cmbFilterPayment = new ComboBox
            {
                Location = new Point(570, 17),
                Size = new Size(130, 23),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterPayment.Items.Add("Tous");
            foreach (PaymentStatus status in Enum.GetValues(typeof(PaymentStatus)))
            {
                cmbFilterPayment.Items.Add(status);
            }
            cmbFilterPayment.SelectedIndex = 0;
            cmbFilterPayment.SelectedIndexChanged += CmbFilterPayment_SelectedIndexChanged;

            // Bouton de rafraîchissement
            btnRefresh = new Button
            {
                Text = "Actualiser",
                Location = new Point(720, 15),
                Size = new Size(80, 28),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.Click += BtnRefresh_Click;

            topPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch, lblFilterType, cmbFilterType, lblFilterPayment, cmbFilterPayment, btnRefresh });

            // Panel inférieur pour les boutons d'action
            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(37, 37, 38)
            };

            btnAddClient = new Button
            {
                Text = "Ajouter",
                Location = new Point(15, 15),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddClient.Click += BtnAddClient_Click;

            btnEditClient = new Button
            {
                Text = "Modifier",
                Location = new Point(125, 15),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnEditClient.Click += BtnEditClient_Click;

            btnDeleteClient = new Button
            {
                Text = "Supprimer",
                Location = new Point(235, 15),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(204, 51, 51),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnDeleteClient.Click += BtnDeleteClient_Click;

            // Nouveaux boutons d'action
            btnContact = new Button
            {
                Text = "Enregistrer Contact",
                Location = new Point(345, 15),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(100, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnContact.Click += BtnContact_Click;

            btnScheduleFollowUp = new Button
            {
                Text = "Programmer Suivi",
                Location = new Point(485, 15),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(100, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnScheduleFollowUp.Click += BtnScheduleFollowUp_Click;

            // Label pour le nombre total de clients
            lblTotalClients = new Label
            {
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = false,
                Size = new Size(200, 30),
                Location = new Point(bottomPanel.Width - 220, 15),
                ForeColor = Color.White,
                Text = "Clients: 0"
            };

            bottomPanel.Controls.AddRange(new Control[] { 
                btnAddClient, btnEditClient, btnDeleteClient, 
                btnContact, btnScheduleFollowUp,
                lblTotalClients 
            });

            // DataGridView pour la liste des clients
            dgvClients = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(30, 30, 30),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Style du DataGridView
            dgvClients.EnableHeadersVisualStyles = false;
            dgvClients.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvClients.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvClients.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(45, 45, 48);
            dgvClients.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            dgvClients.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvClients.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            dgvClients.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvClients.ColumnHeadersHeight = 35;

            dgvClients.DefaultCellStyle.BackColor = Color.FromArgb(37, 37, 38);
            dgvClients.DefaultCellStyle.ForeColor = Color.White;
            dgvClients.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgvClients.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvClients.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvClients.GridColor = Color.FromArgb(51, 51, 55);
            dgvClients.RowTemplate.Height = 25;

            dgvClients.CellDoubleClick += DgvClients_CellDoubleClick;
            dgvClients.SelectionChanged += DgvClients_SelectionChanged;

            // Définir les colonnes
            ConfigureColumns();

            this.Controls.AddRange(new Control[] { dgvClients, topPanel, bottomPanel });
        }

        private void ConfigureColumns()
        {
            dgvClients.Columns.Clear();
            
            dgvClients.Columns.Add("Id", "ID");
            dgvClients.Columns.Add("Name", "Nom");
            dgvClients.Columns.Add("ContactPerson", "Contact");
            dgvClients.Columns.Add("Phone", "Téléphone");
            dgvClients.Columns.Add("City", "Ville");
            dgvClients.Columns.Add("Type", "Type");
            dgvClients.Columns.Add("Services", "Services");
            dgvClients.Columns.Add("TotalFees", "Total mensuel");
            dgvClients.Columns.Add("PaymentStatus", "Statut paiement");
            dgvClients.Columns.Add("LastContact", "Dernier contact");
            dgvClients.Columns.Add("FollowUp", "Prochain suivi");
            
            dgvClients.Columns["Id"].Visible = false;
            dgvClients.Columns["Name"].Width = 150;
            dgvClients.Columns["ContactPerson"].Width = 120;
            dgvClients.Columns["Phone"].Width = 100;
            dgvClients.Columns["City"].Width = 100;
            dgvClients.Columns["Type"].Width = 80;
            dgvClients.Columns["Services"].Width = 70;
            dgvClients.Columns["TotalFees"].Width = 90;
            dgvClients.Columns["PaymentStatus"].Width = 110;
            dgvClients.Columns["LastContact"].Width = 120;
            dgvClients.Columns["FollowUp"].Width = 120;
        }

        private void LoadClients()
        {
            dgvClients.Rows.Clear();

            // Appliquer les filtres
            string searchText = txtSearch.Text;
            
            List<Client> filteredClients = clientManager.SearchClients(searchText);

            // Filtre par type de client
            if (cmbFilterType.SelectedIndex > 0)
            {
                ClientType selectedType = (ClientType)cmbFilterType.SelectedItem;
                filteredClients = filteredClients.Where(c => c.Type == selectedType).ToList();
            }

            // Filtre par statut de paiement
            if (cmbFilterPayment.SelectedIndex > 0)
            {
                PaymentStatus selectedStatus = (PaymentStatus)cmbFilterPayment.SelectedItem;
                filteredClients = filteredClients.Where(c => c.PaymentStatus == selectedStatus).ToList();
            }

            // Mettre à jour l'affichage
            foreach (Client client in filteredClients)
            {
                int rowIndex = dgvClients.Rows.Add();
                DataGridViewRow row = dgvClients.Rows[rowIndex];

                row.Cells["Id"].Value = client.Id;
                row.Cells["Name"].Value = client.Name;
                row.Cells["ContactPerson"].Value = client.ContactPerson;
                row.Cells["Phone"].Value = client.Phone;
                row.Cells["City"].Value = client.City;
                row.Cells["Type"].Value = client.Type.ToString();
                row.Cells["Services"].Value = client.GetServicesCount();
                row.Cells["TotalFees"].Value = $"{client.TotalMonthlyFees:C2}";
                row.Cells["PaymentStatus"].Value = client.GetPaymentStatusDescription();
                row.Cells["LastContact"].Value = client.LastContactDate.ToString("dd/MM/yyyy");
                row.Cells["FollowUp"].Value = client.NextFollowUpDate.HasValue ? client.NextFollowUpDate.Value.ToString("dd/MM/yyyy") : "-";

                // Appliquer une couleur selon le statut de paiement
                switch (client.PaymentStatus)
                {
                    case PaymentStatus.Late:
                        row.DefaultCellStyle.BackColor = Color.FromArgb(80, 60, 0);
                        break;
                    case PaymentStatus.VeryLate:
                        row.DefaultCellStyle.BackColor = Color.FromArgb(80, 30, 30);
                        break;
                }

                // Mettre en surbrillance les suivis à effectuer aujourd'hui
                if (client.NextFollowUpDate.HasValue && client.NextFollowUpDate.Value.Date == DateTime.Today)
                {
                    row.Cells["FollowUp"].Style.BackColor = Color.FromArgb(0, 80, 0);
                    row.Cells["FollowUp"].Style.Font = new Font(dgvClients.DefaultCellStyle.Font, FontStyle.Bold);
                }
                // Mettre en surbrillance les suivis en retard
                else if (client.NextFollowUpDate.HasValue && client.NextFollowUpDate.Value.Date < DateTime.Today)
                {
                    row.Cells["FollowUp"].Style.BackColor = Color.FromArgb(80, 0, 0);
                    row.Cells["FollowUp"].Style.Font = new Font(dgvClients.DefaultCellStyle.Font, FontStyle.Bold);
                }
            }

            lblTotalClients.Text = $"Clients: {filteredClients.Count}";
            UpdateButtonStates();
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadClients();
        }

        private void CmbFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadClients();
        }

        private void CmbFilterPayment_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadClients();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadClients();
            LogManager.LogAction("Liste des clients rafraîchie");
        }

        private void DgvClients_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = dgvClients.SelectedRows.Count > 0;
            btnEditClient.Enabled = hasSelection;
            btnDeleteClient.Enabled = hasSelection;
            btnContact.Enabled = hasSelection;
            btnScheduleFollowUp.Enabled = hasSelection;
        }

        private void BtnAddClient_Click(object sender, EventArgs e)
        {
            ClientEditForm form = new ClientEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadClients();
                LogManager.LogAction("Nouveau client ajouté");
            }
        }

        private void BtnEditClient_Click(object sender, EventArgs e)
        {
            if (dgvClients.SelectedRows.Count > 0)
            {
                EditSelectedClient();
            }
        }

        private void BtnDeleteClient_Click(object sender, EventArgs e)
        {
            if (dgvClients.SelectedRows.Count > 0)
            {
                DeleteSelectedClient();
            }
        }

        private void BtnContact_Click(object sender, EventArgs e)
        {
            if (dgvClients.SelectedRows.Count > 0)
            {
                string clientId = dgvClients.SelectedRows[0].Cells["Id"].Value?.ToString();
                if (!string.IsNullOrEmpty(clientId))
                {
                    clientManager.RecordContactWithClient(clientId);
                    LoadClients();
                    MessageBox.Show(
                        "Contact enregistré avec succès.",
                        "Contact client",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }

        private void BtnScheduleFollowUp_Click(object sender, EventArgs e)
        {
            if (dgvClients.SelectedRows.Count > 0)
            {
                string clientId = dgvClients.SelectedRows[0].Cells["Id"].Value?.ToString();
                if (!string.IsNullOrEmpty(clientId))
                {
                    using (DateSelectionForm form = new DateSelectionForm())
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            clientManager.ScheduleFollowUp(clientId, form.SelectedDate);
                            LoadClients();
                            LogManager.LogAction($"Suivi programmé pour le client {clientId}");
                        }
                    }
                }
            }
        }

        private void DgvClients_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                EditSelectedClient();
            }
        }

        private void EditSelectedClient()
        {
            string clientId = dgvClients.SelectedRows[0].Cells["Id"].Value?.ToString();
            Client client = clientManager.GetClientById(clientId);

            if (client != null)
            {
                ClientEditForm form = new ClientEditForm(client);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadClients();
                    LogManager.LogAction($"Client modifié : {client.Name}");
                }
            }
        }

        private void DeleteSelectedClient()
        {
            string clientId = dgvClients.SelectedRows[0].Cells["Id"].Value?.ToString();
            string clientName = dgvClients.SelectedRows[0].Cells["Name"].Value?.ToString();

            DialogResult result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer le client '{clientName}' ?\n\nCette action est irréversible.",
                "Confirmation de suppression",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                if (clientManager.DeleteClient(clientId))
                {
                    LoadClients();
                    LogManager.LogAction($"Client supprimé : {clientName}");
                }
                else
                {
                    MessageBox.Show(
                        "Une erreur est survenue lors de la suppression du client.",
                        "Erreur",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
    }

    // Formulaire pour la sélection d'une date
    public class DateSelectionForm : Form
    {
        private DateTimePicker dtpDate;
        private Button btnOk;
        private Button btnCancel;

        public DateTime SelectedDate { get; private set; }

        public DateSelectionForm()
        {
            InitializeComponent();
            SelectedDate = DateTime.Now.AddDays(7); // Date par défaut
            dtpDate.Value = SelectedDate;
        }

        private void InitializeComponent()
        {
            this.Size = new Size(320, 160);
            this.Text = "Viple - Sélection de date";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;

            Label lblPrompt = new Label
            {
                Text = "Sélectionnez la date du prochain suivi :",
                Location = new Point(20, 20),
                Size = new Size(280, 20),
                ForeColor = Color.White
            };

            dtpDate = new DateTimePicker
            {
                Location = new Point(20, 50),
                Size = new Size(280, 25),
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Today,
                Value = DateTime.Today.AddDays(7)
            };

            btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(130, 90),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOk.Click += BtnOk_Click;

            btnCancel = new Button
            {
                Text = "Annuler",
                DialogResult = DialogResult.Cancel,
                Location = new Point(220, 90),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            this.Controls.AddRange(new Control[] { lblPrompt, dtpDate, btnOk, btnCancel });
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            SelectedDate = dtpDate.Value.Date;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}