using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Database;
using UnityEngine;
using Newtonsoft.Json;

namespace WMK
{
    public class DBManager : MonoBehaviour
    {
        private DatabaseReference m_databaseReference;
        [SerializeField] private string m_url;

        private void Awake()
        {
            // Set up the Editor before calling into the realtime database.
            FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(m_url);

            // Get the root reference location of the database.
            m_databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        }

        private async UniTask<DataSnapshot> ReadDataAsync(string path)
        {
            var dataSnapshot = await FirebaseDatabase.DefaultInstance
                .GetReference(path)
                .GetValueAsync();

            if (dataSnapshot.Exists)
            {
                DebugStatic.Log($"Data read successfully from {path}");
                return dataSnapshot;
            }
            else
            {
                DebugStatic.LogError($"No data exists at {path}");
                return null;
            }
        }
        
        public async UniTask<CardBaseData> ReadCardDataAsync(string path)
        {
            DataSnapshot dataSnapshot = await ReadDataAsync(path);
            if (dataSnapshot.Value != null)
                return JsonUtility.FromJson<CardBaseData>(dataSnapshot.GetRawJsonValue());
            else
                return null;
        }

        public async UniTask WriteCardDataAsync(string path, CardBaseData cardData)
        {
            await FirebaseDatabase.DefaultInstance
                .GetReference(path)
                .SetRawJsonValueAsync(JsonConvert.SerializeObject(cardData));

            DebugStatic.Log($"Card data written successfully to {path}");
        }
    }
}
