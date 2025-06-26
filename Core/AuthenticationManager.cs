// Viple FilesVersion - AuthenticationManager 1.0.2 - Date 26/06/2025 02:23
// Application créée par Viple SAS

using System;
using System.Threading.Tasks;
using VipleManagement.Models;

namespace VipleManagement.Core
{
    /// <summary>
    /// Gestionnaire d'authentification
    /// </summary>
    public static class AuthenticationManager
    {
        /// <summary>
        /// Utilisateur actuellement connecté
        /// </summary>
        public static User CurrentUser { get; private set; }
        
        /// <summary>
        /// Indique si un utilisateur est connecté
        /// </summary>
        public static bool IsLoggedIn => CurrentUser != null;
        
        /// <summary>
        /// Événement déclenché lors de la connexion d'un utilisateur
        /// </summary>
        public static event EventHandler<User> UserLoggedIn;
        
        /// <summary>
        /// Événement déclenché lors de la déconnexion d'un utilisateur
        /// </summary>
        public static event EventHandler UserLoggedOut;
        
        /// <summary>
        /// Authentifier un utilisateur en utilisant l'API
        /// </summary>
        public static async Task<bool> LoginAsync(string username, string password)
        {
            // Vérifier d'abord si le serveur est disponible
            if (!await ApiClient.IsServerAvailableAsync())
            {
                LogManager.LogError("Impossible de se connecter au serveur");
                return false;
            }
            
            // Authentifier l'utilisateur via l'API
            User user = await ApiClient.AuthenticateAsync(username, password);
            
            if (user != null)
            {
                CurrentUser = user;
                
                // Déclencher l'événement
                UserLoggedIn?.Invoke(null, CurrentUser);
                
                LogManager.LogAction($"Utilisateur {username} connecté avec succès");
                return true;
            }
            
            LogManager.LogAction($"Échec de connexion pour l'utilisateur {username}");
            return false;
        }
        
        /// <summary>
        /// Déconnecter l'utilisateur actuel
        /// </summary>
        public static void Logout()
        {
            if (CurrentUser != null)
            {
                string username = CurrentUser.Username;
                CurrentUser = null;
                
                // Déclencher l'événement
                UserLoggedOut?.Invoke(null, EventArgs.Empty);
                
                LogManager.LogAction($"Utilisateur {username} déconnecté");
            }
        }
        
        /// <summary>
        /// Vérifier si l'utilisateur courant a un certain rôle
        /// </summary>
        public static bool HasRole(UserRole role)
        {
            return CurrentUser != null && CurrentUser.Role == role;
        }
        
        /// <summary>
        /// Vérifier si l'utilisateur courant peut gérer les clients
        /// </summary>
        public static bool CanManageClients()
        {
            return CurrentUser != null && 
                   (CurrentUser.Role == UserRole.Administrator || 
                    CurrentUser.Role == UserRole.Manager || 
                    CurrentUser.Role == UserRole.Support);
        }
        
        /// <summary>
        /// Vérifier si l'utilisateur courant peut gérer les services
        /// </summary>
        public static bool CanManageServices()
        {
            return CurrentUser != null && 
                   (CurrentUser.Role == UserRole.Administrator || 
                    CurrentUser.Role == UserRole.Manager || 
                    CurrentUser.Role == UserRole.Technician);
        }
        
        /// <summary>
        /// Vérifier si l'utilisateur courant peut gérer les produits
        /// </summary>
        public static bool CanManageProducts()
        {
            return CurrentUser != null && 
                   (CurrentUser.Role == UserRole.Administrator || 
                    CurrentUser.Role == UserRole.Manager);
        }
        
        /// <summary>
        /// Vérifier si l'utilisateur courant peut gérer les utilisateurs
        /// </summary>
        public static bool CanManageUsers()
        {
            return CurrentUser != null && CurrentUser.Role == UserRole.Administrator;
        }
    }
}