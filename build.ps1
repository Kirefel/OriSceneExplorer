msbuild /t:Build /restore /p:Configuration=Release /p:OutDir=..\build

rm -R .\release -ErrorAction SilentlyContinue

mkdir .\release\

cp .\build\OriSceneExplorer.dll .\release\
cp .\mod.json .\release\

rm .\OriSceneExplorer.zip -ErrorAction SilentlyContinue
powershell Compress-Archive .\release\* .\OriSceneExplorer.zip
