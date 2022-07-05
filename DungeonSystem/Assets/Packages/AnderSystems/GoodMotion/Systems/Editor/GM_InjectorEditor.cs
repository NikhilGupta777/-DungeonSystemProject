using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GM_Injector))]
public class GM_InjectorEditor : EditorWindow
{
    [MenuItem("GameObject/Create Other/AnderSystems/Create GoodMotion System")]
    static void Create()
    {
        ShowGMInjectorWindow();
    }

    public static void ShowGMInjectorWindow()
    {
        GetWindow<GM_InjectorEditor>("Create new Good Motion Player");
    }

    public GM_Movement PlayerMovement;
    GameObject PlayerPrefab;
    void OnGUI()
    {
        Texture GM_Logo = EditorGUIUtility.FindTexture("Assets/Packages/AnderSystems/GoodMotion/Systems/Editor/EditorImages/GMLogo.png");
        GUILayout.Box(GM_Logo);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Required Components:", MessageType.None);
        PlayerPrefab = (GameObject)EditorGUILayout.ObjectField("Select Character Prefab: ", PlayerPrefab, typeof(GameObject),true);
        if (!PlayerPrefab)
        {
            EditorGUILayout.HelpBox("You need to select a character Prefab to Continue!", MessageType.Info);
        }

        if (PlayerMovement)
        {
            AvatarLabel();
        }

        if (PlayerPrefab)
        {
            if (!PlayerMovement)
            {
                if (GUILayout.Button("Continue"))
                {
                    if (EditorUtility.DisplayDialog("Confirm Character", "Create GoodMotion Systems using ''" + PlayerPrefab.name + "''", "Yes", "No"))
                    {
                        PlayerMovement = Instantiate(PlayerPrefab.gameObject).AddComponent<GM_Movement>();
                        PlayerMovement.gameObject.AddComponent<Animator>();
                        PlayerMovement.gameObject.AddComponent<Rigidbody>();
                        PlayerMovement.gameObject.AddComponent<CapsuleCollider>();
                        PlayerMovement.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath("Assets/Packages/AnderSystems/Utility/New Animator Controller.controller", typeof(RuntimeAnimatorController));
                    }
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Cancel"))
                {
                    if (!EditorUtility.DisplayDialog("Exit from Good Motion Character Createion?", "You will lost all changes make here", "Back", "Exit"))
                    {
                        Close();
                    }
                }
            }
            else
            {
                if (PlayerAvatar)
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Done"))
                    {
                        if (AddGmJump)
                        {
                            GM_Jump Jump = PlayerMovement.gameObject.AddComponent<GM_Jump>();
                            Jump.anim = PlayerMovement.GetComponent<Animator>();
                            Jump.GM = PlayerMovement.GetComponent<GM_Movement>();
                            Jump.rb = PlayerMovement.GetComponent<Rigidbody>();
                        }

                        if (AddGPP)
                        {
                            GM_Jump GPP = PlayerMovement.gameObject.AddComponent<GM_Jump>();
                            GPP.anim = PlayerMovement.GetComponent<Animator>();
                            GPP.GM = PlayerMovement.GetComponent<GM_Movement>();
                            GPP.rb = PlayerMovement.GetComponent<Rigidbody>();
                        }

                        if (CreateCameraRig)
                        {
                            CreateCameras();
                        }

                        if (EditorUtility.DisplayDialog("Your character has been created!", "Don't forget to set it up correctly." + "\n" +
    "You can use the template provided by GoodMotion as a reference.", "Done"))
                        {
                            Close();
                        }
                    }
                } else
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Cancel"))
                    {
                        if (!EditorUtility.DisplayDialog("Exit from Good Motion Character Createion?", "You will lost all changes make here", "Back", "Exit"))
                        {
                            Close();
                        }
                    }
                }
            }
        } else
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel"))
            {
                if(!EditorUtility.DisplayDialog("Exit from Good Motion Character Createion?", "You will lost all changes make here", "Back", "Exit"))
                {
                    Close();
                }
            }
        }
    }

    Avatar PlayerAvatar;
    void AvatarLabel()
    {
        PlayerAvatar = (Avatar)EditorGUILayout.ObjectField("Select Player Avatar: ", PlayerAvatar, typeof(Avatar), true);
        if (!PlayerAvatar)
        {
            EditorGUILayout.HelpBox("Now, you need to set up Player Avatar", MessageType.Info);
        } else
        {
            if (!PlayerAvatar.isHuman)
            {
                EditorGUILayout.HelpBox("Your Player avatar needs to be human", MessageType.Warning);
            } else
            {
                ExtraComponents();
            }
        }
    }
    bool AddGmJump, AddGPP, CreateCameraRig;
    void ExtraComponents()
    {
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Extra Scripts", MessageType.None);
        AddGmJump = EditorGUILayout.Toggle("Include JumpSystem", AddGmJump);
        AddGPP = EditorGUILayout.Toggle("Include GoodPlayerPhysics", AddGPP);
        CreateCameraRig = EditorGUILayout.Toggle("Create Camera Rig", CreateCameraRig);

        if(AddGmJump && !AddGPP)
        {
            EditorGUILayout.HelpBox("You need to configure ''GM_Jump'' on inspector", MessageType.Info);
        }

        if (!AddGmJump && AddGPP)
        {
            EditorGUILayout.HelpBox("You need to configure ''GPP_Base'' on inspector", MessageType.Info);
        }

        if (AddGmJump && AddGPP)
        {
            EditorGUILayout.HelpBox("You need to configure ''GPP_Base'' and ''GM_Jump'' on inspector", MessageType.Info);
        }

        if (CreateCameraRig)
        {
            EditorGUILayout.HelpBox("You need to configure the camera positions", MessageType.Info);
        }
    }

    Transform Orbit;
    Transform OrbitCenter;
    Camera Cam0;
    Camera Cam1;
    Camera Cam2;
    Camera Cam3;
    Camera Cam4;
    void CreateCameras()
    {
        PlayerMovement.GameplayCamParams = new AnderSystems.GoodMotion.cam.GameplayCam.Params();
        Orbit = Instantiate(new GameObject()).transform;
        OrbitCenter = Instantiate(new GameObject()).transform;
        OrbitCenter.transform.position = PlayerMovement.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head).position;
        OrbitCenter.transform.rotation = PlayerMovement.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head).rotation;
        Cam0 = Instantiate(new GameObject()).AddComponent<Camera>();
        Cam1 = Instantiate(new GameObject()).AddComponent<Camera>();
        Cam2 = Instantiate(new GameObject()).AddComponent<Camera>();
        Cam3 = Instantiate(new GameObject()).AddComponent<Camera>();
        Cam4 = Instantiate(new GameObject()).AddComponent<Camera>();

        Cam0.transform.parent = Orbit;
        Cam1.transform.parent = Orbit;
        Cam2.transform.parent = Orbit;
        Cam3.transform.parent = Orbit;
        Cam4.transform.parent = Orbit;

        Cam1.enabled = false;
        Cam2.enabled = false;
        Cam3.enabled = false;
        Cam4.enabled = false;

        Orbit.name = "Orbit";
        OrbitCenter.name = "OrbitCenter";

        Cam0.name = "GameplayCam";
        Cam1.name = "IdleCam";
        Cam2.name = "SprintCam";
        Cam3.name = "RunCam";
        Cam4.name = "AimCam";

        Orbit.transform.parent = PlayerMovement.transform;
        OrbitCenter.transform.parent = PlayerMovement.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);

        PlayerMovement.GameplayCamParams.GameplayCam = Cam0;
        PlayerMovement.GameplayCamParams.IdleCam = Cam1;
        PlayerMovement.GameplayCamParams.IdleLerp = 1;
        PlayerMovement.GameplayCamParams.SprintCam = Cam2;
        PlayerMovement.GameplayCamParams.SprintLerp = 1;
        PlayerMovement.GameplayCamParams.RunningCam = Cam3;
        PlayerMovement.GameplayCamParams.RunningLerp = 3;
        PlayerMovement.GameplayCamParams.AimCam = Cam4;

        PlayerMovement.GameplayCamParams.Orbit = Orbit;
        PlayerMovement.GameplayCamParams.OrbitCenter = OrbitCenter;

    }
}

