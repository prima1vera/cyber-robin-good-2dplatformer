using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullHealthPickup : MonoBehaviour
{
    public GameObject pickupEffect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            playerHealth.ReceiveHealing(playerHealth.maximumHealth);
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }
            Destroy(this.gameObject);
        }
    }
}
