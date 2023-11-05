using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Minimax.GamePlay.GridSystem;
using UnityEngine;

namespace Minimax.ScriptableObjects
{
    [Serializable]
    public class OverlayColorData
    {
        public OverlayType overlayType; // 오버레이 타입
        public Color       color;       // 색상
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/OverlayColorSO")]
    public class OverlayColorSO : ScriptableObject
    {
        [SerializeField] private List<OverlayColorData> m_overlayColorDatas = new();

        // 오버레이 타입에 따른 초기 색상을 가져옵니다.
        public Color GetInitialColor(OverlayType overlayType)
        {
            foreach (var data in m_overlayColorDatas)
                if (data.overlayType == overlayType)
                    return data.color;
            return Color.white; // 기본값 반환
        }
    }
}