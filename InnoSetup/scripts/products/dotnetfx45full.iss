// requires Windows Vista SP2 (x86 and x64), Windows 7 SP1 (x86 and x64), Windows Server 2008 R2 SP1 (x64), Windows Server 2008 SP2 (x86 and x64)
// requires Windows Installer 3.1
// requires Internet Explorer 5.01
// WARNING: express setup (downloads and installs the components depending on your OS) if you want to deploy it on cd or network download the full bootsrapper on website below
// http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992
 
[CustomMessages]
dotnetfx45full_title=.NET Framework 4.5 Full
 
dotnetfx45full_size=3 MB - 197 MB
 
;http://www.microsoft.com/globaldev/reference/lcid-all.mspx
en.dotnetfx45full_lcid=''
;de.dotnetfx45full_lcid='/lcid 1031 '
 

[Code]
const
	dotnetfx45full_url = 'http://download.microsoft.com/download/B/A/4/BA4A7E71-2906-4B2D-A0E1-80CF16844F5F/dotNetFx45_Full_setup.exe';
 
procedure dotnetfx45full();
begin
	if (not netfxinstalled(NetFx45Full, '')) then
		AddProduct('dotNetFx45_Full_setup.exe',
			CustomMessage('dotnetfx45full_lcid') + '/q /passive /norestart',
			CustomMessage('dotnetfx45full_title'),
			CustomMessage('dotnetfx45full_size'),
			dotnetfx45full_url,
			false, false);
end;