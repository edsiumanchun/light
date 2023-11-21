using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "CacheObjectData", menuName = "Custom/CacheObjectData")]
public class CacheObjectData : ScriptableObject
{
    public int rowIndex;
    public int columnIndex;
    public string type;
    public string stage;
}

public class CacheObjectController : MonoBehaviour
{
    public CacheObjectData cacheObjectData;
    string oldTag = "";
    private void Start()
    {
        //if(GetComponent<Rigidbody2D>() == null){
        //    return;
        //}

        //if(GetComponent<Collider2D>() == null){
        //    return;
        //}
        // Set the collision detection mode to ContinuousDynamic to prevent tunneling
        //GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.ContinuousDynamic;

        // Set the collider material's bounciness and friction to desired values
    }

    public void PurifyCacheData()
    {
        //CacheController.SetObjectCache(cacheObjectData.stage, cacheObjectData);
    }
}