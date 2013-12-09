REM Replace <root>, <instance>, <username> and <password>
copy <root>\Apps\WebInterface\InstanceSettings_<instance>.config <root>\Apps\WebInterface\InstanceSettings.config
msbuild <root>\Caloom.sln /p:DeployOnBuild=true /p:PublishProfile=Websites.pubxml /p:username=<username> /p:password=<password> /p:AllowUntrustedCertificate=True