pushd apps\workertemplate\caloomui
call git pull
copy /Y caloomxml\OIPBallInstance.xml ..\..\..\Abstractions\AbstractionContent\TheBallCore\In\Content_v1_0\OIPBallInstance.xml
popd
Abstractions\absbuilder\AbstractionBuilder\bin\Debug\AbstractionBuilder.exe
"c:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe" Caloom.sln /Rebuild
pushd Apps\WorkerTemplate\CaloomUI\caloomhtml\UI\docs
..\..\..\..\..\..\tools\TheBallTool\bin\Debug\TheBallTool.exe PthyN4ltdw2Lh6CrfWfMpTSd49AEu8vlwhkkfCeUZ/Pq9b8oZcjI9rDNP4qFwOH2XXbMY8mIfXBr/qGiZ8ugcQ==
popd