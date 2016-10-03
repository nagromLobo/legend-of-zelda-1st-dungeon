@echo off

REM eecs-494-unity-canvas.bat Version 1.5

SET DP0=%~dp0%
SET TURNIN=%1
SET UNITY=C:\Program Files\Unity\Editor\Unity.exe
SET ZIP=C:\Program Files\7-Zip\7z.exe

SET ZIPFILE=%DP0%\%TURNIN%.7z
SET APP=%DP0%\%TURNIN%.app
SET EXE=%DP0%\%TURNIN%.exe
SET EXEDATA=%DP0%\%TURNIN%_Data
SET TEMPDIR=%TEMP%
SET REPODIR=%TEMPDIR%\%TURNIN%_Repo
SET REPOLINK=%TEMPDIR%\%TURNIN%_Link
SET EL=0

IF "%1" == "" (

ECHO(
ECHO Usage: eecs-494-unity-canvas.bat TURNIN_NAME
ECHO(

EXIT /B 1
)

ECHO(

IF EXIST "%ZIPFILE%" DEL /Q "%ZIPFILE%"
IF EXIST "%APP%" RMDIR /S /Q "%APP%"
IF EXIST "%EXE%" DEL /Q "%EXE%"
IF EXIST "%EXEDATA%" RMDIR /S /Q "%EXEDATA%"
IF EXIST "%REPOLINK%" RMDIR /S /Q "%REPOLINK%"

ECHO "%UNITY%" -quit -batchmode -buildOSXUniversalPlayer "%APP%" -buildWindowsPlayer "%EXE%" -projectPath "%DP0%"
"%UNITY%" -quit -batchmode -buildOSXUniversalPlayer "%APP%" -buildWindowsPlayer "%EXE%" -projectPath "%DP0%"
IF ERRORLEVEL 1 (
  SET EL=1
  ECHO Build failed.
) ELSE (
  IF EXIST "%REPODIR%" RMDIR /S /Q "%REPODIR%"

  cd "%TEMPDIR%"

  ECHO MKLINK /J "%REPOLINK%" "%DP0%"
  MKLINK /J "%REPOLINK%" "%DP0%"
  
  ECHO git clone "%REPOLINK%" "%REPODIR%"
  git clone "%REPOLINK%" "%REPODIR%"
  IF ERRORLEVEL 1 (
    SET EL=1

    CD "%DP0%"
  ) ELSE (
    IF EXIST "%REPODIR%\.git" RMDIR /S /Q "%REPODIR%\.git"

    CD "%DP0%"

    IF %EL%==0 (
      ECHO "%ZIP%" a "%ZIPFILE%" "%APP%" "%EXE%" "%EXEDATA%" "%REPODIR%"
      "%ZIP%" a "%ZIPFILE%" "%APP%" "%EXE%" "%EXEDATA%" "%REPODIR%"
      IF ERRORLEVEL 1 SET EL=1
    )
  )

  IF EXIST "%APP%" RMDIR /S /Q "%APP%"
  IF EXIST "%EXE%" DEL /Q "%EXE%"
  IF EXIST "%EXEDATA%" RMDIR /S /Q "%EXEDATA%"
  IF EXIST "%REPODIR%" RMDIR /S /Q "%REPODIR%"
  IF EXIST "%REPOLINK%" RMDIR /S /Q "%REPOLINK%"
)

ECHO(

IF %EL%==0 (
  CertUtil -hashfile "%ZIPFILE%" sha512
  PowerShell Get-FileHash "%ZIPFILE%" -Algorithm SHA256
) ELSE (
  ECHO Something went wrong. See above for details.
)

EXIT /B
