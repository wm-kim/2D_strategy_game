using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WMK
{
    public class CardBaseData : ScriptableObject
    {
        public string CardName;
        public string Description;
        public Sprite CardSprite;
        
        public int Cost;
    }
}
