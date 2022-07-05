using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSettings;

namespace AnderSystems
{
    public static class Utility
    {
        public static GM_Movement GetLocalPlayer()
        {
            return GM_Movement.LocalPlayer;
        }

        static Vector3 LastPos;
        public static Vector3 GetTransformVelocity(Transform Entity)
        {
            Vector3 Speed = (LastPos - Entity.position) / Time.deltaTime;
            LastPos = Entity.position;
            //Debug.Log(Entity.name + " velocity: " + Speed);
            return Speed;
        }
        public static Vector3 GetTransformVelocity(Transform Entity, bool debug)
        {
            Vector3 Speed = (LastPos - Entity.position) / Time.deltaTime;
            LastPos = Entity.position;
            //Debug.Log(Entity.name + " velocity: " + Speed);
            if (debug)
            {
                Debug.Log("Transform velocity of: ''" + Entity.name + "'' is: " + Speed,null);
            }
            return Speed;
        }
        public static Quaternion GetGroundRotation(RaycastHit hit)
        {
            Quaternion Rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            return new Quaternion(Rotation.x, Rotation.y, Rotation.z, Rotation.w);
        }
        public static Quaternion GetGroundRotation(RaycastHit hit, Transform Entity)
        {
            Quaternion Rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            return new Quaternion(Rotation.x, Entity.rotation.y, Rotation.z, Rotation.w);
        }

        public static float GetGroundAngle(Transform Target, Vector3 Direction, LayerMask GroundLayer, float MaxDetection)
        {
            float angle = default;
            RaycastHit hit;
            Vector3 StartPos = (Target.position) + Direction;
            if (Physics.Linecast(StartPos + (Vector3.up * (MaxDetection * 2)), StartPos - (Vector3.up * (MaxDetection * 2)), out hit, GroundLayer))
            {
                angle = (hit.point.y - Target.position.y) * 37.5f;
            }
            return angle;
        }

        public static float GetGroundAngle(RaycastHit hit)
        {
            Quaternion RotationA = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Quaternion RotationB = Quaternion.identity;

            

            Debug.Log("Quaternion.Angle(" + Quaternion.Angle(RotationA, RotationB) + ")");
            return Quaternion.Angle(RotationA, RotationB);
        }

        public static Vector3 GetCollisionAngle(ContactPoint contact)
        {
            Vector3 CollisionAngle = default;
            CollisionAngle.x = Mathf.DeltaAngle(Quaternion.LookRotation(contact.point).eulerAngles.x, 0);
            CollisionAngle.y = Mathf.DeltaAngle(Quaternion.LookRotation(contact.point).eulerAngles.y, 0);
            CollisionAngle.z = Mathf.DeltaAngle(Quaternion.LookRotation(contact.point).eulerAngles.z, 0);
            return CollisionAngle;
        }

    }

    namespace GoodMotion
    {


        /// <summary>
        /// Controls Player Physics Params like: Ground Detection or Freeze player
        /// </summary>
        public static class PlayerPhysics
        {
            [System.Serializable]
            public class physics
            {
                public bool UsePhysics = true;
                [Header("Ground Detection")]
                public LayerMask GroundLayers;
                public float DetectionDistance;
                public float GroundDistance;
            }

            /// <summary>
            /// Bool automatically check if have ground detection
            /// </summary>
            /// <param name="Entity">The Target</param>
            /// <param name="detection">Detection Params</param>
            /// <returns></returns>
            public static bool DetectGround(Transform Entity, physics detection)
            {
                //bool IsGrounded = new bool();
                //if(Physics.Linecast(Entity.position + Vector3.up, Entity.position + (Vector3.down * detection.GroundDistance), detection.GroundLayers))
                //{
                //    IsGrounded = true;
                //} else
                //{
                //    if(!Physics.Linecast(Entity.position + Vector3.up, Entity.position + (Vector3.down * detection.DetectionDistance), detection.GroundLayers))
                //    {
                //        IsGrounded = false;
                //    }
                //}

                //return IsGrounded;
                return Physics.Linecast(Entity.position + Vector3.up, Entity.position + (Vector3.down * detection.DetectionDistance), detection.GroundLayers);
            }

            /// <summary>
            /// Bool automatically check if have ground detection with custom distance
            /// </summary>
            /// <param name="Entity">The Target</param>
            /// <param name="detection">Detection Params</param>
            /// <param name="CustomDistance">Distance of detection</param>
            /// <returns></returns>
            public static bool DetectGround(Transform Entity, physics detection, float CustomDistance)
            {
                return Physics.Linecast(Entity.position + Vector3.up, Entity.position + (Vector3.down * CustomDistance), detection.GroundLayers);
            }

            /// <summary>
            /// RaycastHit to ground Detection
            /// </summary>
            /// <param name="Entity">The Target</param>
            /// <param name="detection">Detection Params</param>
            /// <returns></returns>
            public static RaycastHit DetectGroundRay(Transform Entity, physics detection)
            {
                RaycastHit hit;
                Physics.Linecast(Entity.position + Vector3.up, Entity.position + (Vector3.down * detection.DetectionDistance), out hit, detection.GroundLayers);
                return hit;
            }

            /// <summary>
            /// RaycastHit to ground Detection with custom Distance
            /// </summary>
            /// <param name="Entity">The Target</param>
            /// <param name="detection">Detection Params</param>
            /// <param name="CustomDistance">Distance of detection</param>
            /// <returns></returns>
            public static RaycastHit DetectGroundRay(Transform Entity, physics detection, float CustomDistance)
            {
                RaycastHit hit;
                Physics.Linecast(Entity.position + Vector3.up, Entity.position + (Vector3.down * CustomDistance), out hit, detection.GroundLayers);
                return hit;
            }

            /// <summary>
            /// Place Player On Ground
            /// </summary>
            /// <param name="Player">The Player Target</param>
            /// <param name="detection">Detection Params</param>
            /// <param name="ChangePhysics">If can change RigidBody physics (Works only if Rb is on Target Object)</param>
            public static void PlacePlayerOnGround(Transform Player, physics detection, bool ChangePhysics)
            {
                if (ChangePhysics)
                {
                    Rigidbody rb = Player.GetComponent<Rigidbody>();
                    rb.freezeRotation = true;

                    if (DetectGround(Player, detection))
                    {
                        Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    }

                    Player.GetComponent<Rigidbody>().useGravity = !DetectGround(Player, detection);
                    Player.GetComponent<Animator>().applyRootMotion = DetectGround(Player, detection);

                    if (!Player.GetComponent<Rigidbody>())
                    {
                        Debug.LogError("(GoodMotion PlayerPhysics error on: PlacePlayerOnGround()) The ''Target'' Player don't have a RigidBody");
                    }
                }

                if (DetectGround(Player, detection))
                {
                    //Player.transform.position = DetectGroundRay(Player, detection).point;
                    Player.transform.position = Vector3.Lerp(Player.transform.position,
                    DetectGroundRay(Player, detection).point + (Vector3.up * detection.GroundDistance), 
                    (Mathf.Abs(Utility.GetTransformVelocity(Player).magnitude) + 5) * Time.deltaTime);
                }


            }

            /// <summary>
            /// Place A entity on ground using Physics settings
            /// </summary>
            /// <param name="Entity">The Target</param>
            /// <param name="detection">Detection Params</param>
            public static void PlaceEntityOnGround(Transform Entity, physics detection)
            {
                if (DetectGround(Entity, detection))
                {
                    Entity.transform.position =
                    DetectGroundRay(Entity, detection).point + (Vector3.up * detection.GroundDistance);
                }
            }

            /// <summary>
            /// Freeze player with physics (Works only with GM_Movement)
            /// </summary>
            /// <param name="Player">Target</param>
            public static void FreezePlayer(GM_Movement Player)
            {
                Player.rb.detectCollisions = false;
                Player.rb.freezeRotation = true;
                Player.rb.useGravity = false;
                Player.rb.velocity = Vector3.zero;
                Player.IsFreezed = true;
                Player.Controls = false;
            }

            /// <summary>
            /// Freeze Only Player Movement but keep Physics (Works only with GM_Movement)
            /// </summary>
            /// <param name="Player">Target</param>
            /// <param name="DisableMainCollider">Disables the collider moored to the player</param>
            public static void FreezePlayerWithoutPhyiscs(GM_Movement Player, bool DisableMainCollider)
            {
                Player.IsFreezed = true;
                Player.rb.GetComponent<Collider>().isTrigger = DisableMainCollider;
                Player.Controls = false;
            }

            /// <summary>
            /// Freeze Only Player Controls but keep Physics and Movement(Works only with GM_Movement)
            /// </summary>
            /// <param name="Player">Target</param>
            public static void FreezePlayerControls(GM_Movement Player)
            {
                Player.Controls = false;
            }

            /// <summary>
            /// Unfreeze player
            /// </summary>
            /// <param name="Player">Target</param>
            public static void UnFreezePlayer(GM_Movement Player)
            {
                Player.rb.GetComponent<Collider>().isTrigger = false;
                Player.rb.detectCollisions = true;
                Player.IsFreezed = false;
                Player.Controls = true;
            }
        }

        /// <summary>
        /// Controls Player Movement like: LookAt or Move Player To
        /// </summary>
        public static class Movement
        {
            public enum movementType
            {
                ThirdPerson, FirstPerson
            }
            [System.Serializable]
            public class movement
            {
                public movementType MovementType;
                public bool UseRootMotion = true;
                public float ThurnSpeed = 3;
                public float MoveSpeed = 1f;
                public float SprintSpeed = 1.5f;
                public float RunSpeed = 2f;
                public bool OnWalk { get; set; }
                public bool OnSprint { get; set; }
                public bool OnRun { get; set; }
                public bool OnAim { get; set; }
                
            }

            /// <summary>
            /// Get player Inputs
            /// </summary>
            /// <returns></returns>
            public static Vector3 GetPlayerMovInputs()
            {
                Vector3 MoveInputs = new Vector3(Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.WalkVertical), 0,
                    Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.WalkHorizontal));
                return MoveInputs;
            }

            /// <summary>
            /// Get Player Inputs relative to Entity Position (Input + Entity)
            /// </summary>
            /// <param name="Entity">Entity to relative</param>
            /// <returns></returns>
            public static Vector3 GetPlayerMovInputsRelative(Transform Entity)
            {
                Vector3 MoveInputs = new Vector3();
                if (Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.WalkVertical) != 0 ||
                    Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.WalkHorizontal) != 0)
                {
                    MoveInputs = new Vector3(Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.WalkVertical), 0,
                        Inputs.GetButtonAxis(GameConfigurations.MainSettings.Controls.WalkHorizontal));
                }
                return MoveInputs + Entity.position;
            }

            /// <summary>
            /// Make Entity look at Target with lerp or not
            /// </summary>
            /// <param name="Entity">Target</param>
            /// <param name="Position">Coordinates to look at</param>
            /// <param name="ThurnSpeed">Thurn speed (set 0 if dont wanna)</param>
            public static void EntityLookAt(Transform Entity, Vector3 Position, float ThurnSpeed)
            {
                Vector3 RelativePos = Position - Entity.position;
                Quaternion RelativeRotation = Quaternion.LookRotation(RelativePos);
                if (ThurnSpeed == 0)
                {
                    Entity.transform.rotation = new Quaternion(Entity.transform.rotation.x, RelativeRotation.y, Entity.transform.rotation.z, RelativeRotation.w);
                }
                else
                {
                    Quaternion Rotation = new Quaternion(Entity.transform.rotation.x, RelativeRotation.y, Entity.transform.rotation.z, RelativeRotation.w);
                    Entity.transform.rotation = Quaternion.Lerp(Entity.transform.rotation, Rotation, ThurnSpeed * Time.deltaTime);
                }
            }

            /// <summary>
            /// Make Entity look at Target with lerp or not, Relative to Y position
            /// </summary>
            /// <param name="Entity">Target</param>
            /// <param name="Position">Coordinates to look at</param>
            /// <param name="ThurnSpeed">Thurn speed (set 0 if dont wanna)</param>
            /// <param name="Relative">Relative Object</param>
            public static void EntityLookAtRelative(Transform Entity, Vector3 Position, float ThurnSpeed, Transform Relative, float Angle)
            {
                Vector3 RelativePos = Position - Entity.position;
                Vector3 RelativeRotation = Quaternion.LookRotation(RelativePos).eulerAngles;
                Quaternion FinalRotation = Quaternion.Euler(RelativeRotation.x, RelativeRotation.y + (Relative.eulerAngles.y + Angle),
                        RelativeRotation.z);
                if (ThurnSpeed == 0)
                {
                    Entity.transform.rotation = FinalRotation;
                }
                else
                {
                    Entity.transform.rotation = Quaternion.Lerp(Entity.transform.rotation, FinalRotation, ThurnSpeed * Time.deltaTime);
                }
            }

            /// <summary>
            /// Get Rotation of look at + Y Rotation from another Transform
            /// </summary>
            /// <param name="Entity">Target</param>
            /// <param name="Position">Posision to Look At</param>
            /// <param name="Relative">Relative Object</param>
            /// <param name="Angle">Y shift</param>
            /// <returns></returns>
            public static Quaternion GetEntityLookAtRelative(Transform Entity, Vector3 Position, Transform Relative, float Angle)
            {
                Vector3 RelativePos = Position - Entity.position;
                Vector3 RelativeRotation = Quaternion.LookRotation(RelativePos).eulerAngles;
                Quaternion FinalRotation = Quaternion.Euler(RelativeRotation.x, RelativeRotation.y + (Relative.eulerAngles.y + Angle),
                        RelativeRotation.z);
                return FinalRotation;
            }

            /// <summary>
            /// Teleport Entity to coordinates
            /// </summary>
            /// <param name="Entity">The Target</param>
            /// <param name="Position">Coordinates to teleport</param>
            public static void MoveEntityTo(Transform Entity, Vector3 Position)
            {
                Entity.transform.position = Position;
            }
            /// <summary>
            /// Move Entity to position
            /// </summary>
            /// <param name="Entity">Target</param>
            /// <param name="Position">Wanted Position</param>
            /// <param name="speed">Step Speed</param>
            public static void MoveEntityTo(Transform Entity, Vector3 Position, float speed)
            {
                Vector3 RelativePos = Position - Entity.position;
                Entity.transform.Translate(RelativePos * (speed * Time.deltaTime), Space.World);
            }

            /// <summary>
            /// Move Entity to position and ignore Physics
            /// </summary>
            /// <param name="Entity">Target</param>
            /// <param name="Position">Wanted Position</param>
            /// <param name="speed">Step Speed</param>
            public static void MoveEntityTo(Rigidbody Entity, Vector3 Position, float speed)
            {
                Entity.detectCollisions = false;
                Entity.useGravity = false;
                Entity.velocity = Vector3.zero;
                Vector3 RelativePos = Position - Entity.position;
                Entity.transform.Translate(RelativePos * (speed * Time.deltaTime), Space.World);
            }

            /// <summary>
            /// Move Entity to position
            /// </summary>
            /// <param name="Entity">Target</param>
            /// <param name="Position">Wanted Position</param>
            /// <param name="speed">Step Speed</param>
            /// <param name="Distance">Min Distance</param>
            public static void MoveEntityTo(Transform Entity, Vector3 Position, float speed, float Distance, Space space)
            {
                if (Vector3.Distance(Entity.position, Position) >= Distance)
                {
                    Vector3 RelativePos = Position - Entity.position;
                    Entity.transform.Translate(RelativePos * (speed * Time.deltaTime), space);
                }
            }
        }

        /// <summary>
        /// Controls Player Animations like: IK or Base Animations
        /// </summary>
        public static class Animations
        {
            [System.Serializable]
            public class AnimationsSetup
            {
                public bool UseAnimations = true;
                public bool CustomAnimator;
                public Animator anim;
                [System.Serializable]
                public class InverseKinematics
                {
                    public bool UseIK = true;
                    public LayerMask FootIKLayers;
                    public float LegSize;
                    public float FootIKWeight;
                    public string LFootIKAnim;
                    public string RFootIKAnim;
                    public float LFootIK { get; set; }
                    public float RFootIK { get; set; }
                }
                [SerializeField]
                public InverseKinematics IK;
                public string GroundedAnim = "Grounded";
                public string WalkAnim = "Walk";
                public string SprintAnim = "Sprint";
                public string RunAnim = "Run";
                public string DirectionAnim = "Direction";
                public string InclinationAnim = "Inclination";
                public string FallVelocityAnim = "FallVelocity";
                public string AimAnim = "Aim";
                //Extra
                public bool UseExtraWallAnimation = true;
            }

            /// <summary>
            /// Animate the player
            /// </summary>
            /// <param name="AnimationSetup">The Animation Setup</param>
            /// <param name="Grounded">If Is Grounded Param</param>
            /// <param name="Walking">If Walking Param</param>
            /// <param name="Sprint">If Sprint param</param>
            /// <param name="Run">If Running param</param>
            /// <param name="Direction">Direction Anim</param>
            /// <param name="Inclination">Inclination Anim</param>
            public static void BaseAnimations(AnimationsSetup AnimationSetup,bool Grounded, bool Walking, bool Sprint, bool Run, float Direction, float Inclination, float FallVelocity, bool Aim)
            {
                AnimationSetup.anim.SetBool(AnimationSetup.GroundedAnim, Grounded);
                if (!Grounded)
                {
                    AnimationSetup.anim.SetFloat(AnimationSetup.FallVelocityAnim, FallVelocity);
                }
                AnimationSetup.anim.SetBool(AnimationSetup.WalkAnim, Walking);
                AnimationSetup.anim.SetBool(AnimationSetup.SprintAnim, Sprint);
                AnimationSetup.anim.SetBool(AnimationSetup.RunAnim, Run);
                AnimationSetup.anim.SetBool(AnimationSetup.AimAnim, Aim);
                AnimationSetup.anim.SetFloat(AnimationSetup.DirectionAnim, Direction);
                AnimationSetup.anim.SetFloat(AnimationSetup.InclinationAnim, Inclination);
            }

            /// <summary>
            /// Check Y Angle from Contact point
            /// </summary>
            /// <param name="Target">Target</param>
            /// <param name="ContactPoint">Contact Point</param>
            /// <returns></returns>
            public static float GetWallParam(Transform Target, ContactPoint ContactPoint)
            {
                float Angle = Quaternion.LookRotation(ContactPoint.point - Target.transform.position).eulerAngles.y;
                return Mathf.DeltaAngle(Target.eulerAngles.y, Angle);
            }

            /// <summary>
            /// Animate Only "Wall Animation"
            /// </summary>
            /// <param name="AnimationSetup">Setup</param>
            /// <param name="DetectionAngle">Angle</param>
            /// <param name="InWallCollision">If in collision</param>
            public static void ExtraWallAnimation(AnimationsSetup AnimationSetup, float DetectionAngle, bool InWallCollision)
            {
                //Extra
                if (InWallCollision)
                {
                    AnimationSetup.anim.SetFloat("ExtraWallAnimation", Mathf.Lerp(AnimationSetup.anim.GetFloat("ExtraWallAnimation"),
                        DetectionAngle, 3 * Time.deltaTime));
                } else
                {
                    AnimationSetup.anim.SetFloat("ExtraWallAnimation", Mathf.Lerp(AnimationSetup.anim.GetFloat("ExtraWallAnimation"),0,
                        3 * Time.deltaTime));
                }
                AnimationSetup.anim.SetBool("InCollision", InWallCollision);
            }

            /// <summary>
            /// Run Inverse Kinematics
            /// </summary>
            /// <param name="animations">Setup</param>
            public static void ExecuteInverseKinematics(AnimationsSetup animations)
            {
                //Foots IK
                animations.IK.LFootIK = animations.anim.GetFloat(animations.IK.LFootIKAnim);
                animations.IK.RFootIK = animations.anim.GetFloat(animations.IK.RFootIKAnim);

                if (GetBoneGround(animations.anim.GetBoneTransform(HumanBodyBones.LeftFoot), animations.IK.FootIKLayers,
                    animations.IK.LegSize).collider != null)
                {
                    animations.anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, animations.IK.LFootIK * animations.IK.FootIKWeight);
                    animations.anim.SetIKPosition(
                        AvatarIKGoal.LeftFoot, GetBoneGround(animations.anim.GetBoneTransform(HumanBodyBones.LeftFoot), animations.IK.FootIKLayers,
                        animations.IK.LegSize).point);
                }

                if (GetBoneGround(animations.anim.GetBoneTransform(HumanBodyBones.RightFoot), animations.IK.FootIKLayers,
                    animations.IK.LegSize).collider != null)
                {
                    animations.anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, animations.IK.RFootIK * animations.IK.FootIKWeight);
                    animations.anim.SetIKPosition(
                        AvatarIKGoal.RightFoot, GetBoneGround(animations.anim.GetBoneTransform(HumanBodyBones.RightFoot), animations.IK.FootIKLayers,
                        animations.IK.LegSize).point);
                }
            }

            /// <summary>
            /// Get RaycastHit from bone position
            /// </summary>
            /// <param name="Bone">Bone Transform</param>
            /// <param name="layers">Layers to detect</param>
            /// <param name="Distance">Max Distance</param>
            /// <returns></returns>
            public static RaycastHit GetBoneGround(Transform Bone, LayerMask layers, float Distance)
            {
                RaycastHit hit;
                Physics.Linecast(Bone.position + (Vector3.up * Distance), Bone.position - (Vector3.up * (Distance * 1.5f)), out hit, layers);
                return hit;
            }
        }

        public static class Polishing
        {
            [System.Serializable]
            public class SoundsParticles
            {
                public Animator anim;
                [System.Serializable]
                public class footSteps
                {
                    [SerializeField]
                    public GameObject LeftFootParticle { get; set; }
                    [SerializeField]
                    public GameObject RightFootParticle { get; set; }
                    public bool UseFootStepsParticle = true;
                    public SoundParticles[] Particles;
                    public string LeftFootAnimationVar, RightFootAnimationVar;
                }
                [SerializeField]
                public footSteps FootSteps;
                public bool UseClothingSound;
                public string ClothingSoundAnim;
                public float VolumeControl;
                public AudioSource ClothingSound;

                public float Slips = 32f;
                public string SlipAnimation = "SlipsDown";
            }

            /// <summary>
            /// Runs player particles and sounds
            /// </summary>
            /// <param name="soundsParticles">Particles Setup</param>
            /// <param name="RunValue">If Player Running (For Footstep Particles)</param>
            /// <param name="DetectionLayers">Layers of detection</param>
            public static void ExecuteParticles(SoundsParticles soundsParticles, bool RunValue, LayerMask DetectionLayers)
            {
                for (int i = 0; i < soundsParticles.FootSteps.Particles.Length; i++)
                {
                    if (soundsParticles.anim.GetFloat(soundsParticles.FootSteps.LeftFootAnimationVar) >= 0.8f)
                    {
                        if (!soundsParticles.FootSteps.LeftFootParticle)
                        {
                            soundsParticles.FootSteps.LeftFootParticle = SoundParticles.SpawnParticle(
                            soundsParticles.FootSteps.Particles[i], soundsParticles.FootSteps.LeftFootParticle,
                            soundsParticles.anim.GetBoneTransform(HumanBodyBones.RightFoot),
                            Random.Range(0, soundsParticles.FootSteps.Particles.Length),
                            RunValue, DetectionLayers, true);
                        }
                    }

                    if (soundsParticles.anim.GetFloat(soundsParticles.FootSteps.RightFootAnimationVar) >= 0.8f)
                    {
                        if (!soundsParticles.FootSteps.RightFootParticle)
                        {
                            soundsParticles.FootSteps.RightFootParticle = SoundParticles.SpawnParticle(
                                soundsParticles.FootSteps.Particles[i], soundsParticles.FootSteps.RightFootParticle,
                                soundsParticles.anim.GetBoneTransform(HumanBodyBones.RightFoot),
                                Random.Range(0, soundsParticles.FootSteps.Particles.Length),
                                RunValue, DetectionLayers,true);
                        }
                    }


                    if (soundsParticles.anim.GetFloat(soundsParticles.FootSteps.LeftFootAnimationVar) < 0.8f)
                    {
                        soundsParticles.FootSteps.LeftFootParticle = null;
                    }

                    if (soundsParticles.anim.GetFloat(soundsParticles.FootSteps.RightFootAnimationVar) < 0.8f)
                    {
                        soundsParticles.FootSteps.RightFootParticle = null;
                    }

                    if (soundsParticles.UseClothingSound)
                    {
                        soundsParticles.ClothingSound.volume = soundsParticles.anim.GetFloat(soundsParticles.ClothingSoundAnim) *
                            soundsParticles.VolumeControl;
                        soundsParticles.ClothingSound.pitch = soundsParticles.anim.GetFloat(soundsParticles.ClothingSoundAnim) * 1.5f;
                    }
                }
            }
        }

        namespace cam
        {
            /// <summary>
            /// Some utilities for Camera
            /// </summary>
            public static class CamUtility
            {
                /// <summary>
                /// Change camera with Lerping
                /// </summary>
                /// <param name="CameraA">The Camera Target</param>
                /// <param name="CameraB">The final Target</param>
                /// <param name="Lerping">Lerping value</param>
                /// <param name="ChangeParent">If change parent</param>
                public static void ChangeCamWithLerping(Camera CameraA, Camera CameraB, float Lerping, bool ChangeParent)
                {
                    if (ChangeParent)
                    {
                        CameraA.transform.parent = CameraB.transform.parent;
                    }

                    CameraA.transform.position = Vector3.Lerp(CameraA.transform.position,
                    CameraB.transform.position, Lerping * Time.deltaTime);

                    CameraA.transform.rotation = Quaternion.Lerp(CameraA.transform.rotation,
                        CameraB.transform.rotation, Lerping * Time.deltaTime);

                    CameraA.fieldOfView = Mathf.Lerp(CameraA.fieldOfView, CameraB.fieldOfView, Lerping * Time.deltaTime);
                }
            }

            public static class GameplayCam
            {

                static Camera MainGameplayCam;
                static Transform MainOrbit;
                static Vector2 OrbitAxis;

                [System.Serializable]
                public class Params
                {
                    public Camera GameplayCam;
                    public Transform Orbit;
                    public Transform OrbitCenter;
                    public float MinY, MaxY;

                    public Camera IdleCam, SprintCam, RunningCam, AimCam;
                    public float IdleLerp, SprintLerp, RunningLerp;

                    public bool UseCameraCollider;
                    public LayerMask CameraCollisionMask;
                }

                /// <summary>
                /// Set Camera States (Running Idle Sprint)
                /// </summary>
                /// <param name="cameraParams">Camera configs</param>
                static void CameraStates(Params cameraParams)
                {
                    if (gameplayCamState == GameplayCamState.Idle)
                    {
                        CurrentGameplayCam = cameraParams.IdleCam;
                        CamUtility.ChangeCamWithLerping(MainGameplayCam, cameraParams.IdleCam, cameraParams.IdleLerp, false);
                    }
                    if (gameplayCamState == GameplayCamState.Sprint)
                    {
                        CurrentGameplayCam = cameraParams.SprintCam;
                        CamUtility.ChangeCamWithLerping(MainGameplayCam, cameraParams.SprintCam, cameraParams.SprintLerp, false);
                    }
                    if (gameplayCamState == GameplayCamState.Run)
                    {
                        CurrentGameplayCam = cameraParams.RunningCam;
                        CamUtility.ChangeCamWithLerping(MainGameplayCam, cameraParams.RunningCam, cameraParams.RunningLerp, false);
                    }

                    if (gameplayCamState == GameplayCamState.Custom)
                    {
                        CamUtility.ChangeCamWithLerping(MainGameplayCam, CurrentGameplayCam, CurrentGameplayLerp, false);
                    }
                }

                /// <summary>
                /// Execute gameplay Cam (Works better in FixedUpdate)
                /// </summary>
                /// <param name="camera">Camera Params</param>
                public static void Execute(Params cameraParams)
                {
                    if (!CurrentGameplayCam)
                    {
                        CurrentGameplayCam = cameraParams.IdleCam;
                    }
                    if (MainGameplayCam == null)
                    {
                        MainGameplayCam = cameraParams.GameplayCam;
                    }

                    if (MainOrbit == null)
                    {
                        MainOrbit = cameraParams.Orbit;
                        MainOrbit.transform.parent = null;
                    }

                    MainOrbit.transform.position = Vector3.Lerp(MainOrbit.transform.position, cameraParams.OrbitCenter.position, (5) * Time.deltaTime);

                    if (cameraParams.UseCameraCollider)
                    {
                        RaycastHit hit;
                        if (Physics.Linecast(GetOrbit().position, CurrentGameplayCam.transform.position, out hit, cameraParams.CameraCollisionMask))
                        {
                            GetGameplayCam().transform.position = hit.point + (hit.normal * (GetGameplayCam().nearClipPlane * 1.3f));
                        } else
                        {
                            CameraStates(cameraParams);
                        }
                    } else
                    {
                        CameraStates(cameraParams);
                    }
                }

                /// <summary>
                /// Get Main Orbit Transform
                /// </summary>
                /// <returns></returns>
                public static Transform GetOrbit()
                {
                    return MainOrbit;
                }

                /// <summary>
                /// Get Main Gameplay Cam
                /// </summary>
                /// <returns></returns>
                public static Camera GetGameplayCam()
                {
                    return MainGameplayCam;
                }

                public enum GameplayCamState
                {
                    Default,Idle,Sprint,Run, Custom
                }
                private static GameplayCamState gameplayCamState;

                public static Camera CurrentGameplayCam;
                public static float CurrentGameplayLerp;
                public static void SetGameplayCamParams(Camera Cam, float Lerping)
                {
                    SetGameplayCamState(GameplayCamState.Custom);
                    CurrentGameplayCam = Cam;
                    CurrentGameplayLerp = Lerping;
                }

                /// <summary>
                /// Set Gameplay Camera State
                /// </summary>
                /// <param name="State">The current State of Gameplay Camera</param>
                public static void SetGameplayCamState(GameplayCamState State)
                {
                    gameplayCamState = State;
                }

                /// <summary>
                /// Get Gameplay Camera State
                /// </summary>
                /// <param name="State">The current State of Gameplay Camera</param>
                public static GameplayCamState GetGameplayCamState()
                {
                    return gameplayCamState;
                }



                //Orbit Controls
                /// <summary>
                /// Control Orbit
                /// </summary>
                /// <param name="X">X Rotation</param>
                /// <param name="Y">Y Rotation</param>
                public static void SetOrbitAxis(float X, float Y)
                {
                    OrbitAxis.x = X; OrbitAxis.y = Y;
                    MainOrbit.transform.eulerAngles = OrbitAxis;
                }
                /// <summary>
                /// Control Orbit with Lerping
                /// </summary>
                /// <param name="X">X Rotation</param>
                /// <param name="Y">Y Rotation</param>
                /// <param name="Lerp">Lerping</param>
                public static void SetOrbitAxis(float X, float Y, float Lerp)
                {
                    OrbitAxis.x = Mathf.LerpAngle(OrbitAxis.x, X, Lerp);
                    OrbitAxis.y = Mathf.LerpAngle(OrbitAxis.y, Y, Lerp);
                    MainOrbit.transform.eulerAngles = OrbitAxis;
                }

                /// <summary>
                /// Make Orbit LookAt Coordinates
                /// </summary>
                /// <param name="Coordinates"></param>
                public static void SetOrbitLookAt(Vector3 Coordinates)
                {
                    Vector3 RelativePos = MainOrbit.transform.position - Coordinates;
                    OrbitAxis = Quaternion.LookRotation(RelativePos).eulerAngles;
                    MainOrbit.transform.eulerAngles = OrbitAxis;
                }

                /// <summary>
                /// Make Orbit LookAt Coordinates with Lerping
                /// </summary>
                /// <param name="Coordinates"></param>
                /// <param name="Lerp"></param>
                public static void SetOrbitLookAt(Vector3 Coordinates, float Lerp)
                {
                    Vector3 RelativePos = Coordinates - MainOrbit.transform.position;
                    OrbitAxis.x = Mathf.LerpAngle(OrbitAxis.x, Quaternion.LookRotation(RelativePos).eulerAngles.x, Lerp);
                    OrbitAxis.y = Mathf.LerpAngle(OrbitAxis.y, Quaternion.LookRotation(RelativePos).eulerAngles.y, Lerp);

                    MainOrbit.transform.eulerAngles = OrbitAxis;
                }

                /// <summary>
                /// Control Orbit Optimized to Mouse Movement
                /// </summary>
                /// <param name="X">X Rotation</param>
                /// <param name="Y">Y Rotation</param>
                /// <param name="Max">Max Rotation</param>
                /// <param name="Min">Min Rotation</param>
                public static void SetOrbitAxisMouse(float X, float Y, float Max, float Min)
                {
                    OrbitAxis.x = Mathf.Clamp(OrbitAxis.x + Y, Min, Max); OrbitAxis.y = OrbitAxis.y + X;
                    MainOrbit.transform.eulerAngles = OrbitAxis;
                }
            }
        }
    }
}
