using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WMK
{
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            UINavigation.Push<MainUIView>();
        }
    }
}
