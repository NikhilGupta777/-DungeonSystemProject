using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicOC : MonoBehaviour
{
    public Transform RayCenter;
    public LayerMask DetectionLayers;
    Collider[] AllCollidersOnCorridor;
    void OnTriggerEnter(Collider other)
    {
        //if (other.tag == "Player")
        //{
            //AllCollidersOnCorridor = Physics.OverlapCapsule(RayCenter.position - (RayCenter.forward * 1000) +
                //RayCenter.position + (RayCenter.forward * 1000), DetectionLayers, 5,);
        //}
    }
}
