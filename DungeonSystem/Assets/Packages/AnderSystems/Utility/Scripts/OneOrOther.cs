using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneOrOther : MonoBehaviour
{
    public bool Randomize;
    public int Index;
    public GameObject[] Objects;

    void OnEnable()
    {
        if (Randomize)
        {
            EnableOneOrOther(Random.Range(0, Objects.Length - 1));
        }
    }
    public void EnableOneOrOther(int Index)
    {
        for (int i = 0; i < Objects.Length; i++)
        {
            Objects[i].SetActive(false);
        }
        Objects[Index].SetActive(true);
    }
}
