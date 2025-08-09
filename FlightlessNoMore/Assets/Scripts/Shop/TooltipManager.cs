using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [SerializeField] private GameObject tooltipObject;
    [SerializeField] private TextMeshProUGUI tooltipText;

    private bool isShowing = false;
    private string currentMessage = "";

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void Show(string message, Vector3 position)
    {
        tooltipObject.SetActive(true);
        tooltipText.text = message;
        currentMessage = message;
        isShowing = true;
    }

    public void Hide()
    {
        tooltipObject.SetActive(false);
        isShowing = false;
    }

    private void Update()
    {
        if (isShowing)
        {
            Vector3 offset = new Vector3(200f, 100f, 0f); // マウスの右下
            tooltipObject.transform.position = Input.mousePosition + offset;
        }
    }
}
