using UnityEngine;

public class AnimationEventProxy : MonoBehaviour
{
    private PlayerController playerController;

    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    public void OnAttackFire()
    {
        if (playerController != null)
        {
            playerController.OnAttackFire();
        }
    }
}
