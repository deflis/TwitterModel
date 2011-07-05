using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NekoVampire.TwitterCore
{
    public interface IStatus
    {
        Int64 ID { get; }
        string UserName { get; }
        string Text { get; }
        DateTime CreatedAt { get; }
    }
}
