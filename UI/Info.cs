using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoController : MonoBehaviour
{
    public Vector2 contentSize;
    public Vector2 cellSize = new Vector2(188f, 200f);
    public int cellCount = 10;

    public List<GameObject> gameObjects;

    public List<Sprite> commonImages;
    public AudioSource bgmAudioSource;
    public GameObject canvasObject;
    public GameObject backgroundImage;
    public List<EnemyData> enemyDataList;
    public List<Sprite> avatars;
    public List<Sprite> keyImages;
    private void Start()
    {
        keyImages = DataHolder.keyImages;

        gameObjects = new List<GameObject>();
        commonImages = DataHolder.commonImages;
        SetupCanvas();
    }

    private void SetupCanvas()
    {
        canvasObject = Utils.CreateCanvas("Info", true);
        Button button = canvasObject.AddComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        Vector2 backgroundPosition = new Vector2(0, 0f);

        backgroundImage = Utils.CreateImage(DataHolder.commonImages[((int)UICommon.Panel)], backgroundPosition, canvasObject.transform);
        // Create the audio source
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        // Other initialization code...
        canvasObject.SetActive(false);

        Utils.CreateNavigationBar("怪物資訊", new Vector2(0f, 193f), canvasObject.transform, () =>
        {
            canvasObject.SetActive(false);
        });
    }

    public void SetupButtons()
    {
        foreach(GameObject gameObject in gameObjects){
            Destroy(gameObject);
        }
        int rowCount = 1;
        int columnCount = avatars.Count;

        float totalWidth = columnCount * cellSize.x;
        float totalHeight = rowCount * cellSize.y;

        float startX = -(totalWidth / 2) + (cellSize.x / 2);
        float startY = totalHeight / 2 - (cellSize.y / 2) ;

        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                List<Sprite> icons = new List<Sprite>{
                    avatars[row],
                    keyImages[9],
                    keyImages[10]
                };
                int index = row * columnCount + column;

                float xPos = startX + column * cellSize.x;
                float yPos = startY - row * cellSize.y - 33;
                Debug.Log(index + "????" + xPos + "???");
                EnemyData enemyData = enemyDataList[index];
                List<string> stringList = new List<string>()
                {
                    enemyData.name.Replace("Boss", "").Replace("Enemy", ""),
                    enemyData.hp.ToString(),
                    enemyData.damage.ToString(),
                };
                GameObject cell = Utils.CreateAvatarAndRows(keyImages[7], icons, stringList, new Vector2(xPos, yPos), canvasObject.transform);
                cell.name = index.ToString();
                gameObjects.Add(cell);
                cell.transform.localScale = new Vector3(1,1,1);
            }
        }
    }

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}
