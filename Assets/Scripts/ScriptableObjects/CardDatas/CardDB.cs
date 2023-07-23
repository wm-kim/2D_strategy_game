using UnityEngine;

namespace Minimax
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Cards/CardDB")]
    public class CardDB : ScriptableObject
    {
        public CardBaseData[] cardDatas;
    }
}