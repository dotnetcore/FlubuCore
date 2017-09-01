using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using FlubuCore.Context;
using FlubuCore.IO;
using Newtonsoft.Json;

namespace FlubuCore.Packaging
{
    public class Zipper : IZipper
    {
        public const string MetadataFileName = "_zipmetadata.json";
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
            _taskContext.LogInfo($"Zipping {zipFileName}");
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
                foreach (FileFullPath fileToZip in tmpList)
                {
                    LocalPath debasedFileName = fileToZip.ToFullPath().DebasePath(baseDir);
                    newFile.CreateEntryFromFile(fileToZip.ToString(), debasedFileName, CompressionLevel.Optimal);
                }
            }
        }

        private List<FileFullPath> RemoveDuplicateFiles(List<FileFullPath> filesToZip, FullPath baseDir)
        {
            ZipMetadata metadata = new ZipMetadata();
            List<FileFullPath> list = new List<FileFullPath>();

            while (filesToZip.Count > 0)
            {
                FileFullPath current = filesToZip[filesToZip.Count - 1];
                var currentDebase = current.ToFullPath().DebasePath(baseDir);

                filesToZip.RemoveAt(filesToZip.Count - 1);
                ZipMetadataItem metaItem = new ZipMetadataItem { FileName = currentDebase };
                metaItem.DestinationFiles.Add(currentDebase);
                metadata.Items.Add(metaItem);
                list.Add(current);

                byte[] firstHash = CalculateHash(current.ToString());
                FileInfo currentInfo = new FileInfo(current.ToString());

                for (int i = filesToZip.Count - 1; i >= 0; i--)
                {
                    FileFullPath tmp = filesToZip[i];

                    if (tmp.FileName != current.FileName)
                        continue;

                    FileInfo tmpInfo = new FileInfo(tmp.ToString());

                    if (tmpInfo.Length != currentInfo.Length)
                        continue;

                    byte[] secondHash = CalculateHash(tmp.ToString());

                    if (!HashEqual(firstHash, secondHash))
                        continue;

                    metaItem.DestinationFiles.Add(tmp.ToFullPath().DebasePath(baseDir));
                    filesToZip.RemoveAt(i);
                }
            }

            string metadataFile = Path.Combine(baseDir.ToString(), MetadataFileName);
            File.WriteAllText(metadataFile, JsonConvert.SerializeObject(metadata));
            list.Add(new FileFullPath(metadataFile));
            return list;
        }

        private byte[] CalculateHash(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            using (var hash = MD5.Create())
            {
                return hash.ComputeHash(stream);
            }
        }

        private bool HashEqual(byte[] firstHash, byte[] secondHash)
        {
            for (int i = 0; i < firstHash.Length; i++)
            {
                if (firstHash[i] != secondHash[i])
                    return false;
            }

            return true;
        }
    }
}