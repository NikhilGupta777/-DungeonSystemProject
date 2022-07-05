using System.Collections;
using System.Collections.Generic;
using AnderSystems;
using AnderSystems.GoodMotion;
using UnityEngine;

public class NoClip : MonoBehaviour
{
    public Transform GameplayCam;
    public GameObject NoClipCam;
    public GameObject NoClipTarget;
    public bool DisablePlayerController = true;
    public bool FreezeTime = true;

    static GameObject InstantiatedNoClipCam;
    static float NoClipSpeed = 0.5f;

    public static void StartNoClip(GameObject noClipCam, Vector3 pos, Quaternion rot)
    {
        InstantiatedNoClipCam = Instantiate(noClipCam, pos, rot);
    }
    public static void StopNoClip()
    {
        Destroy(InstantiatedNoClipCam.gameObject);
        InstantiatedNoClipCam = null;
        Time.timeScale = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!InstantiatedNoClipCam)
            {
                StartNoClip(NoClipCam, GameplayCam.position, GameplayCam.rotation);
                if (DisablePlayerController)
                {
                    PlayerPhysics.FreezePlayer(Utility.GetLocalPlayer());
                }

                if (FreezeTime)
                {
                    Time.timeScale = 0.001f;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopNoClip();
            if (DisablePlayerController)
            {
                PlayerPhysics.UnFreezePlayer(Utility.GetLocalPlayer());
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            NoClipTarget.transform.position = InstantiatedNoClipCam.transform.position;
            NoClipTarget.transform.rotation = InstantiatedNoClipCam.transform.rotation;
            StopNoClip();
            if (DisablePlayerController)
            {
                PlayerPhysics.UnFreezePlayer(Utility.GetLocalPlayer());
            }
        }

        if (InstantiatedNoClipCam != null)
        {
            if (Input.GetKey(KeyCode.W))
            {
                InstantiatedNoClipCam.transform.Translate(Vector3.forward * NoClipSpeed, Space.Self);
            }

            if (Input.GetKey(KeyCode.S))
            {
                InstantiatedNoClipCam.transform.Translate(-Vector3.forward * NoClipSpeed, Space.Self);
            }

            if (Input.GetKey(KeyCode.A))
            {
                InstantiatedNoClipCam.transform.Translate(-Vector3.right * NoClipSpeed, Space.Self);
            }

            if (Input.GetKey(KeyCode.D))
            {
                InstantiatedNoClipCam.transform.Translate(Vector3.right * NoClipSpeed, Space.Self);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                InstantiatedNoClipCam.transform.Translate(Vector3.down * NoClipSpeed, Space.Self);
            }

            if (Input.GetKey(KeyCode.E))
            {
                InstantiatedNoClipCam.transform.Translate(Vector3.up * NoClipSpeed, Space.Self);
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                InstantiatedNoClipCam.transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
            }


            if (Input.GetKey(KeyCode.LeftShift))
            {
                NoClipSpeed += (0.05f);
            } else
            {
                NoClipSpeed = 0.1f;
            }


        }
    }
}
