using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicOcclusionCulling : MonoBehaviour
{
    public Camera cam;
    [System.Serializable]
    public class cullingRays
    {
        public Vector2 StartPos = new Vector2(.5f,.5f);
        public LayerMask BlockLayers;
        public float Size = 3;
        public float Distance = 800;
    }
    [SerializeField]
    public cullingRays[] CullingRays;

    public List<CullingTarget> AllCollidersOnView = new List<CullingTarget>();
    public List<CullingTarget> AllCollidersOutOfView = new List<CullingTarget>();

    void LateUpdate()
    {
        AllCollidersOnView.Clear();
        AllCollidersOutOfView = FindObjectsOfType<CullingTarget>().ToList();

        for (int r = 0; r < CullingRays.Length; r++)
        {
            RaycastHit hit;
            Vector3 EndPos = cam.ViewportPointToRay(CullingRays[r].StartPos).GetPoint(CullingRays[r].Distance);
            if (Physics.Linecast(cam.transform.position, EndPos, out hit, CullingRays[r].BlockLayers))
            {
                Debug.DrawLine(cam.transform.position, hit.point);
                Collider[] colliders = Physics.OverlapCapsule(cam.transform.position, hit.point, CullingRays[r].Size);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].GetComponent<CullingTarget>())
                    {
                        AllCollidersOnView.Add(colliders[i].GetComponent<CullingTarget>());
                    }
                }
            } else
            {
                Collider[] colliders = Physics.OverlapCapsule(cam.transform.position, EndPos, CullingRays[r].Size);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].GetComponent<CullingTarget>())
                    {
                        AllCollidersOnView.Add(colliders[i].GetComponent<CullingTarget>());
                    }
                }
            }
        }

        for (int i = 0; i < AllCollidersOnView.Count; i++)
        {
            AllCollidersOutOfView.Remove(AllCollidersOnView[i]);
            AllCollidersOnView[i].EnableModel(true);
        }

        for (int i = 0; i < AllCollidersOutOfView.Count; i++)
        {
            AllCollidersOutOfView[i].EnableModel(false);
        }
    }
}
