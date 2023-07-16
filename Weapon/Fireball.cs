using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public int damage = 1; // The damage of the fireball

    void OnTriggerEnter2D(Collider2D other)
    {
        // If the fireball hits the player, reduce their HP and destroy the fireball
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.TakeDamage(damage);
            Destroy(gameObject);
        }

        // If the fireball hits the floor or a wall, destroy the fireball
        if (other.CompareTag("Floor") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}