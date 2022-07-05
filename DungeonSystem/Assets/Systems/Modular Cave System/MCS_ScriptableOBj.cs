using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModularDungeonTemplate", menuName = "Create new Modular Dungeon Templates")]
public class MCS_ScriptableOBj : ScriptableObject
{
    [SerializeField]
    public List<MCS_PrefabSpawner._Prefabs> Prefabs = new List<MCS_PrefabSpawner._Prefabs>(); //New prefab list
    public GameObject DeadEnd;
}
