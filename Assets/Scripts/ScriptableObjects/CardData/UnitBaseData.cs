using Minimax.GamePlay.INetworkSerialize;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax.ScriptableObjects.CardDatas
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Cards/UnitBaseData")]
    public class UnitBaseData: CardBaseData
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
            instance.CardId = data.CardId;
            instance.Cost = data.Cost;
            instance.Attack = data.Attack;
            instance.Health = data.Health;
            instance.MoveRange = data.MoveRange;
            instance.AttackRange = data.AttackRange;
            return instance; 
        }
    }
}