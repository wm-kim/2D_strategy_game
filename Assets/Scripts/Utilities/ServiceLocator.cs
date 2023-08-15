using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minimax.Utilities
{
    public class ServiceLocator
    {
        private IDictionary<Type, MonoBehaviour> m_services = new Dictionary<Type, MonoBehaviour>();
        private IDictionary<string, MonoBehaviour> m_namedServices = new Dictionary<string, MonoBehaviour>();

        public void RegisterService<T>(T service, string serviceName = null) where T : MonoBehaviour
        {
            var type = typeof(T);
            if (!m_services.ContainsKey(type))
            {
                m_services[type] = service;
            }
            else
            {
                Debug.LogWarning($"Service of type {type} is already registered.");
            }
            
            if (!string.IsNullOrEmpty(serviceName))
            {
                if (!m_namedServices.ContainsKey(serviceName))
                {
                    m_namedServices[serviceName] = service;
                }
                else
                {
                    Debug.LogWarning($"Service of name {serviceName} is already registered.");
                }
            }
        }
        
        public T GetService<T>() where T : MonoBehaviour
        {
            var type = typeof(T);
            if (m_services.TryGetValue(type, out var service))
            {
                return (T)service;
            }
            else
            {
                Debug.LogError($"No service of type {type} is registered.");
                return null;
            }
        }
        
        public MonoBehaviour GetService(string serviceName)
        {
            if (m_namedServices.TryGetValue(serviceName, out var service))
            {
                return service;
            }
            else
            {
                Debug.LogError($"No service with name {serviceName} is registered.");
                return null;
            }
        }
        
        public void UnregisterService<T>() where T : MonoBehaviour
        {
            var type = typeof(T);
            if (m_services.ContainsKey(type))
            {
                var service = m_services[type];
                m_services.Remove(type);
            }
            else
            {
                Debug.LogWarning($"No service of type {type} is registered.");
            }
        }
        
        public void UnregisterService(string serviceName)
        {
            if (m_namedServices.ContainsKey(serviceName))
            {
                var service = m_namedServices[serviceName];
                m_namedServices.Remove(serviceName);
            }
            else
            {
                Debug.LogWarning($"No service with name {serviceName} is registered.");
            }
        }
    }
}
