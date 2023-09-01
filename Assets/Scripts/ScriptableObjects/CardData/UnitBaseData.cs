using UnityEngine;

namespace Minimax.ScriptableObjects.CardDatas
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Cards/UnitBaseData")]
    public class UnitBaseData: CardBaseData
    {
        public int Attack;
        public int Health;
        public int Movement;
        
        public UnitBaseData()
        {
            CardType = CardType.Unit;
        }
    }
}