@echo off

set exe=gentools\protoc-client.exe
set inPath=proto\
set outPath=..\..\UnityFramework\Assets\Scripts\PB\

@REM for /f "delims=" %%i in ('dir /b proto "Proto/*.proto"') do %Path% -i:Proto/%%i -o:../../project-s-client/Assets/Scripts/Hotfix/NetProxy/Messages/%%~ni.cs

for /f "delims=" %%i in ('dir /b proto "proto/*.proto"') do ( 
    %exe% --proto_path=%inPath% --csharp_out=%outPath%  %inPath%%%i 
    echo Encode proto to cs success %%i
)

@REM for /f "delims=" %%i in ('dir /b proto "Proto/*.proto"') do echo %%i

pause