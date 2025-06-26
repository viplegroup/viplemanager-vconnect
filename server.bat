@echo off
:: Viple FilesVersion - server.bat 1.0.0 - Date 26/06/2025
:: Application créée par Viple SAS

title Viple Database Server
echo **************************************************
echo *                                                *
echo *            VIPLE DATABASE SERVER               *
echo *                                                *
echo *        (c) 2025 Viple SAS - Version 1.0       *
echo *                                                *
echo **************************************************
echo.
echo Initialisation du serveur de base de donnees...

:: Vérifier si le service HTTP est déjà en cours d'exécution
netstat -an | find ":8080" > nul
if %ERRORLEVEL% EQU 0 (
    echo ERREUR: Le port 8080 est déjà utilisé. Arrêtez le service existant ou changez le port.
    pause
    exit /b 1
)

:: Vérifier si .NET SDK est installé
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERREUR: .NET SDK n'est pas installé ou n'est pas dans votre PATH
    echo Veuillez télécharger et installer .NET SDK depuis https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

:: Créer les répertoires nécessaires
if not exist "server_data" mkdir server_data
if not exist "server_data\users" mkdir server_data\users
if not exist "server_data\clients" mkdir server_data\clients
if not exist "server_data\services" mkdir server_data\services
if not exist "server_data\products" mkdir server_data\products
if not exist "server_data\logs" mkdir server_data\logs

:: Vérifier si le serveur existe, sinon le créer
if not exist "VipleDataServer" (
    echo Création du serveur de données...
    dotnet new webapi -n VipleDataServer -o VipleDataServer --no-https
    cd VipleDataServer
    
    echo Ajout des packages nécessaires...
    dotnet add package Microsoft.AspNetCore.Cors
    dotnet add package Microsoft.EntityFrameworkCore.InMemory
    dotnet add package System.Security.Cryptography.ProtectedData
    
    cd ..
    
    echo Création des fichiers du serveur...
    call :create_server_files
)

:: Lancer le serveur
echo.
echo Démarrage du serveur sur http://localhost:8080
echo Pour arrêter le serveur, fermez cette fenêtre ou appuyez sur Ctrl+C
echo.
cd VipleDataServer
start /B dotnet run --urls "http://localhost:8080"

:: Créer un fichier pour indiquer que le serveur est démarré
echo %DATE% %TIME% > ..\server_data\server_running.txt

echo.
echo Le serveur est maintenant actif et écoute sur le port 8080
echo.
echo --------------------------------------------------
echo LOG DU SERVEUR:
echo --------------------------------------------------
echo.

:: Afficher les événements du serveur
type ..\server_data\logs\server_log.txt 2>nul
echo [%DATE% %TIME%] Serveur démarré >> ..\server_data\logs\server_log.txt

:: Boucle pour garder la fenêtre ouverte et afficher les nouveaux logs
:loop
    timeout /t 2 /nobreak > nul
    type ..\server_data\logs\server_log.txt 2>nul
    goto loop

:: Fin du script principal, ce qui suit sont des fonctions

:create_server_files
    :: Créer le contrôleur d'API pour les utilisateurs
    echo namespace VipleDataServer.Controllers > VipleDataServer\Controllers\UsersController.cs
    echo { >> VipleDataServer\Controllers\UsersController.cs
    echo     using Microsoft.AspNetCore.Mvc; >> VipleDataServer\Controllers\UsersController.cs
    echo     using System; >> VipleDataServer\Controllers\UsersController.cs
    echo     using System.Collections.Generic; >> VipleDataServer\Controllers\UsersController.cs
    echo     using System.IO; >> VipleDataServer\Controllers\UsersController.cs
    echo     using System.Threading.Tasks; >> VipleDataServer\Controllers\UsersController.cs
    echo     using System.Xml.Serialization; >> VipleDataServer\Controllers\UsersController.cs
    echo     using VipleDataServer.Models; >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo     [ApiController] >> VipleDataServer\Controllers\UsersController.cs
    echo     [Route("api/[controller]")] >> VipleDataServer\Controllers\UsersController.cs
    echo     public class UsersController : ControllerBase >> VipleDataServer\Controllers\UsersController.cs
    echo     { >> VipleDataServer\Controllers\UsersController.cs
    echo         private static readonly string UsersFilePath = "..\\server_data\\users\\users.vff"; >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo         [HttpGet] >> VipleDataServer\Controllers\UsersController.cs
    echo         public async Task^<ActionResult^<List^<User^>^>^> Get() >> VipleDataServer\Controllers\UsersController.cs
    echo         { >> VipleDataServer\Controllers\UsersController.cs
    echo             try >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 var users = await LoadUsers(); >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction("Utilisateurs récupérés"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 return Ok(users); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo             catch (Exception ex) >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction($"Erreur: {ex.Message}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 return StatusCode(500, "Une erreur est survenue lors de la récupération des utilisateurs"); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo         } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo         [HttpGet("{id}")] >> VipleDataServer\Controllers\UsersController.cs
    echo         public async Task^<ActionResult^<User^>^> Get(string id) >> VipleDataServer\Controllers\UsersController.cs
    echo         { >> VipleDataServer\Controllers\UsersController.cs
    echo             try >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 var users = await LoadUsers(); >> VipleDataServer\Controllers\UsersController.cs
    echo                 var user = users.Find(u =^> u.Id == id); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 if (user == null) >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     LogAction($"Utilisateur non trouvé: {id}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                     return NotFound(); >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction($"Utilisateur récupéré: {user.Username}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 return Ok(user); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo             catch (Exception ex) >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction($"Erreur: {ex.Message}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 return StatusCode(500, "Une erreur est survenue lors de la récupération de l'utilisateur"); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo         } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo         [HttpPost] >> VipleDataServer\Controllers\UsersController.cs
    echo         public async Task^<ActionResult^<User^>^> Post([FromBody] UserCreateRequest request) >> VipleDataServer\Controllers\UsersController.cs
    echo         { >> VipleDataServer\Controllers\UsersController.cs
    echo             try >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 var users = await LoadUsers(); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 // Vérifier si le nom d'utilisateur existe déjà >> VipleDataServer\Controllers\UsersController.cs
    echo                 if (users.Any(u =^> u.Username.ToLower() == request.User.Username.ToLower())) >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     LogAction($"Création utilisateur échouée: Le nom d'utilisateur existe déjà: {request.User.Username}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                     return BadRequest("Ce nom d'utilisateur existe déjà"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 // Générer le sel et hacher le mot de passe >> VipleDataServer\Controllers\UsersController.cs
    echo                 request.User.PasswordSalt = GenerateSalt(); >> VipleDataServer\Controllers\UsersController.cs
    echo                 request.User.PasswordHash = HashPassword(request.Password, request.User.PasswordSalt); >> VipleDataServer\Controllers\UsersController.cs
    echo                 request.User.Id = Guid.NewGuid().ToString(); >> VipleDataServer\Controllers\UsersController.cs
    echo                 request.User.CreationDate = DateTime.Now; >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 // Ajouter l'utilisateur à la liste >> VipleDataServer\Controllers\UsersController.cs
    echo                 users.Add(request.User); >> VipleDataServer\Controllers\UsersController.cs
    echo                 await SaveUsers(users); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction($"Utilisateur créé: {request.User.Username}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 return CreatedAtAction(nameof(Get), new { id = request.User.Id }, request.User); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo             catch (Exception ex) >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction($"Erreur: {ex.Message}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 return StatusCode(500, "Une erreur est survenue lors de la création de l'utilisateur"); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo         } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo         [HttpPut("{id}")] >> VipleDataServer\Controllers\UsersController.cs
    echo         public async Task^<ActionResult^<User^>^> Put(string id, [FromBody] UserUpdateRequest request) >> VipleDataServer\Controllers\UsersController.cs
    echo         { >> VipleDataServer\Controllers\UsersController.cs
    echo             try >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 var users = await LoadUsers(); >> VipleDataServer\Controllers\UsersController.cs
    echo                 var index = users.FindIndex(u =^> u.Id == id); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 if (index == -1) >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     LogAction($"Mise à jour utilisateur échouée: Utilisateur non trouvé: {id}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                     return NotFound(); >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 // Mettre à jour l'utilisateur >> VipleDataServer\Controllers\UsersController.cs
    echo                 if (!string.IsNullOrEmpty(request.Password)) >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     // Si un nouveau mot de passe est fourni, le hacher >> VipleDataServer\Controllers\UsersController.cs
    echo                     request.User.PasswordSalt = GenerateSalt(); >> VipleDataServer\Controllers\UsersController.cs
    echo                     request.User.PasswordHash = HashPassword(request.Password, request.User.PasswordSalt); >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo                 else >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     // Conserver le mot de passe existant >> VipleDataServer\Controllers\UsersController.cs
    echo                     request.User.PasswordHash = users[index].PasswordHash; >> VipleDataServer\Controllers\UsersController.cs
    echo                     request.User.PasswordSalt = users[index].PasswordSalt; >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 // Conserver certaines propriétés >> VipleDataServer\Controllers\UsersController.cs
    echo                 request.User.Id = id; >> VipleDataServer\Controllers\UsersController.cs
    echo                 request.User.CreationDate = users[index].CreationDate; >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 // Remplacer l'utilisateur >> VipleDataServer\Controllers\UsersController.cs
    echo                 users[index] = request.User; >> VipleDataServer\Controllers\UsersController.cs
    echo                 await SaveUsers(users); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction($"Utilisateur mis à jour: {request.User.Username}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 return Ok(request.User); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo             catch (Exception ex) >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction($"Erreur: {ex.Message}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 return StatusCode(500, "Une erreur est survenue lors de la mise à jour de l'utilisateur"); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo         } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo         [HttpDelete("{id}")] >> VipleDataServer\Controllers\UsersController.cs
    echo         public async Task^<ActionResult^> Delete(string id) >> VipleDataServer\Controllers\UsersController.cs
    echo         { >> VipleDataServer\Controllers\UsersController.cs
    echo             try >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 var users = await LoadUsers(); >> VipleDataServer\Controllers\UsersController.cs
    echo                 var user = users.Find(u =^> u.Id == id); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 if (user == null) >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     LogAction($"Suppression utilisateur échouée: Utilisateur non trouvé: {id}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                     return NotFound(); >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 // Vérifier s'il s'agit du dernier administrateur >> VipleDataServer\Controllers\UsersController.cs
    echo                 if (user.Role == UserRole.Administrator && >> VipleDataServer\Controllers\UsersController.cs
    echo                     users.Count(u =^> u.Role == UserRole.Administrator) ^<= 1) >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     LogAction("Suppression utilisateur échouée: Impossible de supprimer le dernier administrateur"); >> VipleDataServer\Controllers\UsersController.cs
    echo                     return BadRequest("Impossible de supprimer le dernier administrateur"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 users.Remove(user); >> VipleDataServer\Controllers\UsersController.cs
    echo                 await SaveUsers(users); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction($"Utilisateur supprimé: {user.Username}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 return Ok(); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo             catch (Exception ex) >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction($"Erreur: {ex.Message}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 return StatusCode(500, "Une erreur est survenue lors de la suppression de l'utilisateur"); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo         } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo         [HttpPost("authenticate")] >> VipleDataServer\Controllers\UsersController.cs
    echo         public async Task^<ActionResult^<User^>^> Authenticate([FromBody] AuthRequest request) >> VipleDataServer\Controllers\UsersController.cs
    echo         { >> VipleDataServer\Controllers\UsersController.cs
    echo             try >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 var users = await LoadUsers(); >> VipleDataServer\Controllers\UsersController.cs
    echo                 var user = users.Find(u =^> u.Username.ToLower() == request.Username.ToLower()); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 if (user == null) >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     LogAction($"Authentification échouée: Utilisateur non trouvé: {request.Username}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                     return Unauthorized(); >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 // Vérifier si l'utilisateur est actif >> VipleDataServer\Controllers\UsersController.cs
    echo                 if (!user.IsActive) >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     LogAction($"Authentification échouée: Compte inactif: {request.Username}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                     return Unauthorized("Compte inactif"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 // Vérifier le mot de passe >> VipleDataServer\Controllers\UsersController.cs
    echo                 string hashedPassword = HashPassword(request.Password, user.PasswordSalt); >> VipleDataServer\Controllers\UsersController.cs
    echo                 if (hashedPassword != user.PasswordHash) >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     LogAction($"Authentification échouée: Mot de passe incorrect pour {request.Username}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                     return Unauthorized(); >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 // Mettre à jour la date de dernière connexion >> VipleDataServer\Controllers\UsersController.cs
    echo                 user.LastLogin = DateTime.Now; >> VipleDataServer\Controllers\UsersController.cs
    echo                 await SaveUsers(users); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction($"Authentification réussie pour {request.Username}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 return Ok(user); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo             catch (Exception ex) >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction($"Erreur: {ex.Message}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 return StatusCode(500, "Une erreur est survenue lors de l'authentification"); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo         } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo         // Méthodes utilitaires >> VipleDataServer\Controllers\UsersController.cs
    echo         private async Task^<List^<User^>^> LoadUsers() >> VipleDataServer\Controllers\UsersController.cs
    echo         { >> VipleDataServer\Controllers\UsersController.cs
    echo             try >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 Directory.CreateDirectory(Path.GetDirectoryName(UsersFilePath)); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 if (System.IO.File.Exists(UsersFilePath)) >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     using (StreamReader reader = new StreamReader(UsersFilePath)) >> VipleDataServer\Controllers\UsersController.cs
    echo                     { >> VipleDataServer\Controllers\UsersController.cs
    echo                         XmlSerializer serializer = new XmlSerializer(typeof(List^<User^>)); >> VipleDataServer\Controllers\UsersController.cs
    echo                         return (List^<User^>)serializer.Deserialize(reader); >> VipleDataServer\Controllers\UsersController.cs
    echo                     } >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo                 else >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     // Créer un utilisateur administrateur par défaut >> VipleDataServer\Controllers\UsersController.cs
    echo                     var users = new List^<User^>(); >> VipleDataServer\Controllers\UsersController.cs
    echo                     var adminUser = new User >> VipleDataServer\Controllers\UsersController.cs
    echo                     { >> VipleDataServer\Controllers\UsersController.cs
    echo                         Id = Guid.NewGuid().ToString(), >> VipleDataServer\Controllers\UsersController.cs
    echo                         Username = "admin", >> VipleDataServer\Controllers\UsersController.cs
    echo                         FullName = "Administrateur", >> VipleDataServer\Controllers\UsersController.cs
    echo                         Email = "admin@viple.fr", >> VipleDataServer\Controllers\UsersController.cs
    echo                         Role = UserRole.Administrator, >> VipleDataServer\Controllers\UsersController.cs
    echo                         IsActive = true, >> VipleDataServer\Controllers\UsersController.cs
    echo                         CreationDate = DateTime.Now, >> VipleDataServer\Controllers\UsersController.cs
    echo                         PasswordSalt = GenerateSalt() >> VipleDataServer\Controllers\UsersController.cs
    echo                     }; >> VipleDataServer\Controllers\UsersController.cs
    echo                     adminUser.PasswordHash = HashPassword("admin", adminUser.PasswordSalt); >> VipleDataServer\Controllers\UsersController.cs
    echo                     users.Add(adminUser); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                     await SaveUsers(users); >> VipleDataServer\Controllers\UsersController.cs
    echo                     return users; >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo             catch (Exception ex) >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction($"Erreur lors du chargement des utilisateurs: {ex.Message}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 throw; >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo         } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo         private async Task SaveUsers(List^<User^> users) >> VipleDataServer\Controllers\UsersController.cs
    echo         { >> VipleDataServer\Controllers\UsersController.cs
    echo             try >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 Directory.CreateDirectory(Path.GetDirectoryName(UsersFilePath)); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 using (StreamWriter writer = new StreamWriter(UsersFilePath)) >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     XmlSerializer serializer = new XmlSerializer(typeof(List^<User^>)); >> VipleDataServer\Controllers\UsersController.cs
    echo                     serializer.Serialize(writer, users); >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo             catch (Exception ex) >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 LogAction($"Erreur lors de l'enregistrement des utilisateurs: {ex.Message}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 throw; >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo         } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo         private string GenerateSalt() >> VipleDataServer\Controllers\UsersController.cs
    echo         { >> VipleDataServer\Controllers\UsersController.cs
    echo             byte[] saltBytes = new byte[32]; >> VipleDataServer\Controllers\UsersController.cs
    echo             using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create()) >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 rng.GetBytes(saltBytes); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo             return Convert.ToBase64String(saltBytes); >> VipleDataServer\Controllers\UsersController.cs
    echo         } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo         private string HashPassword(string password, string salt) >> VipleDataServer\Controllers\UsersController.cs
    echo         { >> VipleDataServer\Controllers\UsersController.cs
    echo             byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password); >> VipleDataServer\Controllers\UsersController.cs
    echo             byte[] saltBytes = Convert.FromBase64String(salt); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo             byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes.Length]; >> VipleDataServer\Controllers\UsersController.cs
    echo             Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length); >> VipleDataServer\Controllers\UsersController.cs
    echo             Buffer.BlockCopy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo             using (var sha256 = System.Security.Cryptography.SHA256.Create()) >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 byte[] hashBytes = sha256.ComputeHash(combinedBytes); >> VipleDataServer\Controllers\UsersController.cs
    echo                 return Convert.ToBase64String(hashBytes); >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo         } >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo         private void LogAction(string message) >> VipleDataServer\Controllers\UsersController.cs
    echo         { >> VipleDataServer\Controllers\UsersController.cs
    echo             try >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 string logPath = "..\\server_data\\logs\\server_log.txt"; >> VipleDataServer\Controllers\UsersController.cs
    echo                 Directory.CreateDirectory(Path.GetDirectoryName(logPath)); >> VipleDataServer\Controllers\UsersController.cs
    echo. >> VipleDataServer\Controllers\UsersController.cs
    echo                 using (StreamWriter writer = new StreamWriter(logPath, true)) >> VipleDataServer\Controllers\UsersController.cs
    echo                 { >> VipleDataServer\Controllers\UsersController.cs
    echo                     writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}"); >> VipleDataServer\Controllers\UsersController.cs
    echo                 } >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo             catch >> VipleDataServer\Controllers\UsersController.cs
    echo             { >> VipleDataServer\Controllers\UsersController.cs
    echo                 // Ignorer les erreurs de journalisation >> VipleDataServer\Controllers\UsersController.cs
    echo             } >> VipleDataServer\Controllers\UsersController.cs
    echo         } >> VipleDataServer\Controllers\UsersController.cs
    echo     } >> VipleDataServer\Controllers\UsersController.cs
    echo } >> VipleDataServer\Controllers\UsersController.cs

    :: Créer les modèles
    mkdir VipleDataServer\Models
    
    echo using System; > VipleDataServer\Models\User.cs
    echo using System.Collections.Generic; >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo namespace VipleDataServer.Models >> VipleDataServer\Models\User.cs
    echo { >> VipleDataServer\Models\User.cs
    echo     /// ^<summary^> >> VipleDataServer\Models\User.cs
    echo     /// Rôles disponibles pour les utilisateurs >> VipleDataServer\Models\User.cs
    echo     /// ^</summary^> >> VipleDataServer\Models\User.cs
    echo     public enum UserRole >> VipleDataServer\Models\User.cs
    echo     { >> VipleDataServer\Models\User.cs
    echo         Administrator,   // Administrateur avec tous les droits >> VipleDataServer\Models\User.cs
    echo         Manager,         // Gestionnaire avec droits étendus >> VipleDataServer\Models\User.cs
    echo         Technician,      // Technicien avec accès limité >> VipleDataServer\Models\User.cs
    echo         Support,         // Support avec accès client >> VipleDataServer\Models\User.cs
    echo         Viewer           // Visualisation uniquement >> VipleDataServer\Models\User.cs
    echo     } >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo     /// ^<summary^> >> VipleDataServer\Models\User.cs
    echo     /// Représente un utilisateur du système >> VipleDataServer\Models\User.cs
    echo     /// ^</summary^> >> VipleDataServer\Models\User.cs
    echo     [Serializable] >> VipleDataServer\Models\User.cs
    echo     public class User >> VipleDataServer\Models\User.cs
    echo     { >> VipleDataServer\Models\User.cs
    echo         /// ^<summary^> >> VipleDataServer\Models\User.cs
    echo         /// Identifiant unique de l'utilisateur >> VipleDataServer\Models\User.cs
    echo         /// ^</summary^> >> VipleDataServer\Models\User.cs
    echo         public string Id { get; set; } = Guid.NewGuid().ToString(); >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo         /// ^<summary^> >> VipleDataServer\Models\User.cs
    echo         /// Nom d'utilisateur (login) >> VipleDataServer\Models\User.cs
    echo         /// ^</summary^> >> VipleDataServer\Models\User.cs
    echo         public string Username { get; set; } = string.Empty; >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo         /// ^<summary^> >> VipleDataServer\Models\User.cs
    echo         /// Mot de passe haché >> VipleDataServer\Models\User.cs
    echo         /// ^</summary^> >> VipleDataServer\Models\User.cs
    echo         public string PasswordHash { get; set; } = string.Empty; >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo         /// ^<summary^> >> VipleDataServer\Models\User.cs
    echo         /// Sel utilisé pour le hachage du mot de passe >> VipleDataServer\Models\User.cs
    echo         /// ^</summary^> >> VipleDataServer\Models\User.cs
    echo         public string PasswordSalt { get; set; } = string.Empty; >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo         /// ^<summary^> >> VipleDataServer\Models\User.cs
    echo         /// Nom complet de l'utilisateur >> VipleDataServer\Models\User.cs
    echo         /// ^</summary^> >> VipleDataServer\Models\User.cs
    echo         public string FullName { get; set; } = string.Empty; >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo         /// ^<summary^> >> VipleDataServer\Models\User.cs
    echo         /// Email de l'utilisateur >> VipleDataServer\Models\User.cs
    echo         /// ^</summary^> >> VipleDataServer\Models\User.cs
    echo         public string Email { get; set; } = string.Empty; >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo         /// ^<summary^> >> VipleDataServer\Models\User.cs
    echo         /// Rôle de l'utilisateur >> VipleDataServer\Models\User.cs
    echo         /// ^</summary^> >> VipleDataServer\Models\User.cs
    echo         public UserRole Role { get; set; } = UserRole.Viewer; >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo         /// ^<summary^> >> VipleDataServer\Models\User.cs
    echo         /// Indique si l'utilisateur est actif >> VipleDataServer\Models\User.cs
    echo         /// ^</summary^> >> VipleDataServer\Models\User.cs
    echo         public bool IsActive { get; set; } = true; >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo         /// ^<summary^> >> VipleDataServer\Models\User.cs
    echo         /// Date de dernière connexion >> VipleDataServer\Models\User.cs
    echo         /// ^</summary^> >> VipleDataServer\Models\User.cs
    echo         public DateTime LastLogin { get; set; } = DateTime.MinValue; >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo         /// ^<summary^> >> VipleDataServer\Models\User.cs
    echo         /// Date de création du compte >> VipleDataServer\Models\User.cs
    echo         /// ^</summary^> >> VipleDataServer\Models\User.cs
    echo         public DateTime CreationDate { get; set; } = DateTime.Now; >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo         /// ^<summary^> >> VipleDataServer\Models\User.cs
    echo         /// Préférences utilisateur (thème, langue, etc.) >> VipleDataServer\Models\User.cs
    echo         /// ^</summary^> >> VipleDataServer\Models\User.cs
    echo         public Dictionary^<string, string^> Preferences { get; set; } = new Dictionary^<string, string^>(); >> VipleDataServer\Models\User.cs
    echo     } >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo     public class UserCreateRequest >> VipleDataServer\Models\User.cs
    echo     { >> VipleDataServer\Models\User.cs
    echo         public User User { get; set; } >> VipleDataServer\Models\User.cs
    echo         public string Password { get; set; } >> VipleDataServer\Models\User.cs
    echo     } >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo     public class UserUpdateRequest >> VipleDataServer\Models\User.cs
    echo     { >> VipleDataServer\Models\User.cs
    echo         public User User { get; set; } >> VipleDataServer\Models\User.cs
    echo         public string Password { get; set; } >> VipleDataServer\Models\User.cs
    echo     } >> VipleDataServer\Models\User.cs
    echo. >> VipleDataServer\Models\User.cs
    echo     public class AuthRequest >> VipleDataServer\Models\User.cs
    echo     { >> VipleDataServer\Models\User.cs
    echo         public string Username { get; set; } >> VipleDataServer\Models\User.cs
    echo         public string Password { get; set; } >> VipleDataServer\Models\User.cs
    echo     } >> VipleDataServer\Models\User.cs
    echo } >> VipleDataServer\Models\User.cs

    :: Modifier Program.cs pour activer CORS
    echo using Microsoft.AspNetCore.Builder; > VipleDataServer\Program.cs
    echo using Microsoft.Extensions.DependencyInjection; >> VipleDataServer\Program.cs
    echo using Microsoft.Extensions.Hosting; >> VipleDataServer\Program.cs
    echo using System.Text.Json.Serialization; >> VipleDataServer\Program.cs
    echo. >> VipleDataServer\Program.cs
    echo var builder = WebApplication.CreateBuilder(args); >> VipleDataServer\Program.cs
    echo. >> VipleDataServer\Program.cs
    echo // Add services to the container. >> VipleDataServer\Program.cs
    echo builder.Services.AddControllers().AddJsonOptions(options =^> >> VipleDataServer\Program.cs
    echo { >> VipleDataServer\Program.cs
    echo     options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); >> VipleDataServer\Program.cs
    echo     options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; >> VipleDataServer\Program.cs
    echo }); >> VipleDataServer\Program.cs
    echo. >> VipleDataServer\Program.cs
    echo // Ajouter CORS pour permettre à l'application cliente de se connecter >> VipleDataServer\Program.cs
    echo builder.Services.AddCors(options =^> >> VipleDataServer\Program.cs
    echo { >> VipleDataServer\Program.cs
    echo     options.AddPolicy("AllowAll", builder =^> >> VipleDataServer\Program.cs
    echo     { >> VipleDataServer\Program.cs
    echo         builder.AllowAnyOrigin() >> VipleDataServer\Program.cs
    echo                .AllowAnyMethod() >> VipleDataServer\Program.cs
    echo                .AllowAnyHeader(); >> VipleDataServer\Program.cs
    echo     }); >> VipleDataServer\Program.cs
    echo }); >> VipleDataServer\Program.cs
    echo. >> VipleDataServer\Program.cs
    echo builder.Services.AddEndpointsApiExplorer(); >> VipleDataServer\Program.cs
    echo builder.Services.AddSwaggerGen(); >> VipleDataServer\Program.cs
    echo. >> VipleDataServer\Program.cs
    echo var app = builder.Build(); >> VipleDataServer\Program.cs
    echo. >> VipleDataServer\Program.cs
    echo // Configure the HTTP request pipeline. >> VipleDataServer\Program.cs
    echo if (app.Environment.IsDevelopment()) >> VipleDataServer\Program.cs
    echo { >> VipleDataServer\Program.cs
    echo     app.UseSwagger(); >> VipleDataServer\Program.cs
    echo     app.UseSwaggerUI(); >> VipleDataServer\Program.cs
    echo } >> VipleDataServer\Program.cs
    echo. >> VipleDataServer\Program.cs
    echo app.UseCors("AllowAll"); >> VipleDataServer\Program.cs
    echo. >> VipleDataServer\Program.cs
    echo app.UseAuthorization(); >> VipleDataServer\Program.cs
    echo. >> VipleDataServer\Program.cs
    echo app.MapControllers(); >> VipleDataServer\Program.cs
    echo. >> VipleDataServer\Program.cs
    echo app.Run(); >> VipleDataServer\Program.cs

    echo "Fichiers du serveur créés avec succès."
    exit /b 0