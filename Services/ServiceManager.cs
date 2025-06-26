// Viple FilesVersion - ServiceManager 1.0.2 - Date 26/06/2025
// Application créée par Viple SAS

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VipleManagement.Core;
using VipleManagement.Models;

namespace VipleManagement.Services
{
    public class ServiceManager
    {
        private static readonly string ServicesFilePath = Path.Combine("vipledata", "services.vff");
        private static readonly HttpClient httpClient = new HttpClient();
        private List<Service> services;

        public ServiceManager()
        {
            LoadServices();
        }

        public List<Service> GetAllServices()
        {
            return new List<Service>(services);
        }

        public List<Service> GetServicesByCategory(ServiceCategory category)
        {
            return services.Where(s => s.Category == category).ToList();
        }

        public List<Service> GetActiveServices()
        {
            return services.Where(s => s.IsActive).ToList();
        }

        public Service GetServiceById(string serviceId)
        {
            return services.FirstOrDefault(s => s.Id == serviceId);
        }

        public bool AddService(Service service)
        {
            try
            {
                if (string.IsNullOrEmpty(service.Id))
                {
                    service.Id = Guid.NewGuid().ToString();
                }
                
                services.Add(service);
                SaveServices();
                LogManager.LogAction($"Service ajouté: {service.Name}");
                
                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de l'ajout du service: {ex.Message}", ex);
                return false;
            }
        }

        public bool UpdateService(Service service)
        {
            try
            {
                int index = services.FindIndex(s => s.Id == service.Id);
                if (index < 0)
                {
                    return false;
                }
                
                services[index] = service;
                SaveServices();
                LogManager.LogAction($"Service mis à jour: {service.Name}");
                
                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la mise à jour du service: {ex.Message}", ex);
                return false;
            }
        }

        public bool DeleteService(string serviceId)
        {
            try
            {
                int index = services.FindIndex(s => s.Id == serviceId);
                if (index < 0)
                {
                    return false;
                }
                
                string serviceName = services[index].Name;
                services.RemoveAt(index);
                SaveServices();
                LogManager.LogAction($"Service supprimé: {serviceName}");
                
                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la suppression du service: {ex.Message}", ex);
                return false;
            }
        }

        public async Task<bool> CheckServiceStatusAsync(string serviceId)
        {
            try
            {
                Service service = GetServiceById(serviceId);
                if (service == null)
                {
                    return false;
                }

                // Vérifier si le service nécessite une surveillance
                if (!service.RequiresMonitoring || string.IsNullOrEmpty(service.MonitoringUrl))
                {
                    // Service sans URL de surveillance - statut manuel à maintenir
                    service.LastChecked = DateTime.Now;
                    service.LastStatusMessage = "Statut mis à jour manuellement";
                    SaveServices(); // Enregistrer pour préserver l'horodatage
                    return true;
                }

                // Simuler une vérification de statut réelle
                bool isUp;
                string statusMessage;
                ServiceStatus newStatus;

                try
                {
                    // Utiliser Task.Run pour rendre le code réellement asynchrone
                    return await Task.Run(async () => {
                        // Simulation d'une requête HTTP vers l'URL de surveillance
                        // Dans une application réelle, nous ferions un appel HTTP réel
                        if (service.MonitoringUrl.Contains("error"))
                        {
                            isUp = false;
                            statusMessage = "Erreur lors de la connexion au service";
                            newStatus = ServiceStatus.Error;
                        }
                        else if (service.MonitoringUrl.Contains("warning"))
                        {
                            isUp = true;
                            statusMessage = "Service accessible mais avec des latences élevées";
                            newStatus = ServiceStatus.Warning;
                        }
                        else if (service.MonitoringUrl.Contains("maintenance"))
                        {
                            isUp = false;
                            statusMessage = "Service en maintenance programmée";
                            newStatus = ServiceStatus.Maintenance;
                        }
                        else if (service.MonitoringUrl.Contains("inactive"))
                        {
                            isUp = false;
                            statusMessage = "Service inactif";
                            newStatus = ServiceStatus.Inactive;
                        }
                        else
                        {
                            // Simuler un scénario aléatoire (90% de temps de service opérationnel)
                            Random random = new Random();
                            isUp = random.Next(100) < 90;
                            if (isUp)
                            {
                                statusMessage = "Service répond normalement";
                                newStatus = ServiceStatus.Running;
                            }
                            else
                            {
                                statusMessage = "Service non-répondant";
                                newStatus = ServiceStatus.Error;
                            }
                        }

                        // Mettre à jour le service avec les nouvelles informations
                        service.LastChecked = DateTime.Now;
                        service.LastStatusMessage = statusMessage;
                        
                        // Ne changer le statut que si le service est réellement surveillé
                        if (service.RequiresMonitoring)
                        {
                            service.Status = newStatus;
                        }
                        
                        SaveServices(); // ICI - Enregistrer les modifications
                        await Task.Delay(500); // Simuler un délai réseau
                        return isUp;
                    });
                }
                catch (Exception ex)
                {
                    service.LastChecked = DateTime.Now;
                    service.LastStatusMessage = $"Erreur lors de la vérification: {ex.Message}";
                    service.Status = ServiceStatus.Error;
                    SaveServices(); // ICI - Enregistrer malgré l'erreur
                    throw;
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la vérification du service {serviceId}: {ex.Message}", ex);
                return false;
            }
        }

        public async Task<bool> CheckAllServicesStatusAsync()
        {
            try
            {
                bool overallSuccess = true;
                foreach (var service in services.Where(s => s.RequiresMonitoring && s.IsActive))
                {
                    try
                    {
                        bool result = await CheckServiceStatusAsync(service.Id);
                        if (!result)
                        {
                            overallSuccess = false;
                        }
                    }
                    catch
                    {
                        overallSuccess = false;
                    }
                }
                
                SaveServices(); // Enregistrer après avoir vérifié tous les services
                return overallSuccess;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la vérification de tous les services: {ex.Message}", ex);
                return false;
            }
        }

        public void UpdateServiceStatusManually(string serviceId, ServiceStatus newStatus, string statusMessage = null)
        {
            try
            {
                Service service = GetServiceById(serviceId);
                if (service != null)
                {
                    service.Status = newStatus;
                    service.LastChecked = DateTime.Now;
                    
                    if (!string.IsNullOrEmpty(statusMessage))
                    {
                        service.LastStatusMessage = statusMessage;
                    }
                    else
                    {
                        service.LastStatusMessage = "Statut mis à jour manuellement";
                    }
                    
                    SaveServices();
                    LogManager.LogAction($"Statut du service {service.Name} mis à jour manuellement: {newStatus}");
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la mise à jour manuelle du statut: {ex.Message}", ex);
            }
        }

        public List<Client> GetClientsWithService(string serviceId)
        {
            try
            {
                ClientManager clientManager = new ClientManager();
                List<Client> allClients = clientManager.GetAllClients();
                
                return allClients.Where(c => c.SubscribedServiceIds != null && 
                                       c.SubscribedServiceIds.Contains(serviceId)).ToList();
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la recherche des clients avec le service {serviceId}: {ex.Message}", ex);
                return new List<Client>();
            }
        }
        
        public async Task<List<Client>> GetClientsWithOutdatedPaymentsAsync()
        {
            // Utiliser Task.Run pour exécuter cette opération de façon asynchrone
            return await Task.Run(() => {
                try
                {
                    ClientManager clientManager = new ClientManager();
                    List<Client> allClients = clientManager.GetAllClients();
                    
                    return allClients.Where(c => 
                        c.PaymentStatus == PaymentStatus.Late || 
                        c.PaymentStatus == PaymentStatus.VeryLate).ToList();
                }
                catch (Exception ex)
                {
                    LogManager.LogError($"Erreur lors de la recherche des clients avec paiements en retard: {ex.Message}", ex);
                    return new List<Client>();
                }
            });
        }

        private void LoadServices()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ServicesFilePath));
                
                if (File.Exists(ServicesFilePath))
                {
                    using (StreamReader reader = new StreamReader(ServicesFilePath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Service>));
                        services = (List<Service>)serializer.Deserialize(reader);
                    }
                }
                else
                {
                    services = CreateSampleServices();
                    SaveServices();
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors du chargement des services: {ex.Message}", ex);
                services = new List<Service>();
            }
        }

        private void SaveServices()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ServicesFilePath));
                
                using (StreamWriter writer = new StreamWriter(ServicesFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Service>));
                    serializer.Serialize(writer, services);
                }
                
                UpdateFilesLog("services.vff", "modifiée");
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de l'enregistrement des services: {ex.Message}", ex);
            }
        }

        private List<Service> CreateSampleServices()
        {
            List<Service> sampleServices = new List<Service>
            {
                new Service
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Hébergement Web",
                    Description = "Serveur d'hébergement de sites web",
                    Category = ServiceCategory.Hosting,
                    Status = ServiceStatus.Running,
                    IsActive = true,
                    RequiresMonitoring = true,
                    MonitoringUrl = "https://www.viple.fr/status/web",
                    MonthlyFee = 29.99m,
                    LastChecked = DateTime.Now,
                    LastStatusMessage = "Service fonctionnel",
                    CreationDate = DateTime.Now.AddMonths(-6)
                },
                new Service
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Serveur Mail",
                    Description = "Service de messagerie électronique",
                    Category = ServiceCategory.Email,
                    Status = ServiceStatus.Warning,
                    IsActive = true,
                    RequiresMonitoring = true,
                    MonitoringUrl = "https://www.viple.fr/status/mail",
                    MonthlyFee = 19.99m,
                    LastChecked = DateTime.Now.AddHours(-2),
                    LastStatusMessage = "Performances dégradées",
                    CreationDate = DateTime.Now.AddMonths(-8)
                },
                new Service
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Sauvegarde Cloud",
                    Description = "Service de sauvegarde dans le cloud",
                    Category = ServiceCategory.Storage,
                    Status = ServiceStatus.Running,
                    IsActive = true,
                    RequiresMonitoring = true,
                    MonitoringUrl = "https://www.viple.fr/status/backup",
                    MonthlyFee = 39.99m,
                    LastChecked = DateTime.Now.AddHours(-1),
                    LastStatusMessage = "Service fonctionnel",
                    CreationDate = DateTime.Now.AddMonths(-4)
                },
                new Service
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Support Premium",
                    Description = "Service de support technique dédié",
                    Category = ServiceCategory.Support,
                    Status = ServiceStatus.Running,
                    IsActive = true,
                    RequiresMonitoring = false,
                    MonthlyFee = 99.99m,
                    LastChecked = DateTime.Now.AddDays(-1),
                    LastStatusMessage = "Service géré manuellement",
                    CreationDate = DateTime.Now.AddMonths(-2)
                }
            };
            
            return sampleServices;
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
        
        // Méthode pour obtenir les produits associés à un service
        public List<string> GetServiceProducts(string serviceId)
        {
            Service service = GetServiceById(serviceId);
            return service?.ProductIds ?? new List<string>();
        }
        
        // Méthode pour associer des produits à un service
        public bool AssociateProductsToService(string serviceId, List<string> productIds)
        {
            Service service = GetServiceById(serviceId);
            if (service != null)
            {
                service.ProductIds = new List<string>(productIds);
                SaveServices();
                return true;
            }
            return false;
        }
        
        // Obtenir les services qui utilisent un produit spécifique
        public List<Service> GetServicesUsingProduct(string productId)
        {
            try
            {
                return services.Where(s => s.ProductIds != null && 
                                     s.ProductIds.Contains(productId)).ToList();
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la recherche des services utilisant le produit {productId}: {ex.Message}", ex);
                return new List<Service>();
            }
        }
        
        // Obtenir les services arrivant à échéance de maintenance
        public List<Service> GetServicesDueForMaintenance(int daysThreshold = 30)
        {
            try
            {
                DateTime thresholdDate = DateTime.Now.AddDays(-daysThreshold);
                
                return services.Where(s => 
                    s.IsActive && 
                    s.LastChecked < thresholdDate).ToList();
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la recherche des services nécessitant une maintenance: {ex.Message}", ex);
                return new List<Service>();
            }
        }
        
        // Statistiques sur les services
        public Dictionary<ServiceCategory, int> GetServiceCountByCategory()
        {
            Dictionary<ServiceCategory, int> result = new Dictionary<ServiceCategory, int>();
            
            foreach (ServiceCategory category in Enum.GetValues(typeof(ServiceCategory)))
            {
                result[category] = services.Count(s => s.Category == category);
            }
            
            return result;
        }
        
        // Obtenir le total des revenus mensuels de tous les services actifs
        public decimal GetTotalMonthlyRevenue()
        {
            return services.Where(s => s.IsActive).Sum(s => s.MonthlyFee);
        }
        
        // Obtenir le nombre total de services par statut
        public Dictionary<ServiceStatus, int> GetServiceCountByStatus()
        {
            Dictionary<ServiceStatus, int> result = new Dictionary<ServiceStatus, int>();
            
            foreach (ServiceStatus status in Enum.GetValues(typeof(ServiceStatus)))
            {
                result[status] = services.Count(s => s.Status == status);
            }
            
            return result;
        }
    }
}