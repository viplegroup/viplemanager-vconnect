// Viple FilesVersion - Service 1.0.2 - Date 26/06/2025
// Application créée par Viple SAS

using System;
using System.Collections.Generic;

namespace VipleManagement.Models
{
    /// <summary>
    /// Catégories de services disponibles
    /// </summary>
    public enum ServiceCategory
    {
        Hosting,       // Hébergement
        Email,         // Services de messagerie
        Storage,       // Stockage et sauvegarde
        Security,      // Sécurité informatique
        Support,       // Support technique
        Maintenance,   // Maintenance
        Development,   // Services de développement
        Network,       // Services réseau
        Cloud,         // Services cloud
        Custom         // Services personnalisés
    }

    /// <summary>
    /// Statuts possibles d'un service
    /// </summary>
    public enum ServiceStatus
    {
        Running,       // Service fonctionnel
        Warning,       // Service fonctionnel avec avertissements
        Error,         // Service en erreur
        Maintenance,   // Service en maintenance
        Inactive       // Service inactif
    }

    /// <summary>
    /// Représente un service proposé par Viple
    /// </summary>
    [Serializable]
    public class Service
    {
        /// <summary>
        /// Identifiant unique du service
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Nom du service
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description du service
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Catégorie du service
        /// </summary>
        public ServiceCategory Category { get; set; } = ServiceCategory.Custom;

        /// <summary>
        /// Statut actuel du service
        /// </summary>
        public ServiceStatus Status { get; set; } = ServiceStatus.Inactive;

        /// <summary>
        /// Indique si le service est actif
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Indique si le service nécessite une surveillance
        /// </summary>
        public bool RequiresMonitoring { get; set; } = false;

        /// <summary>
        /// URL de surveillance du service
        /// </summary>
        public string MonitoringUrl { get; set; } = string.Empty;

        /// <summary>
        /// Tarif mensuel du service
        /// </summary>
        public decimal MonthlyFee { get; set; } = 0;

        /// <summary>
        /// Date de la dernière vérification du service
        /// </summary>
        public DateTime LastChecked { get; set; } = DateTime.Now;

        /// <summary>
        /// Dernier message de statut
        /// </summary>
        public string LastStatusMessage { get; set; } = string.Empty;
        
        /// <summary>
        /// IDs des produits associés à ce service
        /// </summary>
        public List<string> ProductIds { get; set; } = new List<string>();
        
        /// <summary>
        /// Produits associés au service (compatibilité arrière)
        /// </summary>
        public List<string> AssociatedProducts { get; set; } = new List<string>();
        
        /// <summary>
        /// Date de création du service
        /// </summary>
        public DateTime CreationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Renvoie une représentation textuelle du service
        /// </summary>
        public override string ToString()
        {
            return $"{Name} ({Category}) - {Status}";
        }

        /// <summary>
        /// Convertit le statut en texte lisible
        /// </summary>
        public string GetStatusText()
        {
            return Status switch
            {
                ServiceStatus.Running => "En fonctionnement",
                ServiceStatus.Warning => "Avertissement",
                ServiceStatus.Error => "Erreur",
                ServiceStatus.Maintenance => "En maintenance",
                ServiceStatus.Inactive => "Inactif",
                _ => "Inconnu"
            };
        }

        /// <summary>
        /// Indique si le service est surveillable
        /// </summary>
        public bool IsMonitorable()
        {
            return RequiresMonitoring && !string.IsNullOrWhiteSpace(MonitoringUrl);
        }

        /// <summary>
        /// Indique si le statut est critique
        /// </summary>
        public bool IsCritical()
        {
            return Status == ServiceStatus.Error;
        }

        /// <summary>
        /// Calcule le temps écoulé depuis la dernière vérification
        /// </summary>
        public TimeSpan TimeSinceLastCheck()
        {
            return DateTime.Now - LastChecked;
        }
    }
}