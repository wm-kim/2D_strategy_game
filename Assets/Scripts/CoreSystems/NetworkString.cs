using Unity.Collections;
using Unity.Netcode;

namespace Minimax.Utilities
{
    /// <summary>
    /// Wrapping FixedString so that if we want to change string max size in the future, we only do it once here
    /// </summary>
    public struct NetworkString : INetworkSerializable
    {
        private ForceNetworkSerializeByMemcpy<FixedString32Bytes> m_info;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref m_info);
        }

        public override string ToString()
        {
            return m_info.Value.ToString();
        }

        public static implicit operator string(NetworkString s)
        {
            return s.ToString();
        }

        public static implicit operator NetworkString(string s)
        {
            return new NetworkString { m_info = new FixedString32Bytes(s) };
        }
    }
}