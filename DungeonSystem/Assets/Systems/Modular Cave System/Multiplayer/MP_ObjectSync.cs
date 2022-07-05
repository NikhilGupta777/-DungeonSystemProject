using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_ObjectSync : MonoBehaviour
{
    [System.Serializable]
    public class MP_Object
    {
        public int ObjectID;
        [TextArea]
        public string ObjectJSON;
    }
    [SerializeField]
    public MP_Object Object;
}
