using System.Collections;
using System.Collections.Generic;
using AnderSystems.GoodMotion.cam;
using UnityEngine;

public class MCS_PrefabSpawner : MonoBehaviour
{
    public float LodDistance = 60;
    public bool CanSpawn = true;
    public int Index { get; set; }
    public MCS_Managment Managment { get; set; }
    public int RandomProbability;

    public GameObject Model;

    [SerializeField]
    public _Prefabs PrefabHere; //Selected Prefab to Spawn

    [System.Serializable]
    public class _Prefabs //The Prefab Class
    {
        public GameObject Prefab; //Object
        [Range(0,100)]
        public int Probability = 100; //Probability to Spawn
    }

    public MCS_ScriptableOBj Template;

    [SerializeField]
    public List<_Prefabs> SelectedPrefabs()
    {
        List<_Prefabs> _selectedPrefabs = new List<_Prefabs>(Template.Prefabs);
        RandomProbability = Random.Range(0, 100);
        for (int i = 0; i < _selectedPrefabs.Count; i++)
        {
            if (_selectedPrefabs[i].Probability < RandomProbability)
            {
                _selectedPrefabs.RemoveAt(i);
            }
        }
        return _selectedPrefabs;
    }

    public void Generate(out _Prefabs Target)
    {
        List<_Prefabs> _selectedPrefabs = SelectedPrefabs();

        int RandomValue = Random.Range(0, _selectedPrefabs.Count);

        _Prefabs target = new _Prefabs();

        target.Probability = _selectedPrefabs[RandomValue].Probability;
        target.Prefab = Instantiate(_selectedPrefabs[RandomValue].Prefab, this.transform);
        Target = target;

        Target.Prefab.name = Target.Prefab.name.Replace("(Clone)", "");

        SetPrefabCorrectPosition();
    }

    public void SetPrefabCorrectPosition()
    {
        PrefabHere.Prefab.transform.localEulerAngles = Vector3.zero;
        PrefabHere.Prefab.transform.localPosition = new Vector3(PrefabHere.Prefab.GetComponent<BoxCollider>().size.x *
           PrefabHere.Prefab.transform.localScale.x, 0);
    }


    void Awake()
    {
        Managment = GetComponentInParent<MCS_Managment>();
        Managment.Spawners.Add(this);
        Index = Managment.Spawners.IndexOf(this);
    }

    void Start()
    {
        if (!CanSpawn)
            return;
        if(Managment.Spawners.Count <= Managment.Limit)
        {
            Generate(out PrefabHere);
        }

        Invoke("CheckDeadEnd", 1);
        //InvokeRepeating("DistanceEnable", 1,1);
    }

    public void InstantiateDeadEnd()
    {
        PrefabHere = new _Prefabs();
        PrefabHere.Prefab = Instantiate(Template.DeadEnd, this.transform);
        PrefabHere.Prefab.transform.localPosition = Vector3.zero;
        PrefabHere.Prefab.transform.localEulerAngles = Vector3.zero;
    }

    public void DistanceEnable()
    {
        if (PrefabHere.Prefab)
        {
            if (Vector3.Distance(GameplayCam.GetGameplayCam().transform.position, PrefabHere.Prefab.transform.position) <= LodDistance)
            {
                PrefabHere.Prefab.SetActive(true);
            }
            else
            {
                PrefabHere.Prefab.SetActive(false);
                PrefabHere.Prefab.transform.parent = Managment.transform;
            }
        }
    }

    void CheckDeadEnd()
    {
        if(PrefabHere.Prefab == null)
        {
            InstantiateDeadEnd();
            Debug.Log(gameObject.name + "(" + Index + ") Spawn DeadEnd");
        }
    }
}
