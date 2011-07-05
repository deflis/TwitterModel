using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NekoVampire.Web
{
    public static class DisposableExtentions
    {
        public static IDisposable DisposeSubscribe<T>(this IObservable<T> observableStream)
            where T : IDisposable
        {
            return observableStream.Subscribe(stream => stream.Dispose());
        }

        public static IDisposable DisposeSubscribe<T>(this IObservable<T> observeDisposable, Action<T> onNext)
            where T : IDisposable
        {
            return observeDisposable.Subscribe(disposable => { onNext(disposable); disposable.Dispose(); return; });
        }

        public static IDisposable DisposeSubscribe<T>(this IObservable<T> observeDisposable, Action<T> onNext, Action onCompleted)
            where T : IDisposable
        {
            return observeDisposable.Subscribe(disposable => { onNext(disposable); disposable.Dispose(); return; }, onCompleted);
        }

        public static IDisposable DisposeSubscribe<T>(this IObservable<T> observeDisposable, Action<T> onNext, Action<Exception> onError)
            where T : IDisposable
        {
            return observeDisposable.Subscribe(disposable => { onNext(disposable); disposable.Dispose(); return; }, onError);
        }

        public static IDisposable DisposeSubscribe<T>(this IObservable<T> observeDisposable, Action<T> onNext, Action<Exception> onError, Action onCompleted)
            where T : IDisposable
        {
            return observeDisposable.Subscribe(disposable => { onNext(disposable); disposable.Dispose(); return; }, onError, onCompleted);
        }
    }
}
