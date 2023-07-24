using UnityEngine;

namespace Minimax.ScriptableObjects.CardDatas
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Cards/CardDB")]
    public class CardDB : ScriptableObject
    {
        public CardBaseData[] cardDatas;
    }
}