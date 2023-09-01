using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Minimax.ScriptableObjects.CardDatas
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Cards/CardBaseData")]
    public class CardBaseData : ScriptableObject
    {
        public int CardId;
        public int Cost;
        public string CardName;
        public string Description;
        
        [JsonConverter(typeof(StringEnumConverter))]
        protected CardType CardType;
        
        public CardType GetCardType() => CardType;
        
    }

    public enum CardType
    {
        Undefined,
        Unit,
        Special,
    }
}
