using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.SceneManagement;
using Minimax.Utilities;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
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
        
        private void Start()
        {
            m_startServerButton.onClick.AddListener(GlobalManagers.Instance.Connection.StartServer);
            m_startHostButton.onClick.AddListener(GlobalManagers.Instance.Connection.StartHost);
            m_startClientButton.onClick.AddListener(GlobalManagers.Instance.Connection.StartClient);
            m_startGameButton.onClick.AddListener(RequestLoadGamePlayScene);
            m_connectButton.onClick.AddListener(ConnectToDedicatedServer);
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
    }
}
