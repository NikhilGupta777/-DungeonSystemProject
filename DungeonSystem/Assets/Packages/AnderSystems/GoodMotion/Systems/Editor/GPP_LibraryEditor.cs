using System.Collections;
using System.Collections.Generic;
using AnderSystems.GoodMotion.cam;
using AnderSystems.GoodMotion;
using AnderSystems.GoodMotion.Gpp;
using GameSettings;
using AnderSystems;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GPP_Base))]
public class GPP_LibraryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        GPP_Base Target = (GPP_Base)target;

        if (Target.GoodPhysicsConfiguration.Colliders.Count <= 0)
        {
            Target.GenerateBones = GUILayout.Button("Generate Bones Colliders");
        } else
        {
            Target.GenerateBones = GUILayout.Button("Delete Bones Colliders");
            base.OnInspectorGUI();
        }

        if (!Target.GoodPhysicsConfiguration.anim)
        {
            Target.GoodPhysicsConfiguration.rb = Target.GetComponent<Rigidbody>();
            Target.GoodPhysicsConfiguration.anim = Target.GetComponent<Animator>();
        }

        for (int i = 0; i < Target.GoodPhysicsConfiguration.Colliders.Count; i++)
        {
            Target.GoodPhysicsConfiguration.Colliders[i].material = Target.GoodPhysicsConfiguration.BodyMaterial;
        }
        
        GenerateBones(Target);
    }

    void GenerateBones(GPP_Base Target)
    {
        if (Target.GenerateBones)
        {
            if (Target.GoodPhysicsConfiguration.Colliders.Count > 0)
            {
                if (EditorUtility.DisplayDialog("Delete Bones Colliders?", "This action will delete current colliders on bones only.", "Ok", "Cancel"))
                {
                    for (int i = 0; i < Target.GoodPhysicsConfiguration.Colliders.Count; i++)
                    {
                        DestroyImmediate(Target.GoodPhysicsConfiguration.Colliders[i].GetComponent<CharacterJoint>());
                        DestroyImmediate(Target.GoodPhysicsConfiguration.Colliders[i].GetComponent<Rigidbody>());
                        DestroyImmediate(Target.GoodPhysicsConfiguration.Colliders[i]);
                    }
                    Target.GoodPhysicsConfiguration.Colliders.Clear();
                }

            } else
            {
                if (!Target.GoodPhysicsConfiguration.anim.avatar)
                {
                    EditorUtility.DisplayDialog("Bone Colliders Creation Failure!", "You animator not have avatar", "Ok");
                } else
                {
                    if (!Target.GoodPhysicsConfiguration.anim.avatar.isHuman)
                    {
                        EditorUtility.DisplayDialog("Bone Colliders Creation Failure!", "You animator is not a human type", "Ok");
                    } else
                    {
                        if (EditorUtility.DisplayDialog("Good Player Physics.", "Generate Bones Colliders?", "Yes", "No"))
                        {
                            Target.CreateBones();
                            EditorUtility.DisplayDialog("Good Player Physics.", Target.GoodPhysicsConfiguration.Colliders.Count + " " +
                                "bones has generated!", "Ok");
                            for (int i = 0; i < Target.GoodPhysicsConfiguration.Colliders.Count; i++)
                            {
                                Target.GoodPhysicsConfiguration.Colliders[i].isTrigger = true;
                            }
                        }
                    }
                }
            }

            Target.GenerateBones = false;
        }
    }
}
