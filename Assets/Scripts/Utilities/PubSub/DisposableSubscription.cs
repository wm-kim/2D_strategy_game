using System;

namespace Minimax.Utilities.PubSub
{
    /// <summary>
    /// This class is a handle to an active Message Channel subscription and when disposed it unsubscribes from said channel.
    /// </summary>
    public class DisposableSubscription<T> : IDisposable
    {
        /// <summary>
        /// 구독하고 있는 채널입니다.
        /// </summary>
        private IMessageChannel<T> m_MessageChannel;
        private Action<T> m_Handler;

        private bool m_IsDisposed;
        
        public DisposableSubscription(IMessageChannel<T> messageChannel, Action<T> handler)
        {
            m_MessageChannel = messageChannel;
            m_Handler = handler;
        }
        
        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                m_IsDisposed = true;
                
                // 채널이 아직 Dispose되지 않았다면, 채널의 구독을 해지합니다.
                if (!m_MessageChannel.IsDisposed)
                {
                    m_MessageChannel.Unsubscribe(m_Handler);
                }

                m_Handler = null;
                m_MessageChannel = null;
            }
        }
    }
}
