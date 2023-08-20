using System;
using System.Collections.Generic;

namespace Minimax.Utilities.PubSub
{
    public class MessageChannelManager
    {
        // 일반 메시지 채널 저장
        private readonly Dictionary<Type, object> m_MessageChannels = new Dictionary<Type, object>();

        // 메시지 채널을 생성합니다.
        public IMessageChannel<T> RegisterMessageChannel<T>()
        {
            var messageType = typeof(T);

            // 이미 등록된 메시지 채널이 있다면, 해당 채널을 반환합니다.
            if (m_MessageChannels.TryGetValue(messageType, out var channel))
            {
                DebugWrapper.LogWarning($"Message channel for {messageType.Name} already exists. Returning existing channel.");
                return (IMessageChannel<T>)channel;
            }

            // 등록된 메시지 채널이 없다면, 새로운 메시지 채널을 생성하고 저장합니다.
            var messageChannel = new MessageChannel<T>();
            m_MessageChannels.Add(messageType, messageChannel);
            return messageChannel;
        }
        
        // 메시지 채널을 제거합니다.
        public void UnregisterMessageChannel<T>()
        {
            var messageType = typeof(T);
            
            // 등록된 메시지 채널이 있다면, 해당 채널을 제거합니다.
            if (m_MessageChannels.TryGetValue(messageType, out var channel))
            {
                ((IDisposable)channel).Dispose();
                m_MessageChannels.Remove(messageType);
            }
            else
            {
                DebugWrapper.LogWarning($"Message channel for {messageType.Name} does not exist.");
            }
        }
    }
}