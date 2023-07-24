using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Minimax.ScriptableObjects.CardDatas
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Cards/CardBaseData")]
    public class CardBaseData : ScriptableObject
    {
        public string CardName;
        public string Description;
        
        [JsonConverter(typeof(StringEnumConverter))]
        public CardType CardType;
        public Sprite CardSprite;
        
        public int Cost;
    }

    public enum CardType
    {
        Undefined,
        Unit,
        Spell,
    }
}
