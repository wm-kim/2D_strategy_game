using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace WMK
{
    public class ServiceLocator : MonoBehaviour
    {
        private static readonly IDictionary<Type, MonoBehaviour> m_serviceReferences = new Dictionary<Type, MonoBehaviour>();

        /// <summary>
        /// Get a service of type T. If the service is not found in the scene, it will throw an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="forced">If true, it will force to instantiate a new service if it is not found in the scene.</param>
        /// <returns>MonoBehaviour of type T</returns>
        ///  warning : be careful when using this method in multi-threaded environment
        public static T GetService<T>(bool forced = false) where T : MonoBehaviour, IService
        {
            var type = typeof(T);
            if (IsRegistered<T>())
            {
                return m_serviceReferences[type] as T;
            }
            else
            {
                T service = FindObjectOfType<T>();
                if (service == null)
                {
                    if (forced)
                    {
                        service = new GameObject(type.Name).AddComponent<T>();
                        service.name = type.Name;
                        Register(service);
                    }
                    else
                    {
                        throw new ServiceLocatorException($"Service {type.Name} not found in the scene.");
                    }
                }
                return service;
            }
        }
        
        public static bool IsRegistered(Type type) => m_serviceReferences.ContainsKey(type);
        public static bool IsRegistered<T>() => IsRegistered(typeof(T));
        
        public static void Register<T>(T service) where T : MonoBehaviour, IService
        {
            var type = typeof(T);
            if (IsRegistered<T>()) Debug.LogWarning($"Service {type.Name} is already registered.");
            else m_serviceReferences.Add(type, service);
        }
        
        public static void Unregister<T>() where T : MonoBehaviour, IService
        {
            var type = typeof(T);
            if (IsRegistered<T>()) m_serviceReferences.Remove(type);
            else Debug.LogWarning($"Service {type.Name} is not registered.");
        }
    }
    
    public interface IService { }
    
    public class ServiceLocatorException : Exception
    {
        public ServiceLocatorException(string message) : base(message) { }
    }
}