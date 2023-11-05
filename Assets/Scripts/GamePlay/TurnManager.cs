using System;
using Minimax.CoreSystems;
using Minimax.Definitions;
using Minimax.UnityGamingService.Multiplayer;
using TMPro;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Debug = Utilities.Debug;
using Random = UnityEngine.Random;

namespace Minimax.GamePlay
{
    // TODO : separate out visual logic from this class into client turn manager
    public class TurnManager : NetworkBehaviour
    {
        public static TurnManager Instance { get; private set; }

        [Header("References")] [SerializeField]
        private NetworkTimer m_networkTimer;

        [SerializeField] private Button          m_endTurnButton;
        [SerializeField] private TextMeshProUGUI m_turnText;

        /// <summary>
        /// 서버에서 턴이 시작될 때 호출됩니다. 인자는 플레이어 번호입니다.
        /// </summary>
        public event Action<int> OnServerTurnStart;

        /// <summary>
        /// 클라이언트에서 턴이 시작될 때 호출됩니다. 인자는 플레이어 번호입니다.
        /// </summary>
        public event Action<int> OnClientTurnStart;

        private NetworkManager       m_networkManager => NetworkManager.Singleton;
        private NetworkVariable<int> m_whosTurn       = new(-1);
        private int                  m_myPlayerNumber = -1;

        // if my player number is -1, it means it's not set yet
        // automatically call SetMyPlayerNumberServerRpc to set it
        public int MyPlayerNumber
        {
            get
            {
                if (m_myPlayerNumber == -1) SetMyPlayerNumberServerRpc();

                return m_myPlayerNumber;
            }
        }

        public int OpponentPlayerNumber =>
            SessionPlayerManager.Instance.GetOpponentPlayerNumber(MyPlayerNumber);

        public bool IsMyTurn => m_whosTurn.Value == MyPlayerNumber;

        private void Awake()
        {
            Instance = this;
            m_endTurnButton.onClick.AddListener(RequestEndTurn);
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                m_whosTurn.OnValueChanged += OnTurnChanged;
                SetMyPlayerNumberServerRpc();
            }

            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient) m_whosTurn.OnValueChanged -= OnTurnChanged;

            base.OnNetworkDespawn();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetMyPlayerNumberServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var senderClientId = serverRpcParams.Receive.SenderClientId;
            var playerNumber   = SessionPlayerManager.Instance.GetPlayerNumber(senderClientId);
            SetMyPlayerNumberClientRpc(playerNumber, SessionPlayerManager.Instance.ClientRpcParams[senderClientId]);
        }

        [ClientRpc]
        private void SetMyPlayerNumberClientRpc(int myPlayerNumber, ClientRpcParams clientRpcParams = default)
        {
            m_myPlayerNumber = myPlayerNumber;
            Debug.Log($"My Player Number is {m_myPlayerNumber}");
        }

        public void StartInitialTurn()
        {
            if (!IsServer) return;

            DecideWhoGoesFirst();
            m_networkTimer.ConFig(Define.TurnTimeLimit, StartNewTurn);
            OnServerTurnStart?.Invoke(m_whosTurn.Value);
            m_networkTimer.StartTimer();
        }

        /// <summary>
        /// 랜덤으로 누가 먼저 턴을 시작할지 결정합니다.
        /// </summary>
        private void DecideWhoGoesFirst()
        {
            if (!IsServer) return;

            m_whosTurn.Value = Random.Range(0, 2);
            Debug.Log($"Player {m_whosTurn.Value} goes first");
        }

        private void StartNewTurn()
        {
            if (!IsServer) return;
            m_whosTurn.Value = SessionPlayerManager.Instance.GetOpponentPlayerNumber(m_whosTurn.Value);
            OnServerTurnStart?.Invoke(m_whosTurn.Value);
            m_networkTimer.StartTimer();
        }

        /// <summary>
        /// 클라이언트가 턴을 끝내기를 요청합니다.
        /// </summary>
        private void RequestEndTurn()
        {
            if (!IsClient) return;
            EndTurnServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void EndTurnServerRpc(ServerRpcParams serverRpcParams = default)
        {
            // check if it's the player's turn
            var senderClientId = serverRpcParams.Receive.SenderClientId;
            if (!CheckIfPlayerTurn(senderClientId)) return;
            m_networkTimer.EndTimerImmediately();
        }

        private void OnTurnChanged(int oldPlayerNumber, int newPlayerNumber)
        {
            Debug.Log($"Player {newPlayerNumber} Turn Starts");
            OnClientTurnStart?.Invoke(newPlayerNumber);
            var isMyTurn = newPlayerNumber == MyPlayerNumber;
            m_turnText.text              = isMyTurn ? "End Turn" : "Opponent's Turn";
            m_endTurnButton.interactable = isMyTurn;
        }

        /// <summary>
        /// Used by clients to check if it's their turn, and log if it is not.
        /// </summary>
        public bool CheckIfMyTurn(string logMessage = "")
        {
            if (!IsMyTurn)
            {
                Debug.LogError(
                    $"Player {MyPlayerNumber} request denied to {logMessage} because it's not their turn");
                return false;
            }

            return true;
        }

        /// <summary>
        /// used by server to check if the player is allowed to do something
        /// and log if it is not.
        /// </summary>
        public bool CheckIfPlayerTurn(ulong clientId)
        {
            if (!IsServer) return false;

            var playerNumber = SessionPlayerManager.Instance.GetPlayerNumber(clientId);
            return CheckIfPlayerTurn(playerNumber);
        }

        public bool CheckIfPlayerTurn(int playerNumber)
        {
            if (!IsServer) return false;

            if (playerNumber != m_whosTurn.Value)
            {
                Debug.LogError($"Player {playerNumber} request denied because it's not their turn");
                return false;
            }

            return true;
        }
    }
}