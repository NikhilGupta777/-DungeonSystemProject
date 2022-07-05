using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMeshes : MonoBehaviour
{
    public MeshFilter[] ObjectsToCombine;
    public Material[] Materials;
    List<MeshFilter> ObjectsToCombineA = new List<MeshFilter>();
    List<MeshFilter> ObjectsToCombineB = new List<MeshFilter>();
    List<MeshFilter> ObjectsToCombineC = new List<MeshFilter>();
    List<MeshFilter> ObjectsToCombineD = new List<MeshFilter>();
    List<MeshFilter> ObjectsToCombineE = new List<MeshFilter>();
    List<MeshFilter> ObjectsToCombineF = new List<MeshFilter>();
    public bool CombineAllChild;

    void GetAllChild()
    {
        if (CombineAllChild)
        {
            ObjectsToCombine = GetComponentsInChildren<MeshFilter>();
        }
    }

    void Combine(List<MeshFilter> objectsToCombine)
    {
        CombineInstance[] combine = new CombineInstance[objectsToCombine.Count];

        int x = 0;
        while (x < objectsToCombine.Count)
        {
            combine[x].mesh = objectsToCombine[x].sharedMesh;
            combine[x].transform = objectsToCombine[x].transform.localToWorldMatrix;
            objectsToCombine[x].gameObject.SetActive(false);

            x++;
        }

        GameObject newObject = new GameObject();
        newObject.transform.parent = this.transform;
        newObject.AddComponent<MeshRenderer>();
        newObject.AddComponent<MeshFilter>().mesh = new Mesh();
        newObject.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        newObject.gameObject.SetActive(true);
    }
    void SeparatePerMaterial()
    {
        for (int i = 0; i < ObjectsToCombine.Length; i++)
        {
            if (ObjectsToCombine[i].GetComponent<Renderer>().material == Materials[0])
            {
                ObjectsToCombineA.Add(ObjectsToCombine[i]);
            }
            if (ObjectsToCombine[i].GetComponent<Renderer>().material == Materials[1])
            {
                ObjectsToCombineB.Add(ObjectsToCombine[i]);
            }
            if (ObjectsToCombine[i].GetComponent<Renderer>().material == Materials[2])
            {
                ObjectsToCombineC.Add(ObjectsToCombine[i]);
            }
            if (ObjectsToCombine[i].GetComponent<Renderer>().material == Materials[3])
            {
                ObjectsToCombineD.Add(ObjectsToCombine[i]);
            }
            if (ObjectsToCombine[i].GetComponent<Renderer>().material == Materials[4])
            {
                ObjectsToCombineE.Add(ObjectsToCombine[i]);
            }
            if (ObjectsToCombine[i].GetComponent<Renderer>().material == Materials[5])
            {
                ObjectsToCombineF.Add(ObjectsToCombine[i]);
            }
        }
        Combine(ObjectsToCombineA);
        Combine(ObjectsToCombineB);
        Combine(ObjectsToCombineC);
        Combine(ObjectsToCombineD);
        Combine(ObjectsToCombineE);
        Combine(ObjectsToCombineF);
    }

    void Start()
    {
        Invoke("SeparatePerMaterial", 3f);
        Invoke("Combine", 4f);
    }
}
