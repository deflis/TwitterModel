using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NekoVampire.TwitterCore
{
    interface IAccount
    {
        string Name { get; }
        Int64 UserID { get; }
    }
}
