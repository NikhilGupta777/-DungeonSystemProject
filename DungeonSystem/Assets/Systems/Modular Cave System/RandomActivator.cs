using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomActivator : MonoBehaviour
{
    float RandomProb;

    [System.Serializable]
    public class _Objects
    {
        public GameObject Object;
        [Range(0,100)]
        public float Probability = 100;
    }
    [SerializeField]
    public List<_Objects> objects = new List<_Objects>();
    List<_Objects> objectsListed = new List<_Objects>();

    void GenerateNewList(float probability)
    {
        objectsListed = new List<_Objects>(objects);
        for (int i = 0; i < objectsListed.Count; i++)
        {
            if(objectsListed[i].Probability <= probability)
            {
                objectsListed.RemoveAt(i);
            }
        }
    }

    void Activate(List<_Objects> _objects, int ItemToActivate)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].Object.SetActive(false);
        }

        _objects[ItemToActivate].Object.SetActive(true);
    }

    void Start()
    {
        RandomProb = Random.Range(0, 100);
        GenerateNewList(RandomProb);
        Activate(objectsListed, Random.Range(0, objectsListed.Count));
    }
}
