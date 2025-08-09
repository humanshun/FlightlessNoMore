using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TypeWing", menuName = "Scriptable Objects/TypeWing")]
public class TypeWing : ScriptableObject
{
    public List<PartData> wingParts;
}
