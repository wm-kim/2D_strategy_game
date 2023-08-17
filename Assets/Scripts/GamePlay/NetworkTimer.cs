using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    public class NetworkTimer : NetworkBehaviour
    {
        private NetworkVariable<bool> _countdownStarted = new NetworkVariable<bool>(false);
        
        
    }
}
