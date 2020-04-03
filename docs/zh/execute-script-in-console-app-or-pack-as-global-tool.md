You can create global tool by adding below code into your console application. It can just be used as normal console application ofcourse. Debuging your build script is also easier with console application.

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