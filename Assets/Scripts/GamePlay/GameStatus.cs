using System;
using System.Collections.Generic;
using Minimax.Utilities;
using Minimax.Utilities.PubSub;
using Unity.Netcode;

namespace Minimax.GamePlay
{
    public enum GameState
    {
        Invalid,
        GameStarted,
        GameEnded
    }
    
    public struct GameResultMessage : INetworkSerializeByMemcpy
    {
        public int LoserPlayerNumber;
    }
    
    /// <summary>
    /// 게임 세션의 결과를 클라이언트에게 전달하고 받기 위한 채널을 제공하며
    /// 게임의 상태를 저장하고 관리합니다.
    /// TODO : Store the game result into cloud save.
    /// TODO : Wrap publish method and check if client is still connected.
    /// </summary>
    public class GameStatus
    {
        private NetworkedMessageChannel<GameResultMessage> m_gameResultChannel;
        private GameState m_gameState = GameState.Invalid;
        
        /// <summary>
        /// 아직 게임이 시작되지 않았을 때, 게임 결과를 구독하려는 클라이언트들의 리스트를 저장합니다.
        /// </summary>
        private List<Action<GameResultMessage>> m_pendingGameResultSubscribers = new List<Action<GameResultMessage>>();
        
        public event Action<GameState> OnGameStateChanged;
        
        public void StartGame()
        {
            m_gameResultChannel = new NetworkedMessageChannel<GameResultMessage>();
            // add pending subscribers
            AddPendingSubscribers();
            
            m_gameState = GameState.GameStarted;
            OnGameStateChanged?.Invoke(m_gameState);
        }
        
        /// <summary>
        /// 게임이 끝났음을 클라이언트에게 알립니다.
        /// </summary>
        public void EndGameWithResult(int loserPlayerNumber)
        {
            m_gameResultChannel.Publish(new GameResultMessage { LoserPlayerNumber = loserPlayerNumber });
            m_gameState = GameState.GameEnded;
            OnGameStateChanged?.Invoke(m_gameState);
            m_gameResultChannel.Dispose();
            m_gameResultChannel = null;
        }
        
        private void AddPendingSubscribers()
        {
            foreach (var subscriber in m_pendingGameResultSubscribers)
            {
                m_gameResultChannel.Subscribe(subscriber);
            }
            m_pendingGameResultSubscribers.Clear();
        }
        
        /// <summary>
        /// Wrap the subscribe method of the game result channel.
        /// </summary>
        /// <param name="handler"></param>
        public void SubscribeToGameResult(Action<GameResultMessage> handler)
        {
            // 만약 게임이 아직 시작되지 않았다면, pending list에 추가합니다.
            switch (m_gameState)
            {
                case GameState.Invalid:
                    m_pendingGameResultSubscribers.Add(handler);
                    break;
                case GameState.GameStarted:
                    m_gameResultChannel.Subscribe(handler);
                    break;
                case GameState.GameEnded:
                    DebugWrapper.LogError("Cannot subscribe to game result after game has ended.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void UnsubscribeFromGameResult(Action<GameResultMessage> handler)
        {
            switch (m_gameState)
            {
                case GameState.Invalid:
                case GameState.GameEnded:
                    m_pendingGameResultSubscribers.Remove(handler);
                    break;
                case GameState.GameStarted:
                    m_gameResultChannel.Unsubscribe(handler);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}