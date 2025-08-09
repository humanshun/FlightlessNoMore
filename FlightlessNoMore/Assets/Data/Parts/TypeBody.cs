using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TypeBody", menuName = "Scriptable Objects/TypeBody")]
public class TypeBody : ScriptableObject
{
    public List<PartData> bodyParts;
}
