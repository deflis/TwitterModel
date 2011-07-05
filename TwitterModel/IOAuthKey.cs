using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;

namespace NekoVampire.TwitterCore
{
    public interface IOAuthKey
    {
        string ConsumerKey { get; }
        string ConsumerSecret { get; }

        bool HasAccessToken { get; set; }
        string AccessToken { get; set; }
        SecureString AccessTokenSecret { get; set; }

        string requestTokenURL { get; }
        string accessTokenURL { get; }
        string authorizeURL { get; }
    }
}
