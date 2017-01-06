using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.IO;
using Newtonsoft.Json;

namespace FlubuCore.Packaging
{
    public class Zipper : IZipper
    {
        private readonly ITaskContextInternal _taskContext;

        public Zipper(ITaskContextInternal taskContext)
        {
            _taskContext = taskContext;
        }

        public void ZipFiles(
            FileFullPath zipFileName,
            FullPath baseDir,
            IEnumerable<FileFullPath> filesToZip,
            bool optimizeFiles)
        {
            _taskContext.LogInfo(string.Format("Zipping {0}", zipFileName));
            var zipFileFullPath = zipFileName.ToFullPath();
            if (File.Exists(zipFileFullPath))
            {
                File.Delete(zipFileFullPath);
            }

            List<FileFullPath> tmpList = filesToZip.ToList();

            if (optimizeFiles)
            {
                tmpList = RemoveDuplicateFiles(tmpList, baseDir);
            }

            using (ZipArchive newFile = ZipFile.Open(zipFileFullPath, ZipArchiveMode.Create))
            {
                foreach (var fileToZip in tmpList)
                {
                    LocalPath debasedFileName = fileToZip.ToFullPath().DebasePath(baseDir);
                    newFile.CreateEntryFromFile(fileToZip.ToString(), debasedFileName, CompressionLevel.Optimal);
                }
            }
        }

        private List<FileFullPath> RemoveDuplicateFiles(List<FileFullPath> filesToZip, FullPath baseDir)
        {
            Dictionary<string, List<string>> mapping = new Dictionary<string, List<string>>();
            List<FileFullPath> list = new List<FileFullPath>();
            while (filesToZip.Count > 0)
            {
                FileFullPath current = filesToZip[filesToZip.Count - 1];
                filesToZip.RemoveAt(filesToZip.Count - 1);
                List<string> currentItems = new List<string> { current.ToFullPath().ToString() };
                mapping.Add(current.FileName, currentItems);

                for (int i = filesToZip.Count - 1; i >= 0; i--)
                {
                    FileFullPath tmp = filesToZip[i];

                    if (tmp.FileName != current.FileName || tmp.Length != current.Length)
                        continue;

                    currentItems.Add(tmp.ToFullPath().DebasePath(baseDir));
                    filesToZip.RemoveAt(i);
                }
            }

            string metadata = Path.Combine(baseDir.ToString(), "_zipmetadata.json");
            File.WriteAllText(metadata, JsonConvert.SerializeObject(mapping));
            list.Add(new FileFullPath(metadata));
            return list;
        }
    }
}