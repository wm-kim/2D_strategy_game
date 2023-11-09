using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Minimax.ScriptableObjects.CardData
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Cards/CardBaseData")]
    public class CardBaseData : ScriptableObject
    {
        [Header("General Info")]
        public int CardId;

        public int    Cost;
        public string CardName;
        public string Description;

        [Header("Ability Prefab")]
        [SerializeField]
        private GameObject m_abilityPrefab;

        [JsonConverter(typeof(StringEnumConverter))]
        protected CardType CardType;

        public CardType GetCardType()
        {
            return CardType;
        }
    }

    public enum CardType
    {
        Undefined,
        Unit,
        Special
    }
}