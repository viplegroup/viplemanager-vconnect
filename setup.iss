; Viple FilesVersion - setup.iss 1.0.0 - Date 26/06/2025
; Application créée par Viple SAS

#define MyAppName "Viple Management"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Viple SAS"
#define MyAppURL "https://www.viple.fr"
#define MyAppExeName "VipleManagement.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
AppId={{1E9C5B0F-E01E-4895-A814-834625ABB1A0}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
OutputDir=installer
OutputBaseFilename=VipleManagementSetup
Compression=lzma
SolidCompression=yes
SetupIconFile=viple_ressources\images\viple.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
WizardStyle=modern

[Languages]
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "builds\release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent