// Viple FilesVersion - ClientManager 1.0.1 - Date 26/06/2025
// Application créée par Viple SAS

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VipleManagement.Core;
using VipleManagement.Models;

namespace VipleManagement.Services
{
    public class ClientManager
    {
        private static readonly string ClientsFilePath = Path.Combine("vipledata", "clients.vff");
        private List<Client> clients;

        public ClientManager()
        {
            LoadClients();
        }

        public List<Client> GetAllClients()
        {
            return new List<Client>(clients);
        }

        public List<Client> GetActiveClients()
        {
            return clients.Where(c => c.IsActive).ToList();
        }

        public List<Client> GetClientsByType(ClientType clientType)
        {
            return clients.Where(c => c.Type == clientType).ToList();
        }

        public List<Client> GetClientsWithService(string serviceId)
        {
            return clients.Where(c => c.SubscribedServiceIds.Contains(serviceId)).ToList();
        }

        public List<Client> SearchClients(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllClients();

            searchText = searchText.ToLower();
            return clients.Where(c =>
                c.Name.ToLower().Contains(searchText) ||
                c.Email.ToLower().Contains(searchText) ||
                c.Phone.Contains(searchText) ||
                c.ContactPerson.ToLower().Contains(searchText) ||
                c.City.ToLower().Contains(searchText) ||
                c.Notes.ToLower().Contains(searchText)
            ).ToList();
        }

        public Client GetClientById(string clientId)
        {
            return clients.FirstOrDefault(c => c.Id == clientId);
        }

        public bool AddClient(Client client)
        {
            try
            {
                // Vérifier si un client avec le même nom existe déjà
                if (clients.Any(c => c.Name.Equals(client.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }

                // Si l'ID n'est pas défini, en générer un nouveau
                if (string.IsNullOrEmpty(client.Id))
                {
                    client.Id = Guid.NewGuid().ToString();
                }

                clients.Add(client);
                SaveClients();
                LogManager.LogAction($"Client ajouté : {client.Name}");

                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de l'ajout du client : {ex.Message}", ex);
                return false;
            }
        }

        public bool UpdateClient(Client client)
        {
            try
            {
                int index = clients.FindIndex(c => c.Id == client.Id);
                if (index < 0)
                {
                    return false;
                }

                clients[index] = client;
                SaveClients();
                LogManager.LogAction($"Client mis à jour : {client.Name}");

                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la mise à jour du client : {ex.Message}", ex);
                return false;
            }
        }

        public bool DeleteClient(string clientId)
        {
            try
            {
                Client client = GetClientById(clientId);
                if (client == null)
                {
                    return false;
                }

                clients.RemoveAll(c => c.Id == clientId);
                SaveClients();
                LogManager.LogAction($"Client supprimé : {client.Name}");

                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la suppression du client : {ex.Message}", ex);
                return false;
            }
        }

        public async Task<List<Client>> GetClientsWithOutdatedPaymentsAsync()
        {
            return await Task.Run(() => 
            {
                return clients.Where(c => 
                    c.PaymentStatus == PaymentStatus.Late || 
                    c.PaymentStatus == PaymentStatus.VeryLate).ToList();
            });
        }

        private void LoadClients()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ClientsFilePath));

                if (File.Exists(ClientsFilePath))
                {
                    using (StreamReader reader = new StreamReader(ClientsFilePath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Client>));
                        clients = (List<Client>)serializer.Deserialize(reader);
                    }
                }
                else
                {
                    clients = CreateSampleClients();
                    SaveClients();
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors du chargement des clients : {ex.Message}", ex);
                clients = new List<Client>();
            }
        }

        private void SaveClients()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ClientsFilePath));

                using (StreamWriter writer = new StreamWriter(ClientsFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Client>));
                    serializer.Serialize(writer, clients);
                }

                UpdateFilesLog("clients.vff", "modifiée");
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de l'enregistrement des clients : {ex.Message}", ex);
            }
        }

        private List<Client> CreateSampleClients()
        {
            List<Client> sampleClients = new List<Client>
            {
                new Client
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Entreprise ABC",
                    Address = "123 Avenue Principale",
                    City = "Paris",
                    PostalCode = "75001",
                    Email = "contact@abc.fr",
                    Phone = "01 23 45 67 89",
                    ContactPerson = "Jean Dupont",
                    Type = ClientType.Standard,
                    Notes = "Client fidèle depuis 2022",
                    SubscribedServiceIds = new List<string>(),
                    TotalMonthlyFees = 120.00m,
                    LastInvoiceDate = DateTime.Now.AddMonths(-1),
                    PaymentStatus = PaymentStatus.UpToDate
                },
                new Client
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Mairie de Lyon",
                    Address = "1 Place de la Mairie",
                    City = "Lyon",
                    PostalCode = "69001",
                    Email = "contact@mairie-lyon.fr",
                    Phone = "04 56 78 90 12",
                    ContactPerson = "Marie Martin",
                    Type = ClientType.Government,
                    Notes = "Contrat renouvelable annuellement",
                    SubscribedServiceIds = new List<string>(),
                    TotalMonthlyFees = 450.00m,
                    LastInvoiceDate = DateTime.Now.AddMonths(-2),
                    PaymentStatus = PaymentStatus.PendingPayment
                },
                new Client
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "StartupNext",
                    Address = "42 Rue de l'Innovation",
                    City = "Bordeaux",
                    PostalCode = "33000",
                    Email = "hello@startupnext.io",
                    Phone = "05 67 89 01 23",
                    ContactPerson = "Hugo Tech",
                    Type = ClientType.Premium,
                    Notes = "Startup en forte croissance, sensible aux prix",
                    SubscribedServiceIds = new List<string>(),
                    TotalMonthlyFees = 299.99m,
                    LastInvoiceDate = DateTime.Now.AddDays(-15),
                    PaymentStatus = PaymentStatus.UpToDate
                }
            };

            return sampleClients;
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

        // Méthodes supplémentaires pour la gestion avancée des clients
        
        public List<Client> GetClientsDueForFollowUp()
        {
            return clients.Where(c => c.NextFollowUpDate.HasValue && c.NextFollowUpDate.Value.Date <= DateTime.Today).ToList();
        }
        
        public void UpdateClientPaymentStatus(string clientId, PaymentStatus newStatus)
        {
            Client client = GetClientById(clientId);
            if (client != null)
            {
                client.PaymentStatus = newStatus;
                SaveClients();
                LogManager.LogAction($"Statut de paiement mis à jour pour {client.Name} : {newStatus}");
            }
        }
        
        public void AssignClientToUser(string clientId, string userId)
        {
            Client client = GetClientById(clientId);
            if (client != null)
            {
                client.AssignedToUserId = userId;
                SaveClients();
                LogManager.LogAction($"Client {client.Name} assigné à l'utilisateur {userId}");
            }
        }
        
        public void ScheduleFollowUp(string clientId, DateTime followUpDate)
        {
            Client client = GetClientById(clientId);
            if (client != null)
            {
                client.NextFollowUpDate = followUpDate;
                SaveClients();
                LogManager.LogAction($"Suivi programmé pour {client.Name} le {followUpDate:dd/MM/yyyy}");
            }
        }
        
        public void RecordContactWithClient(string clientId)
        {
            Client client = GetClientById(clientId);
            if (client != null)
            {
                client.LastContactDate = DateTime.Now;
                SaveClients();
                LogManager.LogAction($"Contact enregistré avec {client.Name}");
            }
        }
    }
}