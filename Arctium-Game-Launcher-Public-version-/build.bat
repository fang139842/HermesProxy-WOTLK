@echo off
chcp 65001 >nul 2>&1
setlocal enabledelayedexpansion

:: ============================================================
::  Arctium Game Launcher - 编译脚本
:: ============================================================

set "SCRIPT_DIR=%~dp0"
set "PROJECT_DIR=%SCRIPT_DIR%src"
set "BUILD_OUTPUT=%SCRIPT_DIR%build\Release\bin"

:: Color codes (Windows 10/11 only)
for /f %%A in ('"prompt $E& for %%B in (.) do rem"') do set "ESC=%%A"
set "RED=%ESC%[91m"
set "GREEN=%ESC%[92m"
set "YELLOW=%ESC%[93m"
set "CYAN=%ESC%[96m"
set "RESET=%ESC%[0m"

echo.
echo %CYAN%========================================%RESET%
echo %CYAN%   Arctium Game Launcher - 编译工具   %RESET%
echo %CYAN%========================================%RESET%
echo.
echo  [1] Debug 编译      (多文件，适合开发调试)
echo  [2] Release 编译    (多文件，适合发布测试)
echo  [3] 单文件 EXE      (自包含，推荐使用)
echo  [4] 全部编译        (以上全部)
echo.
set /p CHOICE="请选择编译模式 [1-4] (直接回车默认=3): "

if "!CHOICE!"=="" set CHOICE=3

echo.

:: Restore packages
echo %YELLOW%[1/2] 正在还原依赖包...%RESET%
cd /d "%PROJECT_DIR%"
dotnet restore
if errorlevel 1 (
    echo %RED%[!] 还原依赖包失败！%RESET%
    pause
    exit /b 1
)

echo %GREEN%[OK] 依赖包还原成功%RESET%
echo.

:: ============================================================
::  Mode 1: Debug Build
:: ============================================================
if "!CHOICE!"=="1" (
    echo %CYAN%正在编译: Debug (多文件^)%RESET%
    dotnet build -c Debug -r win-x64
    if errorlevel 1 (
        echo %RED%[!] 编译失败！%RESET%
        pause
        exit /b 1
    )
    echo.
    echo %GREEN%========================================%RESET%
    echo %GREEN%   编译完成！%RESET%
    echo %GREEN%========================================%RESET%
    echo.
    echo  输出目录: %BUILD_OUTPUT%\Debug\bin\
    echo  按任意键打开输出文件夹...
    pause
    start "" "%BUILD_OUTPUT%\Debug\bin\"
    exit /b 0
)

:: ============================================================
::  Mode 2: Release Build
:: ============================================================
if "!CHOICE!"=="2" (
    echo %CYAN%正在编译: Release (多文件^)%RESET%
    dotnet build -c Release -r win-x64
    if errorlevel 1 (
        echo %RED%[!] 编译失败！%RESET%
        pause
        exit /b 1
    )
    echo.
    echo %GREEN%========================================%RESET%
    echo %GREEN%   编译完成！%RESET%
    echo %GREEN%========================================%RESET%
    echo.
    echo  输出目录: %BUILD_OUTPUT%\Release\bin\
    echo  按任意键打开输出文件夹...
    pause
    start "" "%BUILD_OUTPUT%\Release\bin\"
    exit /b 0
)

:: ============================================================
::  Mode 3: Single File EXE (default)
:: ============================================================
if "!CHOICE!"=="3" (
    echo %CYAN%正在编译: Release - 单文件自包含 EXE%RESET%

    dotnet publish ^
        -c Release ^
        -r win-x64 ^
        --self-contained true ^
        -p:PublishSingleFile=true ^
        -p:EnableCompressionInSingleFile=true ^
        -p:IncludeNativeLibrariesForSelfExtract=true ^
        -p:ShowDetailedRepositoryCommitSummary=false ^
        -o "%BUILD_OUTPUT%\Release\SingleFile"

    if errorlevel 1 (
        echo %RED%[!] 编译失败！%RESET%
        pause
        exit /b 1
    )
    echo.
    echo %GREEN%========================================%RESET%
    echo %GREEN%   编译完成！%RESET%
    echo %GREEN%========================================%RESET%
    echo.
    echo  输出目录: %BUILD_OUTPUT%\Release\SingleFile\
    echo  文件名:   Arctium Game Launcher.exe
    echo.
    echo  提示: 将此 EXE 复制到魔兽世界游戏目录即可使用
    echo  按任意键打开输出文件夹...
    pause
    start "" "%BUILD_OUTPUT%\Release\SingleFile\"
    exit /b 0
)

:: ============================================================
::  Mode 4: All
:: ============================================================
if "!CHOICE!"=="4" (
    echo %CYAN%正在编译全部目标...%RESET%
    echo.

    :: Debug
    echo %YELLOW%>> 正在编译 Debug...%RESET%
    dotnet build -c Debug -r win-x64
    if errorlevel 1 (
        echo %RED%[!] Debug 编译失败！%RESET%
        pause
        exit /b 1
    )

    :: Release
    echo %YELLOW%>> 正在编译 Release...%RESET%
    dotnet build -c Release -r win-x64
    if errorlevel 1 (
        echo %RED%[!] Release 编译失败！%RESET%
        pause
        exit /b 1
    )

    :: Single File
    echo %YELLOW%>> 正在编译单文件...%RESET%
    dotnet publish ^
        -c Release ^
        -r win-x64 ^
        --self-contained true ^
        -p:PublishSingleFile=true ^
        -p:EnableCompressionInSingleFile=true ^
        -p:IncludeNativeLibrariesForSelfExtract=true ^
        -p:ShowDetailedRepositoryCommitSummary=false ^
        -o "%BUILD_OUTPUT%\Release\SingleFile"
    if errorlevel 1 (
        echo %RED%[!] 单文件编译失败！%RESET%
        pause
        exit /b 1
    )

    echo.
    echo %GREEN%========================================%RESET%
    echo %GREEN%   全部编译完成！%RESET%
    echo %GREEN%========================================%RESET%
    echo.
    echo  Debug:       %BUILD_OUTPUT%\Debug\bin\
    echo  Release:     %BUILD_OUTPUT%\Release\bin\
    echo  SingleFile:  %BUILD_OUTPUT%\Release\SingleFile\
    echo.
    echo  按任意键打开输出文件夹...
    pause
    start "" "%BUILD_OUTPUT%\Release\SingleFile\"
    exit /b 0
)

:: Invalid choice
echo %RED%[!] 无效的选择: !CHOICE!%RESET%
echo.
echo  有效选项: 1, 2, 3, 或 4
pause
