using Minimax.UnityGamingService.Multiplayer;
using UnityEngine;

namespace Minimax.CoreSystems
{
    public class ApplicationController : MonoBehaviour
    {
        [Header("Frame Rate Settings")]
        public int targetFrameRate = 60;

        private void Start()
        {
            // Set the target frame rate for the application
            Application.targetFrameRate = targetFrameRate;
            // Optional: If using vSync, make sure it's disabled to make this effective
            QualitySettings.vSyncCount = 0;
        }
    }
}