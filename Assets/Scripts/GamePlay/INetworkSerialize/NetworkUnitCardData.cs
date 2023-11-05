using System.Text;
using Minimax.ScriptableObjects.CardData;
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
        public int MoveRange;

        public int AttackRange;
        // TODO : add abilities as dictionary, but dictionary is not serializable, so use double array instead

        public NetworkUnitCardData(UnitBaseData data)
        {
            CardId      = data.CardId;
            Cost        = data.Cost;
            Attack      = data.Attack;
            Health      = data.Health;
            MoveRange   = data.MoveRange;
            AttackRange = data.AttackRange;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CardId: {CardId}");
            sb.AppendLine($"Cost: {Cost}");
            sb.AppendLine($"Attack: {Attack}");
            sb.AppendLine($"Health: {Health}");
            sb.AppendLine($"MoveRange: {MoveRange}");
            sb.AppendLine($"AttackRange: {AttackRange}");
            return sb.ToString();
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref CardId);
            serializer.SerializeValue(ref Cost);
            serializer.SerializeValue(ref Attack);
            serializer.SerializeValue(ref Health);
            serializer.SerializeValue(ref MoveRange);
            serializer.SerializeValue(ref AttackRange);
        }
    }
}