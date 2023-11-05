using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Services.CloudSave;
using Newtonsoft.Json;
using UnityEngine;

namespace Minimax.UnityGamingService.CloudSave
{
    public static class CloudService
    {
        // 지정된 키로 데이터를 비동기적으로 로드하고 역직렬화합니다.
        public static async UniTask<T> Load<T>(string key)
        {
            var query = await Call(CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { key }));
            return query.TryGetValue(key, out var value) ? Deserialize<T>(value) : default;
        }

        private static T Deserialize<T>(string input)
        {
            if (typeof(T) == typeof(string)) return (T)(object)input;
            return JsonConvert.DeserializeObject<T>(input);
        }

        private static async UniTask Call(Task action)
        {
            try
            {
                await action;
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        private static async UniTask<T> Call<T>(Task<T> action)
        {
            try
            {
                return await action;
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }

            return default;
        }
    }
}