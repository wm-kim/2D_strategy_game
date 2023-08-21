using System;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace Minimax
{
    public enum SendType
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    
    public class WebRequestManager : MonoBehaviour
    {
        

        /// <summary>
        /// GET 방식으로 데이터를 요청하고 제네릭 형식으로 반환합니다.
        /// </summary>
        /// <typeparam name="T">반환할 데이터의 형식입니다.</typeparam>
        /// <param name="url">데이터를 요청할 URL입니다.</param>
        /// <param name="sendType">데이터를 요청할 방식입니다.</param>
        /// <param name="headers">데이터를 요청할 때 함께 보낼 헤더 정보입니다.</param>
        /// <param name="jsonBody">데이터를 요청할 때 함께 보낼 JSON 형식의 문자열입니다.</param>
        /// <returns>요청한 형식의 데이터를 반환합니다.</returns>
        public static async UniTask<T> Request<T>(string url, SendType sendType,Dictionary<string, string> headers = null, string jsonBody = null)
        {
            await CheckNetwork();
    
            // time out 설정
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(Define.TimeOutSeconds));

            // 웹 요청 생성
            UnityWebRequest request = new UnityWebRequest(url, sendType.ToString());
            
            // Body에 데이터를 담습니다.
            request.downloadHandler = new DownloadHandlerBuffer();
            if (!string.IsNullOrEmpty(jsonBody))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
            
            // Header 정보를 추가합니다.
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }
            
            try
            {
                var res = await request.SendWebRequest().WithCancellation(cts.Token);
                if (request.result != UnityWebRequest.Result.Success)
                {
                    DebugWrapper.LogError($"Request Failed: {url}, Type: {sendType}, Error: {request.error}");
                    return default(T);
                }
                else
                {
                    string jsonResultString = request.downloadHandler.text;
                    DebugWrapper.Log("Request Success: " + jsonResultString);
                    // JsonUtility를 사용하여 JSON 문자열을 제네릭 형식으로 변환합니다.
                    return JsonConvert.DeserializeObject<T>(jsonResultString);
                }
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {
                    DebugWrapper.LogError($"Request Time Out: {url}, Type: {sendType}");
                }
            }

            return default(T);
        }

        private static async UniTask CheckNetwork()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                DebugWrapper.LogError("The network is not reachable.");
                await UniTask.WaitUntil(() => Application.internetReachability != NetworkReachability.NotReachable);
                DebugWrapper.Log("The network is connected.");
            }
        }
        
    }
}
