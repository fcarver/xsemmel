del /Q output\*.exe
cd..
del /Q Xemmel\bin\Release\*
msbuild XSemmel.sln /p:Configuration=Release
cd InnoSetup
"C:\Program Files (x86)\Inno Setup 5\ISCC.exe" setup.iss