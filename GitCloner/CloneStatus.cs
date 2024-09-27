using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitCloner
{
    public enum CloneStatus
    {
        Success,
        SourceDirectoryNotFound,
        TargetDirectoryCreationFailed,
        CloneFailed,
    }
}
