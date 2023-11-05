using UnityEngine;

namespace Minimax.ScriptableObjects.CardData
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Cards/UnitBaseData")]
    public class SpecialBaseData : CardBaseData
    {
        public SpecialBaseData()
        {
            CardType = CardType.Special;
        }
    }
}