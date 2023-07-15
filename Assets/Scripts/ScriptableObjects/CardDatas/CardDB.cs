using UnityEngine;

namespace WMK
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Cards/CardDB")]
    public class CardDB : ScriptableObject
    {
        public CardBaseData[] cardDatas;
    }
}