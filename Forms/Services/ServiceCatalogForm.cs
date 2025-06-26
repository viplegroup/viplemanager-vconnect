// Viple FilesVersion - ServiceCatalogForm 1.0.2 - Date 26/06/2025 01:31
// Application créée par Viple SAS

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VipleManagement.Models;
using VipleManagement.Services;
using VipleManagement.Core;

namespace VipleManagement.Forms.Services
{
    public class ServiceCatalogForm : Form
    {
        private ServiceManager serviceManager;
        private ProductManager productManager;
        
        private ListView lvServices;
        private ComboBox cmbCategory;
        private Button btnAddService;
        private Button btnEditService;
        private Button btnDeleteService;
        private Button btnDetails;
        
        public ServiceCatalogForm()
        {
            serviceManager = new ServiceManager();
            productManager = new ProductManager();
            
            InitializeComponent();
            LoadServices();
        }
        
        private void InitializeComponent()
        {
            this.Size = new Size(900, 600);
            this.Text = "Viple - Catalogue des services";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            
            // Panel de filtres et d'actions
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(37, 37, 38)
            };
            
            Label lblCategory = new Label
            {
                Text = "Filtrer par catégorie:",
                Location = new Point(20, 20),
                Size = new Size(130, 20),
                ForeColor = Color.White
            };
            
            cmbCategory = new ComboBox
            {
                Location = new Point(150, 19),
                Size = new Size(200, 25),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.Items.Add("Toutes les catégories");
            foreach (ServiceCategory category in Enum.GetValues(typeof(ServiceCategory)))
            {
                cmbCategory.Items.Add(category);
            }
            cmbCategory.SelectedIndex = 0;
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;
            
            btnAddService = new Button
            {
                Text = "Ajouter service",
                Location = new Point(570, 15),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddService.Click += BtnAddService_Click;
            
            btnEditService = new Button
            {
                Text = "Modifier",
                Location = new Point(680, 15),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnEditService.Click += BtnEditService_Click;
            
            btnDeleteService = new Button
            {
                Text = "Supprimer",
                Location = new Point(770, 15),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(204, 51, 51),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnDeleteService.Click += BtnDeleteService_Click;
            
            topPanel.Controls.AddRange(new Control[] { lblCategory, cmbCategory, btnAddService, btnEditService, btnDeleteService });
            
            // Liste des services
            lvServices = new ListView
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                FullRowSelect = true,
                MultiSelect = false,
                View = View.Details,
                HideSelection = false
            };
            lvServices.Columns.Add("ID", 0);
            lvServices.Columns.Add("Nom", 200);
            lvServices.Columns.Add("Catégorie", 120);
            lvServices.Columns.Add("Statut", 120);
            lvServices.Columns.Add("Date de création", 120);
            lvServices.Columns.Add("Prix mensuel", 100);
            lvServices.Columns.Add("Produits associés", 120);
            
            lvServices.SelectedIndexChanged += LvServices_SelectedIndexChanged;
            lvServices.DoubleClick += LvServices_DoubleClick;
            
            // Panel inférieur pour les actions supplémentaires
            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(37, 37, 38)
            };
            
            btnDetails = new Button
            {
                Text = "Détails",
                Location = new Point(790, 10),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnDetails.Click += BtnDetails_Click;
            
            bottomPanel.Controls.Add(btnDetails);
            
            this.Controls.AddRange(new Control[] { topPanel, lvServices, bottomPanel });
        }
        
        private void LoadServices()
        {
            lvServices.Items.Clear();
            
            List<Service> services;
            if (cmbCategory.SelectedIndex == 0) // "Toutes les catégories"
            {
                services = serviceManager.GetAllServices();
            }
            else
            {
                ServiceCategory selectedCategory = (ServiceCategory)cmbCategory.SelectedItem;
                services = serviceManager.GetServicesByCategory(selectedCategory);
            }
            
            foreach (Service service in services)
            {
                ListViewItem item = new ListViewItem(service.Id);
                item.SubItems.Add(service.Name);
                item.SubItems.Add(service.Category.ToString());
                item.SubItems.Add(GetStatusText(service.Status));
                item.SubItems.Add(service.CreationDate.ToString("dd/MM/yyyy"));
                item.SubItems.Add(service.MonthlyFee.ToString("C2"));
                item.SubItems.Add(GetAssociatedProductsCount(service));
                item.Tag = service;
                
                // Colorier selon le statut
                item.ForeColor = GetStatusColor(service.Status);
                
                lvServices.Items.Add(item);
            }
            
            // Redimensionner les colonnes
            lvServices.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            
            // Mettre à jour l'état des boutons
            UpdateButtonStates();
        }
        
        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadServices();
        }
        
        private void LvServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }
        
        private void LvServices_DoubleClick(object sender, EventArgs e)
        {
            if (lvServices.SelectedItems.Count > 0)
            {
                Service selectedService = (Service)lvServices.SelectedItems[0].Tag;
                ServiceDetailForm detailForm = new ServiceDetailForm(selectedService.Id);
                detailForm.ShowDialog();
                LoadServices();
            }
        }
        
        private void UpdateButtonStates()
        {
            bool hasSelection = lvServices.SelectedItems.Count > 0;
            btnEditService.Enabled = hasSelection;
            btnDeleteService.Enabled = hasSelection;
            btnDetails.Enabled = hasSelection;
        }
        
        private void BtnAddService_Click(object sender, EventArgs e)
        {
            ServiceEditForm editForm = new ServiceEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadServices();
            }
        }
        
        private void BtnEditService_Click(object sender, EventArgs e)
        {
            if (lvServices.SelectedItems.Count > 0)
            {
                Service selectedService = (Service)lvServices.SelectedItems[0].Tag;
                ServiceEditForm editForm = new ServiceEditForm(selectedService);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadServices();
                }
            }
        }
        
        private void BtnDeleteService_Click(object sender, EventArgs e)
        {
            if (lvServices.SelectedItems.Count > 0)
            {
                Service selectedService = (Service)lvServices.SelectedItems[0].Tag;
                
                // Vérifier si des clients sont abonnés à ce service
                List<Client> subscribedClients = serviceManager.GetClientsWithService(selectedService.Id);
                if (subscribedClients.Count > 0)
                {
                    MessageBox.Show(
                        $"Impossible de supprimer ce service car {subscribedClients.Count} client(s) y sont abonnés.\n\n" +
                        "Veuillez d'abord désabonner ces clients du service.",
                        "Suppression impossible",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
                
                DialogResult result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer le service '{selectedService.Name}' ?\n\n" +
                    "Cette action est irréversible.",
                    "Confirmation de suppression",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                
                if (result == DialogResult.Yes)
                {
                    bool success = serviceManager.DeleteService(selectedService.Id);
                    if (success)
                    {
                        LoadServices();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Une erreur est survenue lors de la suppression du service.",
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }
        
        private void BtnDetails_Click(object sender, EventArgs e)
        {
            if (lvServices.SelectedItems.Count > 0)
            {
                Service selectedService = (Service)lvServices.SelectedItems[0].Tag;
                ServiceDetailForm detailForm = new ServiceDetailForm(selectedService.Id);
                detailForm.ShowDialog();
                LoadServices();
            }
        }
        
        private string GetStatusText(ServiceStatus status)
        {
            return status switch
            {
                ServiceStatus.Running => "En fonctionnement",
                ServiceStatus.Warning => "Avertissement",
                ServiceStatus.Error => "Erreur",
                ServiceStatus.Maintenance => "En maintenance",
                ServiceStatus.Inactive => "Inactif",
                _ => "Inconnu"
            };
        }
        
        private Color GetStatusColor(ServiceStatus status)
        {
            return status switch
            {
                ServiceStatus.Running => Color.LightGreen,
                ServiceStatus.Warning => Color.Orange,
                ServiceStatus.Error => Color.Red,
                ServiceStatus.Maintenance => Color.LightBlue,
                ServiceStatus.Inactive => Color.Gray,
                _ => Color.White
            };
        }
        
        private string GetAssociatedProductsCount(Service service)
        {
            if (service.ProductIds == null || service.ProductIds.Count == 0)
                return "0";
            
            return service.ProductIds.Count.ToString();
        }
        
        // Méthode pour afficher les produits associés à un service
        private void ShowAssociatedProducts(Service service)
        {
            List<string> productIds = service.ProductIds ?? new List<string>();
            
            if (productIds.Count == 0)
            {
                MessageBox.Show(
                    "Aucun produit associé à ce service.",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Produits associés au service {service.Name}:");
            sb.AppendLine();
            
            foreach (string productId in productIds)
            {
                Product product = productManager.GetProductById(productId);
                if (product != null)
                {
                    sb.AppendLine($"- {product.Name} ({product.Price:C2})");
                    if (!string.IsNullOrEmpty(product.Description))
                    {
                        sb.AppendLine($"  {product.Description}");
                    }
                    sb.AppendLine();
                }
            }
            
            MessageBox.Show(
                sb.ToString(),
                "Produits associés",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}