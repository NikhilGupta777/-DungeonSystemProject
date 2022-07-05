using System.Collections;
using System.Collections.Generic;
using GameSettings;
using AnderSystems;
using AnderSystems.GoodMotion;
using AnderSystems.GoodMotion.cam;
using UnityEngine;

public class GM_Jump : MonoBehaviour
{
    public GM_Movement GM;
    public Animator anim;
    public Rigidbody rb;
    [Space]
    [Header("Params")]
    public LayerMask ClimbLayers;
    public float CharacterHeight;
    public float MaxClimbHeight;
    public float MaxClimbDistance;
    [Header("Animations")]
    public string Climb = "Climb";
    public string ClimbHeight = "ClimbHeight";
    public string CanClimb = "CanClimb";
    /// <summary>
    /// Check if player can climb
    /// </summary>
    /// <returns></returns>
    public bool GetCanClimb()
    {
        return (!Physics.Linecast(GetClimbHeightRay().point, GetClimbHeightRay().point + (Vector3.up * CharacterHeight)) &&
            !Physics.Linecast(anim.GetBoneTransform(HumanBodyBones.Head).position,
            GetClimbHeightRay().point + (Vector3.up * CharacterHeight)));
    }
    /// <summary>
    /// Get height of surface
    /// </summary>
    /// <returns></returns>
    public float GetClimbHeight()
    {
        RaycastHit hit;
        Vector3 fwd = transform.position + (transform.forward * MaxClimbDistance);
        Physics.Linecast(fwd + (Vector3.up * MaxClimbHeight), fwd, out hit, ClimbLayers);
        return hit.point.y - transform.position.y;
    }
    /// <summary>
    /// Get RaycastHit of Climb Height
    /// </summary>
    /// <returns></returns>
    public RaycastHit GetClimbHeightRay()
    {
        RaycastHit hit;
        Vector3 fwd = transform.position + (transform.forward * MaxClimbDistance);
        Physics.Linecast(fwd + (Vector3.up * MaxClimbHeight), fwd, out hit, ClimbLayers);
        return hit;
    }

    bool OnClimb;
    /// <summary>
    /// Stop climb
    /// </summary>
    public void DisableOnClimb()
    {
        OnClimb = false;
        anim.SetBool(Climb, OnClimb);
    }

    Vector3 ClimbLocation;
    /// <summary>
    /// Up Climb Player
    /// </summary>
    public void ClimbUp()
    {
        ClimbLocation = GetClimbHeightRay().point;

        Movement.EntityLookAt(GM.transform, ClimbLocation, 100);
        InvokeRepeating("ClimbUpTeleport", 0, Time.fixedDeltaTime);
    }

    /// <summary>
    /// Climb up (Call tihs function on Animations)
    /// </summary>
    void ClimbUpTeleport()
    {
        PlayerPhysics.FreezePlayer(GM);
        Movement.MoveEntityTo(this.transform, ClimbLocation, 5);
        if(OnClimb == false)
        {
            PlayerPhysics.UnFreezePlayer(GM);
            CancelInvoke();
        }
    }

    //Call voids
    void LateUpdate()
    {
        anim.SetFloat(ClimbHeight, GetClimbHeight());
        anim.SetBool(CanClimb, GetCanClimb());
    }
    void Update()
    {
        if (Inputs.GetButtonDown(GameConfigurations.MainSettings.Controls.Jump) && GM.IsFreezed == false && GM.Controls == true)
        {
            OnClimb = true;
            anim.SetBool(Climb, OnClimb);
            Invoke("DisableOnClimb", 1);
        }
    }
}
