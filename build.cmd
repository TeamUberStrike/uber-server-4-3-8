REM filepath: c:\Uber\code\Client\cmune-backend-master\cmune-backend-master\build.cmd
@echo off

setlocal enabledelayedexpansion

set "Configuration=%~1"
if "%Configuration%"=="" set "Configuration=Debug"

set "ArtifactDir=%~2"
if "%ArtifactDir%"=="" set "ArtifactDir=Artifacts"

REM Check if we're already in Developer Command Prompt
where msbuild >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Using MSBuild from PATH
    set "MSBUILD=msbuild"
    goto :build
)

REM Find and launch Developer Command Prompt environment
set "VSDEVCMD="
set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"

if exist "%VSWHERE%" (
    for /f "usebackq tokens=*" %%i in (`"%VSWHERE%" -latest -requires Microsoft.Component.MSBuild -property installationPath`) do (
        set "VSDEVCMD=%%i\Common7\Tools\VsDevCmd.bat"
    )
)

if exist "%VSDEVCMD%" (
    echo Initializing Visual Studio environment...
    call "%VSDEVCMD%" -no_logo
    set "MSBUILD=msbuild"
) else (
    echo ERROR: Visual Studio Developer Command Prompt not found!
    exit /b 1
)

:build

REM Create absolute path for display, but keep relative for MSBuild
set "ArtifactDirFull=%CD%\%ArtifactDir%"
if not exist "%ArtifactDir%" mkdir "%ArtifactDir%"

echo.
echo ========================================
echo Building all .msbuild projects...
echo ========================================
echo Configuration: %Configuration%
echo ArtifactDir: %ArtifactDirFull%
echo.

set SUCCESS=0
set FAILED=0
set TOTAL=0
set "FAILED_PROJECTS="

for /f "delims=" %%f in ('dir /s /b *.msbuild') do (
    set /a TOTAL+=1
    echo [!TOTAL!] Building: %%~nxf
    echo      Path: %%f
    
    REM Pass relative path to avoid path combination issues
    echo BUILD COMMAND: %MSBUILD% "%%f" /p:Configuration=%Configuration% /p:ArtifactDir=%ArtifactDir% /v:minimal /nologo
    %MSBUILD% "%%f" /p:Configuration=%Configuration% /p:ArtifactDir=%ArtifactDir% /v:minimal /nologo
    
    if !ERRORLEVEL! EQU 0 (
        echo      [SUCCESS] %%~nxf
        set /a SUCCESS+=1
    ) else (
        echo      [FAILED] %%~nxf
        set /a FAILED+=1
        set "FAILED_PROJECTS=!FAILED_PROJECTS!  - %%~nxf^
"
    )
    echo.
)

echo ========================================
echo Build Summary
echo ========================================
echo Total projects: %TOTAL%
echo Successful: %SUCCESS%
echo Failed: %FAILED%
echo Artifacts location: %ArtifactDirFull%
echo.

if %FAILED% GTR 0 (
    echo Failed projects:
    echo %FAILED_PROJECTS%
    echo Some builds failed. [ERROR]
    exit /b 1
) else (
    echo All builds completed successfully! [OK]
    exit /b 0
)