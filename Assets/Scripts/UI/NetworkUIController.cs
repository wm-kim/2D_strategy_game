using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Minimax
{
    public class NetworkUIController : MonoBehaviour
    {
        [SerializeField] private Button m_serverBtn;
        [SerializeField] private Button m_hostBtn;
        [SerializeField] private Button m_clientBtn;

        private void Awake()
        {
            m_serverBtn.onClick.AddListener(() => NetworkManager.Singleton.StartServer());
            m_hostBtn.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
            m_clientBtn.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
        }
    }
}
