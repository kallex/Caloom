rem pushd apps\workertemplate\caloomui
rem call git pull
rem copy /Y caloomxml\OIPBallInstance.xml ..\..\..\Abstractions\AbstractionContent\TheBallCore\In\Content_v1_0\OIPBallInstance.xml
rem popd
rem Abstractions\absbuilder\AbstractionBuilder\bin\Debug\AbstractionBuilder.exe
rem devenv.exe Caloom.sln /Rebuild
pushd Apps\WorkerTemplate\CaloomUI\caloomhtml\UI\docs
..\..\..\..\..\..\tools\TheBallTool\bin\Debug\TheBallTool.exe WNroJbxN3GFbm5a7DclNrPsp9xQF+q7YVwe5AL+l0uiAFmOEl1zQMOhQJR0IruwEzSZKrYuT2U7g1Hhgr8cN0w==
popd