del /Q output\*.exe
cd..
del /Q Xsemmel\bin\Release\*
msbuild Xsemmel.sln /p:Configuration=Release
cd InnoSetup
"C:\Program Files (x86)\Inno Setup 5\ISCC.exe" setup.iss

signtool sign /f xsemmel.pfx /p xxx /t http://timestamp.verisign.com/scripts/timestamp.dll output\*.exe