using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class CustomParts : MonoBehaviour
{
    public GameObject CustomPartsImage;
    public Button bodyCustomButton;
    public Button wingCustomButton;
    public Button tireCustomButton;
    public Button rocketCustomButton;


    public GameObject bodyCustomImage;
    public GameObject wingCustomImage;
    public GameObject tireCustomImage;
    public GameObject rocketCustomImage;

    void Start()
    {
        bodyCustomButton.onClick.AddListener(BodyCustom);
        wingCustomButton.onClick.AddListener(WingCustom);
        tireCustomButton.onClick.AddListener(TireCustom);
        rocketCustomButton.onClick.AddListener(RocketCustom);
    }

    void Update()
    {
        
    }

    private void BodyCustom()
    {
        CustomPartsImage.SetActive(false);
        bodyCustomImage.SetActive(true);
    }
    public void WingCustom()
    {
        CustomPartsImage.SetActive(false);
        wingCustomImage.SetActive(true);
    }
    public void TireCustom()
    {
        CustomPartsImage.SetActive(false);
        tireCustomImage.SetActive(true);
    }
    public void RocketCustom()
    {
        CustomPartsImage.SetActive(false);
        rocketCustomImage.SetActive(true);
    }
}
