; VANTA: BLACK HORIZON - Windows installer
; Requires Inno Setup 6 and a successful Unity Windows x64 build in Builds\\Windows.

#define MyAppName "VANTA: BLACK HORIZON"
#define MyAppVersion "0.1.0"
#define MyAppPublisher "VANTA"
#define MyAppExeName "VANTA_BLACK_HORIZON.exe"

[Setup]
AppId={{7E4F4C4A-2A2D-4B20-A5A5-7C7E0A7D5B11}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\VANTA BLACK HORIZON
DefaultGroupName={#MyAppName}
OutputDir=Builds\Installer
OutputBaseFilename=VANTA_BLACK_HORIZON_Setup
Compression=lzma2
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64compatible
WizardStyle=modern
Uninstallable=yes
DisableProgramGroupPage=yes

[Files]
Source: "..\Builds\Windows\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion

[Icons]
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent
