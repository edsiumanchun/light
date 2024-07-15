using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FireBallController : MonoBehaviour
{

    public int damage = 1; // The damage of the fireball
    public void Fire()
    {
        BoxCollider2D boxCollider2D = this.gameObject.AddComponent<BoxCollider2D>();
        boxCollider2D.size = new Vector2(0.1f, 0.1f);
        boxCollider2D.isTrigger = true;
        this.gameObject.AddComponent<RectTransform>();
        Utils.AddEffect(this.gameObject, "PurpleBall");
        GameObject gameObject = GameObject.Find("Player");
        transform.DOMove(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.3f, -3), 1);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // If the fireball hits the player, reduce their HP and destroy the fireball
        if (other.name.Contains("Player"))
        {
            CacheManager.ChangeHPMP(damage, true);
            Destroy(gameObject);
        }
    }
}