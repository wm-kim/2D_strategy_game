using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Minimax.ScriptableObjects.CardDatas
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Cards/CardBaseData")]
    public class CardBaseData : ScriptableObject
    {
        [Header("General Info")]
        public int CardId;
        public int Cost;
        public string CardName;
        public string Description;
        
        [Header("Card Abilities")]
        public List<CardAbility> CardAbilities = new List<CardAbility>();
        
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
