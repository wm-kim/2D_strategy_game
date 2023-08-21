using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace Minimax
{
    public class WebRequestManager : MonoBehaviour
    {
        [SerializeField] private float m_timeout = 5f;
        
        /// <summary>
        /// GET 방식으로 데이터를 요청합니다.
        /// </summary>
        private async UniTaskVoid GetData(string url)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(m_timeout)); // 5초 타임아웃을 설정합니다.

            try
            {
                using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
                {
                    await webRequest.SendWebRequest().WithCancellation(cts.Token);
                    if (webRequest.result != UnityWebRequest.Result.Success)
                    {
                        DebugWrapper.LogError("GET Request Failed: " + webRequest.error);
                    }
                    else
                    {
                        string jsonResult = webRequest.downloadHandler.text;
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {
                    DebugWrapper.LogError("GET Request Timeout");
                }
            }
        }
    }
}
