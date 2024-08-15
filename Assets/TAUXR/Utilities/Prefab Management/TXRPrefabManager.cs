using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TXRPrefabManager", menuName = "ScriptableObjects/TXRPrefabManager", order = 1)]
public class TXRPrefabManager : ScriptableObject
{
    public TXRPrefab[] Prefabs;
}

[Serializable]
public class TXRPrefab
{
    public GameObject Prefab;
    public string Name;
    public Texture2D Image;
}