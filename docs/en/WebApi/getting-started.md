
## **About**
With FlubuCore web api you can execute FlubuCore scripts remotely. Mainly it is meant to automate deployment of .net or .net core applications from your build server to different environments but it can be used for any other FlubuCore script execution.

Web Api supports:

- Executing flubu scripts on the server where web api is deployed.
- Uploading (deploy or any other) packages to the server where web api is deployed.
- Deleting packages from server where web api is deployed.
- Sending reports back to client.
- Uploading FlubuCore scripts to the server where web api is deployed.
- Automatic updates
- Manual target execution through FlubuCore web app(deployed together with web api)

In this getting started tutorial we will:

- Deploy FlubuCore web api to the server.
- Write .net deployment script that will deploy mvc example application on the sever. You should go through this tutorial even if you want to use FlubuCore.WebApi for .net core application deployment. There are few small differences between using FlubuCore.WebApi for deploying .net core applications and .net applications. All differences are written in this tutorial.
- Write build script that will upload deployment package of the example application to the server and execute deployment script that we wrote.
- Run deployment script through build script remotely.

<a name="requirements"></a>
### **Requirements**

- .net 462+ runtime or .net core runtime installed on the server. Depending on which build of FlubuCore.WebApi you plan to use.

<a name="Web-api-deployment"></a>
### **Web api deployment**

- Get appropriate web api deploy package from https://github.com/flubu-core/flubu.core/releases.
- Copy web api deployment package to the server where you want to execute flubu script.
- Unzip the package.
- Set web api deployment configuration settings in the unzipped DeploymentConfig.json file. More about specific deployment config settings can be found in the configuration file.
- On windows server run deploy.bat to deploy the web api
- On linux/mac server run: dotnet restore and after that: dotnet flubu -s=deploymentscript.cs
- On deployed location run dotnet FlubuCore.WebApi.dll to selfhost web api. You can of course also host it for example on iis...

#### IIS deployment
 How to deploy asp .net core application see: https://docs.microsoft.com/en-us/aspnet/core/publishing/iis?tabs=aspnetcore2x

Some actions might need administration rights like starting / stoping the application pool. If that's the case u have to change identity on the application pool where you hosted the web api. Go to Application pools -> Web api app pool -> Advanced settings -> process model -> Identity and change to user which has admin rights.

<a name="Write-deploy-script"></a>
### **Write deploy script**
Example .net  deploy script can be found [Here](https://github.com/flubu-core/examples/blob/master/DeployScriptExample/BuildScript/DeployScript.cs). If u want to try the example the best way is to just clone the flubu core examples directory. Deploy script for .net core application would be of course slightly different.

Example deploy script for .net application will

 - Create iis application pool if it doesnt exists
 - Stop the application pool
 - Unzip package from /packages directory(which will be uploaded to web api with build script)
 - Copy unziped application to new folder where it is/will be hosted.
 - Create web site on iis for example web application
 - Start the application pool

When you finish writing your deploy script manually copy it to web api deployed location /scripts folder. Web api can also upload scripts but it is disabled by default for obvious security reason. It should stay disabled in most cases.

If needed modify Example DeployScript for your needs.

<a name="Write-build-script"></a>
### **Write build script**
   Example .net build script can be found [here](https://github.com/flubu-core/examples/blob/master/DeployScriptExample/BuildScript/BuildScript.cs)
   
Example .net build script will

 - Get the authentication token
 - Delete old packages from /packages folder on web api.
 - Upload package to web api /packages folder
 - Execute Deployment script in /scripts folder that we manually uploaded

If needed modify Example BuildScript for your needs.

<a name="Run-deploy-script"></a>
### **Run deploy script**
If u cloned example repository just execute at the root foolder:

 `dotnet restore buildscript.csproj ` and `dotnet flubu deploy -s=buildscript\buildscript.cs` in cmd from DeployScriptExample folder

In real case scenario you would probably deploy from your build server after sucesfull build, after merge to release branch, manually execute the job on build server...  

<a name="Security"></a>
### **Security** 
As attacker can do alot of damage if he gains access to web api next security measures should be implemented if possible:

* If possible Flubu web api should not be publicly accessible.
* Always host web api on https.
* Restrict access by ip(config).
* Restrict access by time frame(config). This security measure should be taken if you deploy your application always at same time e.g 11pm. Then time frame when api can be accessed should be set for example from 11pm to 11.15pm,
* Use very strong password(web api user creation).
* Do not disable feature "Restrict access on failed login"(config).
* Enable email notifications when GetToken/Script is executed on api(config).

For detailed description of security settings see appsettings.json file on web api.

<a name="Automatic-update"></a>
### **Automatic update**

You can automatically update FlubuCore web api if new version is available. Just navigate to /UpdateCenter (not /api/UpdateCenter)

<a name="manual-target-execution"></a>
### **Manual target execution remotely through FlubuCore web app**

You can manually execute target through FlubuCore web app. Just navigate to /Script