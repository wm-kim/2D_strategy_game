using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minimax.Utilities
{
    public class ServiceLocator
    {
        private IDictionary<Type, MonoBehaviour> m_services = new Dictionary<Type, MonoBehaviour>();
        private IDictionary<string, MonoBehaviour> m_namedServices = new Dictionary<string, MonoBehaviour>();

        public void RegisterService<T>(T service, string serviceName) where T : MonoBehaviour
        {
            var type = typeof(T);
            if (!m_services.ContainsKey(type))
            {
                m_services[type] = service;
            }
            else
            {
                DebugWrapper.Instance.LogWarning($"Service of type {type} is already registered.");
            }
            
            if (!string.IsNullOrEmpty(serviceName))
            {
                if (!m_namedServices.ContainsKey(serviceName))
                {
                    m_namedServices[serviceName] = service;
                }
                else
                {
                    DebugWrapper.Instance.LogWarning($"Service of name {serviceName} is already registered.");
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
                DebugWrapper.Instance.LogError($"No service of type {type} is registered.");
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
                DebugWrapper.Instance.LogError($"No service with name {serviceName} is registered.");
                return null;
            }
        }
        
        public void UnregisterService(string serviceName)
        {
            if (m_namedServices.TryGetValue(serviceName, out var service))
            {
                var type = service.GetType();
                m_services.Remove(type);
                m_namedServices.Remove(serviceName);
            }
            else
            {
                DebugWrapper.Instance.LogWarning($"No service with name {serviceName} is registered.");
            }
        }
    }
}
