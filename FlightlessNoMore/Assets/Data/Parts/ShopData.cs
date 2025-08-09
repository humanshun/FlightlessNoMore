using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "Scriptable Objects/ShopData")]
public class ShopData : ScriptableObject
{
    public TypeBody typeBody;
    public TypeRocket typeRocket;
    public TypeWing typeWing;
    public TypeTire typeTire;
}
