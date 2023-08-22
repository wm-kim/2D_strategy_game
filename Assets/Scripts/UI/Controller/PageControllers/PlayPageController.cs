using System;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.SceneManagement;
using Minimax.Utilities;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.Controller.PageControllers
{
    public class PlayPageController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button m_startServerButton;

        [SerializeField] private Button m_startHostButton;
        [SerializeField] private Button m_startClientButton;
        [SerializeField] private Button m_startGameButton;

        [SerializeField] private TMP_InputField m_ipInputField;
        [SerializeField] private TMP_InputField m_portInputField;
        [SerializeField] private Button m_connectButton;

        [SerializeField] private Button m_findMatchButton;

        private CreateTicketResponse createTicketResponse;
        private float pollTicketTimer;
        private float pollTicketTimerMax = 1.1f;

        private void Awake()
        {
            m_startServerButton.onClick.AddListener(GlobalManagers.Instance.Connection.StartServer);
            m_startHostButton.onClick.AddListener(GlobalManagers.Instance.Connection.StartHost);
            m_startClientButton.onClick.AddListener(GlobalManagers.Instance.Connection.StartClient);
            m_startGameButton.onClick.AddListener(RequestLoadGamePlayScene);
            m_connectButton.onClick.AddListener(ConnectToDedicatedServer);
            m_findMatchButton.onClick.AddListener(FindMatch);
        }

        private void RequestLoadGamePlayScene()
        {
            GlobalManagers.Instance.Scene.RequestLoadScene(SceneType.GamePlayScene, true);
        }

        private void ConnectToDedicatedServer()
        {
            string ipv4Address = m_ipInputField.text;
            ushort port = ushort.Parse(m_portInputField.text);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port);

            GlobalManagers.Instance.Connection.StartClient();
        }

        private async void FindMatch()
        {
            DebugWrapper.Log("Find Match");

            GlobalManagers.Instance.Popup.RegisterLoadingPopupToQueue("Finding Match...");
            
            // create ticket
            createTicketResponse = await MatchmakerService.Instance.CreateTicketAsync(
                new List<Unity.Services.Matchmaker.Models.Player>
                {
                    new Unity.Services.Matchmaker.Models.Player(AuthenticationService.Instance.PlayerId,
                        new MatchmakingPlayerData
                        {
                            Skill = 100,
                        })
                }, new CreateTicketOptions { QueueName = Define.MatchMakingQueueName });
            
            // Wait a bit, don't poll right away
            pollTicketTimer = pollTicketTimerMax;
        }

        [Serializable]
        public class MatchmakingPlayerData
        {
            public int Skill;
        }

        private void Update()
        {
            if (createTicketResponse != null)
            {
                // Has ticket
                pollTicketTimer -= Time.deltaTime;
                if (pollTicketTimer <= 0f)
                {
                    pollTicketTimer = pollTicketTimerMax;

                    PollMatchmakerTicket();
                }
            }
        }

        private async void PollMatchmakerTicket()
        {
            DebugWrapper.Log("PollMatchmakerTicket");
            TicketStatusResponse ticketStatusResponse =
                await MatchmakerService.Instance.GetTicketAsync(createTicketResponse.Id);

            if (ticketStatusResponse == null)
            {
                // Null means no updates to this ticket, keep waiting
                DebugWrapper.Log("Null means no updates to this ticket, keep waiting");
                return;
            }

            // Not null means there is an update to the ticket
            if (ticketStatusResponse.Type == typeof(MultiplayAssignment))
            {
                // It's a Multiplay assignment
                MultiplayAssignment multiplayAssignment = ticketStatusResponse.Value as MultiplayAssignment;

                DebugWrapper.Log("multiplayAssignment.Status " + multiplayAssignment.Status);
                switch (multiplayAssignment.Status)
                {
                    case MultiplayAssignment.StatusOptions.Found:
                        createTicketResponse = null;
                        GlobalManagers.Instance.Popup.HideCurrentPopup();
                        
                        DebugWrapper.Log(multiplayAssignment.Ip + " " + multiplayAssignment.Port);

                        string ipv4Address = multiplayAssignment.Ip;
                        ushort port = (ushort)multiplayAssignment.Port;
                        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port);
                        GlobalManagers.Instance.Connection.StartClient();
                        break;
                    case MultiplayAssignment.StatusOptions.InProgress:
                        // Still waiting...
                        break;
                    case MultiplayAssignment.StatusOptions.Failed:
                        createTicketResponse = null;
                        DebugWrapper.Log("Failed to create Multiplay server!");
                        GlobalManagers.Instance.Popup.HideCurrentPopup();
                        break;
                    case MultiplayAssignment.StatusOptions.Timeout:
                        createTicketResponse = null;
                        DebugWrapper.Log("Multiplay Timeout!");
                        GlobalManagers.Instance.Popup.HideCurrentPopup();
                        break;
                }
            }
        }
    }
}
