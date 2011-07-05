/*
  Copyright (c) 2011 NekoVampire / Deflis

  This software is provided 'as-is', without any express or implied
  warranty. In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

    1. The origin of this software must not be misrepresented; you must not
    claim that you wrote the original software. If you use this software
    in a product, an acknowledgment in the product documentation would be
    appreciated but is not required.

    2. Altered source versions must be plainly marked as such, and must not be
    misrepresented as being the original software.

    3. This notice may not be removed or altered from any source
    distribution.

  本ソフトウェアは「現状のまま」で、明示であるか暗黙であるかを問わず、
  何らの保証もなく提供されます。 本ソフトウェアの使用によって生じる
  いかなる損害についても、作者は一切の責任を負わないものとします。

  以下の制限に従う限り、商用アプリケーションを含めて、本ソフトウェアを
  任意の目的に使用し、自由に改変して再頒布することをすべての人に許可します。

    1. 本ソフトウェアの出自について虚偽の表示をしてはなりません。
    あなたがオリジナルのソフトウェアを作成したと主張してはなりません。
    あなたが本ソフトウェアを製品内で使用する場合、製品の文書に謝辞を入れていただければ
    幸いですが、必須ではありません。
 
    2. ソースを変更した場合は、そのことを明示しなければなりません。
    オリジナルのソフトウェアであるという虚偽の表示をしてはなりません。

    3. ソースの頒布物から、この表示を削除したり、表示の内容を変更したりしてはなりません。 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security;
using System.Runtime.InteropServices;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace NekoVampire.Web
{
    public class ReactiveWebClient
    {
        /// <summary>NetworkCredentialオブジェクト</summary>
        public NetworkCredential Credentials;

        /// <summary>クッキーコンテナ</summary>
        public CookieContainer Cookie;

        /// <summary>
        /// Requestを初期化する
        /// </summary>
        public ReactiveWebClient()
        {
            Credentials = new NetworkCredential();
            Cookie = new CookieContainer();
        }

        /// <summary>
        /// Requestを初期化する
        /// </summary>
        /// <param name="userName">ユーザーID</param>
        /// <param name="password">パスワード</param>
        public ReactiveWebClient(string userName, string password)
        {
            Credentials = new NetworkCredential(userName, password);
            Cookie = new CookieContainer();
        }

        /// <summary>
        /// Requestを初期化する
        /// </summary>
        /// <param name="cookie">クッキー</param>
        /// <param name="userName">ユーザーID</param>
        /// <param name="password">パスワード</param>
        public ReactiveWebClient(string userName, SecureString password)
        {
            Credentials = new NetworkCredential(userName, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(password)));
            Cookie = new CookieContainer();
        }

        /// <summary>
        /// Requestを初期化する
        /// </summary>
        /// <param name="cookie">クッキー</param>
        public ReactiveWebClient(CookieContainer cookie)
        {
            Credentials = new NetworkCredential();
            Cookie = cookie;
        }

        /// <summary>
        /// Requestを初期化する
        /// </summary>
        /// <param name="cookie">クッキー</param>
        /// <param name="userName">ユーザーID</param>
        /// <param name="password">パスワード</param>
        public ReactiveWebClient(CookieContainer cookie, string userName, string password)
        {
            Credentials = new NetworkCredential(userName, password);
            Cookie = cookie;
        }

        /// <summary>
        /// Requestを初期化する
        /// </summary>
        /// <param name="cookie">クッキー</param>
        /// <param name="userName">ユーザーID</param>
        /// <param name="password">パスワード</param>
        public ReactiveWebClient(CookieContainer cookie, string userName, SecureString password)
        {
            Credentials = new NetworkCredential(userName, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(password)));
            Cookie = cookie;
        }

        /// <summary>ユーザー名</summary>
        public string UserName
        {
            get
            {
                return Credentials.UserName;
            }
            set
            {
                Credentials.UserName = value;
            }
        }

        /// <summary>パスワード</summary>
        public string Password
        {
            get
            {
                return Credentials.Password;
            }
            set
            {
                Credentials.Password = value;
            }
        }

        /// <summary>パスワード</summary>
        public SecureString SecurePassword
        {
            get
            {
                var password = new SecureString();
                foreach (char c in Credentials.Password.ToCharArray())
                    password.AppendChar(c);
                password.IsReadOnly();
                return password;
            }
            set
            {
                Credentials.Password = Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(value));
            }
        }


        /// <summary>HTTPリクエストを行います</summary>
        /// <param name="url">URL</param>
        /// <param name="method">HTTPメソッド</param>
        /// <param name="since">IfModifiedSinceパラメータ</param>
        /// <param name="postData">POSTで送信するデータ</param>
        /// <param name="enc">文字エンコーディング</param>
        /// <param name="headers">HTTPヘッダー</param>
        /// <returns></returns>
        public IObservable<WebResponse> HttpRequest(string url, string method, DateTime since, IDictionary<string, string> postData, Encoding enc, IDictionary<string, string> headers)
        {
            if ((postData != null) && (postData.Count != 0) && (method != "POST"))
            {
                url = url + "?" + RequestUtility.GetPostData(postData, enc);
            }

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method;
            req.Credentials = Credentials;
            req.CookieContainer = Cookie;
            req.IfModifiedSince = since;

            if (headers != null)
                foreach (var header in headers)
                    req.Headers.Add(header.Key, header.Value);

            if ((postData != null) && (postData.Count != 0) && (method == "POST"))
            {
                req.ContentType = "application/x-www-form-urlencoded";
                byte[] data = getEncodedPostData(postData, enc);
                req.ContentLength = data.Length;

                req.ServicePoint.Expect100Continue = false;

                return Observable.Defer(() => new[] { (WebRequest)req }.ToObservable())
                    .Do(_ => _.GetRequestStreamAsObserverable().DisposeSubscribe(stream => stream.Write(data, 0, data.Length), ex => { throw ex; }))
                    .SelectMany(_ => _.GetResponseAsObserverable());
            }
            else
            {
                return req.GetResponseAsObserverable();
            }
        }


        #region アップロードメソッド
        public IObservable<WebResponse> HttpRequest(string url, IDictionary<string, string> postData, Encoding enc, IEnumerable<IPostFile> postFiles)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.Credentials = Credentials;
            req.CookieContainer = Cookie;

            if (postData != null)
            {
                return Observable.Using(() => new PostFileData(), _ => new[] { _ }.ToObservable()
                        .ObserveOn(Scheduler.TaskPool)
                        .Do(post => req.ContentType = "multipart/form-data; boundary=" + post.Boundary)
                        .Do(post => postData.AsParallel().ForAll(line => post.Add(line.Key, line.Value)) )
                        .Do(post => postFiles.AsParallel().ForAll(postFile => post.Add(postFile)))
                        .Select(post => post.ToBytes()))
                    .Do(data => { req.ContentLength = data.Length; })
                    .Do(data =>
                        req.GetRequestStreamAsObserverable()
                            .DisposeSubscribe(stream => stream.Write(data, 0, data.Length)
                        , ex => { throw ex; }))
                    .SelectMany(_ => req.GetResponseAsObserverable());
            }
            else
            {
                return req.GetResponseAsObserverable();
            }
        }

        /// <summary>
        /// エンコードされたPOSTで送信するデータを返します
        /// </summary>
        /// <param name="postData">POSTされるデータ</param>
        /// <param name="enc">エンコーディング</param>
        /// <returns>エンコードされたデータ</returns>
        private byte[] getEncodedPostData(IDictionary<string, string> postData, Encoding enc)
        {
            return Encoding.ASCII.GetBytes(RequestUtility.GetPostData(postData, enc));
        }

        private class PostFileData : IDisposable
        {

            private MemoryStream MemoryStream = new MemoryStream();

            private string _boundary;

            public string Boundary
            {
                get
                {
                    return _boundary;
                }
            }

            public PostFileData()
            {
                _boundary = "--------------------" + Environment.TickCount.ToString();
            }

            public void Add(string name, string value)
            {
                var sb = new StringBuilder();
                sb.AppendLine("--" + Boundary);
                sb.AppendLine("Content-Disposition: form-data; name=\"" + name + "\"");
                sb.AppendLine();
                sb.AppendLine(value);
                var buf = sb.ToString();
                lock (MemoryStream)
                    MemoryStream.Write(Encoding.UTF8.GetBytes(buf), 0, buf.Length);
            }

            public void Add(IPostFile postFile)
            {
                this.Add(postFile.Name, postFile.Stream, postFile.Stream.Length, postFile.FileName, postFile.ContentType);
                postFile.Dispose();
            }

            public void Add(string name, Stream stream, long size, string file, string contentType)
            {
                var sb = new StringBuilder();
                sb.AppendLine("--" + Boundary);
                sb.AppendLine("Content-Disposition: form-data; name=\"" + name + "\"; filename=\"" + file + "\"");
                sb.AppendLine("Content-Type: " + contentType);
                var buf = Encoding.UTF8.GetBytes(sb.ToString());

                lock (MemoryStream)
                {
                    MemoryStream.Write(buf, 0, buf.Length);
                    stream.CopyTo(MemoryStream);
                    MemoryStream.Write(Encoding.ASCII.GetBytes("\n"), 0, "\n".Length);
                }
            }

            public byte[] ToBytes()
            {
                var termination = "--" + _boundary + "--\n";
                using (var stream = new MemoryStream())
                {
                    stream.Write(Encoding.ASCII.GetBytes(termination), 0, termination.Length);
                    MemoryStream.WriteTo(stream);
                    return stream.ToArray();
                }
            }

            #region IDisposable メンバ

            public void Dispose()
            {
                MemoryStream.Dispose();
            }

            #endregion
        }
        #endregion
    }
}
