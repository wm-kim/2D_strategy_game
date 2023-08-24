using System.Collections.Generic;
using Unity.Netcode;

namespace Minimax.UnityGamingService.Multiplayer
{
    /// <summary>
    /// Server에서만 사용되며 Server가 클라이언트에게 RPC를 보낼 때 사용하는 파라미터들을 관리합니다.
    /// 매번 새로 생성하는 것보다 캐싱하는 것이 효율적이기 때문에 사용합니다.
    /// </summary>
    public class ClientRpcParamManager
    {
        private Dictionary<ulong, ClientRpcParams> m_clientRpcParams = new Dictionary<ulong, ClientRpcParams>();

        /// <summary>
        /// 주어진 클라이언트 ID에 대한 RPC 파라미터를 반환합니다. 파라미터가 없으면 새로 생성합니다.
        /// </summary>
        public ClientRpcParams this[ulong clientId]
        {
            get
            {
                if (!m_clientRpcParams.TryGetValue(clientId, out var rpcParams))
                {
                    rpcParams = new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new ulong[] { clientId }
                        }
                    };
                    m_clientRpcParams[clientId] = rpcParams;
                }
                
                return rpcParams;
            }
        }
        
        public void Remove(ulong clientId) => m_clientRpcParams.Remove(clientId);
        
        public void Clear()
        {
            m_clientRpcParams.Clear();
        }
    }
}