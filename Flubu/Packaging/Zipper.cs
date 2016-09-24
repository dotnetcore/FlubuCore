using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Flubu.IO;
using Flubu.Tasks;

namespace Flubu.Packaging
{
    public class Zipper : IZipper
    {
        ////public Zipper(ITaskContext taskContext)
        ////{
        ////    this.taskContext = taskContext;
        ////}

        ////public void ZipFiles(
        ////    FileFullPath zipFileName,
        ////    FullPath baseDir,
        ////    int? compressionLevel,
        ////    IEnumerable<FileFullPath> filesToZip)
        ////{
        ////    taskContext.WriteMessage(string.Format()"Zipping {0}", zipFileName));

        ////    CreateDirectoryTask createDirectoryTask = new CreateDirectoryTask(
        ////        zipFileName.Directory.ToString(),
        ////        false);
        ////    createDirectoryTask.Execute(taskContext);

        ////    using (FileStream zipFileStream = new FileStream(
        ////        zipFileName.ToString(),
        ////        FileMode.Create,
        ////        FileAccess.ReadWrite,
        ////        FileShare.None))
        ////    {
        ////        using (ZipOutputStream zipStream = new ZipOutputStream(zipFileStream))
        ////        {
        ////            if (compressionLevel.HasValue)
        ////                zipStream.SetLevel(compressionLevel.Value);

        ////            buffer = new byte[1024 * 1024];

        ////            foreach (FileFullPath fileName in filesToZip)
        ////            {
        ////                LocalPath debasedFileName = fileName.ToFullPath().DebasePath(baseDir);
        ////                string cleanName = ZipEntry.CleanName(debasedFileName.ToString());

        ////                //environment.LogMessage("Zipping file '{0}'", basedFileName);
        ////                AddFileToZip(fileName, cleanName, zipStream);
        ////            }
        ////        }
        ////    }
        ////}

        ////private void AddFileToZip(FileFullPath fileName, string debasedFileName, ZipOutputStream zipStream)
        ////{
        ////    using (FileStream fileStream = File.OpenRead(fileName.ToString()))
        ////    {
        ////        string fileHeader = String.Empty;
        ////        string fileFooter = String.Empty;

        ////        //if (zipFileHeaderCallback != null)
        ////        //    fileHeader = zipFileHeaderCallback(fileName);

        ////        //if (zipFileFooterCallback != null)
        ////        //    fileFooter = zipFileFooterCallback(fileName);

        ////        ZipEntry entry = new ZipEntry(debasedFileName);
        ////        entry.DateTime = File.GetLastWriteTime(fileName.ToString());
        ////        entry.Size = fileStream.Length + fileHeader.Length + fileFooter.Length;
        ////        zipStream.PutNextEntry(entry);

        ////        int sourceBytes;

        ////        WriteTextToZipStream(fileHeader, zipStream);

        ////        while (true)
        ////        {
        ////            sourceBytes = fileStream.Read(buffer, 0, buffer.Length);

        ////            if (sourceBytes == 0)
        ////                break;

        ////            zipStream.Write(buffer, 0, sourceBytes);
        ////        }

        ////        WriteTextToZipStream(fileFooter, zipStream);
        ////    }
        ////}

        ////private static void WriteTextToZipStream(string text, ZipOutputStream zipStream)
        ////{
        ////    if (text.Length > 0)
        ////    {
        ////        byte[] bytes = Encoding.ASCII.GetBytes(text.ToString());
        ////        zipStream.Write(bytes, 0, bytes.Length);
        ////    }
        ////}

        ////private byte[] buffer;
        ////private readonly ITaskContext taskContext;
    }
}