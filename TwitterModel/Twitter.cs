using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Codeplex.Data;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using NekoVampire.Web;

namespace NekoVampire.TwitterCore
{
    public class Twitter
    {
        ReactiveOAuthClient client;
        public Twitter(IOAuthKey authKey)
        {
            this.AuthKey = authKey;
            if(authKey.HasAccessToken)
                client = new ReactiveOAuthClient(authKey.ConsumerKey, authKey.ConsumerSecret, authKey.requestTokenURL, authKey.accessTokenURL, authKey.authorizeURL, authKey.AccessToken, authKey.AccessTokenSecret);
            else
                client = new ReactiveOAuthClient(authKey.ConsumerKey, authKey.ConsumerSecret, authKey.requestTokenURL, authKey.accessTokenURL, authKey.authorizeURL);
        }

        protected IOAuthKey AuthKey;
        protected const string ApiUrl = "http://api.twitter.com/1/";
        protected ConcurrentDictionary<long, IStatus> statusCache = new ConcurrentDictionary<long, IStatus>();

        public ReadOnlyCollection<IStatus> StatusCache
        {
            get { return new ReadOnlyCollection<IStatus>(statusCache.Values.ToArray()); }
        }

        #region TwitterAPI
        public IObservable<IStatus> GetPublicTimeline()
        {
            var sb = new StringBuilder(ApiUrl);
            sb.Append("statuses/public_timeline.json");
            return client.HttpRequest(sb.ToString(), "GET")
                .GetResponseText()
                .ObserveOn(Scheduler.TaskPool)
                .SelectMany(s => (dynamic[])DynamicJson.Parse(s))
                .Select(status => new Status(status))
                .Do(status => statusCache.AddOrUpdate(status.ID, status, (id, oldItem) => status));
        }
        #endregion

        public IObservable<IStatus> GetUserStreams()
        {
            return client.HttpRequest("https://userstream.twitter.com/2/user.json", "GET")
                .GetResponseLines()
                .ObserveOn(Scheduler.TaskPool)
                .Select(s => DynamicJson.Parse(s))
                .Select(status => new Status(status))
                .Do(status => statusCache.AddOrUpdate(status.ID, status, (id, oldItem) => status));
        }

        public IObservable<IStatus> GetPublicTimelineInterval(TimeSpan t)
        {
            return Observable.Timer(t)
                .Repeat()
                .TimeInterval()
                .SelectMany(x => this.GetPublicTimeline());
        }
    }
}
