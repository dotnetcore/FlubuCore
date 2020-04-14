To execute a acript in console application add the following code to `Program.cs`:

```c#
    public class Program
    {
        public static void Main(string[] args)
        {
            var engine = new FlubuEngine();
            engine.RunScript<MyScript>(args);
        }
    }
```
After that you can create a global tool from your console application and share your script with other developers through global tool.  

[How to create a global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create)

If you want to execute script directly in a console application it is also recomended to add `.flubu` file to the root of your repository because when you execute script through console application working folder of the script will be `/bin/{BuildConfiguration}` and some target's might fail of that because working folder is not correct.

When FlubuCore starts to run a build script, it searches `.flubu` file automatically, and it keeps searching it all the way up through parent directories until that file is found or reached the root directory of current drive. The location where a .flubu file is found will be used as the "work directory" during a build process, and a correct "work directory" is crucial  for us to use relative path in our  build scripts.

FlubuCore command line tool has an option to help you create `.flubu` file,  simply type  `flubu setup` and answer questions.