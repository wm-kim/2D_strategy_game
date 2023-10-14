using System.Runtime.Serialization;

namespace Minimax.ScriptableObjects.CardDatas
{
    [System.Serializable]
    public class CardAbility
    {
        public enum AbilityName
        {
            AttackRange,
        }
        
        public AbilityName Name;
        public int Value;
        
        public CardAbility(AbilityName name, int value)
        {
            Name = name;
            Value = value;
        }
        
        public override string ToString()
        {
            return $"[{Name}: {Value}]";
        }
    }
}