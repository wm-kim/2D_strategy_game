using Unity.Collections;
using Unity.Netcode;

namespace Utilities.PubSub
{
    // 서버에서 메시지를 보내면 그 메시지가 클라이언트들에게도 전달되고 동시에 서버 내부에서도 공유되는 채널입니다.
    // 클라이언트와 서버 모두 이 채널을 구독해서 메시지를 받을 수 있습니다.
    public class NetworkedMessageChannel<T> : MessageChannel<T> where T : unmanaged, INetworkSerializeByMemcpy
    {
        private NetworkManager m_NetworkManager;
        private string         m_Name;

        public NetworkedMessageChannel()
        {
            m_Name           = $"{typeof(T).FullName}NetworkMessageChannel";
            m_NetworkManager = NetworkManager.Singleton;

            if (m_NetworkManager == null)
            {
                DebugWrapper.LogError("NetworkedMessageChannel must be created after NetworkManager is initialized.");
                return;
            }

            m_NetworkManager.OnClientConnectedCallback += OnClientConnected;
            if (m_NetworkManager.IsListening) RegisterHandler();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
                if (m_NetworkManager != null && m_NetworkManager.CustomMessagingManager != null)
                    m_NetworkManager.CustomMessagingManager.UnregisterNamedMessageHandler(m_Name);
            base.Dispose();
        }

        private void OnClientConnected(ulong clientId)
        {
            RegisterHandler();
        }

        private void RegisterHandler()
        {
            // Only register message handler on clients
            if (!m_NetworkManager.IsServer)
                m_NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(m_Name,
                    ReceiveMessageThroughNetwork);
        }

        public override void Publish(T message)
        {
            if (m_NetworkManager.IsServer)
            {
                // send message to clients, then publish locally
                SendMessageThroughNetwork(message);
                base.Publish(message);
            }
            else
            {
                DebugWrapper.LogError("Only a server can publish in a NetworkedMessageChannel");
            }
        }

        public void Publish(T message, ulong clientId)
        {
            if (m_NetworkManager.IsServer)
                SendMessageThroughNetwork(message, clientId);
            else
                DebugWrapper.LogError("Only a server can publish in a NetworkedMessageChannel");
        }


        private void SendMessageThroughNetwork(T message)
        {
            using var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
            writer.WriteValueSafe(message);
            m_NetworkManager.CustomMessagingManager.SendNamedMessageToAll(m_Name, writer);
        }

        private void SendMessageThroughNetwork(T message, ulong clientId)
        {
            using var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
            writer.WriteValueSafe(message);
            m_NetworkManager.CustomMessagingManager.SendNamedMessage(m_Name, clientId, writer);
        }

        private void ReceiveMessageThroughNetwork(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out T message);
            base.Publish(message);
        }
    }
}