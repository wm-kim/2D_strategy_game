using DG.Tweening;
using Minimax.GamePlay.PlayerHand;
using Minimax.GamePlay.Unit;
using UnityEngine;

namespace Minimax.GamePlay.GridSystem
{
    public class MapCursorController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private MyHandInteractionManager m_handInteraction;

        [SerializeField] private ClientMap         m_clientMap;
        [SerializeField] private ClientUnitManager m_clientUnitManager;

        [Header("Visuals")] [SerializeField] private SpriteRenderer m_hoverOverlay;
        [SerializeField]                     private SpriteRenderer m_selectOverlay;

        [Header("Settings")] [SerializeField] [Range(0, 1)]
        private float m_hoverOverlayFadeDuration = 0.2f;

        [SerializeField] [Range(0, 1)] private float m_targetAlpha = 0.20f;

        private void Awake()
        {
            SetSelectOverlayAlpha(0);
            m_hoverOverlay.gameObject.SetActive(false);
            m_selectOverlay.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            m_clientMap.OnTouchOverMap        += OnTouchOverMap;
            m_clientMap.OnTouchOutsideOfMap   += OnTouchOutsideOfMap;
            m_clientMap.OnTouchEndOverMap     += OnTouchEndOverClientMap;
            m_clientMap.OnTapMap              += OnTapMap;
            m_clientUnitManager.OnUnitSpawned += OnUnitSpawned;
        }

        private void OnDisable()
        {
            m_clientMap.OnTouchOverMap        -= OnTouchOverMap;
            m_clientMap.OnTouchOutsideOfMap   -= OnTouchOutsideOfMap;
            m_clientMap.OnTouchEndOverMap     -= OnTouchEndOverClientMap;
            m_clientMap.OnTapMap              -= OnTapMap;
            m_clientUnitManager.OnUnitSpawned -= OnUnitSpawned;
        }

        private void OnTouchOverMap(ClientCell clientCell)
        {
            // Show hover overlay only when Player Select a card to play
            if (!m_handInteraction.IsSelecting) return;
            ShowHoverOverlay(clientCell.transform.position);
        }

        private void OnTouchOutsideOfMap()
        {
            HideHoverOverlay();
        }

        private void OnTouchEndOverClientMap(ClientCell clientCell)
        {
            HideHoverOverlay();
        }

        private void OnTapMap(ClientCell clientCell)
        {
            ShowSelectOverlay(clientCell.transform.position);
        }

        private void SetSelectOverlayAlpha(float alpha)
        {
            var temp = m_selectOverlay.color;
            temp.a                = alpha;
            m_selectOverlay.color = temp;
        }

        private void OnUnitSpawned(ClientCell clientCell)
        {
            var unitUID    = m_clientMap[clientCell.Coord].CurrentUnitUID;
            var clientUnit = ClientUnit.UnitsCreatedThisGame[unitUID];
            if (clientUnit.IsMine) ShowSelectOverlay(clientCell.transform.position);
        }

        private void HideHoverOverlay()
        {
            if (m_hoverOverlay.gameObject.activeSelf) m_hoverOverlay.gameObject.SetActive(false);
        }

        private void ShowHoverOverlay(Vector3 targetPosition)
        {
            if (!m_hoverOverlay.gameObject.activeSelf)
                m_hoverOverlay.gameObject.SetActive(true);

            m_hoverOverlay.transform.position = targetPosition;
        }

        private void ShowSelectOverlay(Vector3 targetPosition)
        {
            if (!m_selectOverlay.gameObject.activeSelf)
                m_selectOverlay.gameObject.SetActive(true);

            SetSelectOverlayAlpha(0);

            DOTween.ToAlpha(() => m_selectOverlay.color,
                x => m_selectOverlay.color = x, m_targetAlpha, m_hoverOverlayFadeDuration);

            m_selectOverlay.transform.position = targetPosition;
        }
    }
}