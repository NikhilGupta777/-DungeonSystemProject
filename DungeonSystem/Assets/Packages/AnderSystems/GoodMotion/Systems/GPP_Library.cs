using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnderSystems.GoodMotion.Gpp
{
    public static class GoodPhysics
    {
        [System.Serializable]
        public class GoodPhysicsConfiguration
        {
            public Rigidbody rb;
            public Animator anim;
            public PhysicMaterial BodyMaterial;
            [Space]
            public bool AutoWakeUp;
            [Header("Keeps 0 if you dont wanna enable this")]
            public float MaxSlope;
            public List<CapsuleCollider> Colliders;
            [System.Serializable]
            public class _Animations
            {
                public string EnableGoodPhysicsAnimation = "GP_Enabled";
                public string ContactAnimation = "InCollision";
                public string RotationSpeedAnimation = "GP_Rotation";
            }
            [SerializeField]
            public _Animations Animations;
            public bool GP_Enabled { get; set; }
            public float GP_Rotation { get; set; }
            public bool GP_Contact { get; set; }
        }


        /// <summary>
        /// Enable Bones Colliders
        /// </summary>
        /// <param name="Target">Player Target</param>
        /// <param name="Enable">Enable</param>
        public static void EnableBones(GPP_Base Target, bool Enable)
        {
            if (!Enable)
            {
                Target.GoodPhysicsConfiguration.anim.GetBoneTransform(HumanBodyBones.Spine).GetComponent<CharacterJoint>().connectedBody = null;
            }
            else
            {
                Target.GoodPhysicsConfiguration.anim.GetBoneTransform(HumanBodyBones.Spine).GetComponent<CharacterJoint>().connectedBody =
                    Target.GoodPhysicsConfiguration.rb;
            }
            Target.GoodPhysicsConfiguration.anim.enabled = !Enable;
            Target.GetComponent<Collider>().enabled = !Enable;
            for (int i = 0; i < Target.GoodPhysicsConfiguration.Colliders.Count; i++)
            {
                Target.GoodPhysicsConfiguration.Colliders[i].isTrigger = !Enable;

            }
        }

        /// <summary>
        /// Automatic Generate Bones Colliders (You can edit manuality on Configurations)
        /// </summary>
        /// <param name="Target">Player Target</param>
        public static void GenerateBonesColliders(GoodPhysicsConfiguration Target)
        {
            //Chest
            Target.Colliders.Add(AddBoneCollider(Target.anim.GetBoneTransform(HumanBodyBones.Spine), Target.anim.GetBoneTransform(HumanBodyBones.Chest), Target.rb.transform));
            Target.Colliders.Add(AddBoneCollider(Target.anim.GetBoneTransform(HumanBodyBones.Chest), Target.anim.GetBoneTransform(HumanBodyBones.Neck), Target.anim.GetBoneTransform(HumanBodyBones.Spine)));

            //Arms
            Target.Colliders.Add(AddBoneCollider(Target.anim.GetBoneTransform(HumanBodyBones.LeftUpperArm), Target.anim.GetBoneTransform(HumanBodyBones.LeftLowerArm), Target.anim.GetBoneTransform(HumanBodyBones.Spine)));
            Target.Colliders.Add(AddBoneCollider(Target.anim.GetBoneTransform(HumanBodyBones.LeftLowerArm), Target.anim.GetBoneTransform(HumanBodyBones.LeftHand), Target.anim.GetBoneTransform(HumanBodyBones.LeftUpperArm)));

            Target.Colliders.Add(AddBoneCollider(Target.anim.GetBoneTransform(HumanBodyBones.RightUpperArm), Target.anim.GetBoneTransform(HumanBodyBones.RightLowerArm), Target.anim.GetBoneTransform(HumanBodyBones.Chest)));
            Target.Colliders.Add(AddBoneCollider(Target.anim.GetBoneTransform(HumanBodyBones.RightLowerArm), Target.anim.GetBoneTransform(HumanBodyBones.RightHand), Target.anim.GetBoneTransform(HumanBodyBones.RightUpperArm)));

            //Legs
            Target.Colliders.Add(AddBoneCollider(Target.anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg), Target.anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg), Target.anim.GetBoneTransform(HumanBodyBones.Spine)));
            Target.Colliders.Add(AddBoneCollider(Target.anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg), Target.anim.GetBoneTransform(HumanBodyBones.LeftFoot), Target.anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg)));

            Target.Colliders.Add(AddBoneCollider(Target.anim.GetBoneTransform(HumanBodyBones.RightUpperLeg), Target.anim.GetBoneTransform(HumanBodyBones.RightLowerLeg), Target.anim.GetBoneTransform(HumanBodyBones.Spine)));
            Target.Colliders.Add(AddBoneCollider(Target.anim.GetBoneTransform(HumanBodyBones.RightLowerLeg), Target.anim.GetBoneTransform(HumanBodyBones.RightFoot), Target.anim.GetBoneTransform(HumanBodyBones.RightUpperLeg)));
        }

        /// <summary>
        /// Add Collider to single Bone
        /// </summary>
        /// <param name="Bone">The Target Bone</param>
        /// <param name="NextBone">Child Bone</param>
        /// <param name="PrevBone">Parent Bone</param>
        /// <returns></returns>
        public static CapsuleCollider AddBoneCollider(Transform Bone, Transform NextBone, Transform PrevBone)
        {
            CapsuleCollider _collider = Bone.gameObject.AddComponent<CapsuleCollider>();
            float Distance = NextBone.localPosition.y;
            _collider.height = Distance * 1.5f;
            _collider.center = new Vector3(0, Distance / 2,0);
            _collider.radius = Distance / 3;

            //Bone.gameObject.AddComponent<Rigidbody>();
            //CharacterJoint join = Bone.gameObject.AddComponent<CharacterJoint>();
            //join.connectedBody = PrevBone.GetComponent<Rigidbody>();



            return _collider;
        }

        /// <summary>
        /// Enable Good Player Physics
        /// </summary>
        /// <param name="Target">Target to Enable</param>
        /// <param name="Force">Start force</param>
        public static void StartGoodPhysics(GPP_Base Target, Vector3 Force)
        {
            for (int i = 0; i < Target.GoodPhysicsConfiguration.Colliders.Count; i++)
            {
                Target.GoodPhysicsConfiguration.Colliders[i].isTrigger = false;
            }

            Target.GoodPhysicsConfiguration.anim.applyRootMotion = false;
            Target.GoodPhysicsConfiguration.GP_Enabled = true;

            Target.GoodPhysicsConfiguration.anim.SetBool(Target.GoodPhysicsConfiguration.Animations.ContactAnimation,
                Target.GoodPhysicsConfiguration.GP_Contact);
            Target.GoodPhysicsConfiguration.anim.SetBool(Target.GoodPhysicsConfiguration.Animations.EnableGoodPhysicsAnimation,
    Target.GoodPhysicsConfiguration.GP_Enabled);
            Target.GoodPhysicsConfiguration.anim.SetFloat(Target.GoodPhysicsConfiguration.Animations.RotationSpeedAnimation,
Target.GoodPhysicsConfiguration.GP_Rotation);

            PlayerPhysics.FreezePlayerWithoutPhyiscs(Target.Base, true);
            Target.GoodPhysicsConfiguration.rb.freezeRotation = false;
            Target.GoodPhysicsConfiguration.rb.drag = 0;
            Target.GoodPhysicsConfiguration.rb.angularDrag = 0.2f;
            Target.GoodPhysicsConfiguration.rb.useGravity = true;

            Target.transform.Rotate(Force, Space.Self);
        }

        /// <summary>
        /// Enable Good Player Physics
        /// </summary>
        /// <param name="Target">Target to Enable</param>
        public static void StartGoodPhysics(GPP_Base Target)
        {
            StartGoodPhysics(Target, Vector3.zero);
        }

        /// <summary>
        /// Disable Good Player Physics
        /// </summary>
        /// <param name="Target">Target to Enable</param>
        public static void StopGoodPhysics(GPP_Base Target)
        {
            Target.StopParticles();

            RaycastHit hit;
            Physics.Linecast(Target.transform.position + Vector3.up, Target.transform.position - (Vector3.up * 100), out hit, Target.Base.playerPhysics.GroundLayers);
            Target.WakeUpPosition = hit.point + (hit.normal * 0.15f); //Fix Player Position to Wake Up

            Target.GoodPhysicsConfiguration.GP_Enabled = false; //Wake Up

            Target.GoodPhysicsConfiguration.anim.applyRootMotion = true;
            Target.GoodPhysicsConfiguration.GP_Enabled = false;
            Target.GoodPhysicsConfiguration.rb.freezeRotation = true;
            Target.GoodPhysicsConfiguration.rb.isKinematic = true;

            Target.GoodPhysicsConfiguration.anim.SetBool(Target.GoodPhysicsConfiguration.Animations.ContactAnimation,
    Target.GoodPhysicsConfiguration.GP_Contact);
            Target.GoodPhysicsConfiguration.anim.SetBool(Target.GoodPhysicsConfiguration.Animations.EnableGoodPhysicsAnimation,
    Target.GoodPhysicsConfiguration.GP_Enabled);
            Target.GoodPhysicsConfiguration.anim.SetFloat(Target.GoodPhysicsConfiguration.Animations.RotationSpeedAnimation,
Target.GoodPhysicsConfiguration.GP_Rotation);

            for (int i = 0; i < Target.GoodPhysicsConfiguration.Colliders.Count; i++)
            {
                Target.GoodPhysicsConfiguration.Colliders[i].isTrigger = true;
            }
            Target.GetComponent<Rigidbody>().velocity = Vector3.zero;
            PlayerPhysics.FreezePlayerControls(Target.Base);
            Target.Invoke("UnFreeze", 1);
            //Target.transform.rotation = Quaternion.Euler(0, Target.transform.eulerAngles.y, 0);
            //PlayerPhysics.PlacePlayerOnGround(Target.transform, Target.Base.playerPhysics, true);
        }

        /// <summary>
        /// Run Good Player Physics Animations
        /// </summary>
        /// <param name="Target">Target</param>
        public static void ExecuteGPPAnimations(GPP_Base Target)
        {
            Target.GoodPhysicsConfiguration.anim.SetBool(Target.GoodPhysicsConfiguration.Animations.ContactAnimation,
Target.GoodPhysicsConfiguration.GP_Contact);
            Target.GoodPhysicsConfiguration.anim.SetBool(Target.GoodPhysicsConfiguration.Animations.EnableGoodPhysicsAnimation,
    Target.GoodPhysicsConfiguration.GP_Enabled);
            Target.GoodPhysicsConfiguration.anim.SetFloat(Target.GoodPhysicsConfiguration.Animations.RotationSpeedAnimation,
Target.GoodPhysicsConfiguration.GP_Rotation);
        }

        /// <summary>
        /// Enable RigidBody from bone (Not used on Main System)
        /// </summary>
        /// <param name="bone">Target</param>
        public static void EnableBoneRb(Transform bone)
        {
            Rigidbody rb = bone.GetComponent<Rigidbody>();
            CharacterJoint Join = bone.GetComponent<CharacterJoint>();
            rb.detectCollisions = true;
            rb.isKinematic = false;
        }
    }
}
