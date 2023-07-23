using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Minimax
{
    public class PlayerProfileController : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_playerNameText;
        
        public void SetPlayerName(string name) => m_playerNameText.text = name;
    }
}
