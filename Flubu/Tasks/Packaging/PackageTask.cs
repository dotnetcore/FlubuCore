using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flubu.Context;
using Flubu.Packaging;

namespace Flubu.Tasks.Packaging
{
    public class PackageTask : TaskBase
    {
      

        public override string Description  => "Packages specified folders.";  
        protected override int DoExecute(ITaskContext context)
        {
            ICopier copier = new Copier(context);
            IZipper zipper = new Zipper(context);
            IDirectoryFilesLister directoryFilesLister = new DirectoryFilesLister();
            StandardPackageDef packageDef = new StandardPackageDef();

            return 0;
        }
    }
}
