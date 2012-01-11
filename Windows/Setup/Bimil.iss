[Setup]
AppName=Bimil
AppVerName=Bimil 1.00 (alpha)
DefaultDirName={pf}\Josip Medved\Bimil
OutputBaseFilename=bimil100a5
OutputDir=..\Releases
SourceDir=..\Binaries
AppId=JosipMedved_Bimil
AppMutex=Global\JosipMedved_Bimil
AppPublisher=Josip Medved
AppPublisherURL=http://www.jmedved.com/bimil/
UninstallDisplayIcon={app}\Bimil.exe
AlwaysShowComponentsList=no
ArchitecturesInstallIn64BitMode=x64
DisableProgramGroupPage=yes
MergeDuplicateFiles=yes
MinVersion=0,5.1.2600
PrivilegesRequired=admin
ShowLanguageDialog=no
SolidCompression=yes
ChangesAssociations=yes
DisableWelcomePage=yes

[Files]
Source: "Bimil.exe";  DestDir: "{app}";  Flags: ignoreversion;

[Icons]
Name: "{userstartmenu}\Bimil"; Filename: "{app}\Bimil.exe"

[Registry]
Root: HKCU; Subkey: "Software\Josip Medved\Bimil"; ValueType: dword; ValueName: "Installed"; ValueData: "1"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Josip Medved"; Flags: uninsdeletekeyifempty

Root: HKCR; Subkey: ".bimil"; ValueType: string; ValueName: ""; ValueData: "BimilFile"; Flags: uninsclearvalue;

Root: HKCR; Subkey: "BimilFile";                     ValueType: none;                                                                             Flags: uninsdeletekey;
Root: HKCR; Subkey: "BimilFile";                     ValueType: string; ValueName: "";                  ValueData: "Bimil";
Root: HKCR; Subkey: "BimilFile\DefaultIcon";         ValueType: string; ValueName: "";                  ValueData: "{app}\Bimil.exe";
Root: HKCR; Subkey: "BimilFile\shell\Open";          ValueType: string; ValueName: "MultiSelectModel";  ValueData: "Player";
Root: HKCR; Subkey: "BimilFile\shell\Open\command";  ValueType: string; ValueName: "";                  ValueData: """{app}\Bimil.exe"" ""%1""";

[Run]
Filename: "{app}\Bimil.exe";  Description: "Launch application now";  Flags: postinstall nowait skipifsilent runasoriginaluser unchecked
