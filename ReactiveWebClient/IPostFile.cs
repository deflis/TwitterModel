using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NekoVampire.Web
{
    public interface IPostFile : IDisposable
    {
        string Name { get; }
        string FileName { get; }
        string ContentType { get; }
        Stream Stream { get; }

    }
}
