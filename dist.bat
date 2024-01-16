if exist release\ (
	rmdir /s /q release
)

if exist release.zip (
	del release.zip
)

dotnet clean
dotnet publish -c Release

mkdir release

XCOPY modpack release /i /s

mkdir release\plugin

XCOPY DiscordReaderMod\bin\Release\netstandard2.1\publish release\plugin /i /s

XCOPY README.md release\

del release\plugin\*.deps.json

echo "Zip release folder then export to mod manager or thunderstore!"
