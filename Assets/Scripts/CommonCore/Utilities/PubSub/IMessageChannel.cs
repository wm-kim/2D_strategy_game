using System;

namespace Utilities.PubSub
{
    public interface IPublisher<T>
    {
        void Publish(T message);
    }

    public interface ISubscriber<T>
    {
        IDisposable Subscribe(Action<T> handler);
        void        Unsubscribe(Action<T> handler);
    }

    public interface IMessageChannel<T> : IPublisher<T>, ISubscriber<T>, IDisposable
    {
        /// <summary>
        ///  Indicates if the message channel is disposed
        /// </summary>
        bool IsDisposed { get; }
    }

    public interface IBufferedMessageChannel<T> : IMessageChannel<T>
    {
        /// <summary>
        /// Indicates whether there's a buffered message waiting.
        /// </summary>
        bool HasBufferedMessage { get; }

        /// <summary>
        /// Indicates whether there's a buffered message waiting.
        /// </summary>
        T BufferedMessage { get; }
    }
}