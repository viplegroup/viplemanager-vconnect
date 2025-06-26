// Viple FilesVersion - ServicesStatusForm 1.0.2 - Date 26/06/2025
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
    public partial class ServicesStatusForm : Form
    {
        private ServiceManager serviceManager;
        private System.Windows.Forms.Timer? refreshTimer;
        private FlowLayoutPanel? servicesPanel;
        private Button? btnRefresh;
        private Button? btnCheckAll;
        private ComboBox? cmbCategoryFilter;

        public ServicesStatusForm()
        {
            InitializeComponent();
            serviceManager = new ServiceManager();
            SetupUI();
            LoadServices();
            InitializeRefreshTimer();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1000, 600);
            this.Text = "Viple - État des services";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
        }

        private void SetupUI()
        {
            // Panel de filtres et d'actions
            Panel filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(37, 37, 38)
            };

            Label lblCategory = new Label
            {
                Text = "Catégorie:",
                ForeColor = Color.White,
                Location = new Point(10, 15),
                Size = new Size(70, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbCategoryFilter = new ComboBox
            {
                Location = new Point(85, 14),
                Size = new Size(150, 25),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategoryFilter.Items.Add("Toutes les catégories");
            foreach (ServiceCategory category in Enum.GetValues(typeof(ServiceCategory)))
            {
                cmbCategoryFilter.Items.Add(category);
            }
            cmbCategoryFilter.SelectedIndex = 0;
            cmbCategoryFilter.SelectedIndexChanged += CmbCategoryFilter_SelectedIndexChanged;

            btnRefresh = new Button
            {
                Text = "Actualiser",
                Location = new Point(700, 10),
                Size = new Size(120, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnRefresh.Click += BtnRefresh_Click;

            btnCheckAll = new Button
            {
                Text = "Vérifier tous",
                Location = new Point(830, 10),
                Size = new Size(120, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnCheckAll.Click += BtnCheckAll_Click;

            filterPanel.Controls.AddRange(new Control[] { lblCategory, cmbCategoryFilter, btnRefresh, btnCheckAll });

            // Panel principal de contenus avec scroll
            Panel mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(30, 30, 30)
            };

            // FlowLayoutPanel pour afficher les services
            servicesPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.FromArgb(30, 30, 30),
                Padding = new Padding(10)
            };

            mainContainer.Controls.Add(servicesPanel);
            this.Controls.AddRange(new Control[] { mainContainer, filterPanel });
        }

        private void LoadServices()
        {
            if (servicesPanel == null || cmbCategoryFilter == null) return;
            
            servicesPanel.Controls.Clear();
            
            List<Service> services;
            if (cmbCategoryFilter.SelectedIndex == 0) // "Toutes les catégories"
            {
                services = serviceManager.GetAllServices();
            }
            else
            {
                ServiceCategory selectedCategory = (ServiceCategory)cmbCategoryFilter.SelectedItem;
                services = serviceManager.GetServicesByCategory(selectedCategory);
            }

            foreach (var service in services)
            {
                Panel serviceCard = CreateServiceCard(service);
                servicesPanel.Controls.Add(serviceCard);
            }

            LogManager.LogAction("État des services chargé");
        }

        private Panel CreateServiceCard(Service service)
        {
            Panel card = new Panel
            {
                Size = new Size(300, 180),
                Margin = new Padding(10),
                BackColor = Color.FromArgb(37, 37, 38),
                BorderStyle = BorderStyle.None,
                Tag = service // Stocker le service dans la propriété Tag pour un accès facile
            };

            // Border personnalisée
            card.Paint += (s, e) => {
                var borderColor = GetStatusColor(service.Status);
                using (var pen = new Pen(borderColor, 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                }
            };

            // Titre du service
            Label lblName = new Label
            {
                Text = service.Name,
                Location = new Point(10, 10),
                Size = new Size(280, 25),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            // Catégorie
            Label lblCategory = new Label
            {
                Text = $"Catégorie: {service.Category}",
                Location = new Point(10, 40),
                Size = new Size(280, 20),
                ForeColor = Color.Silver,
                Font = new Font("Segoe UI", 9)
            };

            // Statut
            Panel statusPanel = new Panel
            {
                Location = new Point(10, 65),
                Size = new Size(280, 30),
                BackColor = Color.FromArgb(45, 45, 48)
            };

            Panel statusIndicator = new Panel
            {
                Location = new Point(10, 5),
                Size = new Size(20, 20),
                BackColor = GetStatusColor(service.Status)
            };

            Label lblStatus = new Label
            {
                Text = $"Statut: {GetStatusText(service.Status)}",
                Location = new Point(40, 5),
                Size = new Size(230, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };

            statusPanel.Controls.AddRange(new Control[] { statusIndicator, lblStatus });

            // Dernière vérification
            Label lblLastCheck = new Label
            {
                Text = $"Dernière vérification: {service.LastChecked:dd/MM/yyyy HH:mm:ss}",
                Location = new Point(10, 100),
                Size = new Size(280, 20),
                ForeColor = Color.Silver,
                Font = new Font("Segoe UI", 8)
            };

            // Message de statut
            Label lblStatusMessage = new Label
            {
                Text = service.LastStatusMessage ?? "Aucune information",
                Location = new Point(10, 120),
                Size = new Size(280, 20),
                ForeColor = Color.Silver,
                Font = new Font("Segoe UI", 8)
            };

            // Menu contextuel pour le changement de statut manuel
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Modifier le statut", null, (s, e) => ShowStatusChangeDialog(service.Id, card));
            contextMenu.Items.Add("Vérifier", null, async (s, e) => await CheckService(service.Id, card));
            contextMenu.Items.Add("Détails", null, (s, e) => ShowServiceDetails(service.Id));
            card.ContextMenuStrip = contextMenu;

            // Bouton de vérification
            Button btnCheck = new Button
            {
                Text = "Vérifier",
                Location = new Point(210, 145),
                Size = new Size(80, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Tag = service.Id
            };
            btnCheck.Click += async (s, e) => await CheckService(service.Id, card);

            card.Controls.AddRange(new Control[] { lblName, lblCategory, statusPanel, lblLastCheck, lblStatusMessage, btnCheck });

            return card;
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

        private void ShowStatusChangeDialog(string serviceId, Panel card)
        {
            using (ServiceStatusChangeForm form = new ServiceStatusChangeForm(serviceId, serviceManager))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Le statut a été mis à jour, rafraîchir la carte de service
                    UpdateServiceCard(serviceId, card);
                }
            }
        }

        private void ShowServiceDetails(string serviceId)
        {
            ServiceDetailForm detailForm = new ServiceDetailForm(serviceId);
            detailForm.ShowDialog();
            
            // Rafraîchir les services après la fermeture du formulaire de détails
            // car le statut ou d'autres informations peuvent avoir été modifiés
            LoadServices();
        }

        private async Task CheckService(string serviceId, Panel card)
        {
            try
            {
                // Trouver le bouton de vérification dans la carte
                Button? btnCheck = null;
                foreach (Control control in card.Controls)
                {
                    if (control is Button button && button.Text == "Vérifier")
                    {
                        btnCheck = button;
                        break;
                    }
                }

                if (btnCheck != null)
                {
                    string originalText = btnCheck.Text;
                    btnCheck.Enabled = false;
                    btnCheck.Text = "Vérification...";
                    
                    try
                    {
                        // Effectuer la vérification de service
                        bool status = await serviceManager.CheckServiceStatusAsync(serviceId);
                        
                        // Une fois la vérification terminée, mettre à jour la carte
                        Service service = serviceManager.GetServiceById(serviceId);
                        if (service != null)
                        {
                            UpdateServiceCard(serviceId, card);
                        }
                        
                        LogManager.LogAction($"Service vérifié: {serviceId} - Statut: {status}");
                    }
                    finally
                    {
                        // S'assurer que le bouton est réactivé
                        btnCheck.Enabled = true;
                        btnCheck.Text = originalText;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification du service: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void UpdateServiceCard(string serviceId, Panel card)
        {
            // Obtenir le service à jour
            Service updatedService = serviceManager.GetServiceById(serviceId);
            if (updatedService != null)
            {
                // Mettre à jour le Tag de la carte avec le service mis à jour
                card.Tag = updatedService;
                
                // Mettre à jour l'indicateur de statut
                foreach (Control control in card.Controls)
                {
                    if (control is Panel panel && panel.Controls.Count > 0)
                    {
                        foreach (Control panelControl in panel.Controls)
                        {
                            if (panelControl is Panel statusIndicator && panelControl.Size.Width == 20)
                            {
                                statusIndicator.BackColor = GetStatusColor(updatedService.Status);
                            }
                            else if (panelControl is Label statusLabel && panelControl.Location.X == 40)
                            {
                                statusLabel.Text = $"Statut: {GetStatusText(updatedService.Status)}";
                            }
                        }
                    }
                    else if (control is Label label)
                    {
                        if (control.Location.Y == 100) // Dernière vérification
                        {
                            label.Text = $"Dernière vérification: {updatedService.LastChecked:dd/MM/yyyy HH:mm:ss}";
                        }
                        else if (control.Location.Y == 120) // Message de statut
                        {
                            label.Text = updatedService.LastStatusMessage ?? "Aucune information";
                        }
                    }
                }
                
                // Forcer la redécoration de la carte pour mettre à jour la bordure
                card.Invalidate();
            }
        }

        private void CmbCategoryFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LoadServices();
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadServices();
        }

        private async void BtnCheckAll_Click(object? sender, EventArgs e)
        {
            if (btnCheckAll == null || btnRefresh == null) return;
            
            btnCheckAll.Enabled = false;
            btnCheckAll.Text = "Vérification en cours...";
            btnRefresh.Enabled = false;
            
            try
            {
                await serviceManager.CheckAllServicesStatusAsync();
                LoadServices();
                
                LogManager.LogAction("Tous les services ont été vérifiés");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification des services: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCheckAll.Enabled = true;
                btnCheckAll.Text = "Vérifier tous";
                btnRefresh.Enabled = true;
            }
        }

        private void InitializeRefreshTimer()
        {
            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 5 * 60 * 1000; // 5 minutes
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private async void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            await serviceManager.CheckAllServicesStatusAsync();
            LoadServices();
            LogManager.LogAction("Services vérifiés automatiquement");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (refreshTimer != null)
                {
                    refreshTimer.Stop();
                    refreshTimer.Dispose();
                }
            }
            
            base.Dispose(disposing);
        }
    }
    
    // Formulaire pour modifier manuellement le statut d'un service
    public class ServiceStatusChangeForm : Form
    {
        private ComboBox? cmbStatus;
        private TextBox? txtStatusMessage;
        private Button? btnOk;
        private Button? btnCancel;
        private string serviceId;
        private ServiceManager serviceManager;
        private Service? service;

        public ServiceStatusChangeForm(string serviceId, ServiceManager serviceManager)
        {
            this.serviceId = serviceId;
            this.serviceManager = serviceManager;
            this.service = serviceManager.GetServiceById(serviceId);
            
            if (service == null)
            {
                throw new Exception("Service introuvable");
            }
            
            InitializeComponent();
            PopulateForm();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 250);
            this.Text = $"Viple - Modifier le statut de {service?.Name}";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;

            Label lblPrompt = new Label
            {
                Text = "Sélectionnez le nouveau statut du service :",
                Location = new Point(20, 20),
                Size = new Size(350, 20),
                ForeColor = Color.White
            };

            Label lblStatus = new Label
            {
                Text = "Statut :",
                Location = new Point(20, 50),
                Size = new Size(70, 20),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbStatus = new ComboBox
            {
                Location = new Point(100, 50),
                Size = new Size(260, 25),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (ServiceStatus status in Enum.GetValues(typeof(ServiceStatus)))
            {
                cmbStatus.Items.Add(status);
            }

            Label lblMessage = new Label
            {
                Text = "Message :",
                Location = new Point(20, 80),
                Size = new Size(70, 20),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtStatusMessage = new TextBox
            {
                Location = new Point(100, 80),
                Size = new Size(260, 60),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                Multiline = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(200, 170),
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
                Location = new Point(290, 170),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            this.Controls.AddRange(new Control[] { lblPrompt, lblStatus, cmbStatus, lblMessage, txtStatusMessage, btnOk, btnCancel });
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void PopulateForm()
        {
            if (cmbStatus != null && service != null)
            {
                cmbStatus.SelectedItem = service.Status;
            }
            
            if (txtStatusMessage != null && service != null)
            {
                txtStatusMessage.Text = service.LastStatusMessage;
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbStatus?.SelectedItem == null) return;
                
                ServiceStatus newStatus = (ServiceStatus)cmbStatus.SelectedItem;
                string statusMessage = txtStatusMessage?.Text ?? string.Empty;
                
                // Appeler la méthode qui met à jour le statut et sauvegarde le service
                serviceManager.UpdateServiceStatusManually(serviceId, newStatus, statusMessage);
                
                LogManager.LogAction($"Statut du service {service?.Name} modifié manuellement: {newStatus}");
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour du statut: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
    }
}