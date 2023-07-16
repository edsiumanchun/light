using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Handle collision with the player here
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.isOnFloor = true;
            }
        }
        else if (other.CompareTag("Boss"))
        {
            // Handle collision with the boss here
            BossController bossController = other.GetComponent<BossController>();
            if (bossController != null)
            {
                // bossController.SetIsGrounded(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Handle exit collision with the player here
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.isOnFloor = false;
            }
        }
        else if (other.CompareTag("Boss"))
        {
            // Handle exit collision with the boss here
            BossController bossController = other.GetComponent<BossController>();
            if (bossController != null)
            {
                // bossController.SetIsGrounded(false);
            }
        }
    }
}