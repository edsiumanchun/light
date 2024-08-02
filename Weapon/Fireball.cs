// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using DG.Tweening;

// public class FireBallController : MonoBehaviour
// {
//     public int totalHP = 0;
//     public int totalMP = 0;
//     public void Fire(string type)
//     {
//         BoxCollider2D boxCollider2D = this.gameObject.AddComponent<BoxCollider2D>();
//         boxCollider2D.size = new Vector2(0.03f, 0.03f);
//         boxCollider2D.isTrigger = true;
//         this.gameObject.AddComponent<RectTransform>();
//         SpriteRenderer spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
//         int i = type == "Greed" ? 0 : type == "Hatred" ? 1 : 2 ;
//         // spriteRenderer.sprite = DataHolder.bullets[i];
//         Effects.AddEffect(this.gameObject, "Stone/Electric" + type);
//         GameObject gameObject = GameObject.Find("Player");
//         transform.DOMove(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.3f, -3), 1f);
//     }
//     void OnTriggerEnter2D(Collider2D other)
//     {
//         // If the fireball hits the player, reduce their HP and destroy the fireball
//         if (other.name.Contains("Player"))
//         {
//             if(totalHP != 0 || totalMP != 0){
//                 CacheManager.ChangeHPWithMP(-1 * totalHP, -1 * totalMP);
//             }
//             Destroy(gameObject);
//         }
//     }
// }