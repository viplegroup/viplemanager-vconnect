// Viple FilesVersion - ClientEditForm 1.0.2 - Date 26/06/2025
// Application créée par Viple SAS

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using VipleManagement.Core;
using VipleManagement.Models;
using VipleManagement.Services;

namespace VipleManagement.Forms.Clients
{
    public partial class ClientEditForm : Form
    {
        private Client client;
        private ClientManager clientManager;
        private ServiceManager serviceManager;

        private TextBox? txtName;
        private TextBox? txtAddress;
        private TextBox? txtCity;
        private TextBox? txtPostalCode;
        private TextBox? txtEmail;
        private TextBox? txtPhone;
        private TextBox? txtContactPerson;
        private TextBox? txtNotes;
        private ComboBox? cmbType;
        private ComboBox? cmbPaymentStatus;
        private NumericUpDown? nudMonthlyFees;
        private DateTimePicker? dtpNextFollowUp;
        private CheckBox? chkIsActive;
        private Button? btnSave;
        private Button? btnCancel;
        private ListBox? lstServices;
        private Button? btnAddService;
        private Button? btnRemoveService;

        public ClientEditForm(Client? client = null)
        {
            clientManager = new ClientManager();
            serviceManager = new ServiceManager();
            
            // Si un client est fourni, utiliser une copie pour éviter de modifier l'original directement
            if (client != null)
            {
                this.client = new Client
                {
                    Id = client.Id,
                    Name = client.Name,
                    Address = client.Address,
                    City = client.City,
                    PostalCode = client.PostalCode,
                    Email = client.Email,
                    Phone = client.Phone,
                    ContactPerson = client.ContactPerson,
                    Notes = client.Notes,
                    IsActive = client.IsActive,
                    Type = client.Type,
                    CreationDate = client.CreationDate,
                    LastContactDate = client.LastContactDate,
                    NextFollowUpDate = client.NextFollowUpDate,
                    TotalMonthlyFees = client.TotalMonthlyFees,
                    LastInvoiceDate = client.LastInvoiceDate,
                    PaymentStatus = client.PaymentStatus,
                    AssignedToUserId = client.AssignedToUserId,
                    SubscribedServiceIds = new List<string>(client.SubscribedServiceIds)
                };
            }
            else
            {
                this.client = new Client();
            }
            
            InitializeComponent();
            SetupUI();
            PopulateForm();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(700, 600);
            this.Text = string.IsNullOrEmpty(client.Id) ? "Viple - Nouveau client" : $"Viple - Modifier {client.Name}";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
        }

        private void SetupUI()
        {
            Panel leftPanel = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(320, 520),
                BackColor = Color.FromArgb(37, 37, 38),
                Padding = new Padding(10)
            };

            Panel rightPanel = new Panel
            {
                Location = new Point(350, 20),
                Size = new Size(320, 520),
                BackColor = Color.FromArgb(37, 37, 38),
                Padding = new Padding(10)
            };

            // --- Informations générales (panneau de gauche) ---
            Label lblTitle1 = new Label
            {
                Text = "Informations générales",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 10),
                Size = new Size(300, 30)
            };

            // Nom du client
            Label lblName = new Label
            {
                Text = "Nom *",
                Location = new Point(10, 50),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };

            txtName = new TextBox
            {
                Location = new Point(120, 50),
                Size = new Size(180, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Adresse
            Label lblAddress = new Label
            {
                Text = "Adresse",
                Location = new Point(10, 80),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };

            txtAddress = new TextBox
            {
                Location = new Point(120, 80),
                Size = new Size(180, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Ville
            Label lblCity = new Label
            {
                Text = "Ville",
                Location = new Point(10, 110),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };

            txtCity = new TextBox
            {
                Location = new Point(120, 110),
                Size = new Size(180, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Code postal
            Label lblPostalCode = new Label
            {
                Text = "Code postal",
                Location = new Point(10, 140),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };

            txtPostalCode = new TextBox
            {
                Location = new Point(120, 140),
                Size = new Size(180, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Email
            Label lblEmail = new Label
            {
                Text = "Email *",
                Location = new Point(10, 170),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };

            txtEmail = new TextBox
            {
                Location = new Point(120, 170),
                Size = new Size(180, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Téléphone
            Label lblPhone = new Label
            {
                Text = "Téléphone *",
                Location = new Point(10, 200),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };

            txtPhone = new TextBox
            {
                Location = new Point(120, 200),
                Size = new Size(180, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Contact
            Label lblContactPerson = new Label
            {
                Text = "Personne contact",
                Location = new Point(10, 230),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };

            txtContactPerson = new TextBox
            {
                Location = new Point(120, 230),
                Size = new Size(180, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Type de client
            Label lblType = new Label
            {
                Text = "Type",
                Location = new Point(10, 260),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };

            cmbType = new ComboBox
            {
                Location = new Point(120, 260),
                Size = new Size(180, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (ClientType type in Enum.GetValues(typeof(ClientType)))
            {
                cmbType.Items.Add(type);
            }

            // Statut du client
            chkIsActive = new CheckBox
            {
                Text = "Client actif",
                Location = new Point(120, 290),
                Size = new Size(180, 20),
                BackColor = Color.FromArgb(37, 37, 38),
                ForeColor = Color.White,
                Checked = true
            };

            // Notes
            Label lblNotes = new Label
            {
                Text = "Notes",
                Location = new Point(10, 320),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };

            txtNotes = new TextBox
            {
                Location = new Point(10, 350),
                Size = new Size(290, 100),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true
            };

            // --- Informations complémentaires (panneau de droite) ---
            Label lblTitle2 = new Label
            {
                Text = "Informations avancées",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 10),
                Size = new Size(300, 30)
            };

            // Total mensuel
            Label lblMonthlyFees = new Label
            {
                Text = "Total mensuel (€)",
                Location = new Point(10, 50),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };

            nudMonthlyFees = new NumericUpDown
            {
                Location = new Point(120, 50),
                Size = new Size(180, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 2,
                Increment = 10
            };
            nudMonthlyFees.Controls[0].BackColor = Color.FromArgb(51, 51, 55); // Boutons spin

            // Statut de paiement
            Label lblPaymentStatus = new Label
            {
                Text = "Statut paiement",
                Location = new Point(10, 80),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };

            cmbPaymentStatus = new ComboBox
            {
                Location = new Point(120, 80),
                Size = new Size(180, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (PaymentStatus status in Enum.GetValues(typeof(PaymentStatus)))
            {
                cmbPaymentStatus.Items.Add(status);
            }

            // Date du prochain suivi
            Label lblNextFollowUp = new Label
            {
                Text = "Prochain suivi",
                Location = new Point(10, 110),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };

            dtpNextFollowUp = new DateTimePicker
            {
                Location = new Point(120, 110),
                Size = new Size(180, 20),
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Today
            };

            // Services souscrits
            Label lblServices = new Label
            {
                Text = "Services souscrits",
                Location = new Point(10, 140),
                Size = new Size(300, 20),
                ForeColor = Color.White
            };

            lstServices = new ListBox
            {
                Location = new Point(10, 170),
                Size = new Size(300, 200),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Boutons d'ajout/suppression de services
            Panel servicesButtonPanel = new Panel
            {
                Location = new Point(10, 380),
                Size = new Size(300, 40),
                BackColor = Color.FromArgb(37, 37, 38)
            };

            btnAddService = new Button
            {
                Text = "Ajouter service",
                Location = new Point(0, 5),
                Size = new Size(145, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddService.Click += BtnAddService_Click;

            btnRemoveService = new Button
            {
                Text = "Retirer service",
                Location = new Point(155, 5),
                Size = new Size(145, 30),
                BackColor = Color.FromArgb(204, 51, 51),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRemoveService.Click += BtnRemoveService_Click;

            servicesButtonPanel.Controls.AddRange(new Control[] { btnAddService, btnRemoveService });

            // Boutons d'enregistrement et d'annulation
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(45, 45, 48)
            };

            btnSave = new Button
            {
                Text = "Enregistrer",
                Location = new Point(460, 10),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Annuler",
                Location = new Point(570, 10),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.Click += BtnCancel_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnSave, btnCancel });

            // Ajouter les contrôles au panneau de gauche
            leftPanel.Controls.AddRange(new Control[] {
                lblTitle1, lblName, txtName,
                lblAddress, txtAddress,
                lblCity, txtCity,
                lblPostalCode, txtPostalCode,
                lblEmail, txtEmail,
                lblPhone, txtPhone,
                lblContactPerson, txtContactPerson,
                lblType, cmbType,
                chkIsActive,
                lblNotes, txtNotes
            });

            // Ajouter les contrôles au panneau de droite
            rightPanel.Controls.AddRange(new Control[] {
                lblTitle2,
                lblMonthlyFees, nudMonthlyFees,
                lblPaymentStatus, cmbPaymentStatus,
                lblNextFollowUp, dtpNextFollowUp,
                lblServices, lstServices,
                servicesButtonPanel
            });

            this.Controls.AddRange(new Control[] { leftPanel, rightPanel, buttonPanel });
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void PopulateForm()
        {
            if (txtName != null) txtName.Text = client.Name;
            if (txtAddress != null) txtAddress.Text = client.Address;
            if (txtCity != null) txtCity.Text = client.City;
            if (txtPostalCode != null) txtPostalCode.Text = client.PostalCode;
            if (txtEmail != null) txtEmail.Text = client.Email;
            if (txtPhone != null) txtPhone.Text = client.Phone;
            if (txtContactPerson != null) txtContactPerson.Text = client.ContactPerson;
            if (txtNotes != null) txtNotes.Text = client.Notes;
            
            if (cmbType != null) cmbType.SelectedItem = client.Type;
            if (chkIsActive != null) chkIsActive.Checked = client.IsActive;
            
            if (nudMonthlyFees != null) nudMonthlyFees.Value = client.TotalMonthlyFees;
            if (cmbPaymentStatus != null) cmbPaymentStatus.SelectedItem = client.PaymentStatus;
            
            if (client.NextFollowUpDate.HasValue && dtpNextFollowUp != null)
            {
                dtpNextFollowUp.Value = client.NextFollowUpDate.Value;
            }
            else if (dtpNextFollowUp != null)
            {
                dtpNextFollowUp.Value = DateTime.Now.AddDays(30);
            }
            
            RefreshServicesList();
        }

        private void RefreshServicesList()
        {
            if (lstServices == null || btnRemoveService == null) return;
            
            lstServices.Items.Clear();
            
            foreach (string serviceId in client.SubscribedServiceIds)
            {
                var service = serviceManager.GetServiceById(serviceId);
                if (service != null)
                {
                    lstServices.Items.Add($"{service.Name} ({service.MonthlyFee:C2}/mois)");
                }
            }
            
            // Désactiver le bouton de suppression si aucun service n'est sélectionné
            btnRemoveService.Enabled = lstServices.SelectedIndex >= 0;
        }

        private void BtnAddService_Click(object? sender, EventArgs e)
        {
            using (ServiceSelectorForm form = new ServiceSelectorForm(client.SubscribedServiceIds))
            {
                if (form.ShowDialog() == DialogResult.OK && form.SelectedServiceId != null)
                {
                    if (!client.SubscribedServiceIds.Contains(form.SelectedServiceId))
                    {
                        client.SubscribedServiceIds.Add(form.SelectedServiceId);
                        RefreshServicesList();
                        
                        // Mettre à jour le total mensuel
                        var service = serviceManager.GetServiceById(form.SelectedServiceId);
                        if (service != null && nudMonthlyFees != null)
                        {
                            nudMonthlyFees.Value += service.MonthlyFee;
                        }
                    }
                }
            }
        }

        private void BtnRemoveService_Click(object? sender, EventArgs e)
        {
            if (lstServices == null || nudMonthlyFees == null || lstServices.SelectedIndex < 0) return;
            
            if (lstServices.SelectedIndex >= 0 && lstServices.SelectedIndex < client.SubscribedServiceIds.Count)
            {
                string serviceId = client.SubscribedServiceIds[lstServices.SelectedIndex];
                var service = serviceManager.GetServiceById(serviceId);
                
                if (service != null)
                {
                    DialogResult result = MessageBox.Show(
                        $"Voulez-vous vraiment retirer le service '{service.Name}' de ce client ?",
                        "Confirmation",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                    
                    if (result == DialogResult.Yes)
                    {
                        client.SubscribedServiceIds.RemoveAt(lstServices.SelectedIndex);
                        RefreshServicesList();
                        
                        // Mettre à jour le total mensuel
                        nudMonthlyFees.Value -= service.MonthlyFee;
                        if (nudMonthlyFees.Value < 0)
                            nudMonthlyFees.Value = 0;
                    }
                }
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (ValidateForm())
            {
                // Copier les données du formulaire dans l'objet client
                if (txtName != null) client.Name = txtName.Text;
                if (txtAddress != null) client.Address = txtAddress.Text;
                if (txtCity != null) client.City = txtCity.Text;
                if (txtPostalCode != null) client.PostalCode = txtPostalCode.Text;
                if (txtEmail != null) client.Email = txtEmail.Text;
                if (txtPhone != null) client.Phone = txtPhone.Text;
                if (txtContactPerson != null) client.ContactPerson = txtContactPerson.Text;
                if (txtNotes != null) client.Notes = txtNotes.Text;
                if (chkIsActive != null) client.IsActive = chkIsActive.Checked;
                if (cmbType != null && cmbType.SelectedItem != null) client.Type = (ClientType)cmbType.SelectedItem;
                if (nudMonthlyFees != null) client.TotalMonthlyFees = nudMonthlyFees.Value;
                if (cmbPaymentStatus != null && cmbPaymentStatus.SelectedItem != null) client.PaymentStatus = (PaymentStatus)cmbPaymentStatus.SelectedItem;
                if (dtpNextFollowUp != null) client.NextFollowUpDate = dtpNextFollowUp.Value;

                // Sauvegarder le client
                bool success;
                if (string.IsNullOrEmpty(client.Id) || client.Id == Guid.Empty.ToString())
                {
                    // Nouveau client
                    client.Id = Guid.NewGuid().ToString();
                    client.CreationDate = DateTime.Now;
                    success = clientManager.AddClient(client);
                }
                else
                {
                    // Client existant
                    success = clientManager.UpdateClient(client);
                }

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        "Une erreur est survenue lors de l'enregistrement du client.",
                        "Erreur",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    this.DialogResult = DialogResult.None;
                }
            }
            else
            {
                this.DialogResult = DialogResult.None;
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateForm()
        {
            // Validation du nom
            if (txtName == null || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Le nom du client est obligatoire.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName?.Focus();
                return false;
            }

            // Validation de l'email
            if (txtEmail == null || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("L'adresse email est obligatoire.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail?.Focus();
                return false;
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("L'adresse email n'est pas valide.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail?.Focus();
                return false;
            }

            // Validation du téléphone
            if (txtPhone == null || string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Le numéro de téléphone est obligatoire.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone?.Focus();
                return false;
            }

            // Validation du type
            if (cmbType == null || cmbType.SelectedIndex < 0)
            {
                MessageBox.Show("Veuillez sélectionner un type de client.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbType?.Focus();
                return false;
            }

            return true;
        }

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
    }

    // Formulaire pour la sélection d'un service
    public class ServiceSelectorForm : Form
    {
        private ListView? lvServices;
        private TextBox? txtSearch;
        private ComboBox? cmbCategory;
        private Button? btnOk;
        private Button? btnCancel;
        private ServiceManager serviceManager;
        private List<string> excludedServiceIds;

        public string? SelectedServiceId { get; private set; }

        public ServiceSelectorForm(List<string> excludedServiceIds)
        {
            serviceManager = new ServiceManager();
            this.excludedServiceIds = excludedServiceIds;
            SelectedServiceId = null;
            
            InitializeComponent();
            LoadServices();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 400);
            this.Text = "Viple - Sélection d'un service";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;

            // Panel supérieur pour la recherche et le filtre
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(37, 37, 38)
            };

            Label lblSearch = new Label
            {
                Text = "Recherche:",
                Location = new Point(15, 15),
                Size = new Size(70, 20),
                ForeColor = Color.White
            };

            txtSearch = new TextBox
            {
                Location = new Point(90, 15),
                Size = new Size(150, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            Label lblCategory = new Label
            {
                Text = "Catégorie:",
                Location = new Point(250, 15),
                Size = new Size(70, 20),
                ForeColor = Color.White
            };

            cmbCategory = new ComboBox
            {
                Location = new Point(325, 15),
                Size = new Size(150, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.Items.Add("Toutes");
            foreach (ServiceCategory category in Enum.GetValues(typeof(ServiceCategory)))
            {
                cmbCategory.Items.Add(category);
            }
            cmbCategory.SelectedIndex = 0;
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;

            topPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch, lblCategory, cmbCategory });

            // Liste des services
            lvServices = new ListView
            {
                Location = new Point(10, 60),
                Size = new Size(470, 280),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                FullRowSelect = true,
                MultiSelect = false,
                View = View.Details
            };
            lvServices.Columns.Add("ID", 0);
            lvServices.Columns.Add("Nom", 200);
            lvServices.Columns.Add("Catégorie", 100);
            lvServices.Columns.Add("Prix mensuel", 100);
            lvServices.DoubleClick += LvServices_DoubleClick;

            // Boutons
            btnOk = new Button
            {
                Text = "OK",
                Location = new Point(310, 350),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                DialogResult = DialogResult.OK
            };
            btnOk.Click += BtnOk_Click;

            btnCancel = new Button
            {
                Text = "Annuler",
                Location = new Point(400, 350),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] { topPanel, lvServices, btnOk, btnCancel });
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            lvServices.SelectedIndexChanged += LvServices_SelectedIndexChanged;
        }

        private void LoadServices()
        {
            if (lvServices == null || cmbCategory == null || txtSearch == null) return;
            
            lvServices.Items.Clear();

            // Filtrer par catégorie et texte de recherche
            List<Service> services = serviceManager.GetAllServices();
            string searchText = txtSearch.Text.ToLower();

            if (cmbCategory.SelectedIndex > 0)
            {
                ServiceCategory selectedCategory = (ServiceCategory)cmbCategory.SelectedItem;
                services = services.Where(s => s.Category == selectedCategory).ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                services = services.Where(s => 
                    s.Name.ToLower().Contains(searchText) || 
                    s.Description.ToLower().Contains(searchText)
                ).ToList();
            }

            // Exclure les services déjà associés au client
            services = services.Where(s => !excludedServiceIds.Contains(s.Id)).ToList();

            foreach (var service in services)
            {
                ListViewItem item = new ListViewItem(service.Id);
                item.SubItems.Add(service.Name);
                item.SubItems.Add(service.Category.ToString());
                item.SubItems.Add(service.MonthlyFee.ToString("C2"));
                item.Tag = service;
                
                lvServices.Items.Add(item);
            }
            
            // Mettre à jour l'état du bouton OK
            UpdateOkButtonState();
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            LoadServices();
        }

        private void CmbCategory_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LoadServices();
        }

        private void LvServices_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateOkButtonState();
        }

        private void UpdateOkButtonState()
        {
            if (btnOk == null || lvServices == null) return;
            btnOk.Enabled = lvServices.SelectedItems.Count > 0;
        }

        private void LvServices_DoubleClick(object? sender, EventArgs e)
        {
            if (btnOk != null && btnOk.Enabled)
            {
                btnOk.PerformClick();
            }
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (lvServices == null || lvServices.SelectedItems.Count == 0) return;
            
            ListViewItem selectedItem = lvServices.SelectedItems[0];
            SelectedServiceId = selectedItem.Text; // ID du service
            
            this.DialogResult = DialogResult.OK;
        }
    }
}