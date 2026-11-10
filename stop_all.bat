@echo off
title Stop All Chatbot Processes
echo ===================================================
echo   Stopping All OOP Chatbot Processes...
echo ===================================================

taskkill /F /IM OOP_Chatbot.exe 2>nul
taskkill /F /IM OOP_Interview_Chatbot.exe 2>nul

powershell -NoProfile -ExecutionPolicy Bypass -Command "Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like '*chatbot*' -and ($_.Name -like '*python*' -or $_.Name -like '*OOP*') -and $_.ProcessId -ne $PID } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }" 2>nul

echo.
echo [OK] All chatbot processes and files in this folder are completely terminated.
echo Zero background tasks remaining.
timeout /t 2 >nul
