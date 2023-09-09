using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class AssetsData
{
    public List<AssetData> assets;
}

[System.Serializable]
public class AssetData
{
    public string name; // Name of the asset
    public string path; // Path to the asset
    public List<Vector2> positions; // List of positions for the asset

    [System.NonSerialized]
    public object asset; // The loaded asset

    public List<GameObject> CreateObjects(bool includeCollider, GameObject parent)
    {
        List<GameObject> objects = new List<GameObject>();

        int i = 0;
        foreach (Vector2 position in positions)
        {
            // Create a new empty GameObject
            GameObject newObject = new GameObject(name);

            // Add a SpriteRenderer component and set the sprite
            SpriteRenderer spriteRenderer = newObject.AddComponent<SpriteRenderer>();

            // Use the loaded asset as the sprite
            spriteRenderer.sprite = asset as Sprite;

            // Add a collider if includeCollider is true
            if (includeCollider)
            {
                newObject.AddComponent<BoxCollider2D>();
            }

            else
            {
                newObject.transform.rotation = Quaternion.identity;
            }
            // Set the parent of the GameObject (optional)
            if(parent != null) {
                newObject.transform.SetParent(parent.transform);   
            }
            objects.Add(newObject);

            // position
            newObject.transform.position = new Vector3(position.x, position.y, name == "background" ? 0 : -1);
            if (name == "background") {
                DataHolder.background = newObject;
            } else if (name == "player")
            {
                DataHolder.player = newObject;
            } else if(name == "wall" || name == "door"){
                SpriteRenderer backgroundSprite = DataHolder.background.GetComponent<SpriteRenderer>();
                spriteRenderer.drawMode = SpriteDrawMode.Tiled;
                float width = backgroundSprite.sprite.bounds.size.x;
                float height = backgroundSprite.sprite.bounds.size.y;
                float screenWidth = Camera.main.orthographicSize * 2f * width / height;
                float screenHeight = Camera.main.orthographicSize * 2f;


// for door, I want it to have switch 9 case by vector position
//(-1,1),(0,1) this will rotate,(1,1), 
//(-1,0),(1,0), 
//(-1,-1),(0,-1),(1,-1).

//(-1,1), (-1,-1), (1,1) and (1,-1)

                if (name == "door")
                {
                    DataHolder.doors = new List<GameObject>();
                    float doorOffsetY = 0f;
                    float doorOffsetX = 0f;
                    if (position == new Vector2(1, -1) || position == new Vector2(-1, -1))
                    {
                        doorOffsetY = -1 * spriteRenderer.sprite.bounds.size.y / 2f;
                    } else if (position == new Vector2(1, 1) || position == new Vector2(-1, 1)){
                        doorOffsetY = spriteRenderer.sprite.bounds.size.y / 2f;
                    }
                    if (position == new Vector2(0, 1) || position == new Vector2(0, -1)) { 
                        newObject.transform.Rotate(0f, 0f, 90f);
                        doorOffsetY += spriteRenderer.sprite.bounds.size.x / 2f * position.y;
                    } else {
                        doorOffsetX = -1 * spriteRenderer.sprite.bounds.size.x / 2f * position.x;
                    }
                    // Set the position
                    newObject.transform.position = new Vector3(screenWidth / 2f * position.x + doorOffsetX, (screenHeight / 2f + doorOffsetY) * position.y, -3);
                    DataHolder.doors.Add(newObject);
                }
                else
                {
                    SpriteRenderer doorSpriteRenderer = DataHolder.doors[0].GetComponent<SpriteRenderer>();
                    float wallThickness = doorSpriteRenderer.bounds.size.x;
                    float offset = spriteRenderer.size.y / 2　+ wallThickness / 2f;
                    switch (i)
                    {
                        case 3:
                            // Create a hole at the left bottom with the default size
                            newObject.transform.position = new Vector3(-screenWidth / 2f - offset, 0, -1);
                            spriteRenderer.size = new Vector2(spriteRenderer.size.x, height);
                            break;
                        case 1:
                            // Default behavior for index 1
                            newObject.transform.position = new Vector3(screenWidth / 2f + offset, 0f, -1);
                            spriteRenderer.size = new Vector2(spriteRenderer.size.x, height);
                            break;
                        case 0:
                            // Default behavior for index 0
                            newObject.transform.position = new Vector3(0f, -screenHeight / 2f - offset, -1);
                            spriteRenderer.size = new Vector2(width, spriteRenderer.size.y);
                            break;
                        case 2:
                            // Default behavior for index 2
                            newObject.transform.position = new Vector3(0f, screenHeight / 2f + offset, -1);
                            spriteRenderer.size = new Vector2(width, spriteRenderer.size.y);
                            break;
                        default:
                            // code block
                            break;
                    }
                }
            }
            i++;
        }

        return objects;
    }
}