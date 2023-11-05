using System;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace Minimax
{
    public enum SendType
    {
        GET,
        POST
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
        public static async UniTask<T> RequestAsync<T>(string url, SendType sendType,
            Dictionary<string, string> headers = null, string jsonBodyString = null)
        {
            DebugWrapper.Log($"Request: {url}, Type: {sendType}");

            await CheckNetwork();

            // time out 설정
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(Define.WebTimeOutSeconds));

            // 웹 요청 생성
            var request = new UnityWebRequest(url, sendType.ToString());

            // Body에 데이터를 담습니다.
            request.downloadHandler = new DownloadHandlerBuffer();
            if (!string.IsNullOrEmpty(jsonBodyString))
            {
                DebugWrapper.Log($"Request Body: {jsonBodyString}");
                var bodyRaw = Encoding.UTF8.GetBytes(jsonBodyString);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            // Header 정보를 추가합니다.
            if (headers != null)
                foreach (var header in headers)
                    request.SetRequestHeader(header.Key, header.Value);

            try
            {
                var res = await request.SendWebRequest().WithCancellation(cts.Token);
                if (request.result != UnityWebRequest.Result.Success)
                {
                    DebugWrapper.LogError($"Request Failed: {url}, Type: {sendType}, Error: {request.error}");
                    return default;
                }
                else
                {
                    var jsonResultString = request.downloadHandler.text;
                    DebugWrapper.Log("Request Success");

                    if (string.IsNullOrEmpty(jsonResultString))
                    {
                        DebugWrapper.Log($"Response is Empty: {url}, Type: {sendType}");
                        return default;
                    }

                    DebugWrapper.Log($"Response: {jsonResultString}");
                    // JsonUtility를 사용하여 JSON 문자열을 제네릭 형식으로 변환합니다.
                    return JsonConvert.DeserializeObject<T>(jsonResultString);
                }
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                    DebugWrapper.LogError($"Request Time Out: {url}, Type: {sendType}");
            }

            return default;
        }

        /// <summary>
        /// 기대하는 반환값이 없는 경우에 사용합니다.
        /// </summary>
        public static UniTask RequestAsync(string url, SendType sendType, Dictionary<string, string> headers = null,
            string jsonBodyString = null)
        {
            return RequestAsync<object>(url, sendType, headers, jsonBodyString);
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

#if DEDICATED_SERVER
        public static async UniTask<T> ServerRunCloudCodeModuleEndpointAsync<T>(string moduleName, string functionName, Dictionary<string, object> args
 = null)
        {
            var baseUrl =
 "https://cloud-code.services.api.unity.com/v1/projects/816ef5ed-e7dd-431e-8f1c-28bf1e818173/modules";
            var url = $"{baseUrl}/{moduleName}/{functionName}";
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {GlobalManagers.Instance.Connection.DedicatedServer.ServerBearerToken}"},
                {"Content-Type", "application/json"},
            };
            
            Dictionary<string, object> jsonBody = new Dictionary<string, object>
            {
                {"params", args},
            };
            
            string jsonBodyString = JsonConvert.SerializeObject(jsonBody);
            OutputClass<T> outputClass =
 await RequestAsync<OutputClass<T>>(url, SendType.POST, headers, jsonBodyString);
            return outputClass.output;
        }
        
        public static async UniTask ServerRunCloudCodeModuleEndpointAsync(string moduleName, string functionName, Dictionary<string, object> args
 = null)
        {
            await ServerRunCloudCodeModuleEndpointAsync<object>(moduleName, functionName, args);
        }

        /// <summary>
        /// UGS web doc에 따르면 출력 값이 output 필드에 담겨서 나온다고 합니다.
        /// https://services.docs.unity.com/cloud-code/v1/index.html#tag/Cloud-Code/operation/runScript
        /// </summary>
        private class OutputClass<T>
        {
            public T output;
        }
#endif
    }
}