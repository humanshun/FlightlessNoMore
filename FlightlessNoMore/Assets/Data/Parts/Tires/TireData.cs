using UnityEngine;

[CreateAssetMenu(fileName = "New Tire", menuName = "Parts/Tire")]
public class TireData : PartData
{
    [Header("空気抵抗")]
    [Tooltip("値が高いほど空気抵抗が大きくなります。")]
    public AirResistance airResistance;  // 抗力のプロパティ

    [Header("地上加速度")]
    [Tooltip("値が高いほど地上での加速が大きくなります。")]
    public Torque torque;  // トルク

    [System.Serializable]
    public class AirResistance
    {
        [HideInInspector] public string displayName = "空気抵抗";
        public float value;
    }

    [System.Serializable]
    public class Torque
    {
        [HideInInspector] public string displayName = "地上加速度";
        public float value;
    }
}
