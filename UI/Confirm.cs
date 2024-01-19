using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmController : MonoBehaviour
{
    public Vector2 contentSize;
    public Vector2 cellSize = new Vector2(188f, 200f);
    public int cellCount = 10;

    public List<Sprite> commonImages;
    public AudioSource bgmAudioSource;
    public GameObject canvasObject;
    public GameObject backgroundImage;
    public static List<EnemyData> enemyDataList;
    public List<Sprite> keyImages;
    private void Start()
    {
        keyImages = DataHolder.keyImages;
        string[] substrings = this.gameObject.name.Split( "Enemy" );
        print(this.gameObject.name);
        enemyDataList = MainController.enemyDataList;

        commonImages = DataHolder.commonImages;
        SetupCanvas();
        SetupButtons();
    }

    private void SetupCanvas()
    {
        canvasObject = Utils.CreateCanvas("Confirm", true);
        Button button = canvasObject.AddComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        Vector2 backgroundPosition = new Vector2(0, 0f);

        backgroundImage = Utils.CreateImage(DataHolder.commonImages[((int)UICommon.Panel)], backgroundPosition, canvasObject.transform);
        // Create the audio source
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        // Other initialization code...
        canvasObject.SetActive(false);
    }

    private void SetupButtons()
    {
        Utils.CreateNavigationBar("確認", new Vector2(0f, 193f), canvasObject.transform, () =>
        {
            canvasObject.SetActive(false);
        });
        int rowCount = 1;
        int columnCount = 3;

        float totalWidth = columnCount * cellSize.x;
        float totalHeight = rowCount * cellSize.y;

        float startX = -(totalWidth / 2) + (cellSize.x / 2);
        float startY = totalHeight / 2 - (cellSize.y / 2) ;

        List<Sprite> icons = new List<Sprite>{
            keyImages[0],
            keyImages[0],
            keyImages[1],
            keyImages[2],
            keyImages[3]
        };
        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                int index = row * columnCount + column;

                float xPos = startX + column * cellSize.x;
                float yPos = startY - row * cellSize.y - 33;
            
                EnemyData enemyData = enemyDataList[index];
                List<string> stringList = new List<string>()
                {
                    enemyData.name,
                    enemyData.hp.ToString(),
                    enemyData.damage.ToString(),
                    enemyData.defend.ToString(),
                    enemyData.coin.ToString()
                };
                GameObject cell = Utils.CreateAvatarAndRows(keyImages[8], icons, stringList, new Vector2(xPos, yPos), canvasObject.transform);
            }
        }
    }

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}
