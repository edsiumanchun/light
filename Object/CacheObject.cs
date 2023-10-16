using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "CacheObjectData", menuName = "Custom/CacheObjectData")]
public class CacheObjectData : ScriptableObject
{
    public int rowIndex;
    public int columnIndex;
    public string type;
    public string stage;
    public int floor;
}

public class CacheObjectController : MonoBehaviour
{
    public CacheObjectData cacheObjectData;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}