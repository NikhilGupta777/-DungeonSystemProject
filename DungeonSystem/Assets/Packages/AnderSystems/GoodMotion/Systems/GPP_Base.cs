using System.Collections;
using System.Collections.Generic;
using AnderSystems.GoodMotion.cam;
using AnderSystems.GoodMotion;
using AnderSystems.GoodMotion.Gpp;
using GameSettings;
using AnderSystems;
using UnityEngine;

public class GPP_Base : MonoBehaviour
{
    //Key to Enable GPP
    public KeyCode KeyToEnableGP;
    public bool GenerateBones { get; set; } //Toggle to Automatic Generate Bones
    public GM_Movement Base; //The Main Base
    [SerializeField]
    public GoodPhysics.GoodPhysicsConfiguration GoodPhysicsConfiguration; //Configurations
    public SoundParticles[] Particles; //Contact Particles
    public AudioSource DraggingSound; //Dragging Sound Prefab
    AudioSource InstantiatedDragging { get; set; } //Dragging Sound Game Object

    #region Bones Generation
    /// <summary>
    /// Destroy Bones Collider on Player
    /// </summary>
    public void DestroyBones()
    {
        for (int i = 0; i < GoodPhysicsConfiguration.Colliders.Count; i++)
        {
            Destroy(GoodPhysicsConfiguration.Colliders[i]);
        }
    }
    /// <summary>
    /// Automatic create all bones Colliders
    /// </summary>
    public void CreateBones()
    {
        GoodPhysics.GenerateBonesColliders(GoodPhysicsConfiguration);
    }
    #endregion

    public Vector3 WakeUpPosition { get; set; } //Locantion to Wake Up
    float OnMaxSlopeTime;

    /// <summary>
    /// Direct Wake Up Player
    /// </summary>
    public void WakeUp()
    {
        //This funciton exist only to delayed Wake Up
        GoodPhysics.StopGoodPhysics(this);
    }

    /// <summary>
    /// Direct UnFreeze Player from GPP only
    /// </summary>
    public void UnFreeze()
    {
        //Fix Player Position
        transform.position = WakeUpPosition;
        GoodPhysicsConfiguration.rb.isKinematic = false;
        PlayerPhysics.UnFreezePlayer(Base);
    }
    public void StopParticles()
    {
        GoodPhysicsConfiguration.GP_Contact = false;
        if (InstantiatedDragging)
        {
            InstantiatedDragging.volume = 0; //Set volume to 0
        }
        InstantiatedDragging = null; //Remove Dragging Particle;
        CancelInvoke("WakeUp");
    }

    //Call Voids
    void Update()
    {
        //To enable using Key
        if (Input.GetKeyDown(KeyToEnableGP))
        {
            if (GoodPhysicsConfiguration.GP_Enabled)
            {
                GoodPhysics.StopGoodPhysics(this);
            } else
            {
                GoodPhysics.StartGoodPhysics(this, Vector3.zero);
            }
        }
    }
    void FixedUpdate()
    {
        // Verify if GPP is enabled
        if (GoodPhysicsConfiguration.GP_Enabled)
        {
            // Lock Y Local Rotation
            Vector3 RotateValue = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);
            GoodPhysicsConfiguration.GP_Rotation = Mathf.Lerp(GoodPhysicsConfiguration.anim.GetFloat(GoodPhysicsConfiguration.Animations.RotationSpeedAnimation)
                , Mathf.DeltaAngle(RotateValue.magnitude, 0), 5f * Time.deltaTime); //Keeps Y local Rotation Locked but dont broken physics

            //Execute animations
            GoodPhysics.ExecuteGPPAnimations(this);

            //Run In Not Collision Contact
            if (!GoodPhysicsConfiguration.GP_Contact)
            {
                GoodPhysicsConfiguration.rb.angularVelocity = Vector3.Lerp(GoodPhysicsConfiguration.rb.angularVelocity,
                    Vector3.zero, 0.8f * Time.deltaTime); //Clear angular velocity

                //Correct Rotation on Air (For Better Result)
                Quaternion CorrectedRotation = Quaternion.Euler(0, transform.eulerAngles.y, 180f);
                transform.rotation = Quaternion.Lerp(transform.rotation, CorrectedRotation, 0.3f * Time.deltaTime); //Apply Rotation With Lerping
            } else
            {
                GoodPhysicsConfiguration.rb.angularVelocity *= ((GoodPhysicsConfiguration.rb.velocity.magnitude / 30) + 0.8f);
            }

            //Correct Angular Velocity
            Vector3 CorrectedAngular = new Vector3(0, GoodPhysicsConfiguration.rb.angularVelocity.y, GoodPhysicsConfiguration.rb.angularVelocity.z);
            GoodPhysicsConfiguration.rb.angularVelocity = CorrectedAngular;
        } else
        {
            //Lerping Rotation On Wake Up
            Quaternion WantedRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, WantedRotation, 1.2f * Time.deltaTime);
        }
    }
    void LateUpdate()
    {
        //Automatic WakeUp Player
        if(GoodPhysicsConfiguration.GP_Contact && (Mathf.Abs(GoodPhysicsConfiguration.rb.velocity.x) +
            Mathf.Abs(GoodPhysicsConfiguration.rb.velocity.y) +
            Mathf.Abs(GoodPhysicsConfiguration.rb.velocity.z)) <= 20 && GoodPhysicsConfiguration.GP_Enabled &&
            GoodPhysicsConfiguration.AutoWakeUp)
        {
            CancelInvoke("WakeUp");
            Invoke("WakeUp", (GoodPhysicsConfiguration.rb.velocity.magnitude/10)); //Wake Up after 1 second
        }

        //"Slip" to fall
        if (GoodPhysicsConfiguration.MaxSlope != 0)
        {
            if (Mathf.Abs(Base.GroundAngle) > GoodPhysicsConfiguration.MaxSlope && Base.playerMovement.OnWalk &&
                PlayerPhysics.DetectGround(this.transform, Base.playerPhysics) && !GoodPhysicsConfiguration.GP_Contact &&
                Utility.GetGroundRotation(PlayerPhysics.DetectGroundRay(this.transform, Base.playerPhysics)).eulerAngles.magnitude >
                GoodPhysicsConfiguration.MaxSlope && !GoodPhysicsConfiguration.GP_Enabled)
            {
                OnMaxSlopeTime += Time.deltaTime; //Time to Slope (To prevent accidents)
            }
            else
            {
                OnMaxSlopeTime = 0;
            }

            if (OnMaxSlopeTime >= 3)
            {
                GoodPhysics.StartGoodPhysics(this, Vector3.zero); //"Slip out"
            }
        }
    }

    //Collision Voids
    void OnCollisionEnter(Collision collision)
    {
        if (!GoodPhysicsConfiguration.GP_Enabled) // Check if GPP is Enabled
            return;
        for (int i = 0; i < Particles.Length; i++)
        {
            //Spawn Contact Particles
            SoundParticles.SpawnParticle(Particles[i], collision.contacts[0].point, collision.contacts[0].normal, collision.impulse.magnitude, 0);
        }

        //Check if have InstantiatedDragging and sets volume to 0
        if (InstantiatedDragging)
        {
            InstantiatedDragging.volume = 0;
        }
        //Spawn Dragging Particle,
        if (GoodPhysicsConfiguration.rb.velocity.magnitude > 2)
        {
            if (!InstantiatedDragging)
            {
                InstantiatedDragging = Instantiate(DraggingSound.gameObject, collision.contacts[0].point,
                Quaternion.identity).GetComponent<AudioSource>(); //Instantiate Dragging
                //Reset Pith and Volume
                InstantiatedDragging.pitch = 0;
                InstantiatedDragging.volume = 0;
            }
        }
    }
    void OnCollisionStay(Collision collision)
    {
        //Set Contact Value
        CancelInvoke("StopParticles");
        GoodPhysicsConfiguration.GP_Contact = true;

        if (!GoodPhysicsConfiguration.GP_Enabled) //Check if Enabled
            return;
        for (int i = 0; i < Particles.Length; i++)
        {
            //Spawn Particle
            SoundParticles.SpawnParticle(Particles[i], collision.contacts[0].point, collision.contacts[0].normal, collision.impulse.magnitude, 0);
        }

        if (InstantiatedDragging)
        {
            InstantiatedDragging.transform.position = collision.contacts[0].point; //Move Dragging Particle to contact point

            //Set Pith and Volume from Dragging Particle
            InstantiatedDragging.pitch = Mathf.Lerp(InstantiatedDragging.pitch,
                Mathf.Clamp((GoodPhysicsConfiguration.rb.velocity.magnitude / 30) + 0.2f, 0.5f, 2f), 5 * Time.deltaTime);
            InstantiatedDragging.volume = Mathf.Lerp(InstantiatedDragging.volume, 0.8f, 5 * Time.deltaTime);

            if (GoodPhysicsConfiguration.rb.velocity.magnitude <= 3)
            {
                InstantiatedDragging.volume = 0;
                InstantiatedDragging = null;
            }
        } else
        {
            if (GoodPhysicsConfiguration.rb.velocity.magnitude >= 3)
            {
                InstantiatedDragging = Instantiate(DraggingSound.gameObject, collision.contacts[0].point,
                Quaternion.identity).GetComponent<AudioSource>(); //Instantiate Dragging
            }
        }

        Invoke("StopParticles", 1);
    }
    void OnTriggerExit(Collider collision)
    {
        //Set Contact Value
        GoodPhysicsConfiguration.GP_Contact = false;
        StopParticles();
    }
}
