using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.PlayerHand;
using UnityEngine;

namespace Minimax
{
    public class MapCursorController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ClientMyHandManager m_playerHandManager;
        [SerializeField] private Map m_map;
        [SerializeField] private SpriteRenderer m_hoverOverlay;
        [SerializeField] private SpriteRenderer m_selectOverlay;
        
        [Header("Settings")]
        [SerializeField, Range(0, 1)]
        private float m_hoverOverlayFadeDuration = 0.2f;
        
        private void Awake()
        {
            SetSelectOverlayAlpha(0);
            m_hoverOverlay.gameObject.SetActive(false);
            m_selectOverlay.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            m_map.OnTouchOverMap += OnTouchOverMap;
            m_map.OnTouchOutsideOfMap += OnTouchOutsideOfMap;
            m_map.OnTouchEndOverMap += OnTouchEndOverMap;
            m_map.OnTap += OnTap;
        }
        
        private void OnDisable()
        {
            m_map.OnTouchOverMap -= OnTouchOverMap;
            m_map.OnTouchOutsideOfMap -= OnTouchOutsideOfMap;
            m_map.OnTouchEndOverMap -= OnTouchEndOverMap;
            m_map.OnTap -= OnTap;
        }

        private void OnTouchOverMap(Cell cell)
        {
            // Show hover overlay only when Player Select a card to play
            if(!m_playerHandManager.IsSelecting) return;
            
            if (!m_hoverOverlay.gameObject.activeSelf) 
                m_hoverOverlay.gameObject.SetActive(true);
            
            m_hoverOverlay.transform.position = cell.WorldPos;
        }
        
        private void OnTouchOutsideOfMap()
        {
            if (m_hoverOverlay.gameObject.activeSelf)
            {
                m_hoverOverlay.gameObject.SetActive(false);
            }
        }
        
        private void OnTouchEndOverMap(Cell cell)
        {
            if (m_hoverOverlay.gameObject.activeSelf)
            {
                m_hoverOverlay.gameObject.SetActive(false);
            }
        }

        private void OnTap(Cell cell)
        {
            if(!m_selectOverlay.gameObject.activeSelf) 
                m_selectOverlay.gameObject.SetActive(true);
            
            SetSelectOverlayAlpha(0);
            
            // Tween alpha
            DOTween.ToAlpha(() => m_selectOverlay.color, 
                x => m_selectOverlay.color = x, 0.25f, m_hoverOverlayFadeDuration);
            
            m_selectOverlay.transform.position = cell.WorldPos;
        }
        
        private void SetSelectOverlayAlpha(float alpha)
        {
            Color temp = m_selectOverlay.color;
            temp.a = alpha;
            m_selectOverlay.color = temp;
        }
    }
}
