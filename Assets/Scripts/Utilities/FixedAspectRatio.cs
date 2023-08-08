using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax.Utilities
{
    [RequireComponent(typeof(Camera))]
    public class FixedAspectRatio : MonoBehaviour
    {
        private int m_sreenSizeX = 0;
        private int m_screenSizeY = 0;
 
        #region rescale camera
        private void RescaleCamera()
        {
 
            if (Screen.width == m_sreenSizeX && Screen.height == m_screenSizeY) return;
 
            float targetaspect = 9.0f / 16.0f;
            float windowaspect = (float)Screen.width / (float)Screen.height;
            float scaleheight = windowaspect / targetaspect;
            Camera camera = GetComponent<Camera>();
 
            if (scaleheight < 1.0f)
            {
                Rect rect = camera.rect;
 
                rect.width = 1.0f;
                rect.height = scaleheight;
                rect.x = 0;
                rect.y = (1.0f - scaleheight) / 2.0f;
 
                camera.rect = rect;
            }
            else // add pillarbox
            {
                float scalewidth = 1.0f / scaleheight;
 
                Rect rect = camera.rect;
 
                rect.width = scalewidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scalewidth) / 2.0f;
                rect.y = 0;
 
                camera.rect = rect;
            }
 
            m_sreenSizeX = Screen.width;
            m_screenSizeY = Screen.height;
        }
 
        void OnPreCull()
        {
            if (Application.isEditor) return;
            Rect wp = Camera.main.rect;
            Rect nr = new Rect(0, 0, 1, 1);
 
            Camera.main.rect = nr;
            GL.Clear(true, true, Color.black);
       
            Camera.main.rect = wp;
 
        }
 
        // Use this for initialization
        void Start () {
            RescaleCamera();
        }
   
        // Update is called once per frame
        void Update () {
            RescaleCamera();
        }
        #endregion
    }
}