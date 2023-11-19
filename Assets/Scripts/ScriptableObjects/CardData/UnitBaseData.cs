using Minimax.GamePlay.INetworkSerialize;
using UnityEngine;

namespace Minimax.ScriptableObjects.CardData
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Cards/UnitBaseData")]
    public class UnitBaseData : CardBaseData
    {
        [Header("Unit Info")]
        public int Attack;

        public int Health;
        public int MoveRange;
        public int AttackRange;

        public UnitBaseData()
        {
            CardType = CardType.Unit;
        }

        // factory method passing in network unit card data
        public static UnitBaseData CreateInstance(NetworkUnitCardData data)
        {
            var instance = CreateInstance<UnitBaseData>();
            instance.CardId      = data.CardId;
            instance.Cost        = data.Cost;
            instance.Attack      = data.Attack;
            instance.Health      = data.Health;
            instance.MoveRange   = data.MoveRange;
            instance.AttackRange = data.AttackRange;
            return instance;
        }
        
        public override string ToString()
        {
            var sb = base.ToString();
            sb += $"Attack: {Attack}\n";
            sb += $"Health: {Health}\n";
            sb += $"MoveRange: {MoveRange}\n";
            sb += $"AttackRange: {AttackRange}\n";
            return sb;
        }
    }
}