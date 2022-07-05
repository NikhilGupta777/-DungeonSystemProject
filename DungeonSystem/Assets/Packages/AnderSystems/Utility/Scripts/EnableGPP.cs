using System.Collections;
using System.Collections.Generic;
using AnderSystems.GoodMotion.cam;
using AnderSystems.GoodMotion;
using AnderSystems.GoodMotion.Gpp;
using GameSettings;
using AnderSystems;
using UnityEngine;

public class EnableGPP : MonoBehaviour
{
    public float ExplosionForce;

    public GPP_Base PlayerTarget;
    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<GPP_Base>())
        {
            PlayerTarget = other.GetComponent<GPP_Base>();
            GoodPhysics.StartGoodPhysics(PlayerTarget, Vector3.zero);
            PlayerTarget.GoodPhysicsConfiguration.rb.AddExplosionForce(ExplosionForce, this.transform.position, ExplosionForce, ExplosionForce, ForceMode.Impulse);
        }
    }
}
