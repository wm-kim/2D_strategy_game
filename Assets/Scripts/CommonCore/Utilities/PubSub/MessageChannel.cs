using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Utilities.PubSub
{
    public class MessageChannel<T> : IMessageChannel<T>
    {
        /// <summary>
        /// 이 채널에 구독하고 있는 모든 핸들러들의 리스트입니다.
        /// </summary>
        private readonly List<Action<T>> m_MessageHandlers = new();

        /// <summary>
        /// 구독 또는 구독 해지 요청이 발생했을 때 즉시 m_MessageHandlers 리스트를 수정하지 않고, 나중에 한 번에 처리하기 위한 목적으로 사용됩니다.
        /// Key는 핸들러, Value는 true면 추가, false면 제거를 의미합니다.
        /// 이렇게 중간 단계를 두는 이유는 Publish 메서드가 실행되는 도중에 handler list를 수정하면 문제가 발생할 수 있기 때문입니다.
        /// </summary>
        private readonly Dictionary<Action<T>, bool> m_PendingHandlers = new();

        public bool IsDisposed { get; private set; } = false;

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                m_MessageHandlers.Clear();
                m_PendingHandlers.Clear();
            }
        }

        public virtual void Publish(T message)
        {
            // m_PendingHandlers에 있는 핸들러들을 m_MessageHandlers에 추가하거나 제거합니다.
            foreach (var handler in m_PendingHandlers.Keys)
                if (m_PendingHandlers[handler]) // true는 추가 요청
                    m_MessageHandlers.Add(handler);
                else // false는 제거 요청
                    m_MessageHandlers.Remove(handler);

            m_PendingHandlers.Clear();

            // m_MessageHandlers에 있는 핸들러들을 실행합니다.
            foreach (var messageHandler in m_MessageHandlers) messageHandler?.Invoke(message);
        }

        public virtual IDisposable Subscribe(Action<T> handler)
        {
            Assert.IsTrue(!IsSubscribed(handler), "Attempting to subscribe with the same handler more than once");

            // m_PendingHandlers에 핸들러가 있는지 확인합니다.
            if (m_PendingHandlers.ContainsKey(handler))
            {
                // 만약 제거 요청이 이미 있었다면 제거 요청을 취소합니다.
                if (!m_PendingHandlers[handler]) m_PendingHandlers.Remove(handler);
            }
            // 없다면 추가 요청을 합니다.
            else
            {
                m_PendingHandlers[handler] = true;
            }

            var subscription = new DisposableSubscription<T>(this, handler);
            return subscription;
        }

        public void Unsubscribe(Action<T> handler)
        {
            // 핸들러가 구독 중인지 확인합니다.
            if (IsSubscribed(handler))
            {
                // m_PendingHandlers에 핸들러가 있는지 확인합니다.
                if (m_PendingHandlers.ContainsKey(handler))
                {
                    // 만약 추가 요청이 이미 있었다면 추가 요청을 취소합니다.
                    if (m_PendingHandlers[handler]) m_PendingHandlers.Remove(handler);
                }
                // m_PendingHandlers에 핸들러가 없다면 제거 요청을 합니다.
                else
                {
                    m_PendingHandlers[handler] = false;
                }
            }
        }

        private bool IsSubscribed(Action<T> handler)
        {
            var isPendingRemoval = m_PendingHandlers.ContainsKey(handler) && !m_PendingHandlers[handler];
            var isPendingAdding  = m_PendingHandlers.ContainsKey(handler) && m_PendingHandlers[handler];
            return (m_MessageHandlers.Contains(handler) && !isPendingRemoval) || isPendingAdding;
        }
    }
}