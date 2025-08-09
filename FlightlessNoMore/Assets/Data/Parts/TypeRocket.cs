using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TypeRocket", menuName = "Scriptable Objects/TypeRocket")]
public class TypeRocket : ScriptableObject
{
    public List<PartData> rocketParts;
}
