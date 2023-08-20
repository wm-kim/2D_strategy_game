using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Minimax.Utilities.PubSub
{
    public class NetworkedMessageChannelManager
    {
        // 네트워크 메시지 채널 저장
        private readonly Dictionary<Type, object> m_NetworkedMessageChannels = new Dictionary<Type, object>();
        
        // 네트워크 메시지 채널을 생성합니다.
        public IMessageChannel<T> RegisterNetworkedMessageChannel<T>() where T : unmanaged, INetworkSerializeByMemcpy
        {
            var messageType = typeof(T);

            // 이미 등록된 네트워크 메시지 채널이 있다면, 해당 채널을 반환합니다.
            if (m_NetworkedMessageChannels.TryGetValue(messageType, out var channel))
            {
                DebugWrapper.LogWarning($"Networked message channel for {messageType.Name} already exists. Returning existing channel.");
                return (IMessageChannel<T>)channel;
            }
            
            // 등록된 네트워크 메시지 채널이 없다면, 새로운 네트워크 메시지 채널을 생성하고 저장합니다.
            var networkedMessageChannel = new NetworkedMessageChannel<T>();
            m_NetworkedMessageChannels.Add(messageType, networkedMessageChannel);
            return networkedMessageChannel;
        }
        
        // 네트워크 메시지 채널을 제거합니다.
        public void UnregisterNetworkedMessageChannel<T>() where T : unmanaged, INetworkSerializeByMemcpy
        {
            var messageType = typeof(T);
            
            // 등록된 네트워크 메시지 채널이 있다면, 해당 채널을 제거합니다.
            if (m_NetworkedMessageChannels.TryGetValue(messageType, out var channel))
            {
                ((IDisposable)channel).Dispose();
                m_NetworkedMessageChannels.Remove(messageType);
            }
            else
            {
                DebugWrapper.LogWarning($"Networked message channel for {messageType.Name} does not exist.");
            }
        }
    }
}