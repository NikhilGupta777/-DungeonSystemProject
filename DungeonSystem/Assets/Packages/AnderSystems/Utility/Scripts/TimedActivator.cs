using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedActivator : MonoBehaviour
{
    [System.Serializable]
    public enum mode
    {
        Active, Deactive, Destroy
    }
    [SerializeField]
    public mode Mode;
    public GameObject[] ObjTargets;
    public MonoBehaviour[] ScriptTargets;
    public float TimeTo;
    public float RandomRange;

    void Start()
    {
        Invoke("Execute", TimeTo + Random.Range(-RandomRange, RandomRange));
    }

    void Execute()
    {
        if (Mode == mode.Deactive)
        {
            for (int i = 0; i < ObjTargets.Length; i++)
            {
                ObjTargets[i].SetActive(false);
            }

            for (int i = 0; i < ScriptTargets.Length; i++)
            {
                ScriptTargets[i].enabled = false;
            }
        }

        if (Mode == mode.Active)
        {
            for (int i = 0; i < ObjTargets.Length; i++)
            {
                ObjTargets[i].SetActive(true);
            }

            for (int i = 0; i < ScriptTargets.Length; i++)
            {
                ScriptTargets[i].enabled = true;
            }
        }

        if (Mode == mode.Destroy)
        {
            for (int i = 0; i < ObjTargets.Length; i++)
            {
                Destroy(ObjTargets[i]);
            }

            for (int i = 0; i < ScriptTargets.Length; i++)
            {
                Destroy(ScriptTargets[i]);
            }
        }
    }
}
