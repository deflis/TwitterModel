using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NekoVampire.TwitterCore
{
    internal class Status : IStatus
    {
        protected readonly dynamic status;

        internal Status(dynamic status)
        {
            this.status = status;
        }

        public long ID
        {
            get { return status.id; }
        }

        public string UserName
        {
            get { throw new NotImplementedException(); }
        }

        public string Text
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime CreatedAt
        {
            get { throw new NotImplementedException(); }
        }
    }
}
