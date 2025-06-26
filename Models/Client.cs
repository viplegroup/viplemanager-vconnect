// Viple FilesVersion - Client 1.0.1 - Date 26/06/2025
// Application créée par Viple SAS

using System;
using System.Collections.Generic;

namespace VipleManagement.Models
{
    [Serializable]
    public class Client
    {
        // Identifiant unique du client
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        // Informations générales
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        
        // Dates importantes
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime LastContactDate { get; set; } = DateTime.Now;
        public DateTime? NextFollowUpDate { get; set; }
        
        // Statut du client
        public bool IsActive { get; set; } = true;
        public ClientType Type { get; set; } = ClientType.Standard;
        public string Notes { get; set; } = string.Empty;
        
        // Relations avec les services
        public List<string> SubscribedServiceIds { get; set; } = new List<string>();
        
        // Informations financières
        public decimal TotalMonthlyFees { get; set; }
        public DateTime? LastInvoiceDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.UpToDate;
        
        // Informations de suivi
        public string AssignedToUserId { get; set; } = string.Empty;
        
        // Méthodes utiles
        public string GetFullAddress()
        {
            return $"{Address}, {PostalCode} {City}";
        }
        
        public string GetShortInfo()
        {
            return $"{Name} ({Type}) - {Email}";
        }
        
        public int GetServicesCount()
        {
            return SubscribedServiceIds?.Count ?? 0;
        }
        
        public string GetPaymentStatusDescription()
        {
            switch (PaymentStatus)
            {
                case PaymentStatus.UpToDate:
                    return "À jour";
                case PaymentStatus.PendingPayment:
                    return "En attente de paiement";
                case PaymentStatus.Late:
                    return "En retard";
                case PaymentStatus.VeryLate:
                    return "Très en retard";
                default:
                    return "Inconnu";
            }
        }
    }
    
    public enum ClientType
    {
        Prospect,
        Standard,
        Premium,
        Enterprise,
        Government,
        NonProfit
    }
    
    public enum PaymentStatus
    {
        UpToDate,
        PendingPayment,
        Late,
        VeryLate
    }
}