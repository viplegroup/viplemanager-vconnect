// Viple FilesVersion - ApiClient 1.0.0 - Date 26/06/2025
// Application créée par Viple SAS

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VipleManagement.Models;

namespace VipleManagement.Core
{
    /// <summary>
    /// Client API pour communiquer avec le serveur
    /// </summary>
    public class ApiClient
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string baseUrl = "http://localhost:8080/api";
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        static ApiClient()
        {
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        /// <summary>
        /// Vérifier si le serveur est accessible
        /// </summary>
        public static async Task<bool> IsServerAvailableAsync()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{baseUrl}/users");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Authentifier un utilisateur
        /// </summary>
        public static async Task<User> AuthenticateAsync(string username, string password)
        {
            try
            {
                var authData = new
                {
                    Username = username,
                    Password = password
                };
                
                var content = new StringContent(
                    JsonSerializer.Serialize(authData),
                    Encoding.UTF8,
                    "application/json");
                
                HttpResponseMessage response = await httpClient.PostAsync($"{baseUrl}/users/authenticate", content);
                
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<User>(responseBody, jsonOptions);
                }
                
                return null;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur d'authentification: {ex.Message}", ex);
                return null;
            }
        }
        
        /// <summary>
        /// Récupérer tous les utilisateurs
        /// </summary>
        public static async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{baseUrl}/users");
                response.EnsureSuccessStatusCode();
                
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<User>>(responseBody, jsonOptions);
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la récupération des utilisateurs: {ex.Message}", ex);
                return new List<User>();
            }
        }
        
        /// <summary>
        /// Récupérer un utilisateur par ID
        /// </summary>
        public static async Task<User> GetUserByIdAsync(string userId)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{baseUrl}/users/{userId}");
                
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<User>(responseBody, jsonOptions);
                }
                
                return null;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la récupération de l'utilisateur {userId}: {ex.Message}", ex);
                return null;
            }
        }
        
        /// <summary>
        /// Créer un nouvel utilisateur
        /// </summary>
        public static async Task<bool> CreateUserAsync(User user, string password)
        {
            try
            {
                var userData = new
                {
                    User = user,
                    Password = password
                };
                
                var content = new StringContent(
                    JsonSerializer.Serialize(userData),
                    Encoding.UTF8,
                    "application/json");
                
                HttpResponseMessage response = await httpClient.PostAsync($"{baseUrl}/users", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la création de l'utilisateur: {ex.Message}", ex);
                return false;
            }
        }
        
        /// <summary>
        /// Mettre à jour un utilisateur
        /// </summary>
        public static async Task<bool> UpdateUserAsync(User user, string password = null)
        {
            try
            {
                var userData = new
                {
                    User = user,
                    Password = password
                };
                
                var content = new StringContent(
                    JsonSerializer.Serialize(userData),
                    Encoding.UTF8,
                    "application/json");
                
                HttpResponseMessage response = await httpClient.PutAsync($"{baseUrl}/users/{user.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la mise à jour de l'utilisateur {user.Id}: {ex.Message}", ex);
                return false;
            }
        }
        
        /// <summary>
        /// Supprimer un utilisateur
        /// </summary>
        public static async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync($"{baseUrl}/users/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogManager.LogError($"Erreur lors de la suppression de l'utilisateur {userId}: {ex.Message}", ex);
                return false;
            }
        }
    }
}