#define AppName        GetStringFileInfo('..\Binaries\Bimil.exe', 'ProductName')
#define AppVersion     GetStringFileInfo('..\Binaries\Bimil.exe', 'ProductVersion')
#define AppFileVersion GetStringFileInfo('..\Binaries\Bimil.exe', 'FileVersion')
#define AppCompany     GetStringFileInfo('..\Binaries\Bimil.exe', 'CompanyName')
#define AppCopyright   GetStringFileInfo('..\Binaries\Bimil.exe', 'LegalCopyright')
#define AppBase        LowerCase(StringChange(AppName, ' ', ''))
#define AppSetupFile   AppBase + StringChange(AppVersion, '.', '')

#define AppVersionEx   StringChange(AppVersion, '0.00', '')
#ifdef VersionHash
#  if "" != VersionHash
#    define AppVersionEx AppVersionEx + " (" + VersionHash + ")"
#  endif
#endif


[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppCompany}
AppPublisherURL=https://medo64.com/{#AppBase}/
AppCopyright={#AppCopyright}
VersionInfoProductVersion={#AppVersion}
VersionInfoProductTextVersion={#AppVersionEx}
VersionInfoVersion={#AppFileVersion}
DefaultDirName={commonpf}\{#AppCompany}\{#AppName}
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
MinVersion=0,6.0
PrivilegesRequired=admin
ShowLanguageDialog=no
SolidCompression=yes
ChangesAssociations=yes
DisableWelcomePage=yes
LicenseFile=..\Setup\License.rtf


[Messages]
SetupAppTitle=Setup {#AppName} {#AppVersionEx}
SetupWindowTitle=Setup {#AppName} {#AppVersionEx}
BeveledLabel=medo64.com


[Tasks]
Name: extension_psafe3;  GroupDescription: "Associate additional extension:";  Description: "Password Safe 3.x (.psafe3)";  Flags: unchecked;


[Files]
Source: "Bimil.exe";      DestDir: "{app}";                            Flags: ignoreversion;
Source: "Bimil.pdb";      DestDir: "{app}";                            Flags: ignoreversion;
Source: "..\README.md";   DestDir: "{app}";  DestName: "ReadMe.txt";   Flags: overwritereadonly uninsremovereadonly;  Attribs: readonly;
Source: "..\LICENSE.md";  DestDir: "{app}";  DestName: "License.txt";  Flags: overwritereadonly uninsremovereadonly;  Attribs: readonly;
Source: "..\WORDS.md";    DestDir: "{app}";  DestName: "Words.txt";    Flags: overwritereadonly uninsremovereadonly;  Attribs: readonly;


[Icons]
Name: "{userstartmenu}\Bimil"; Filename: "{app}\Bimil.exe"


[Registry]
Root: HKCU; Subkey: "Software\Josip Medved\Bimil"; ValueType: dword; ValueName: "Installed"; ValueData: "1"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Josip Medved"; Flags: uninsdeletekeyifempty

Root: HKCR; Subkey: ".bimil"; ValueType: string; ValueName: ""; ValueData: "BimilFile"; Flags: uninsclearvalue;
Root: HKCR; Subkey: ".psafe3"; ValueType: string; ValueName: ""; ValueData: "BimilFile"; Flags: uninsclearvalue; Tasks: extension_psafe3;

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
