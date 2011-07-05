using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Reactive.Linq;

namespace NekoVampire.Web
{
    public static class WebRequestExtentions
    {
        public static IObservable<WebResponse> GetResponseAsObserverable(this WebRequest req)
        {
            return Observable.FromAsyncPattern<WebResponse>(req.BeginGetResponse, req.EndGetResponse)();
        }

        public static IObservable<Stream> GetRequestStreamAsObserverable(this WebRequest req)
        {
            return Observable.FromAsyncPattern<Stream>(req.BeginGetRequestStream, req.EndGetRequestStream)();
        }

        public static IObservable<String> GetResponseText(this WebResponse res)
        {
            return Observable.Using(() => res.GetResponseStream(), s =>
                    Observable.Using(() => new StreamReader(s), sr => new[] { sr.ReadToEnd() }.ToObservable()));
        }

        public static IObservable<String> GetResponseLines(this WebResponse res)
        {
            return Observable.Using(() => res.GetResponseStream(), s =>
                    Observable.Using(() => new StreamReader(s), sr => Observable.Repeat(sr)))
                .TakeWhile(sr => !sr.EndOfStream)
                .Select(sr => sr.ReadLine());
        }

        public static IObservable<String> GetResponseText(this IObservable<WebResponse> res)
        {
            return res.SelectMany(_ => _.GetResponseText());
        }

        public static IObservable<String> GetResponseLines(this IObservable<WebResponse> res)
        {
            return res.SelectMany(_ => _.GetResponseLines());
        }
    }
}
