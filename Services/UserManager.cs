// Viple FilesVersion - UserManager 1.0.0 - Date 26/06/2025 01:50
// Application créée par Viple SAS

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using VipleManagement.Core;
using VipleManagement.Models;

namespace VipleManagement.Services
{
    /// <summary>
    /// Gestionnaire des utilisateurs du système
    /// </summary>
    public class UserManager
    {
        private static readonly string UsersFilePath = Path.Combine("vipledata", "users.vff");
        private static readonly string UserActionsFilePath = Path.Combine("vipledata", "user_actions.vff");
        private List<User> users;
        private List<UserAction> userActions;
        
        /// <summary>
        /// Événement déclenché quand un utilisateur est ajouté
        /// </summary>
        public event EventHandler<User> UserAdded;
        
        /// <summary>
        /// Événement déclenché quand un utilisateur est modifié
        /// </summary>
        public event EventHandler<User> UserUpdated;
        
        /// <summary>
        /// Événement déclenché quand un utilisateur est supprimé
        /// </summary>
        public event EventHandler<string> UserDeleted;
        
        /// <summary>
        /// Événement déclenché quand un utilisateur se connecte
        /// </summary>
        public event EventHandler<User> UserLoggedIn;
        
        /// <summary>
        /// Événement déclenché quand un utilisateur se déconnecte
        /// </summary>
        public event EventHandler<User> UserLoggedOut;
        
        /// <summary>
        /// Constructeur
        /// </summary>
        public UserManager()
        {
            LoadUsers();
            LoadUserActions();
        }
        
        /// <summary>
        /// Récupérer tous les utilisateurs
        /// </summary>
        public List<User> GetAllUsers()
        {
            return new List<User>(users);
        }
        
        /// <summary>
        /// Récupérer un utilisateur par ID
        /// </summary>
        public User GetUserById(string userId)
        {
            return users.FirstOrDefault(u => u.Id == userId);
        }
        
        /// <summary>
        /// Récupérer un utilisateur par nom d'utilisateur
        /// </summary>
        public User GetUserByUsername(string username)
        {
            return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// Ajouter un nouvel utilisateur
        /// </summary>
        public bool AddUser(User user, string plainPassword)
        {
            try
            {
                // Vérifier si le nom d'utilisateur existe déjà
                if (GetUserByUsername(user.Username) != null)
                {
                    LogManager.LogError($"Impossible d'ajouter l'utilisateur: le nom d'utilisateur '{user.Username}' existe déjà");
                    return false;
                }
                
                // Générer le sel et hacher le mot de passe
                user.PasswordSalt = GenerateSalt();
                user.PasswordHash = HashPassword(plainPassword, user.PasswordSalt);
                
                // Définir la date de création
                user.CreationDate = DateTime.Now;
                
                // Ajouter l'utilisateur à la liste
                users.Add(user);
                SaveUsers();
                
                // Journaliser l'action
                LogUserAction("CREATE_USER", $"Utilisateur '{user.Username}' créé", 
                              user.Id, "User", AuthenticationManager.CurrentUser?.Id ?? "System");
                
                // Déclencher l'événement
                UserAdded?.Invoke(this, user);
                
                LogManager.LogAction($"Utilisateur '{user.Username}' créé avec succès");
                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de l'ajout d'un utilisateur: {ex.Message}", ex);
                return false;
            }
        }
        
        /// <summary>
        /// Mettre à jour un utilisateur existant
        /// </summary>
        public bool UpdateUser(User user, string plainPassword = null)
        {
            try
            {
                // Trouver l'utilisateur existant
                int index = users.FindIndex(u => u.Id == user.Id);
                if (index < 0)
                {
                    LogManager.LogError($"Impossible de mettre à jour l'utilisateur: l'ID '{user.Id}' n'existe pas");
                    return false;
                }
                
                // Si un nouveau mot de passe est fourni, le hacher
                if (!string.IsNullOrEmpty(plainPassword))
                {
                    user.PasswordSalt = GenerateSalt();
                    user.PasswordHash = HashPassword(plainPassword, user.PasswordSalt);
                }
                else
                {
                    // Conserver le mot de passe existant
                    user.PasswordHash = users[index].PasswordHash;
                    user.PasswordSalt = users[index].PasswordSalt;
                }
                
                // Mettre à jour l'utilisateur
                users[index] = user;
                SaveUsers();
                
                // Journaliser l'action
                LogUserAction("UPDATE_USER", $"Utilisateur '{user.Username}' mis à jour", 
                              user.Id, "User", AuthenticationManager.CurrentUser?.Id ?? "System");
                
                // Déclencher l'événement
                UserUpdated?.Invoke(this, user);
                
                LogManager.LogAction($"Utilisateur '{user.Username}' mis à jour avec succès");
                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la mise à jour d'un utilisateur: {ex.Message}", ex);
                return false;
            }
        }
        
        /// <summary>
        /// Supprimer un utilisateur
        /// </summary>
        public bool DeleteUser(string userId)
        {
            try
            {
                // Trouver l'utilisateur à supprimer
                int index = users.FindIndex(u => u.Id == userId);
                if (index < 0)
                {
                    LogManager.LogError($"Impossible de supprimer l'utilisateur: l'ID '{userId}' n'existe pas");
                    return false;
                }
                
                // Sauvegarder le nom d'utilisateur pour la journalisation
                string username = users[index].Username;
                
                // Vérifier qu'il ne s'agit pas du dernier administrateur
                if (users[index].Role == UserRole.Administrator && 
                    users.Count(u => u.Role == UserRole.Administrator) <= 1)
                {
                    LogManager.LogError("Impossible de supprimer le dernier utilisateur administrateur");
                    return false;
                }
                
                // Supprimer l'utilisateur
                users.RemoveAt(index);
                SaveUsers();
                
                // Journaliser l'action
                LogUserAction("DELETE_USER", $"Utilisateur '{username}' supprimé", 
                              userId, "User", AuthenticationManager.CurrentUser?.Id ?? "System");
                
                // Déclencher l'événement
                UserDeleted?.Invoke(this, userId);
                
                LogManager.LogAction($"Utilisateur '{username}' supprimé avec succès");
                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la suppression d'un utilisateur: {ex.Message}", ex);
                return false;
            }
        }
        
        /// <summary>
        /// Authentifier un utilisateur
        /// </summary>
        public bool AuthenticateUser(string username, string password)
        {
            try
            {
                // Trouver l'utilisateur
                User user = GetUserByUsername(username);
                if (user == null)
                {
                    LogManager.LogAction($"Tentative d'authentification échouée: utilisateur '{username}' introuvable");
                    return false;
                }
                
                // Vérifier si l'utilisateur est actif
                if (!user.IsActive)
                {
                    LogManager.LogAction($"Tentative d'authentification échouée: compte '{username}' inactif");
                    return false;
                }
                
                // Vérifier le mot de passe
                string hashedPassword = HashPassword(password, user.PasswordSalt);
                if (!hashedPassword.Equals(user.PasswordHash))
                {
                    LogManager.LogAction($"Tentative d'authentification échouée: mot de passe incorrect pour '{username}'");
                    return false;
                }
                
                // Mettre à jour la date de dernière connexion
                user.LastLogin = DateTime.Now;
                SaveUsers();
                
                // Journaliser l'action
                LogUserAction("USER_LOGIN", $"Utilisateur '{username}' connecté", 
                              user.Id, "User", user.Id);
                
                // Déclencher l'événement
                UserLoggedIn?.Invoke(this, user);
                
                LogManager.LogAction($"Utilisateur '{username}' authentifié avec succès");
                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de l'authentification: {ex.Message}", ex);
                return false;
            }
        }
        
        /// <summary>
        /// Déconnecter un utilisateur
        /// </summary>
        public void LogoutUser(string userId)
        {
            try
            {
                User user = GetUserById(userId);
                if (user != null)
                {
                    // Journaliser l'action
                    LogUserAction("USER_LOGOUT", $"Utilisateur '{user.Username}' déconnecté", 
                                  user.Id, "User", user.Id);
                    
                    // Déclencher l'événement
                    UserLoggedOut?.Invoke(this, user);
                    
                    LogManager.LogAction($"Utilisateur '{user.Username}' déconnecté");
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la déconnexion: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Changer le mot de passe d'un utilisateur
        /// </summary>
        public bool ChangePassword(string userId, string currentPassword, string newPassword)
        {
            try
            {
                // Trouver l'utilisateur
                User user = GetUserById(userId);
                if (user == null)
                {
                    LogManager.LogError($"Impossible de changer le mot de passe: utilisateur avec ID '{userId}' introuvable");
                    return false;
                }
                
                // Vérifier le mot de passe actuel
                string hashedCurrentPassword = HashPassword(currentPassword, user.PasswordSalt);
                if (!hashedCurrentPassword.Equals(user.PasswordHash))
                {
                    LogManager.LogError("Impossible de changer le mot de passe: mot de passe actuel incorrect");
                    return false;
                }
                
                // Générer un nouveau sel et hacher le nouveau mot de passe
                user.PasswordSalt = GenerateSalt();
                user.PasswordHash = HashPassword(newPassword, user.PasswordSalt);
                
                // Sauvegarder les modifications
                SaveUsers();
                
                // Journaliser l'action
                LogUserAction("CHANGE_PASSWORD", "Mot de passe changé", 
                              user.Id, "User", user.Id);
                
                LogManager.LogAction($"Mot de passe de l'utilisateur '{user.Username}' changé avec succès");
                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors du changement de mot de passe: {ex.Message}", ex);
                return false;
            }
        }
        
        /// <summary>
        /// Réinitialiser le mot de passe d'un utilisateur (administrateur uniquement)
        /// </summary>
        public string ResetPassword(string userId)
        {
            try
            {
                // Trouver l'utilisateur
                User user = GetUserById(userId);
                if (user == null)
                {
                    LogManager.LogError($"Impossible de réinitialiser le mot de passe: utilisateur avec ID '{userId}' introuvable");
                    return null;
                }
                
                // Vérifier les droits de l'utilisateur courant
                if (AuthenticationManager.CurrentUser == null || 
                    !AuthenticationManager.CurrentUser.CanManageUsers())
                {
                    LogManager.LogError("Impossible de réinitialiser le mot de passe: droits insuffisants");
                    return null;
                }
                
                // Générer un nouveau mot de passe aléatoire
                string newPassword = GenerateRandomPassword(10);
                
                // Générer un nouveau sel et hacher le nouveau mot de passe
                user.PasswordSalt = GenerateSalt();
                user.PasswordHash = HashPassword(newPassword, user.PasswordSalt);
                
                // Sauvegarder les modifications
                SaveUsers();
                
                // Journaliser l'action
                LogUserAction("RESET_PASSWORD", $"Mot de passe de l'utilisateur '{user.Username}' réinitialisé", 
                              user.Id, "User", AuthenticationManager.CurrentUser?.Id ?? "System");
                
                LogManager.LogAction($"Mot de passe de l'utilisateur '{user.Username}' réinitialisé avec succès");
                return newPassword;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la réinitialisation du mot de passe: {ex.Message}", ex);
                return null;
            }
        }
        
        /// <summary>
        /// Générer un sel aléatoire pour le hachage du mot de passe
        /// </summary>
        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }
        
        /// <summary>
        /// Hacher un mot de passe avec un sel
        /// </summary>
        private string HashPassword(string password, string salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Convert.FromBase64String(salt);
            
            byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length);
            
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
        
        /// <summary>
        /// Générer un mot de passe aléatoire
        /// </summary>
        private string GenerateRandomPassword(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_-+=";
            
            byte[] randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[randomBytes[i] % chars.Length]);
            }
            
            return result.ToString();
        }
        
        /// <summary>
        /// Récupérer les actions d'un utilisateur
        /// </summary>
        public List<UserAction> GetUserActions(string userId)
        {
            return userActions.Where(a => a.UserId == userId).OrderByDescending(a => a.Timestamp).ToList();
        }
        
        /// <summary>
        /// Récupérer toutes les actions utilisateur
        /// </summary>
        public List<UserAction> GetAllUserActions()
        {
            return userActions.OrderByDescending(a => a.Timestamp).ToList();
        }
        
        /// <summary>
        /// Journaliser une action utilisateur
        /// </summary>
        public void LogUserAction(string actionType, string description, string entityId, string entityType, string userId)
        {
            try
            {
                UserAction action = new UserAction
                {
                    ActionType = actionType,
                    Description = description,
                    EntityId = entityId,
                    EntityType = entityType,
                    UserId = userId,
                    Timestamp = DateTime.Now,
                    IpAddress = "127.0.0.1" // Dans une application réelle, on récupérerait l'IP
                };
                
                userActions.Add(action);
                SaveUserActions();
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la journalisation d'une action: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Charger les utilisateurs depuis le fichier
        /// </summary>
        private void LoadUsers()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(UsersFilePath));
                
                if (File.Exists(UsersFilePath))
                {
                    using (StreamReader reader = new StreamReader(UsersFilePath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
                        users = (List<User>)serializer.Deserialize(reader);
                    }
                }
                else
                {
                    users = CreateDefaultUsers();
                    SaveUsers();
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors du chargement des utilisateurs: {ex.Message}", ex);
                users = CreateDefaultUsers();
            }
        }
        
        /// <summary>
        /// Sauvegarder les utilisateurs dans le fichier
        /// </summary>
        private void SaveUsers()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(UsersFilePath));
                
                using (StreamWriter writer = new StreamWriter(UsersFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
                    serializer.Serialize(writer, users);
                }
                
                UpdateFilesLog("users.vff", "modifiée");
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de l'enregistrement des utilisateurs: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Charger les actions utilisateur depuis le fichier
        /// </summary>
        private void LoadUserActions()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(UserActionsFilePath));
                
                if (File.Exists(UserActionsFilePath))
                {
                    using (StreamReader reader = new StreamReader(UserActionsFilePath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<UserAction>));
                        userActions = (List<UserAction>)serializer.Deserialize(reader);
                    }
                }
                else
                {
                    userActions = new List<UserAction>();
                    SaveUserActions();
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors du chargement des actions utilisateur: {ex.Message}", ex);
                userActions = new List<UserAction>();
            }
        }
        
        /// <summary>
        /// Sauvegarder les actions utilisateur dans le fichier
        /// </summary>
        private void SaveUserActions()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(UserActionsFilePath));
                
                // Limiter le nombre d'actions (conserver les 1000 dernières)
                if (userActions.Count > 1000)
                {
                    userActions = userActions.OrderByDescending(a => a.Timestamp).Take(1000).ToList();
                }
                
                using (StreamWriter writer = new StreamWriter(UserActionsFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<UserAction>));
                    serializer.Serialize(writer, userActions);
                }
                
                UpdateFilesLog("user_actions.vff", "modifiée");
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de l'enregistrement des actions utilisateur: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Créer les utilisateurs par défaut
        /// </summary>
        private List<User> CreateDefaultUsers()
        {
            List<User> defaultUsers = new List<User>();
            
            // Utilisateur admin par défaut
            User adminUser = new User
            {
                Username = "admin",
                FullName = "Administrateur",
                Email = "admin@viple.fr",
                Role = UserRole.Administrator,
                IsActive = true,
                CreationDate = DateTime.Now,
                PasswordSalt = GenerateSalt()
            };
            adminUser.PasswordHash = HashPassword("admin", adminUser.PasswordSalt);
            
            defaultUsers.Add(adminUser);
            
            return defaultUsers;
        }
        
        /// <summary>
        /// Mettre à jour le fichier de log
        /// </summary>
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
    }
}