@ECHO OFF
SETLOCAL EnableDelayedExpansion

SET TOOLS_MSBUILD="%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\amd64\msbuild.exe"
SET   TOOLS_NUGET="%USERPROFILE%\Downloads\nuget.exe"


ECHO --- DISCOVER TOOLS
ECHO:

FOR %%I IN (%TOOLS_MSBUILD%) DO (
    IF EXIST %%I IF NOT DEFINED TOOL_MSBUILD SET TOOL_MSBUILD=%%I
)
ECHO MSBuild: %TOOL_MSBUILD%
IF [%TOOL_MSBUILD%]==[] ECHO Not found^^! & GOTO Error

FOR %%I IN (%TOOLS_NUGET%) DO (
    IF EXIST %%I IF NOT DEFINED TOOL_NUGET SET TOOL_NUGET=%%I
)
ECHO NuGet: %TOOL_NUGET%
IF [%TOOL_NUGET%]==[] ECHO Not found^^! & GOTO Error

ECHO:
ECHO:


ECHO --- PACK
ECHO:

RMDIR /Q /S ".\Temp" 2> NUL
MKDIR ".\Temp"

DEL "..\Binaries\*.0.0.0.nupkg" 2> NUL

%TOOL_MSBUILD% ..\Source\PasswordSafe\PasswordSafe.csproj /t:pack /p:IncludeSymbols=true /p:IncludeSource=true /p:Configuration=Release /p:OutputPath=..\..\Setup\Temp\

ECHO:
ECHO:


IF NOT [%TOOL_NUGET%]==[] (
    ECHO --- PUSH
    ECHO:

    ECHO Set API key if needed
    ECHO %TOOL_NUGET% SetApiKey ^<key^>
    ECHO:
    IF EXIST ".\Temp\*.0.0.0.nupkg" (
        ECHO "Not pushing unversioned package."
    ) ELSE (
        ECHO Proceed with pushing packaged to Internet^?
        PAUSE
        IF EXIST ".\Temp\*.nupkg" (
            MKDIR "..\Releases" 2> NUL
            FOR %%I IN (".\Temp\*.nupkg") DO (
                ECHO:
                SET FILE_NAME=%%~nI%%~xI
                IF [!FILE_NAME:~-14!]==[.symbols.nupkg] (
                    ECHO Pushing symbols in !FILE_NAME!
                    %TOOL_NUGET% push .\Temp\!FILE_NAME! -source https://nuget.smbsrc.net/
                ) ELSE (
                    ECHO Pushing package in !FILE_NAME!
                    %TOOL_NUGET% push .\Temp\!FILE_NAME! -source https://nuget.org/
                )
                MOVE .\Temp\!FILE_NAME! ..\Releases\!FILE_NAME! > NUL
            )
            RMDIR /Q /S ".\Temp" 2> NUL
            
            
        ) ELSE (
            ECHO "No packages found."
        )
    )
)


ENDLOCAL
EXIT /B 0


:Error
ENDLOCAL
PAUSE
EXIT /B 1
