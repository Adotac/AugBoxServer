using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiP2P
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);            
        }
    }
}
