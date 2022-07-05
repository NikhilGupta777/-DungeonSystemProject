using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCS_Check : MonoBehaviour
{
    public MCS_PrefabSpawner Main;
    public GameObject CollisionObject;
    MCS_ScriptableOBj Template;
    public int OtherIndex;
    Rigidbody rb;

    void Start()
    {
        Template = Main.Template;
        this.transform.parent = null;
        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        Invoke("DestroyIfNotHaveMain", 2);
    }

    void DestroyIfNotHaveMain()
    {
        if(!Main || !Main.Model)
        {
            Destroy(this.gameObject);
        }
    }

    void Check(MCS_Check other)
    {
        if (other)
        {
            CollisionObject = other.gameObject;
            if (other.Main.Index < Main.Index)
            {
                Main.Managment.Spawners.Remove(Main);
                Destroy(Main.gameObject);
                Main = null;
                Destroy(this.gameObject);
            }
        }
    }

    IEnumerator DelayedCheck(MCS_Check other)
    {
        yield return new WaitForSeconds(1);
        Check(other);
    }

    void OnTriggerEnter(Collider other)
    {
        if (Main != null)
        {
            if (other.GetComponent<MCS_Check>())
            {
                Check(other.GetComponent<MCS_Check>());
                //StartCoroutine(DelayedCheck(other.GetComponent<MCS_Check>()));
            }
        }
    }
}
