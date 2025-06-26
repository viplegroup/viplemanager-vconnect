// Viple FilesVersion - LogManager 1.0.0 - Date 26/06/2025
// Application créée par Viple SAS

using System;
using System.IO;
using System.Text;
using System.Threading;

namespace VipleManagement.Core
{
    /// <summary>
    /// Gestionnaire de journalisation pour l'application Viple Management System
    /// </summary>
    public static class LogManager
    {
        private static readonly string LogDirectory = Path.Combine("vipledata", "logs");
        private static readonly string LogFile = Path.Combine(LogDirectory, $"viple_log_{DateTime.Now:yyyy-MM-dd}.vff");
        private static readonly object LockObject = new object();

        /// <summary>
        /// Initialise le gestionnaire de journalisation
        /// </summary>
        static LogManager()
        {
            try
            {
                // Création du dossier de journalisation s'il n'existe pas
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }

                // Ajouter une entrée de démarrage de l'application
                LogStartup();
            }
            catch (Exception ex)
            {
                // Si on ne peut pas journaliser, on ne peut pas non plus journaliser cette erreur
                // On affiche simplement une erreur dans la console pour le débogage
                Console.WriteLine($"Erreur lors de l'initialisation du système de journalisation: {ex.Message}");
            }
        }

        /// <summary>
        /// Journalise une action utilisateur
        /// </summary>
        /// <param name="message">Message décrivant l'action</param>
        public static void LogAction(string message)
        {
            LogEntry(message, "ACTION");
        }

        /// <summary>
        /// Journalise une erreur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="exception">Exception associée (optionnel)</param>
        public static void LogError(string message, Exception exception = null)
        {
            string logMessage = message;
            if (exception != null)
            {
                logMessage += $" | Exception: {exception.GetType().Name} | Message: {exception.Message}";
                if (exception.StackTrace != null)
                {
                    logMessage += $" | StackTrace: {exception.StackTrace}";
                }
            }
            LogEntry(logMessage, "ERROR");
        }

        /// <summary>
        /// Journalise une connexion utilisateur
        /// </summary>
        /// <param name="username">Nom d'utilisateur</param>
        /// <param name="successful">Indique si la connexion a réussi</param>
        public static void LogLogin(string username, bool successful)
        {
            string status = successful ? "RÉUSSIE" : "ÉCHOUÉE";
            LogEntry($"Connexion {status} pour l'utilisateur {username}", "LOGIN");
        }

        /// <summary>
        /// Journalise une déconnexion utilisateur
        /// </summary>
        /// <param name="username">Nom d'utilisateur</param>
        public static void LogLogout(string username)
        {
            LogEntry($"Déconnexion de l'utilisateur {username}", "LOGOUT");
        }

        /// <summary>
        /// Journalise le démarrage de l'application
        /// </summary>
        private static void LogStartup()
        {
            LogEntry("Application démarrée", "STARTUP");
        }

        /// <summary>
        /// Journalise une entrée avec un type spécifique
        /// </summary>
        /// <param name="message">Message à journaliser</param>
        /// <param name="type">Type de l'entrée</param>
        private static void LogEntry(string message, string type)
        {
            try
            {
                // Format: [Date] [Type] [ThreadID] Message
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{type}] [{Thread.CurrentThread.ManagedThreadId}] {message}";

                // Utiliser un verrou pour éviter les conflits d'écriture
                lock (LockObject)
                {
                    using (StreamWriter writer = new StreamWriter(LogFile, true, Encoding.UTF8))
                    {
                        writer.WriteLine(logEntry);
                    }
                }
            }
            catch (Exception ex)
            {
                // Si on ne peut pas journaliser, on ne peut pas non plus journaliser cette erreur
                // On affiche simplement une erreur dans la console pour le débogage
                Console.WriteLine($"Erreur lors de la journalisation: {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère les entrées de journal pour la date spécifiée
        /// </summary>
        /// <param name="date">Date des entrées à récupérer</param>
        /// <returns>Tableau des entrées de journal</returns>
        public static string[] GetLogEntries(DateTime date)
        {
            string logFilePath = Path.Combine(LogDirectory, $"viple_log_{date:yyyy-MM-dd}.vff");
            if (File.Exists(logFilePath))
            {
                return File.ReadAllLines(logFilePath);
            }
            return new string[0];
        }

        /// <summary>
        /// Récupère les dates pour lesquelles des journaux existent
        /// </summary>
        /// <returns>Tableau des dates</returns>
        public static DateTime[] GetLogDates()
        {
            if (!Directory.Exists(LogDirectory))
            {
                return new DateTime[0];
            }

            string[] files = Directory.GetFiles(LogDirectory, "viple_log_*.vff");
            DateTime[] dates = new DateTime[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(files[i]);
                string dateStr = fileName.Substring(10); // Extraire la date après "viple_log_"
                if (DateTime.TryParse(dateStr, out DateTime date))
                {
                    dates[i] = date.Date;
                }
                else
                {
                    dates[i] = DateTime.MinValue;
                }
            }

            return dates;
        }
    }
}