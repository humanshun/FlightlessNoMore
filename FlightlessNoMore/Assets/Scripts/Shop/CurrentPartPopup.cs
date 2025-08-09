using UnityEngine;
using UnityEngine.UI;

public class CurrentPartPopup : MonoBehaviour
{
    [SerializeField] private Image image;

    public void Setup(PartData part)
    {
        image.sprite = part.partIconImage;
        image.preserveAspect = true; // アスペクト比を保持する
    }
}
