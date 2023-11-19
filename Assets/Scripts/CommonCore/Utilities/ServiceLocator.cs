using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// For locating and referencing services.
    /// This is useful when there is a class that is not a MonoBehaviour but needs to reference a MonoBehaviour.
    /// This could be done by using dependency injection but I don't want to overcomplicate things.
    /// Also could be done by using a singleton pattern but It is not a good practice to make unnecessary singletons.
    /// </summary>
    public class ServiceLocator
    {
        private IDictionary<Type, MonoBehaviour>   m_services      = new Dictionary<Type, MonoBehaviour>();
        private IDictionary<string, MonoBehaviour> m_namedServices = new Dictionary<string, MonoBehaviour>();

        public void RegisterService<T>(T service, string serviceName) where T : MonoBehaviour
        {
            var type = typeof(T);
            if (!m_services.ContainsKey(type))
                m_services[type] = service;
            else
                Debug.LogWarning($"Service of type {type} is already registered.");

            if (!string.IsNullOrEmpty(serviceName))
            {
                if (!m_namedServices.ContainsKey(serviceName))
                    m_namedServices[serviceName] = service;
                else
                    Debug.LogWarning($"Service of name {serviceName} is already registered.");
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
                Debug.LogWarning($"No service with name {serviceName} is registered.");
            }
        }
    }
}