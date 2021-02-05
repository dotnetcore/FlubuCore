## Installing built-in templates

With FlubuCore templates it is possible to create new build or deploy scripts in no time. All you have to do is:

- Install FlubuCore global tool `dotnet tool install --global FlubuCore.Tool`
- Install template with `flubu new <TemplateName>` Available templates names: `empty`, `lib`
- Interactive mode will guide you through how to install the template.
- With `flubu new` command all available templates are listed.

## Creating and installing custom templates. 

It is also possible to create custom templates on github or gitlab servers with just few steps. Add files to the repository that your template should contain such as your Flubu build script file, Flubu project file etc. Anything that is needed in your build project can be added to template.
 
 Sample of template repository can be found at [default template.](#https://github.com/flubu-core/FlubuCore.DefaultTemplate) This template can be installed with `flubu new empty`.  [library template.](#https://github.com/flubu-core/FlubuCore.LibraryTemplate) This template can be installed with `flubu new lib`

 Each template can also contain template configuration file. Configuration file can be in json(Template.json) format or in cs (Template.cs) format. 
 
 Custom template can be installed with command `flubu new -u={RepositoryUrl}` for example `flubu new -u=https://github.com/flubu-core/FlubuCore.DefaultTemplate`
 
Advantages of configuration file in json format:

- Abit faster installation of template as cs file needs to be compiled first
- Some prefere configuration in json over fluent interface
- Probably more suitable for simple templates
 
Advanatages of configuration file in cs format:

- Intelisene with code completion and help. 
- It is possible to add custom code to template for custom template processing when installing the template.  
 
 
### Configuration file options

#### ReplacementTokens

 Each file in template can contain replacement tokens. Replacement tokens will be replaced in interactive mode when installing the template. Replacement tokens must be defined in configuration file.


Sample of Template.json with one replacement token:
```json
{   
	"Tokens": [
		{
			"Token": "{{SolutionFileName}}", // Token to be replaced in template files.
			"Description": "Enter relative path to solution filename:", // Input text shown in interactive mode when entering value for replacement token.
			"Help": null, // Help shown at the bottom of console in the interactive mode.
			"DefaultValue": null, // Default value shown as hint in the beginning in interactive mode.
			"InputType": Files, // Input type of the replacement token. See documentation bellow for available input types.
			"Values": null, // Available values in interactive mode for input type Hint or Options.
			"Files": { // Options for 'Files' InputType.
				"AllowedFileExtension": "sln" // Allowed file extension to be entered in interactive mode.
			}
		}
	]
}
```

Let's say we have following BuildScript.cs file in our template repository which contais `{{SolutionFileName}}` replacement token


```c#
 public class BuildScript : DefaultBuildScript
    {
        [SolutionFileName]
        public string SolutionFileName { get; set; } = "{{SolutionFileName}}";
        
        [BuildConfiguration]
        public string BuildConfiguration { get; set; } = "Release";

        protected override void ConfigureTargets(ITaskContext context)
        {
            var compile = context.CreateTarget("compile")
                .SetDescription("Compiles the solution.")
                .AddCoreTask(x => x.Build());
        }
    }
```

When installing the template `{{SolutionFileName}}` replacement token will be replaced with entered value in interactive mode.


Same as above sample of template configuration json file can be done in template.cs configuration file
```c#
    public class Template : IFlubuTemplate
    {
        public void ConfigureTemplate(IFlubuTemplateBuilder templateBuilder)
        {
            templateBuilder.AddReplacementToken(new TemplateReplacementToken()
            {
                Token = "{{SolutionFileName}}",
                Description = "Enter relative path to solution filename:",
                InputType = InputType.Files,
                Files = new FilesInputType
                {
                    AllowedFileExtension = "sln"
                },
                Help = null,
                DefaultValue = null,
                Values = null,
            });
        }
    }
```

##### Available input types

- `Text`: Regular text can be entered for the replacement token
- `Files`: Files can be entered for the replacement token. Hints for directories and files with tab completion are available  
- `Hints`: With InputType `Hints` property: `Values` must be defined. All values are shown as hints with tab completion in interactive mode.
- `Options`: With InputType `Options` property: `Values` must be defined. All values are shown as options that can be choosen.  

### Custom template processing when installing template with custom code in template.cs configuration file 

Add `IFlubuTemplateTask` interface to your `Template.cs` configuration file. This will add 4 methods where your custom template processing will occur.

Below is a dummy sample that will print some text to the console when installation of the template will start and end.
```c#
 public class Template : IFlubuTemplate, IFlubuTemplateTask
    {
        public void ConfigureTemplate(IFlubuTemplateBuilder templateBuilder)
        {
            templateBuilder.AddReplacementToken(new TemplateReplacementToken()
            {
                Token = "{{SolutionFileName}}",
                Description = "Enter relative path to solution filename:",
                InputType = InputType.Files,
                Files = new FilesInputType
                {
                    AllowedFileExtension = "sln"
                },
                Help = null,
                DefaultValue = null,
                Values = null,
            });
        }

        public void BeforeFileProcessing(TemplateModel template, List<string> files)
        {
            Console.WriteLine("Started installing awesome custom template");
        }

        public void BeforeFileCopy(string sourceFilePath)
        {
        }

        public void AfterFileCopy(string destinationFilePath)
        {
        }

        public void AfterFileProcessing(TemplateModel template)
        {
            Console.WriteLine("Finished installing awesome custom template");
        }
    }
```

- BeforeFileProcessing occures right after the template was downloded from git repository. 
- BeforeFileCopy occures before individual file is copied from the downloaded location to the destination path (where `flubu new` command was exectued). Replacement tokens are replaced before files are copied 
- AfterFileCopy occures after individual file is copied from the downloaded location to the destination path. 
- AfterFileProcessing occures when all files are copied to the destination path