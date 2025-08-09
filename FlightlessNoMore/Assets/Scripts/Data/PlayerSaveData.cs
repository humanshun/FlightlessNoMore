using System.Collections.Generic;

[System.Serializable]

public class PlayerSaveData
{
    public int coins;
    public List<string> purchasedPartNames = new List<string>();
    public List<PartTypePartPair> currentParts;

    public float maxDistance;
    public float maxAltitude;
}

[System.Serializable]
public class PartTypePartPair
{
    public PartType partType;
    public string partName;
}