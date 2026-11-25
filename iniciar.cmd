@echo off
rem Inicia MySQL, API e front-end do RetroGames. Detalhes em scripts\iniciar.ps1.
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\iniciar.ps1"
pause
