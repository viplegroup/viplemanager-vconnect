// Viple FilesVersion - LinksForm 1.0.0 - Date 25/06/2025
// Application créée par Viple SAS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using VipleManagement.Core;

namespace VipleManagement.Forms.Links
{
    public partial class LinksForm : Form
    {
        private ListView lvLinks;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnOpen;
        private WebBrowser webPreview;
        private List<LinkItem> links;
        private string linksFilePath = Path.Combine("vipledata", "links.vff");

        public LinksForm()
        {
            InitializeComponent();
            SetupUI();
            LoadLinks();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(900, 600);
            this.Text = "Viple - Liens utiles";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
        }

        private void SetupUI()
        {
            // SplitContainer principal
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 300,
                BackColor = Color.FromArgb(37, 37, 38)
            };

            // Panel gauche : liste des liens
            Panel leftPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(37, 37, 38)
            };

            lvLinks = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            lvLinks.Columns.Add("Nom", 180);
            lvLinks.Columns.Add("Catégorie", 100);
            lvLinks.SelectedIndexChanged += LvLinks_SelectedIndexChanged;
            lvLinks.DoubleClick += LvLinks_DoubleClick;

            // Panel de boutons sous la liste
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(37, 37, 38)
            };

            btnAdd = new Button
            {
                Text = "Ajouter",
                Location = new Point(10, 10),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button
            {
                Text = "Modifier",
                Location = new Point(100, 10),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button
            {
                Text = "Supprimer",
                Location = new Point(190, 10),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(204, 0, 0),
                ForeColor = Color.White
            };
            btnDelete.Click += BtnDelete_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });
            leftPanel.Controls.AddRange(new Control[] { lvLinks, buttonPanel });

            // Panel droit : aperçu et détails du lien
            Panel rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(37, 37, 38),
                Padding = new Padding(10)
            };

            Label lblPreview = new Label
            {
                Text = "Aperçu du lien",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White
            };

            webPreview = new WebBrowser
            {
                Dock = DockStyle.Fill,
                ScriptErrorsSuppressed = true
            };

            btnOpen = new Button
            {
                Text = "Ouvrir dans le navigateur",
                Dock = DockStyle.Bottom,
                Height = 30,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnOpen.Click += BtnOpen_Click;

            rightPanel.Controls.AddRange(new Control[] { lblPreview, webPreview, btnOpen });

            // Assemblage final
            splitContainer.Panel1.Controls.Add(leftPanel);
            splitContainer.Panel2.Controls.Add(rightPanel);
            this.Controls.Add(splitContainer);
        }

        private void LoadLinks()
        {
            links = new List<LinkItem>();
            
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(linksFilePath));
                
                if (File.Exists(linksFilePath))
                {
                    using (StreamReader reader = new StreamReader(linksFilePath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<LinkItem>));
                        links = (List<LinkItem>)serializer.Deserialize(reader);
                    }
                }
                else
                {
                    // Créer des liens par défaut
                    links.Add(new LinkItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Site Viple",
                        Url = "https://www.viple.fr",
                        Category = "Entreprise",
                        Description = "Site officiel de Viple SAS"
                    });

                    links.Add(new LinkItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Documentation technique",
                        Url = "https://docs.viple.fr",
                        Category = "Support",
                        Description = "Documentation technique des services Viple"
                    });

                    links.Add(new LinkItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Portail client",
                        Url = "https://client.viple.fr",
                        Category = "Clientèle",
                        Description = "Portail d'accès pour les clients Viple"
                    });

                    SaveLinks();
                }

                DisplayLinks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des liens: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveLinks()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(linksFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<LinkItem>));
                    serializer.Serialize(writer, links);
                }

                UpdateFilesLog("links.vff", "modifiée");
                LogManager.LogAction("Liens enregistrés");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement des liens: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateFilesLog(string fileName, string action)
        {
            try
            {
                string logFilePath = "elioslogs-files.txt";
                string logEntry = $"- Fichier : {fileName} {action} le {DateTime.Now:dd/MM/yyyy} à {DateTime.Now:HH:mm}";
                
                if (File.Exists(logFilePath))
                {
                    string[] lines = File.ReadAllLines(logFilePath);
                    bool modifiedSection = false;
                    List<string> newLines = new List<string>();
                    
                    foreach (string line in lines)
                    {
                        newLines.Add(line);
                        if (line.Contains("#Fichiers modifiés") && action == "modifiée")
                        {
                            newLines.Add(logEntry);
                            modifiedSection = true;
                        }
                        else if (line.Contains("#Fichiers créés") && action == "créé")
                        {
                            newLines.Add(logEntry);
                            modifiedSection = true;
                        }
                    }
                    
                    if (!modifiedSection)
                    {
                        newLines.Add(action == "modifiée" ? "#Fichiers modifiés" : "#Fichiers créés");
                        newLines.Add(logEntry);
                    }
                    
                    File.WriteAllLines(logFilePath, newLines);
                }
            }
            catch (Exception)
            {
                // Ignorer les erreurs de journalisation
            }
        }

        private void DisplayLinks()
        {
            lvLinks.Items.Clear();
            
            foreach (var link in links)
            {
                ListViewItem item = new ListViewItem(link.Name);
                item.SubItems.Add(link.Category);
                item.Tag = link;
                lvLinks.Items.Add(item);
            }

            // Désactiver les boutons si aucun lien n'est sélectionné
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnOpen.Enabled = false;
            
            // Effacer la prévisualisation
            webPreview.DocumentText = "<html><body style='background-color:#1E1E1E; color:white; font-family:Segoe UI; text-align:center; padding-top:50px;'><h2>Sélectionnez un lien pour afficher un aperçu</h2></body></html>";
        }

        private void LvLinks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvLinks.SelectedItems.Count > 0)
            {
                LinkItem link = (LinkItem)lvLinks.SelectedItems[0].Tag;
                
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
                btnOpen.Enabled = true;
                
                // Afficher l'aperçu du lien
                try
                {
                    webPreview.Navigate(link.Url);
                }
                catch (Exception)
                {
                    webPreview.DocumentText = "<html><body style='background-color:#1E1E1E; color:white; font-family:Segoe UI; text-align:center; padding-top:50px;'><h2>Impossible d'afficher l'aperçu de ce lien</h2></body></html>";
                }
            }
            else
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
                btnOpen.Enabled = false;
                
                // Effacer la prévisualisation
                webPreview.DocumentText = "<html><body style='background-color:#1E1E1E; color:white; font-family:Segoe UI; text-align:center; padding-top:50px;'><h2>Sélectionnez un lien pour afficher un aperçu</h2></body></html>";
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            LinkEditForm form = new LinkEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                links.Add(form.Link);
                SaveLinks();
                DisplayLinks();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (lvLinks.SelectedItems.Count > 0)
            {
                LinkItem link = (LinkItem)lvLinks.SelectedItems[0].Tag;
                LinkEditForm form = new LinkEditForm(link);
                
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Retrouver l'index du lien et le remplacer
                    for (int i = 0; i < links.Count; i++)
                    {
                        if (links[i].Id == form.Link.Id)
                        {
                            links[i] = form.Link;
                            break;
                        }
                    }
                    
                    SaveLinks();
                    DisplayLinks();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lvLinks.SelectedItems.Count > 0)
            {
                LinkItem link = (LinkItem)lvLinks.SelectedItems[0].Tag;
                
                DialogResult result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer le lien '{link.Name}' ?",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                
                if (result == DialogResult.Yes)
                {
                    links.RemoveAll(l => l.Id == link.Id);
                    SaveLinks();
                    DisplayLinks();
                }
            }
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            if (lvLinks.SelectedItems.Count > 0)
            {
                LinkItem link = (LinkItem)lvLinks.SelectedItems[0].Tag;
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = link.Url,
                        UseShellExecute = true
                    });
                    
                    LogManager.LogAction($"Lien ouvert: {link.Name} ({link.Url})");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Impossible d'ouvrir le lien: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LvLinks_DoubleClick(object sender, EventArgs e)
        {
            BtnOpen_Click(sender, e);
        }
    }

    public class LinkItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime LastVisited { get; set; } = DateTime.MinValue;
        public int VisitCount { get; set; } = 0;
    }

    public partial class LinkEditForm : Form
    {
        private TextBox txtName;
        private TextBox txtUrl;
        private ComboBox cmbCategory;
        private TextBox txtDescription;
        
        public LinkItem Link { get; private set; }
        
        public LinkEditForm(LinkItem link = null)
        {
            Link = link != null ? new LinkItem
            {
                Id = link.Id,
                Name = link.Name,
                Url = link.Url,
                Category = link.Category,
                Description = link.Description,
                DateCreated = link.DateCreated,
                LastVisited = link.LastVisited,
                VisitCount = link.VisitCount
            } : new LinkItem
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.Now
            };
            
            InitializeComponent();
            PopulateForm();
        }
        
        private void InitializeComponent()
        {
            this.Size = new Size(400, 300);
            this.Text = Link.Id == Guid.NewGuid().ToString() ? "Viple - Ajouter un lien" : "Viple - Modifier un lien";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            
            // Nom
            Label lblName = new Label
            {
                Text = "Nom:",
                Location = new Point(20, 20),
                Size = new Size(80, 20),
                ForeColor = Color.White
            };
            
            txtName = new TextBox
            {
                Location = new Point(110, 20),
                Size = new Size(250, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // URL
            Label lblUrl = new Label
            {
                Text = "URL:",
                Location = new Point(20, 50),
                Size = new Size(80, 20),
                ForeColor = Color.White
            };
            
            txtUrl = new TextBox
            {
                Location = new Point(110, 50),
                Size = new Size(250, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // Catégorie
            Label lblCategory = new Label
            {
                Text = "Catégorie:",
                Location = new Point(20, 80),
                Size = new Size(80, 20),
                ForeColor = Color.White
            };
            
            cmbCategory = new ComboBox
            {
                Location = new Point(110, 80),
                Size = new Size(250, 20),
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            
            // Catégories prédéfinies
            cmbCategory.Items.AddRange(new object[] { "Entreprise", "Support", "Clientèle", "Technique", "Divers" });
            cmbCategory.SelectedIndex = 0;
            
            // Description
            Label lblDescription = new Label
            {
                Text = "Description:",
                Location = new Point(20, 110),
                Size = new Size(80, 20),
                ForeColor = Color.White
            };
            
            txtDescription = new TextBox
            {
                Location = new Point(110, 110),
                Size = new Size(250, 80),
                Multiline = true,
                BackColor = Color.FromArgb(51, 51, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // Boutons
            Button btnSave = new Button
            {
                Text = "Enregistrer",
                Location = new Point(190, 220),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnSave.Click += BtnSave_Click;
            
            Button btnCancel = new Button
            {
                Text = "Annuler",
                Location = new Point(280, 220),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White
            };
            
            this.Controls.AddRange(new Control[]
            {
                lblName, txtName,
                lblUrl, txtUrl,
                lblCategory, cmbCategory,
                lblDescription, txtDescription,
                btnSave, btnCancel
            });
            
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }
        
        private void PopulateForm()
        {
            txtName.Text = Link.Name;
            txtUrl.Text = Link.Url;
            
            if (!string.IsNullOrEmpty(Link.Category) && cmbCategory.Items.Contains(Link.Category))
            {
                cmbCategory.SelectedItem = Link.Category;
            }
            
            txtDescription.Text = Link.Description;
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                Link.Name = txtName.Text;
                Link.Url = txtUrl.Text;
                Link.Category = cmbCategory.SelectedItem.ToString();
                Link.Description = txtDescription.Text;
                
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.None;
            }
        }
        
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Le nom du lien est obligatoire.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(txtUrl.Text))
            {
                MessageBox.Show("L'URL du lien est obligatoire.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUrl.Focus();
                return false;
            }
            
            if (!Uri.TryCreate(txtUrl.Text, UriKind.Absolute, out Uri uriResult) || 
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                MessageBox.Show("L'URL n'est pas valide. Veuillez entrer une URL complète commençant par http:// ou https://", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUrl.Focus();
                return false;
            }
            
            return true;
        }
    }
}