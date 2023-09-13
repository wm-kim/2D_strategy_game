#if DEDICATED_SERVER
using System;
using Cysharp.Threading.Tasks;
using Minimax.Utilities;
using Newtonsoft.Json;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;

namespace Minimax.UnityGamingService.Multiplayer
{
    public class MultiplayAllocationService : IDisposable
    {
        private IMultiplayService m_multiplayService;
        private MultiplayEventCallbacks m_serverCallbacks;
        private IServerEvents m_serverEvents;
        string m_AllocationId;
        
        public MultiplayAllocationService()
        {
            try
            {
                m_multiplayService = MultiplayService.Instance;
            }
            catch (Exception ex)
            {
                DebugWrapper.LogWarning($"Error creating Multiplay allocation service.\n{ex}");
            }
        }
        
        /// <summary>
        /// Should be wrapped in a timeout function
        /// </summary>
        public async UniTask<MatchmakingResults> SubscribeAndAwaitMatchmakerAllocation()
        {
            if (m_multiplayService == null) return null;
            m_AllocationId = null;
            m_serverCallbacks = new MultiplayEventCallbacks();
            m_serverCallbacks.Allocate += OnMultiplayAllocation;
            m_serverCallbacks.Deallocate += OnMultiplayDeAllocation;
            m_serverCallbacks.Error += OnMultiplayError;
            m_serverEvents = await m_multiplayService.SubscribeToServerEventsAsync(m_serverCallbacks);

            var allocationID = await AwaitAllocationID();
            var mmPayload = await GetMatchmakerAllocationPayloadAsync();

            return mmPayload;
        }
        
        private async UniTask<string> AwaitAllocationID()
        {
            var config = m_multiplayService.ServerConfig;
            DebugWrapper.Log($"Awaiting DEDICATED_SERVER Allocation. Server Config is:\n" +
                $"-ServerID: {config.ServerId}\n" +
                $"-AllocationID: {config.AllocationId}\n" +
                $"-Port: {config.Port}\n" +
                $"-QPort: {config.QueryPort}\n" +
                $"-logs: {config.ServerLogDirectory}");

            //Waiting on OnMultiplayAllocation() event (Probably wont ever happen in a matchmaker scenario)
            while (string.IsNullOrEmpty(m_AllocationId))
            {
                var configID = config.AllocationId;
                
                if (!string.IsNullOrEmpty(configID) && string.IsNullOrEmpty(m_AllocationId))
                {
                    DebugWrapper.Log($"Config had AllocationID: {configID}");
                    m_AllocationId = configID;
                }
                
                await UniTask.Delay(100);
            }

            return m_AllocationId;
        }
        
        private async UniTask<MatchmakingResults> GetMatchmakerAllocationPayloadAsync()
        {
            var payloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();
            var modelAsJson = JsonConvert.SerializeObject(payloadAllocation, Formatting.Indented);
            DebugWrapper.Log(nameof(GetMatchmakerAllocationPayloadAsync) + ":" + Environment.NewLine + modelAsJson);
            return payloadAllocation;
        }
        
        private void OnMultiplayAllocation(MultiplayAllocation allocation)
        {
            DebugWrapper.Log($"DEDICATED_SERVER OnAllocation: {allocation.AllocationId}");
            if (string.IsNullOrEmpty(allocation.AllocationId))
                return;
            m_AllocationId = allocation.AllocationId;
        }
        
        private void OnMultiplayDeAllocation(MultiplayDeallocation deallocation)
        {
            DebugWrapper.Log(
                $"DEDICATED_SERVER Deallocated : ID: {deallocation.AllocationId}\nEvent: {deallocation.EventId}\nServer{deallocation.ServerId}");
        }
        
        private void OnMultiplayError(MultiplayError error)
        {
            DebugWrapper.Log($"DEDICATED_SERVER Error : {error.Reason}\n{error.Detail}");
        }

        public void Dispose()
        {
            if (m_serverEvents != null)
            {
                m_serverCallbacks.Allocate -= OnMultiplayAllocation;
                m_serverCallbacks.Deallocate -= OnMultiplayDeAllocation;
                m_serverCallbacks.Error -= OnMultiplayError;
            }
            
            m_serverEvents?.UnsubscribeAsync();
        }
    }
}
#endif