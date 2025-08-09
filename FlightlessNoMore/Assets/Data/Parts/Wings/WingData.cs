using UnityEngine;

[CreateAssetMenu(fileName = "New Wing", menuName = "Parts/Wing")]
public class WingData : PartData
{
    [Header("空気抵抗")]
    [Tooltip("値が高いほど空気抵抗が大きくなります。")]
    public AirResistance airResistance; //抗力のプロパティ

    [Header("空中制御")]
    [Tooltip("値が高いほど、空中での操作性が良くなります。")]
    public AirControl airControl; //空中制御

    [System.Serializable]
    public class AirResistance
    {
        [HideInInspector] public string displayName = "空気抵抗";
        public float value;
    }
    
    [System.Serializable]
    public class AirControl
    {
        [HideInInspector] public string displayName = "空中制御";
        public float value;
    }
}
