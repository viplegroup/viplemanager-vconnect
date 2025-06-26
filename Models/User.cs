// Viple FilesVersion - User 1.0.0 - Date 26/06/2025 01:50
// Application créée par Viple SAS

using System;
using System.Collections.Generic;

namespace VipleManagement.Models
{
    /// <summary>
    /// Rôles disponibles pour les utilisateurs
    /// </summary>
    public enum UserRole
    {
        Administrator,   // Administrateur avec tous les droits
        Manager,         // Gestionnaire avec droits étendus
        Technician,      // Technicien avec accès limité
        Support,         // Support avec accès client
        Viewer           // Visualisation uniquement
    }

    /// <summary>
    /// Représente un utilisateur du système
    /// </summary>
    [Serializable]
    public class User
    {
        /// <summary>
        /// Identifiant unique de l'utilisateur
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// Nom d'utilisateur (login)
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// Mot de passe haché
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;
        
        /// <summary>
        /// Sel utilisé pour le hachage du mot de passe
        /// </summary>
        public string PasswordSalt { get; set; } = string.Empty;
        
        /// <summary>
        /// Nom complet de l'utilisateur
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        
        /// <summary>
        /// Email de l'utilisateur
        /// </summary>
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Rôle de l'utilisateur
        /// </summary>
        public UserRole Role { get; set; } = UserRole.Viewer;
        
        /// <summary>
        /// Indique si l'utilisateur est actif
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Date de dernière connexion
        /// </summary>
        public DateTime LastLogin { get; set; } = DateTime.MinValue;
        
        /// <summary>
        /// Date de création du compte
        /// </summary>
        public DateTime CreationDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Préférences utilisateur (thème, langue, etc.)
        /// </summary>
        public Dictionary<string, string> Preferences { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Historique des actions réalisées par l'utilisateur
        /// </summary>
        [NonSerialized]
        private List<UserAction> actionHistory;
        
        /// <summary>
        /// Utilisateur peut-il gérer les clients ?
        /// </summary>
        public bool CanManageClients()
        {
            return Role == UserRole.Administrator || Role == UserRole.Manager || Role == UserRole.Support;
        }
        
        /// <summary>
        /// Utilisateur peut-il gérer les services ?
        /// </summary>
        public bool CanManageServices()
        {
            return Role == UserRole.Administrator || Role == UserRole.Manager || Role == UserRole.Technician;
        }
        
        /// <summary>
        /// Utilisateur peut-il gérer les produits ?
        /// </summary>
        public bool CanManageProducts()
        {
            return Role == UserRole.Administrator || Role == UserRole.Manager;
        }
        
        /// <summary>
        /// Utilisateur peut-il gérer les autres utilisateurs ?
        /// </summary>
        public bool CanManageUsers()
        {
            return Role == UserRole.Administrator;
        }
        
        /// <summary>
        /// Utilisateur peut-il voir les statistiques ?
        /// </summary>
        public bool CanViewStatistics()
        {
            return Role == UserRole.Administrator || Role == UserRole.Manager;
        }
        
        /// <summary>
        /// Représentation textuelle de l'utilisateur
        /// </summary>
        public override string ToString()
        {
            return $"{Username} ({FullName}) - {Role}";
        }
    }
    
    /// <summary>
    /// Représente une action réalisée par un utilisateur
    /// </summary>
    [Serializable]
    public class UserAction
    {
        /// <summary>
        /// ID de l'action
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// ID de l'utilisateur
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// Type d'action réalisée
        /// </summary>
        public string ActionType { get; set; } = string.Empty;
        
        /// <summary>
        /// Description de l'action
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Date et heure de l'action
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Entité concernée par l'action (ID)
        /// </summary>
        public string EntityId { get; set; } = string.Empty;
        
        /// <summary>
        /// Type d'entité concerné
        /// </summary>
        public string EntityType { get; set; } = string.Empty;
        
        /// <summary>
        /// Adresse IP de l'utilisateur
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// Représentation textuelle de l'action
        /// </summary>
        public override string ToString()
        {
            return $"{Timestamp:dd/MM/yyyy HH:mm:ss} - {ActionType}: {Description}";
        }
    }
}