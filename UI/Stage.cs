using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{
    public Vector2 contentSize;
    public Vector2 cellSize = new Vector2(80f, 80f);
    public int cellCount = 10;

    public List<GameObject> buttons;

    public List<Sprite> commonImages;
    public List<Sprite> stageImages;

    public AudioSource bgmAudioSource;
    public GameObject canvasObject;
    public GameObject backgroundImage;

    private void Start()
    {
        buttons = new List<GameObject>();
        commonImages = DataHolder.commonImages;
        stageImages = DataHolder.stageImages;
        SetupCanvas();
        SetupButtons();
    }

    private void SetupCanvas()
    {
        canvasObject = Utils.CreateCanvas("Stage", true);
        Button button = canvasObject.AddComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        Vector2 backgroundPosition = new Vector2(0, 0f);

        backgroundImage = Utils.CreateImage(DataHolder.stageImages[0], backgroundPosition, canvasObject.transform);
        // Create the audio source
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        // Other initialization code...
        canvasObject.SetActive(false);
    }

    private void SetupButtons()
    {
        Utils.CreateNavigationBar("樓層", new Vector2(0f, 193f), canvasObject.transform, () =>
        {
            canvasObject.SetActive(false);
        });
        Utils.CreateSelectionBar("abc", new Vector2(0f, -168f), canvasObject.transform, () =>
        {
        }, () =>
        {
        });
        int rowCount = 3;
        int columnCount = 5;

        float totalWidth = columnCount * cellSize.x;
        float totalHeight = rowCount * cellSize.y;

        float startX = -(totalWidth / 2) + (cellSize.x / 2);
        float startY = totalHeight / 2 - (cellSize.y / 2) + 20;

        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                int index = row * columnCount + column;

                float xPos = startX + column * cellSize.x;
                float yPos = startY - row * cellSize.y;

                GameObject cell = Utils.CreateTextButton(commonImages[((int)UICommon.CommonButton)], new Vector2(xPos, yPos), new Vector2(0, 0), canvasObject.transform, () =>
                {
                    CacheManager.SetCache( (1 + Cache.CurrentFloor).ToString(), index);
                    MainController.LoadFloorAssetsFromJson(index, "HomeScene");
                });

                cell.name = index.ToString();
                cell.GetComponent<Text>().text = index.ToString();
                buttons.Add(cell);
            }
        }
    }

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}
