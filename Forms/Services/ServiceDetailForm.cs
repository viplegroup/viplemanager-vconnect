// Viple FilesVersion - ServiceDetailForm 1.0.0 - Date 26/06/2025
// Application créée par Viple SAS

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VipleManagement.Models;
using VipleManagement.Services;
using VipleManagement.Core;

namespace VipleManagement.Forms.Services
{
    public class ServiceDetailForm : Form
    {
        private string serviceId;
        private Service service;
        private ServiceManager serviceManager;
        private ClientManager clientManager;
        private ProductManager productManager;
        
        private Label lblName;
        private Label lblCategory;
        private Label lblDescription;
        private Label lblStatus;
        private Label lblLastCheck;
        private Label lblLastMessage;
        private Label lblMonthlyFee;
        private Panel statusIndicator;
        private Button btnEdit;
        private Button btnCheckStatus;
        private ListBox lstClients;
        private ListBox lstProducts;
        
        public ServiceDetailForm(string serviceId)
        {
            this.serviceId = serviceId;
            this.serviceManager = new ServiceManager();
            this.clientManager = new ClientManager();
            this.productManager = new ProductManager();
            
            // Récupérer les informations du service
            this.service = serviceManager.GetServiceById(serviceId);
            if (service == null)
            {
                throw new Exception($"Service avec ID {serviceId} introuvable");
            }
            
            InitializeComponent();
            PopulateForm();
            LoadClients();
            LoadProducts();
        }
        
        private void InitializeComponent()
        {
            this.Size = new Size(800, 600);
            this.Text = $"Viple - Détails du service: {service.Name}";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(37, 37, 38),
                Padding = new Padding(10)
            };
            
            lblName = new Label
            {
                Text = service.Name,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(400, 30),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            statusIndicator = new Panel
            {
                Location = new Point(430, 15),
                Size = new Size(30, 30),
                BackColor = GetStatusColor(service.Status)
            };
            
            lblStatus = new Label
            {
                Text = GetStatusText(service.Status),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = GetStatusColor(service.Status),
                Location = new Point(470, 15),
                Size = new Size(200, 30),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            headerPanel.Controls.AddRange(new Control[] { lblName, statusIndicator, lblStatus });
            
            // Panels des détails principaux
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            
            TableLayoutPanel detailsTable = new TableLayoutPanel
            {
                Location = new Point(20, 20),
                Size = new Size(760, 200),
                ColumnCount = 2,
                RowCount = 5,
                Dock = DockStyle.Top,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            
            detailsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            detailsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));
            
            // Catégorie
            Label lblCategoryTitle = new Label
            {
                Text = "Catégorie:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            lblCategory = new Label
            {
                Text = service.Category.ToString(),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            // Description
            Label lblDescriptionTitle = new Label
            {
                Text = "Description:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            lblDescription = new Label
            {
                Text = service.Description,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            // Dernière vérification
            Label lblLastCheckTitle = new Label
            {
                Text = "Dernière vérification:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            lblLastCheck = new Label
            {
                Text = service.LastChecked.ToString("dd/MM/yyyy HH:mm:ss"),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            // Dernier message
            Label lblLastMessageTitle = new Label
            {
                Text = "Dernier message:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            lblLastMessage = new Label
            {
                Text = service.LastStatusMessage,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            // Prix mensuel
            Label lblMonthlyFeeTitle = new Label
            {
                Text = "Prix mensuel:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            lblMonthlyFee = new Label
            {
                Text = service.MonthlyFee.ToString("C2"),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            // Ajouter les contrôles à la table
            detailsTable.Controls.Add(lblCategoryTitle, 0, 0);
            detailsTable.Controls.Add(lblCategory, 1, 0);
            detailsTable.Controls.Add(lblDescriptionTitle, 0, 1);
            detailsTable.Controls.Add(lblDescription, 1, 1);
            detailsTable.Controls.Add(lblLastCheckTitle, 0, 2);
            detailsTable.Controls.Add(lblLastCheck, 1, 2);
            detailsTable.Controls.Add(lblLastMessageTitle, 0, 3);
            detailsTable.Controls.Add(lblLastMessage, 1, 3);
            detailsTable.Controls.Add(lblMonthlyFeeTitle, 0, 4);
            detailsTable.Controls.Add(lblMonthlyFee, 1, 4);
            
            // Boutons d'action
            Panel actionPanel = new Panel
            {
                Location = new Point(20, 240),
                Size = new Size(760, 40),
                Dock = DockStyle.Top,
                Padding = new Padding(0, 10, 0, 0)
            };
            
            btnEdit = new Button
            {
                Text = "Modifier",
                Location = new Point(0, 0),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEdit.Click += BtnEdit_Click;
            
            btnCheckStatus = new Button
            {
                Text = "Vérifier statut",
                Location = new Point(110, 0),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCheckStatus.Click += BtnCheckStatus_Click;
            
            actionPanel.Controls.AddRange(new Control[] { btnEdit, btnCheckStatus });
            
            // Création des onglets pour les clients et produits
            TabControl tabControl = new TabControl
            {
                Location = new Point(20, 290),
                Size = new Size(760, 250),
                Dock = DockStyle.Fill,
                TabStop = false
            };
            
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += (s, e) => 
            {
                Graphics g = e.Graphics;
                Rectangle r = tabControl.GetTabRect(e.Index);
                
                Brush textBrush = new SolidBrush(Color.White);
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                
                g.FillRectangle(new SolidBrush(Color.FromArgb(45, 45, 48)), r);
                
                if (e.Index == tabControl.SelectedIndex)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(0, 122, 204)), r);
                }
                
                g.DrawString(tabControl.TabPages[e.Index].Text, tabControl.Font, textBrush, r, sf);
            };
            
            // Onglet Clients
            TabPage clientsTab = new TabPage("Clients abonnés");
            clientsTab.BackColor = Color.FromArgb(37, 37, 38);
            
            lstClients = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };
            
            clientsTab.Controls.Add(lstClients);
            
            // Onglet Produits
            TabPage productsTab = new TabPage("Produits associés");
            productsTab.BackColor = Color.FromArgb(37, 37, 38);
            
            lstProducts = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };
            
            productsTab.Controls.Add(lstProducts);
            
            tabControl.TabPages.Add(clientsTab);
            tabControl.TabPages.Add(productsTab);
            
            mainPanel.Controls.AddRange(new Control[] { detailsTable, actionPanel, tabControl });
            
            this.Controls.AddRange(new Control[] { headerPanel, mainPanel });
        }
        
        private void PopulateForm()
        {
            lblName.Text = service.Name;
            lblCategory.Text = service.Category.ToString();
            lblDescription.Text = service.Description;
            lblStatus.Text = GetStatusText(service.Status);
            lblLastCheck.Text = service.LastChecked.ToString("dd/MM/yyyy HH:mm:ss");
            lblLastMessage.Text = service.LastStatusMessage ?? "Aucune information";
            lblMonthlyFee.Text = service.MonthlyFee.ToString("C2");
            
            statusIndicator.BackColor = GetStatusColor(service.Status);
            lblStatus.ForeColor = GetStatusColor(service.Status);
        }
        
        private void LoadClients()
        {
            // Récupérer tous les clients qui utilisent ce service
            List<Client> clients = clientManager.GetClientsWithService(serviceId);
            lstClients.Items.Clear();
            
            if (clients.Count == 0)
            {
                lstClients.Items.Add("Aucun client abonné à ce service");
                return;
            }
            
            foreach (var client in clients)
            {
                lstClients.Items.Add($"{client.Name} ({client.Email})");
            }
        }
        
        private void LoadProducts()
        {
            lstProducts.Items.Clear();
            
            List<string> productIds = service.ProductIds ?? new List<string>();
            
            if (productIds.Count == 0)
            {
                lstProducts.Items.Add("Aucun produit associé à ce service");
                return;
            }
            
            foreach (string productId in productIds)
            {
                Product product = productManager.GetProductById(productId);
                if (product != null)
                {
                    lstProducts.Items.Add($"{product.Name} ({product.Price:C2})");
                }
            }
        }
        
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            ServiceEditForm editForm = new ServiceEditForm(service);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // Recharger le service après modification
                service = serviceManager.GetServiceById(serviceId);
                if (service != null)
                {
                    PopulateForm();
                    LoadClients();
                    LoadProducts();
                }
            }
        }
        
        private async void BtnCheckStatus_Click(object sender, EventArgs e)
        {
            btnCheckStatus.Enabled = false;
            btnCheckStatus.Text = "Vérification...";
            
            try
            {
                await serviceManager.CheckServiceStatusAsync(serviceId);
                
                // Recharger les informations du service
                service = serviceManager.GetServiceById(serviceId);
                if (service != null)
                {
                    PopulateForm();
                }
                
                MessageBox.Show(
                    $"Statut vérifié: {GetStatusText(service.Status)}\n\nMessage: {service.LastStatusMessage}",
                    "Vérification terminée",
                    MessageBoxButtons.OK,
                    service.Status == ServiceStatus.Running ? MessageBoxIcon.Information : MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de la vérification du statut: {ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnCheckStatus.Enabled = true;
                btnCheckStatus.Text = "Vérifier statut";
            }
        }
        
        private Color GetStatusColor(ServiceStatus status)
        {
            switch (status)
            {
                case ServiceStatus.Running:
                    return Color.FromArgb(0, 170, 0); // Vert
                case ServiceStatus.Warning:
                    return Color.FromArgb(255, 170, 0); // Orange
                case ServiceStatus.Error:
                    return Color.FromArgb(220, 0, 0); // Rouge
                case ServiceStatus.Maintenance:
                    return Color.FromArgb(0, 120, 215); // Bleu
                case ServiceStatus.Inactive:
                    return Color.FromArgb(120, 120, 120); // Gris
                default:
                    return Color.Gray;
            }
        }
        
        private string GetStatusText(ServiceStatus status)
        {
            switch (status)
            {
                case ServiceStatus.Running:
                    return "En fonctionnement";
                case ServiceStatus.Warning:
                    return "Avertissement";
                case ServiceStatus.Error:
                    return "Erreur";
                case ServiceStatus.Maintenance:
                    return "En maintenance";
                case ServiceStatus.Inactive:
                    return "Inactif";
                default:
                    return "Inconnu";
            }
        }
    }
}