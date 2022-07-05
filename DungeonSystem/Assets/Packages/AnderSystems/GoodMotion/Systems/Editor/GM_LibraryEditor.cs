using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AnderSystems.GoodMotion
{

    /// <summary>
    /// Editor component from GoodMotion
    /// </summary>
    namespace Editor
    {
        public static class GMPhysicsGUI
        {
            /// <summary>
            /// Draw the custom Physics Inspector
            /// </summary>
            /// <param name="PlayerPhys"></param>
            public static void DrawPhysicsGUI(PlayerPhysics.physics PlayerPhys, bool Foldout)
            {
                if (Foldout)
                {
                    PlayerPhys.UsePhysics = EditorGUILayout.BeginToggleGroup("Use Physics", PlayerPhys.UsePhysics);
                    if (PlayerPhys.UsePhysics)
                    {
                        //Draw layermask
                        LayerMask GroundDetectionTemp = EditorGUILayout.MaskField("Ground Detection Layers", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(PlayerPhys.GroundLayers), InternalEditorUtility.layers);
                        PlayerPhys.GroundLayers = GroundDetectionTemp;

                        GUILayout.BeginHorizontal();
                        float TempGroundDetection = EditorGUILayout.FloatField("Detection Distance", PlayerPhys.DetectionDistance);
                        PlayerPhys.DetectionDistance = TempGroundDetection;

                        float TempGroundDistance = EditorGUILayout.FloatField("Ground Distance", PlayerPhys.GroundDistance);
                        PlayerPhys.GroundDistance = TempGroundDistance;
                        PlayerPhys.GroundDistance = Mathf.Clamp(PlayerPhys.GroundDistance, -1000, PlayerPhys.DetectionDistance);
                        GUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndToggleGroup();
                }
            }

            /// <summary>
            /// Draw the 3D scene gizmos view from GroundDetection
            /// </summary>
            /// <param name="Entity">The target to apply</param>
            /// <param name="physics">The physics params</param>
            public static void DrawGroundDetectionGizmos(Transform Entity, PlayerPhysics.physics physics)
            {
                //Draw Ground Distance
                Handles.color = Color.green;
                Handles.DrawWireDisc(Entity.transform.position - (Vector3.up * physics.GroundDistance), Vector3.up, 1);

                //Draw Ground Detection
                Handles.color = Color.blue;
                Handles.DrawWireDisc(Entity.transform.position - (Vector3.up * physics.DetectionDistance), Vector3.up, .5f);
                Handles.DrawLine(Entity.transform.position + (Vector3.up), Entity.position - (Vector3.up * physics.DetectionDistance));
            }
        }

        public static class GMMovementGUI
        {
            public static void DrawMovementGUI(Movement.movement PlayerMov, Animator HaveAnimator, bool Foldout)
            {
                if (Foldout)
                {
                    PlayerMov.MovementType = (Movement.movementType)EditorGUILayout.EnumPopup("Movement Type", PlayerMov.MovementType);
                    PlayerMov.UseRootMotion = EditorGUILayout.Toggle("Use Root Motion", PlayerMov.UseRootMotion);
                    if (PlayerMov.UseRootMotion)
                    {
                        if (!HaveAnimator)
                        {
                            EditorGUILayout.HelpBox("Animator component needed to enable ''UseRootMotion''", MessageType.Error);
                        } else
                        {
                            if(HaveAnimator.runtimeAnimatorController == null)
                            {
                                EditorGUILayout.HelpBox("Animator controller not found to enable ''UseRootMotion''", MessageType.Error);
                            } else
                            {
                                if (HaveAnimator.avatar == null)
                                {
                                    EditorGUILayout.HelpBox("Avatar not configured on Animator controller to enable ''UseRootMotion''", MessageType.Error);
                                } else
                                {
                                    if (!HaveAnimator.applyRootMotion && !Application.isPlaying)
                                    {
                                        if (EditorUtility.DisplayDialog("Enable Root Motion?",
                                            "Enable Root Motion on Animator component?", "Yes", "No"))
                                        {
                                            HaveAnimator.applyRootMotion = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    {

                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                        EditorGUIUtility.labelWidth = 100f;
                        PlayerMov.ThurnSpeed = EditorGUILayout.FloatField("Thurn Speed", PlayerMov.ThurnSpeed, GUILayout.ExpandWidth(false));
                        if (!PlayerMov.UseRootMotion)
                        {
                            EditorGUIUtility.labelWidth = 60f;
                            PlayerMov.MoveSpeed = EditorGUILayout.FloatField("  Walking", PlayerMov.MoveSpeed, GUILayout.ExpandWidth(false));
                            PlayerMov.SprintSpeed = EditorGUILayout.FloatField("  Sprint", PlayerMov.SprintSpeed, GUILayout.ExpandWidth(false));
                            PlayerMov.RunSpeed = EditorGUILayout.FloatField("  Run", PlayerMov.RunSpeed, GUILayout.ExpandWidth(false));
                        }
                        EditorGUIUtility.labelWidth = default;
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        public static class GMCameraManagmentGUI
        {
            public static void DrawCameraManagmentGUI(cam.GameplayCam.Params CamManagment,GM_Movement Target, bool Foldout)
            {
                if (Foldout)
                {
                    CamManagment.GameplayCam = (Camera)EditorGUILayout.ObjectField("Main Gameplay Camera",CamManagment.GameplayCam, typeof(Camera), true);
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Orbit", MessageType.None);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUIUtility.labelWidth = 100f;
                    CamManagment.Orbit = (Transform)EditorGUILayout.ObjectField("Orbit", CamManagment.Orbit, typeof(Transform), true);
                    CamManagment.OrbitCenter = (Transform)EditorGUILayout.ObjectField("Orbit Center", CamManagment.OrbitCenter, typeof(Transform), true);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    CamManagment.MinY = EditorGUILayout.FloatField("MinY", CamManagment.MinY);
                    CamManagment.MaxY = EditorGUILayout.FloatField("MaxY", CamManagment.MaxY);

                    CamManagment.MinY = Mathf.Clamp(CamManagment.MinY, -360, CamManagment.MaxY);
                    CamManagment.MaxY = Mathf.Clamp(CamManagment.MaxY, CamManagment.MinY, 360);

                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Cameras", MessageType.None);
                    EditorGUILayout.BeginHorizontal();
                    CamManagment.IdleCam = (Camera)EditorGUILayout.ObjectField("Idle Camera", CamManagment.IdleCam, typeof(Camera), true);
                    CamManagment.SprintCam = (Camera)EditorGUILayout.ObjectField("Sprint Camera", CamManagment.SprintCam, typeof(Camera), true);
                    CamManagment.RunningCam = (Camera)EditorGUILayout.ObjectField("Running Camera", CamManagment.RunningCam, typeof(Camera), true);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    CamManagment.IdleLerp = EditorGUILayout.FloatField("Idle Lerping", CamManagment.IdleLerp);
                    CamManagment.SprintLerp = EditorGUILayout.FloatField("Sprint Lerping", CamManagment.SprintLerp);
                    CamManagment.RunningLerp = EditorGUILayout.FloatField("Running Lerping", CamManagment.RunningLerp);
                    EditorGUILayout.EndHorizontal();
                    EditorGUIUtility.labelWidth = default;

                    CamManagment.UseCameraCollider = EditorGUILayout.BeginToggleGroup("Use Camera Collider", CamManagment.UseCameraCollider);
                        LayerMask CameraCollisionMaskTemp = EditorGUILayout.MaskField("Camera Collision Layers", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(CamManagment.CameraCollisionMask), InternalEditorUtility.layers);
                        CamManagment.CameraCollisionMask = CameraCollisionMaskTemp;
                    EditorGUILayout.EndToggleGroup();
                }
            }
        }

        public static class GMAnimationsGUI
        {
            public static void DrawAnimationsGUI(Animations.AnimationsSetup Animations, GM_Movement Target, bool Foldout)
            {
                if (Foldout)
                {
                    Animations.UseAnimations = EditorGUILayout.BeginToggleGroup("Use Animations", Animations.UseAnimations);
                    if (Animations.UseAnimations)
                    {
                        //CamManagment.SetGamePlayCam = (Camera)EditorGUILayout.ObjectField("Gameplay Main Cam", CamManagment.SetGamePlayCam, typeof(Camera), true);

                        Animations.CustomAnimator = EditorGUILayout.Toggle("Custom Animator", Animations.CustomAnimator);

                        if (Animations.CustomAnimator)
                        {
                            Animations.anim = (Animator)EditorGUILayout.ObjectField("Animator", Animations.anim, typeof(Animator), true);
                        }
                        else
                        {
                            Animations.anim = Target.GetComponent<Animator>();
                        }

                        // Animations Names
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("Animation Names", MessageType.None);
                        EditorGUILayout.LabelField("Base Movement:");
                        Animations.GroundedAnim = EditorGUILayout.TextField("  Grounded:", Animations.GroundedAnim);
                        Animations.WalkAnim = EditorGUILayout.TextField("  Walk:", Animations.WalkAnim);
                        Animations.RunAnim = EditorGUILayout.TextField("  Run:", Animations.RunAnim);
                        Animations.SprintAnim = EditorGUILayout.TextField("  Sprint:", Animations.SprintAnim);
                        Animations.InclinationAnim = EditorGUILayout.TextField("  Inclination:", Animations.InclinationAnim);
                        Animations.DirectionAnim = EditorGUILayout.TextField("  Direction:", Animations.DirectionAnim);
                        Animations.FallVelocityAnim = EditorGUILayout.TextField("  Fall Velocity:", Animations.FallVelocityAnim);

                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("Inverse Kinematics", MessageType.None);
                        Animations.IK.UseIK = EditorGUILayout.BeginToggleGroup("Use Inverse Kinematics", Animations.IK.UseIK);
                        LayerMask IKDetectionLayers = EditorGUILayout.MaskField("IK Detection Layers", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(Animations.IK.FootIKLayers), InternalEditorUtility.layers);
                        Animations.IK.FootIKLayers = IKDetectionLayers;

                        EditorGUIUtility.labelWidth = 100f;
                        EditorGUILayout.BeginHorizontal();
                        Animations.IK.LegSize = EditorGUILayout.FloatField("Legs Size", Animations.IK.LegSize);
                        Animations.IK.FootIKWeight = EditorGUILayout.Slider("FootsIKWeight", Animations.IK.FootIKWeight, 0,1);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        Animations.IK.LFootIKAnim = EditorGUILayout.TextField("L FootIK Anim Name", Animations.IK.LFootIKAnim);
                        Animations.IK.RFootIKAnim = EditorGUILayout.TextField("R FootIK Anim Name", Animations.IK.RFootIKAnim);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndToggleGroup();
                        EditorGUIUtility.labelWidth = default;
                    }

                    EditorGUILayout.EndToggleGroup();
                }
            }

            public static void GMDrawIKGizmos(Animations.AnimationsSetup AnimationsSetup, GM_Movement Target)
            {
                Handles.color = Color.yellow;
                Handles.DrawLine(AnimationsSetup.anim.GetBoneTransform(HumanBodyBones.LeftFoot).position + (Vector3.up * AnimationsSetup.IK.LegSize),
                    AnimationsSetup.anim.GetBoneTransform(HumanBodyBones.LeftFoot).position - (Vector3.up * AnimationsSetup.IK.LegSize));

                Handles.DrawLine(AnimationsSetup.anim.GetBoneTransform(HumanBodyBones.RightFoot).position + (Vector3.up * AnimationsSetup.IK.LegSize),
                    AnimationsSetup.anim.GetBoneTransform(HumanBodyBones.RightFoot).position - (Vector3.up * AnimationsSetup.IK.LegSize));
            }
        }

        public static class GMParticles
        {
            public static void DrawParticlesGUI(Polishing.SoundsParticles particles,GM_Movement Target, bool Foldout)
            {
                SerializedObject _target = new SerializedObject(Target);

                if (Foldout)
                {
                    particles.anim = Target.animationSetup.anim;

                    particles.FootSteps.UseFootStepsParticle =
    EditorGUILayout.BeginToggleGroup("Use Foot Steps", particles.FootSteps.UseFootStepsParticle);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUIUtility.labelWidth = 120;

                    SerializedProperty SoundsArray = _target.FindProperty("SoundsAndParticles.FootSteps.Particles");

                    EditorGUILayout.PropertyField(SoundsArray);
                    _target.ApplyModifiedProperties();

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (Target.animationSetup.IK.UseIK)
                    {
                        particles.FootSteps.LeftFootAnimationVar = Target.animationSetup.IK.LFootIKAnim;
                        particles.FootSteps.RightFootAnimationVar = Target.animationSetup.IK.RFootIKAnim;
                        EditorGUILayout.HelpBox("Use IK animations to set Footsteps Var", MessageType.Info);
                    }
                    else
                    {
                        particles.FootSteps.LeftFootAnimationVar = EditorGUILayout.TextField("Left Foot Anim Param",
                            particles.FootSteps.LeftFootAnimationVar);
                        particles.FootSteps.RightFootAnimationVar = EditorGUILayout.TextField("Right Foot Anim Param",
                            particles.FootSteps.RightFootAnimationVar);
                    }
                    EditorGUIUtility.labelWidth = default;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndToggleGroup();
                }



                particles.UseClothingSound = EditorGUILayout.BeginToggleGroup("Use Clothes Sound",particles.UseClothingSound);
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 120;
                particles.ClothingSound = (AudioSource)EditorGUILayout.ObjectField("Clothing Sound Source", particles.ClothingSound, typeof(AudioSource),true);
                particles.ClothingSoundAnim = EditorGUILayout.TextField("Clothing Sound Animation", particles.ClothingSoundAnim);
                EditorGUIUtility.labelWidth = default;
                EditorGUILayout.EndHorizontal();
                particles.VolumeControl = EditorGUILayout.Slider("Clothing Volume", particles.VolumeControl, 0, 2);
                EditorGUILayout.EndToggleGroup();
            }
        }
    }
}

