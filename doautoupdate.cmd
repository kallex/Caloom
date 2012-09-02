pushd apps\workertemplate\caloomui
call git pull
copy /Y caloomxml\OIPBallInstance.xml ..\..\..\Abstractions\AbstractionContent\TheBallCore\In\Content_v1_0\OIPBallInstance.xml
popd
Abstractions\absbuilder\AbstractionBuilder\bin\Debug\AbstractionBuilder.exe
"c:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe" Caloom.sln /Rebuild
pushd Apps\WorkerTemplate\CaloomUI\caloomhtml\UI\docs
..\..\..\..\..\..\tools\TheBallTool\bin\Debug\TheBallTool.exe WNroJbxN3GFbm5a7DclNrPsp9xQF+q7YVwe5AL+l0uiAFmOEl1zQMOhQJR0IruwEzSZKrYuT2U7g1Hhgr8cN0w==
popd