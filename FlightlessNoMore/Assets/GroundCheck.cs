using System.Collections;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider; // BoxCollider2Dの参照

     void Start()
    {
        PlayerController2 playerController = GetComponentInParent<PlayerController2>();
        if (playerController != null)
        {
            playerController.groundCheckCollider = boxCollider; // PlayerController2にBoxCollider2Dを設定
        }
    }
}
