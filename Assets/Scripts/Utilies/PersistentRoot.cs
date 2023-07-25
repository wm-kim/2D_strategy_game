using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimax
{
    public class PersistentRoot : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
