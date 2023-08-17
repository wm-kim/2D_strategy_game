using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    public class GameManager : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
            }
        }
    }
}
