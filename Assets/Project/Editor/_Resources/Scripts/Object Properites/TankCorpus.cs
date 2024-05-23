using UnityEngine;

public class TankCorpus : MonoBehaviour
{
    public UIController UIController;
    public TankController tankController;

    private void Start()
    {
        UIController = GameObject.Find("UIController").GetComponent<UIController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        PlayerController playerController = collision.gameObject.GetComponentInParent<PlayerController>();
        if (playerController != null)
        {
            playerController.ReceiveDamage(tankController.selection.player, collision.gameObject, collision,
                playerController.selection.maxHealth, (Input.GetKey(KeyCode.LeftControl) || UIController.isActiveManualControl) && tankController.IsThisCurrentChar());
        }
    }
}
