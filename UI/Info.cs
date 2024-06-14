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

    public GameObject rowTextObject;
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

        Utils.CreateNavigationBar("情報", new Vector2(0f, 198f), canvasObject.transform, () =>
        {
            canvasObject.SetActive(false);
        });
    }

    public string ProcessString(string name, string input)
    {
        if (name.Contains("Silly"))
            return "silly" + " (x" + input + ")";
        else if (name.Contains("Greed"))
            return "greed" + " (x" + input + ")";
        else if (name.Contains("Angry"))
            return "greed" + " (x" + input + ")";
        else if (name.Contains("Boss"))
            return "hp max" + " (+" + input + ")";
        else if (name.Contains("Enemy"))
            return "mp max" + " (+" + input + ")";
        else
            return input;
    }

    public void SetupButtons()
    {
        foreach(GameObject gameObject in gameObjects){
            Destroy(gameObject);
        }
        if(rowTextObject != null) {
            Destroy(rowTextObject);
        }
        int rowCount = 1;
        int columnCount = avatars.Count;

        float totalWidth = columnCount * cellSize.x;
        float totalHeight = rowCount * cellSize.y;

        float startX = -(totalWidth / 2) + (cellSize.x / 2);
        float startY = totalHeight / 2 - (cellSize.y / 2) ;
        string reward = "獎勵\n";
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
                float yPos = startY - row * cellSize.y + 33;
                EnemyData enemyData = enemyDataList[index];
                List<string> stringList = new List<string>()
                {
                    enemyData.name.Replace("Boss", "").Replace("Enemy", ""),
                    enemyData.hp.ToString(),
                    enemyData.damage.ToString(),
                };
                reward = reward + ProcessString(enemyData.name, enemyData.reward.ToString());
                GameObject cell = Utils.CreateAvatarAndRows(keyImages[7], icons, stringList, new Vector2(xPos, yPos), canvasObject.transform);
                cell.name = index.ToString();
                gameObjects.Add(cell);
                cell.transform.localScale = new Vector3(1,1,1);

                Button button = cell.AddComponent<Button>();
                button.onClick.AddListener(() => OnClickButtonWithParameter(index));
            }
        }
        rowTextObject = Utils.CreateText(new Vector2(300, 100), new Vector2(0, -178), canvasObject.transform, TextAnchor.MiddleCenter);
        rowTextObject.GetComponent<Text>().text = reward;
        rowTextObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void OnClickButtonWithParameter(int index)
    {
        switch (index)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
        Debug.Log("132493" + index);
        StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
        storyController.LoadStoryDataFromText("體力不足，無法作戰。");
    }

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}
