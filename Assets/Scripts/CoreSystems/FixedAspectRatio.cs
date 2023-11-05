using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax.Utilities
{
    [RequireComponent(typeof(Camera))]
    public class FixedAspectRatio : MonoBehaviour
    {
        private int m_sreenSizeX  = 0;
        private int m_screenSizeY = 0;

        #region rescale camera

        private void RescaleCamera()
        {
            if (Screen.width == m_sreenSizeX && Screen.height == m_screenSizeY) return;

            var targetaspect = 9.0f / 16.0f;
            var windowaspect = (float)Screen.width / (float)Screen.height;
            var scaleheight  = windowaspect / targetaspect;
            var camera       = GetComponent<Camera>();

            if (scaleheight < 1.0f)
            {
                var rect = camera.rect;

                rect.width  = 1.0f;
                rect.height = scaleheight;
                rect.x      = 0;
                rect.y      = (1.0f - scaleheight) / 2.0f;

                camera.rect = rect;
            }
            else // add pillarbox
            {
                var scalewidth = 1.0f / scaleheight;

                var rect = camera.rect;

                rect.width  = scalewidth;
                rect.height = 1.0f;
                rect.x      = (1.0f - scalewidth) / 2.0f;
                rect.y      = 0;

                camera.rect = rect;
            }

            m_sreenSizeX  = Screen.width;
            m_screenSizeY = Screen.height;
        }

        private void OnPreCull()
        {
            if (Application.isEditor) return;
            var wp = Camera.main.rect;
            var nr = new Rect(0, 0, 1, 1);

            Camera.main.rect = nr;
            GL.Clear(true, true, Color.black);

            Camera.main.rect = wp;
        }

        // Use this for initialization
        private void Start()
        {
            RescaleCamera();
        }

        // Update is called once per frame
        private void Update()
        {
            RescaleCamera();
        }

        #endregion
    }
}