using Minimax.ScriptableObjects.CardDatas;
using Unity.Netcode;

namespace Minimax.GamePlay.INetworkSerialize
{
    /// <summary>
    /// This struct is used to serialize unit card data over network
    /// </summary>
    public struct NetworkUnitCardData : INetworkSerializable
    {
        public int CardId;
        public int Cost;
        public int Attack;
        public int Health;
        public int Movement;
        // TODO : add abilities as dictionary, but dictionary is not serializable, so use double array instead
        
        public NetworkUnitCardData(UnitBaseData data)
        {
            CardId = data.CardId;
            Cost = data.Cost;
            Attack = data.Attack;
            Health = data.Health;
            Movement = data.Movement;
        }

        public override string ToString()
        {
            return $"CardId: {CardId}, Cost: {Cost}, Attack: {Attack}, Health: {Health}, Movement: {Movement}";
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref CardId);
            serializer.SerializeValue(ref Cost);
            serializer.SerializeValue(ref Attack);
            serializer.SerializeValue(ref Health);
            serializer.SerializeValue(ref Movement);
        }
    }
}