using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minimax.Utilities;
using Unity.Services.Multiplay;

namespace Minimax.UnityGamingService.Multiplayer
{
    public class MultiplayServerQueryService : IDisposable
    {
        private IMultiplayService m_multiplayService;
        private IServerQueryHandler m_serverQueryHandler;
        private CancellationTokenSource m_serverCheckCancel;
        
        public MultiplayServerQueryService()
        {
            try
            {
                m_multiplayService = MultiplayService.Instance;
                m_serverCheckCancel = new CancellationTokenSource();
            }
            catch (System.Exception ex)
            {
                DebugWrapper.LogWarning($"Error creating Multiplay allocation service.\n{ex}");
            }
        }
        
        public async UniTask BeginServerQueryHandler()
        {
            if (m_multiplayService == null)
                return;

            m_serverQueryHandler = await m_multiplayService.StartServerQueryHandlerAsync((ushort)Define.MaxConnectedPlayers,
                "MyServerName", "CardWars", "1.0", "Default");
            
            ServerQueryLoop(m_serverCheckCancel.Token).Forget();
        }
        
        public void SetPlayerCount(ushort count) => m_serverQueryHandler.CurrentPlayers = count;
        public void AddPlayer() => m_serverQueryHandler.CurrentPlayers += 1;
        public void RemovePlayer() => m_serverQueryHandler.CurrentPlayers -= 1;
        
        private async UniTask ServerQueryLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                m_serverQueryHandler.UpdateServerCheck();
                await UniTask.Delay(100);
            }
        }

        public void Dispose()
        {
            if (m_serverCheckCancel != null)
            {
                m_serverCheckCancel.Cancel();
            }
        }
    }
}