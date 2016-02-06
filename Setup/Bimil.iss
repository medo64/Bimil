#define AppName        GetStringFileInfo('..\Binaries\Bimil.exe', 'ProductName')
#define AppVersion     GetStringFileInfo('..\Binaries\Bimil.exe', 'ProductVersion')
#define AppFileVersion GetStringFileInfo('..\Binaries\Bimil.exe', 'FileVersion')
#define AppCompany     GetStringFileInfo('..\Binaries\Bimil.exe', 'CompanyName')
#define AppCopyright   GetStringFileInfo('..\Binaries\Bimil.exe', 'LegalCopyright')
#define AppBase        LowerCase(StringChange(AppName, ' ', ''))
#define AppSetupFile   AppBase + StringChange(AppVersion, '.', '')

#define AppVersionEx   StringChange(AppVersion, '0.00', '')
#if "" != VersionHash
#  define AppVersionEx AppVersionEx + " (" + VersionHash + ")"
#endif


[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppCompany}
AppPublisherURL=http://jmedved.com/{#AppBase}/
AppCopyright={#AppCopyright}
VersionInfoProductVersion={#AppVersion}
VersionInfoProductTextVersion={#AppVersionEx}
VersionInfoVersion={#AppFileVersion}
DefaultDirName={pf}\{#AppCompany}\{#AppName}
OutputBaseFilename={#AppSetupFile}
OutputDir=..\Releases
SourceDir=..\Binaries
AppId=JosipMedved_Bimil
CloseApplications="yes"
RestartApplications="no"
AppMutex=Global\JosipMedved_Bimil
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
LicenseFile=..\Setup\License.rtf


[Messages]
SetupAppTitle=Setup {#AppName} {#AppVersionEx}
SetupWindowTitle=Setup {#AppName} {#AppVersionEx}
BeveledLabel=jmedved.com


[Files]
Source: "Bimil.exe";   DestDir: "{app}";  Flags: ignoreversion;
Source: "Bimil.pdb";   DestDir: "{app}";  Flags: ignoreversion;
Source: "ReadMe.txt";  DestDir: "{app}";  Flags: overwritereadonly uninsremovereadonly;  Attribs: readonly;


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
Filename: "{app}\Bimil.exe";   Flags: postinstall nowait skipifsilent runasoriginaluser;                      Description: "Launch application now";
Filename: "{app}\ReadMe.txt";  Flags: postinstall nowait skipifsilent runasoriginaluser unchecked shellexec;  Description: "View ReadMe.txt";


[Code]

procedure InitializeWizard;
begin
  WizardForm.LicenseAcceptedRadio.Checked := True;
end;