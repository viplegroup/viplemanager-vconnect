// Viple FilesVersion - ServiceEditForm 1.0.1 - Date 26/06/2025
// Application créée par Viple SAS

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VipleManagement.Models;
using VipleManagement.Services;
using VipleManagement.Core;

namespace VipleManagement.Forms.Services
{
    public class ServiceEditForm : Form
    {
        private Service service;
        private ServiceManager serviceManager;
        private ProductManager productManager;
        private bool isNewService;
        
        private TextBox txtName;
        private TextBox txtDescription;
        private ComboBox cmbCategory;
        private NumericUpDown nudMonthlyFee;
        private CheckBox chkActive;
        private CheckBox chkRequiresMonitoring;
        private TextBox txtMonitoringUrl;
        private ListBox lstProducts;
        private Button btnAddProduct;
        private Button btnRemoveProduct;
        private Button btnSave;
        private Button btnCancel;
        
        // Liste temporaire pour stocker les produits associés au service
        private List<Product> associatedProducts = new List<Product>();
        
        public ServiceEditForm(Service service = null)
        {
            serviceManager = new ServiceManager();
            productManager = new ProductManager();
            
            if (service != null)
            {
                // Modifier un service existant
                this.service = new Service
                {
                    Id = service.Id,
                    Name = service.Name,
                    Description = service.Description,
                    Category = service.Category,
                    Status = service.Status,
                    IsActive = service.IsActive,
                    RequiresMonitoring = service.RequiresMonitoring,
                    MonitoringUrl = service.MonitoringUrl,
                    MonthlyFee = service.MonthlyFee,
                    LastChecked = service.LastChecked,
                    LastStatusMessage = service.LastStatusMessage,
                    ProductIds = new List<string>(service.ProductIds ?? new List<string>()),
                    CreationDate = service.CreationDate
                };
                isNewService = false;
            }
            else
            {
                // Créer un nouveau service
                this.service = new Service();
                isNewService = true;
            }
            
            // Charger les produits associés
            LoadAssociatedProducts();
            
            InitializeComponent();
            PopulateForm();
        }
        
        private void LoadAssociatedProducts()
        {
            associatedProducts.Clear();
            
            if (service.ProductIds != null)
            {
                foreach (string productId in service.ProductIds)
                {
                    Product product = productManager.GetProductById(productId);
                    if (product != null)
                    {
                        associatedProducts.Add(product);
                    }
                }
            }
        }
        
        private void InitializeComponent()
        {
            this.Size = new Size(600, 650);
            this.Text = isNewService ? "Viple - Nouveau service" : $"Viple - Modifier {service.Name}";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            
            // Informations générales
            GroupBox grpGeneral = new GroupBox
            {
                Text = "Informations générales",
                Location = new Point(20, 20),
                Size = new Size(540, 200),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(37, 37, 38)
            };
            
            Label lblName = new Label
            {
                Text = "Nom *",
                Location = new Point(20, 30),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };
            
            txtName = new TextBox
            {
                Location = new Point(150, 30),
                Size = new Size(370, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            Label lblCategory = new Label
            {
                Text = "Catégorie *",
                Location = new Point(20, 60),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };
            
            cmbCategory = new ComboBox
            {
                Location = new Point(150, 60),
                Size = new Size(370, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (ServiceCategory category in Enum.GetValues(typeof(ServiceCategory)))
            {
                cmbCategory.Items.Add(category);
            }
            
            Label lblDescription = new Label
            {
                Text = "Description",
                Location = new Point(20, 90),
                Size = new Size(100, 20),
                ForeColor = Color.White
            };
            
            txtDescription = new TextBox
            {
                Location = new Point(150, 90),
                Size = new Size(370, 60),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true
            };
            
            Label lblMonthlyFee = new Label
            {
                Text = "Prix mensuel (€) *",
                Location = new Point(20, 160),
                Size = new Size(120, 20),
                ForeColor = Color.White
            };
            
            nudMonthlyFee = new NumericUpDown
            {
                Location = new Point(150, 160),
                Size = new Size(120, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 2,
                Increment = 0.5m
            };
            nudMonthlyFee.Controls[0].BackColor = Color.FromArgb(51, 51, 55); // Boutons spin
            
            grpGeneral.Controls.AddRange(new Control[] {
                lblName, txtName, 
                lblCategory, cmbCategory,
                lblDescription, txtDescription,
                lblMonthlyFee, nudMonthlyFee
            });
            
            // Paramètres
            GroupBox grpSettings = new GroupBox
            {
                Text = "Paramètres",
                Location = new Point(20, 230),
                Size = new Size(540, 120),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(37, 37, 38)
            };
            
            chkActive = new CheckBox
            {
                Text = "Service actif",
                Location = new Point(20, 30),
                Size = new Size(150, 20),
                BackColor = Color.FromArgb(37, 37, 38),
                ForeColor = Color.White,
                Checked = true
            };
            
            chkRequiresMonitoring = new CheckBox
            {
                Text = "Surveillance requise",
                Location = new Point(20, 60),
                Size = new Size(150, 20),
                BackColor = Color.FromArgb(37, 37, 38),
                ForeColor = Color.White
            };
            chkRequiresMonitoring.CheckedChanged += ChkRequiresMonitoring_CheckedChanged;
            
            Label lblMonitoringUrl = new Label
            {
                Text = "URL de surveillance",
                Location = new Point(180, 60),
                Size = new Size(120, 20),
                ForeColor = Color.White
            };
            
            txtMonitoringUrl = new TextBox
            {
                Location = new Point(300, 60),
                Size = new Size(220, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Enabled = false
            };
            
            grpSettings.Controls.AddRange(new Control[] {
                chkActive, chkRequiresMonitoring, lblMonitoringUrl, txtMonitoringUrl
            });
            
            // Produits associés
            GroupBox grpProducts = new GroupBox
            {
                Text = "Produits associés",
                Location = new Point(20, 360),
                Size = new Size(540, 200),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(37, 37, 38)
            };
            
            lstProducts = new ListBox
            {
                Location = new Point(20, 30),
                Size = new Size(500, 120),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            btnAddProduct = new Button
            {
                Text = "Ajouter produit",
                Location = new Point(20, 160),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddProduct.Click += BtnAddProduct_Click;
            
            btnRemoveProduct = new Button
            {
                Text = "Retirer produit",
                Location = new Point(150, 160),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(204, 51, 51),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnRemoveProduct.Click += BtnRemoveProduct_Click;
            
            grpProducts.Controls.AddRange(new Control[] { lstProducts, btnAddProduct, btnRemoveProduct });
            
            // Boutons de validation
            btnSave = new Button
            {
                Text = "Enregistrer",
                Location = new Point(360, 570),
                Size = new Size(100, 30),
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;
            
            btnCancel = new Button
            {
                Text = "Annuler",
                Location = new Point(470, 570),
                Size = new Size(100, 30),
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += BtnCancel_Click;
            
            mainPanel.Controls.AddRange(new Control[] { 
                grpGeneral, grpSettings, grpProducts,
                btnSave, btnCancel
            });
            
            this.Controls.Add(mainPanel);
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
            
            // Surveiller les sélections pour activer/désactiver les boutons
            lstProducts.SelectedIndexChanged += (s, e) => {
                btnRemoveProduct.Enabled = lstProducts.SelectedIndex >= 0;
            };
        }
        
        private void PopulateForm()
        {
            txtName.Text = service.Name;
            txtDescription.Text = service.Description;
            cmbCategory.SelectedItem = service.Category;
            nudMonthlyFee.Value = service.MonthlyFee;
            chkActive.Checked = service.IsActive;
            chkRequiresMonitoring.Checked = service.RequiresMonitoring;
            txtMonitoringUrl.Text = service.MonitoringUrl;
            txtMonitoringUrl.Enabled = service.RequiresMonitoring;
            
            RefreshProductsList();
        }
        
        private void RefreshProductsList()
        {
            lstProducts.Items.Clear();
            
            if (associatedProducts.Count == 0)
            {
                lstProducts.Items.Add("Aucun produit associé");
                lstProducts.Enabled = false;
                btnRemoveProduct.Enabled = false;
                return;
            }
            
            lstProducts.Enabled = true;
            foreach (Product product in associatedProducts)
            {
                lstProducts.Items.Add($"{product.Name} ({product.Price:C2})");
            }
        }
        
        private void ChkRequiresMonitoring_CheckedChanged(object sender, EventArgs e)
        {
            txtMonitoringUrl.Enabled = chkRequiresMonitoring.Checked;
        }
        
        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            ProductSelectorForm selectorForm = new ProductSelectorForm(associatedProducts);
            if (selectorForm.ShowDialog() == DialogResult.OK && selectorForm.SelectedProduct != null)
            {
                associatedProducts.Add(selectorForm.SelectedProduct);
                RefreshProductsList();
            }
        }
        
        private void BtnRemoveProduct_Click(object sender, EventArgs e)
        {
            int selectedIndex = lstProducts.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < associatedProducts.Count)
            {
                Product productToRemove = associatedProducts[selectedIndex];
                
                DialogResult result = MessageBox.Show(
                    $"Voulez-vous vraiment retirer le produit '{productToRemove.Name}' de ce service ?",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                
                if (result == DialogResult.Yes)
                {
                    associatedProducts.RemoveAt(selectedIndex);
                    RefreshProductsList();
                }
            }
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                this.DialogResult = DialogResult.None;
                return;
            }
            
            // Copier les valeurs du formulaire dans l'objet service
            service.Name = txtName.Text;
            service.Description = txtDescription.Text;
            service.Category = (ServiceCategory)cmbCategory.SelectedItem;
            service.MonthlyFee = nudMonthlyFee.Value;
            service.IsActive = chkActive.Checked;
            service.RequiresMonitoring = chkRequiresMonitoring.Checked;
            service.MonitoringUrl = txtMonitoringUrl.Text;
            
            // Mettre à jour la liste des IDs de produits
            service.ProductIds = associatedProducts.Select(p => p.Id).ToList();
            
            // Sauvegarder le service
            bool success;
            if (isNewService)
            {
                success = serviceManager.AddService(service);
            }
            else
            {
                success = serviceManager.UpdateService(service);
            }
            
            if (success)
            {
                LogManager.LogAction($"Service {(isNewService ? "ajouté" : "mis à jour")} : {service.Name}");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    "Une erreur est survenue lors de l'enregistrement du service.",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                this.DialogResult = DialogResult.None;
            }
        }
        
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        private bool ValidateForm()
        {
            // Valider le nom
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Le nom du service est obligatoire.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }
            
            // Valider la catégorie
            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner une catégorie.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return false;
            }
            
            // Valider l'URL de surveillance si nécessaire
            if (chkRequiresMonitoring.Checked && string.IsNullOrWhiteSpace(txtMonitoringUrl.Text))
            {
                MessageBox.Show("L'URL de surveillance est obligatoire lorsque la surveillance est activée.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMonitoringUrl.Focus();
                return false;
            }
            
            return true;
        }
    }
    
    public class ProductSelectorForm : Form
    {
        private ListView lvProducts;
        private Button btnOk;
        private Button btnCancel;
        private ProductManager productManager;
        private List<Product> excludedProducts;
        
        public Product SelectedProduct { get; private set; }
        
        public ProductSelectorForm(List<Product> excludedProducts)
        {
            this.productManager = new ProductManager();
            this.excludedProducts = excludedProducts ?? new List<Product>();
            this.SelectedProduct = null;
            
            InitializeComponent();
            LoadProducts();
        }
        
        private void InitializeComponent()
        {
            this.Size = new Size(500, 400);
            this.Text = "Viple - Sélection d'un produit";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            
            lvProducts = new ListView
            {
                Location = new Point(20, 20),
                Size = new Size(450, 300),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false
            };
            
            lvProducts.Columns.Add("ID", 0);
            lvProducts.Columns.Add("Nom", 250);
            lvProducts.Columns.Add("Prix", 100);
            lvProducts.Columns.Add("Stock", 80);
            
            btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(290, 330),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnOk.Click += BtnOk_Click;
            
            btnCancel = new Button
            {
                Text = "Annuler",
                DialogResult = DialogResult.Cancel,
                Location = new Point(380, 330),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            
            this.Controls.AddRange(new Control[] { lvProducts, btnOk, btnCancel });
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
            
            lvProducts.ItemSelectionChanged += (s, e) => btnOk.Enabled = lvProducts.SelectedItems.Count > 0;
            lvProducts.DoubleClick += (s, e) => { if (btnOk.Enabled) btnOk.PerformClick(); };
        }
        
        private void LoadProducts()
        {
            lvProducts.Items.Clear();
            
            List<Product> availableProducts = productManager.GetAllProducts();
            
            // Exclure les produits déjà associés
            availableProducts = availableProducts.Where(p => !excludedProducts.Any(ep => ep.Id == p.Id)).ToList();
            
            foreach (Product product in availableProducts)
            {
                ListViewItem item = new ListViewItem(product.Id);
                item.SubItems.Add(product.Name);
                item.SubItems.Add($"{product.Price:C2}");
                item.SubItems.Add(product.StockQuantity.ToString());
                item.Tag = product;
                
                lvProducts.Items.Add(item);
            }
            
            if (lvProducts.Items.Count == 0)
            {
                MessageBox.Show(
                    "Tous les produits sont déjà associés à ce service ou aucun produit n'est disponible.",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
        
        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (lvProducts.SelectedItems.Count > 0)
            {
                SelectedProduct = (Product)lvProducts.SelectedItems[0].Tag;
            }
        }
    }
}