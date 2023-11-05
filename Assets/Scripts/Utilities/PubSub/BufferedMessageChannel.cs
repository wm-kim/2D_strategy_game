using System;

namespace Minimax.Utilities.PubSub
{
    /// <summary>
    /// 마지막으로 발행된 메시지를 저장하고(버퍼링하고) 새로운 구독자가 등록될 때 그 메시지를 즉시 수신하게 합니다.
    /// 최근의 게임 상태와 같은 정보를 새로 접속한 플레이어에게 즉시 전달하기 위해 사용될 수 있습니다.
    /// </summary>
    public class BufferedMessageChannel<T> : MessageChannel<T>, IBufferedMessageChannel<T>
    {
        public override void Publish(T message)
        {
            HasBufferedMessage = true;
            BufferedMessage    = message;
            base.Publish(message);
        }

        public override IDisposable Subscribe(Action<T> handler)
        {
            var subscription = base.Subscribe(handler);

            if (HasBufferedMessage) handler?.Invoke(BufferedMessage);

            return subscription;
        }

        /// <summary>
        /// 버퍼링된 메시지가 있는지 여부를 나타냅니다.
        /// </summary>
        public bool HasBufferedMessage { get; private set; } = false;

        public T BufferedMessage { get; private set; }
    }
}