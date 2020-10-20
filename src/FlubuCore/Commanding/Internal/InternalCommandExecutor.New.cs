using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Infrastructure;
using FlubuCore.IO;
using FlubuCore.Scripting;
using FlubuCore.Tasks.Packaging;

namespace FlubuCore.Commanding.Internal
{
    public partial class InternalCommandExecutor
    {
        private const string TmpZipPath = "./template.zip";

        private bool DefaultTemplateCommand => Args.MainCommands.Count == 1;

        internal async Task CreateNewProject()
        {
            if (DefaultTemplateCommand)
            {
                await DownloadAndPrepareProject("https://github.com/flubu-core/EmptyTemplate/archive/master.zip");
            }
        }

        internal async Task DownloadAndPrepareProject(string url)
        {
            using (var client = new HttpClient())
            {
                await client.DownloadFileAsync(url, TmpZipPath);
                var rootDir = FlubuSession.GetRootDirectory().ToString();
                var files = FlubuSession.Tasks().UnzipTask(TmpZipPath, rootDir).NoLog().Execute(FlubuSession);
                
                foreach (var file in files)
                {
                    string sourceFileName = file;
                    var destinationFileName = sourceFileName.Substring(sourceFileName.IndexOf(Path.DirectorySeparatorChar)).TrimStart(Path.DirectorySeparatorChar);
                    var destinationDir = Path.GetDirectoryName(destinationFileName);
                    if (!string.IsNullOrEmpty(destinationDir))
                    {
                        Directory.CreateDirectory(destinationDir);
                    }

                    File.Copy(sourceFileName, destinationFileName, true);
                }

                var tmp = files[0].Substring(rootDir.Length).TrimStart(Path.DirectorySeparatorChar);
                var gitDirName = tmp.Substring(0, tmp.IndexOf(Path.DirectorySeparatorChar));
                Directory.Delete(gitDirName, true);
                File.Delete(TmpZipPath);
            }
        }
    }
}
