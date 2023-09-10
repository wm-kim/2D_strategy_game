using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Minimax.Utilities;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;

namespace Minimax.UnityGamingService.Multiplayer
{
    public class MatchplayBackfiller : IDisposable
    {
        public bool Backfilling { get; private set; } = false;
        
        private CreateBackfillTicketOptions m_createBackfillOptions;
        private BackfillTicket m_localBackfillTicket;
        private const int k_ticketCheckMs = 1000;
        private bool m_localDataDirty = false;
        
        int MatchPlayerCount => m_localBackfillTicket?.Properties.MatchProperties.Players.Count ?? 0;
        
        public MatchplayBackfiller(string connection, string queueName, MatchProperties matchmakerPayloadProperties)
        {
            var backfillProperties = new BackfillTicketProperties(matchmakerPayloadProperties);
            m_localBackfillTicket = new BackfillTicket { Id = matchmakerPayloadProperties.BackfillTicketId, Properties = backfillProperties };

            m_createBackfillOptions = new CreateBackfillTicketOptions
            {
                Connection = connection,
                QueueName = queueName,
                Properties = backfillProperties
            };
        }

        public async UniTask BeginBackfilling()
        {
            if (Backfilling)
            {
                DebugWrapper.LogWarning("Already backfilling, no need to start another.");
                return;
            }

            DebugWrapper.Log(
                $"Starting backfill  Server: {m_localBackfillTicket.Properties.MatchProperties.Players.Count}/{Define.MaxConnectedPlayers}");

            // Create a ticket if we don't have one already (via Allocation)
            if (string.IsNullOrEmpty(m_localBackfillTicket.Id))
                m_localBackfillTicket.Id =
                    await MatchmakerService.Instance.CreateBackfillTicketAsync(m_createBackfillOptions);

            Backfilling = true;
            
#pragma warning disable 4014
            BackfillLoop();
#pragma warning restore 4014
        }

        public void AddPlayerToMatch(string playerId)
        {
            if (!Backfilling)
            {
                DebugWrapper.LogWarning("Can't add users to the backfill ticket before it's been created");
                return;
            }
            
            if (GetPlayerById(playerId) != null)
            {
                DebugWrapper.LogWarning($"Player {playerId} is already in the Match. Ignoring add.");
                return;
            }
            
            var matchmakerPlayer = new Player(playerId);
            
            m_localBackfillTicket.Properties.MatchProperties.Players.Add(matchmakerPlayer);
            m_localBackfillTicket.Properties.MatchProperties.Teams[0].PlayerIds.Add(matchmakerPlayer.Id);
            m_localDataDirty = true;
        }

        public int RemovePlayerFromMatch(string playerId)
        {
            var playerToRemove = GetPlayerById(playerId);
            if (playerToRemove == null)
            {
                DebugWrapper.LogWarning($"Player {playerId} is not in local backfill Data.");
                return MatchPlayerCount;
            }
            
            m_localBackfillTicket.Properties.MatchProperties.Players.Remove(playerToRemove);
            
            // We Only have one team in this game, so this simplifies things here
            m_localBackfillTicket.Properties.MatchProperties.Teams[0].PlayerIds.Remove(playerId);
            m_localDataDirty = true;
            
            return MatchPlayerCount;
        }
        
        public bool NeedsPlayers()
        {
            return MatchPlayerCount < Define.MaxConnectedPlayers;
        }
        
        Player GetPlayerById(string playerId)
        {
            return m_localBackfillTicket.Properties.MatchProperties.Players.FirstOrDefault(p => p.Id.Equals(playerId));
        }

        private async UniTask BackfillLoop()
        {
            while (Backfilling)
            {
                if (m_localDataDirty)
                {
                    await MatchmakerService.Instance.UpdateBackfillTicketAsync(m_localBackfillTicket.Id, m_localBackfillTicket);
                    m_localDataDirty = false;
                }
                else
                {
                    m_localBackfillTicket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(m_localBackfillTicket.Id);
                }

                if (!NeedsPlayers())
                {
                    await StopBackfill();
                    break;
                }
                
                // Backfill Docs recommend a once-per-second approval for backfill tickets
                await UniTask.Delay(k_ticketCheckMs);
            }
        }
                
        public async UniTask StopBackfill()
        {
            if (!Backfilling)
            {
                DebugWrapper.LogError("Can't stop backfilling before we start.");
                return;
            }

            await MatchmakerService.Instance.DeleteBackfillTicketAsync(m_localBackfillTicket.Id);
            Backfilling = false;
            m_localBackfillTicket.Id = null;
        }

        public void Dispose()
        {
            StopBackfill().Forget();
        }
    }
}