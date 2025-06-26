// Viple FilesVersion - Product 1.0.2 - Date 26/06/2025 01:40
// Application créée par Viple SAS

using System;
using System.Collections.Generic;

namespace VipleManagement.Models
{
    /// <summary>
    /// Catégories de produits disponibles
    /// </summary>
    public enum ProductCategory
    {
        Hardware,           // Matériel informatique
        Software,           // Logiciels
        Peripheral,         // Périphériques
        Cable,              // Câbles et connecteurs
        Component,          // Composants
        Consumable,         // Consommables
        Network,            // Équipements réseau
        Security,           // Équipements de sécurité
        License,            // Licences logicielles
        NetworkEquipment,   // Équipements réseaux spécifiques
        SecuritySolution,   // Solutions de sécurité
        Other               // Autres
    }
    
    /// <summary>
    /// Représente un produit physique ou logiciel
    /// </summary>
    [Serializable]
    public class Product
    {
        /// <summary>
        /// Identifiant unique du produit
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// Code produit ou référence
        /// </summary>
        public string ProductCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Nom du produit
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Description du produit
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Catégorie du produit
        /// </summary>
        public ProductCategory Category { get; set; } = ProductCategory.Other;
        
        /// <summary>
        /// Prix unitaire du produit
        /// </summary>
        public decimal Price { get; set; } = 0;
        
        /// <summary>
        /// Quantité en stock
        /// </summary>
        public int StockQuantity { get; set; } = 0;
        
        /// <summary>
        /// Indique si le produit est actif/disponible
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Fournisseur du produit
        /// </summary>
        public string Supplier { get; set; } = string.Empty;
        
        /// <summary>
        /// Fabricant du produit
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;
        
        /// <summary>
        /// Numéro de version du logiciel ou du produit
        /// </summary>
        public string Version { get; set; } = string.Empty;
        
        /// <summary>
        /// Clé de licence pour les logiciels
        /// </summary>
        public string LicenseKey { get; set; } = string.Empty;
        
        /// <summary>
        /// Date d'expiration de la licence
        /// </summary>
        public DateTime? ExpirationDate { get; set; } = null;
        
        /// <summary>
        /// Date d'achat du produit
        /// </summary>
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Date d'ajout du produit
        /// </summary>
        public DateTime CreationDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Date de dernière mise à jour
        /// </summary>
        public DateTime LastUpdateDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Chemin vers l'image du produit
        /// </summary>
        public string ImagePath { get; set; } = string.Empty;
        
        /// <summary>
        /// Seuil d'alerte pour le niveau de stock bas
        /// </summary>
        public int LowStockThreshold { get; set; } = 5;
        
        /// <summary>
        /// Notes supplémentaires
        /// </summary>
        public string Notes { get; set; } = string.Empty;
        
        /// <summary>
        /// Tags/mots-clés pour la recherche
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();
        
        /// <summary>
        /// Indique si le stock est bas
        /// </summary>
        public bool IsLowStock()
        {
            return StockQuantity <= LowStockThreshold;
        }
        
        /// <summary>
        /// Indique si le produit est en stock
        /// </summary>
        public bool IsInStock()
        {
            return StockQuantity > 0;
        }
        
        /// <summary>
        /// Vérifie si la licence est expirée
        /// </summary>
        public bool IsLicenseExpired()
        {
            return ExpirationDate.HasValue && ExpirationDate.Value < DateTime.Now;
        }
        
        /// <summary>
        /// Calcule le nombre de jours avant expiration de la licence
        /// </summary>
        public int DaysUntilExpiration()
        {
            if (!ExpirationDate.HasValue)
                return -1;
                
            TimeSpan timeRemaining = ExpirationDate.Value - DateTime.Now;
            return timeRemaining.Days > 0 ? timeRemaining.Days : 0;
        }
        
        /// <summary>
        /// Calcule l'âge du produit en jours depuis l'achat
        /// </summary>
        public int DaysSincePurchase()
        {
            return (int)(DateTime.Now - PurchaseDate).TotalDays;
        }
        
        /// <summary>
        /// Représentation textuelle du produit
        /// </summary>
        public override string ToString()
        {
            return $"{Name} ({ProductCode}) - {Price:C2}";
        }
    }
}