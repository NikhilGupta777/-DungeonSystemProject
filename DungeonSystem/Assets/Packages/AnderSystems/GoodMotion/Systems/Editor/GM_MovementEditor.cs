using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GameSettings;
using AnderSystems.GoodMotion.Editor;

[CustomEditor(typeof(GM_Movement))]
public class GM_MovementEditor : Editor
{
    bool PhysicsFoldout = true;
    bool MovementFoldout = true;
    bool AnimationFoldout = true;
    bool Particles = true;
    bool CameraFoldout = true;
    //Call voids
    public override void OnInspectorGUI()
    {
        //Get Target
        GM_Movement Target = (GM_Movement)target;

        Target.MainSettings = EditorGUILayout.ObjectField("Main Settings", Target.MainSettings, typeof(GameConfigurations),true) as GameConfigurations;

        //Draw PhysicsGUI
        PhysicsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(PhysicsFoldout, "Physics Configuration");
        GMPhysicsGUI.DrawPhysicsGUI(Target.playerPhysics, PhysicsFoldout);
        EditorGUILayout.EndFoldoutHeaderGroup();

        //Draw PhysicsGUI
        MovementFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(MovementFoldout, "Movement Configuration");
        GMMovementGUI.DrawMovementGUI(Target.playerMovement,Target.GetComponent<Animator>(), MovementFoldout);
        EditorGUILayout.EndFoldoutHeaderGroup();

        AnimationFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(AnimationFoldout, "Animations Configuration");
        GMAnimationsGUI.DrawAnimationsGUI(Target.animationSetup, Target,AnimationFoldout);
        EditorGUILayout.EndFoldoutHeaderGroup();

        Particles = EditorGUILayout.BeginFoldoutHeaderGroup(Particles, "Polishing Configuration");
        GMParticles.DrawParticlesGUI(Target.SoundsAndParticles, Target, Particles);
        EditorGUILayout.EndFoldoutHeaderGroup();

        CameraFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(CameraFoldout, "Camera Configuration");
        GMCameraManagmentGUI.DrawCameraManagmentGUI(Target.GameplayCamParams, Target, CameraFoldout);
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    void OnSceneGUI()
    {
        //Get Target
        GM_Movement Target = (GM_Movement)target;

        //GM CustomEditor
        GMAnimationsGUI.GMDrawIKGizmos(Target.animationSetup, Target);
        GMPhysicsGUI.DrawGroundDetectionGizmos(Target.transform, Target.playerPhysics);
    }
}
