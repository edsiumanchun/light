
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
public class EventController : MonoBehaviour
{
    public static Vector3 StickToPlayer(Vector3 vector3) {

        GameObject player = GameObject.Find("Player");
        return new Vector3(player.transform.position.x - vector3.x, player.transform.position.y - vector3.y, vector3.z);
    }
    public static int ShowEvent(string name)
    { 
        switch(name)
        {
            case "ghostshow":
                Vector3 vector3 = StickToPlayer(new Vector3(-0.4f,0.4f,1f));
                GameObject gameObject = Utils.LoadPrefabAndInstantObject("Enemies", "Enemy0", vector3);
                Renderer renderer = gameObject.GetComponent<Renderer>();
                renderer.sortingOrder = -1;
                return 10;
        }
        return 0;
    }
}
