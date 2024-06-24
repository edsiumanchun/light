using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : PageIndexController
{
    public List<Sprite> commonImages;
    public AudioSource bgmAudioSource;
    public GameObject backgroundImage;

    public List<ItemData> itemDataList;
    public List<Sprite> keyImages;
    public void Start()
    {
        folderName = "UI/Items";
        itemDataList = DataHolder.itemDataList;
        totalItems = itemDataList.Count;
        SetupCanvas();
        base.Start();
        keyImages = DataHolder.keyImages;
        commonImages = DataHolder.commonImages;
    }

    private void SetupCanvas()
    {
        canvasObject = Utils.CreateCanvas("Item", true);
        Button button = canvasObject.AddComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        Vector2 backgroundPosition = new Vector2(0, 0f);

        backgroundImage = Utils.CreateImage(DataHolder.commonImages[((int)UICommon.Panel)], backgroundPosition, canvasObject.transform);
        // Create the audio source
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        // Other initialization code...
        canvasObject.SetActive(false);

        Utils.CreateNavigationBar("背包", new Vector2(0f, 198f), canvasObject.transform, () =>
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

    public override void DisplayPage()
    {
        base.DisplayPage();
        int rowCount = 1;
        int columnCount = GetItemsLeft();

        float totalWidth = columnCount * cellSize.x;
        float totalHeight = rowCount * cellSize.y;

        float startX = -(totalWidth / 2) + (cellSize.x / 2);
        float startY = totalHeight / 2 - (cellSize.y / 2) ;
        string reward = "獎勵\n";
        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                int index = currentPage * itemsPerPage + row * columnCount + column;
                ItemData itemData = itemDataList[index];
                List<Sprite> icons = new List<Sprite>{
                    GetImage(itemData.name),
                    keyImages[9],
                    keyImages[10]
                };

                float xPos = startX + column * cellSize.x;
                float yPos = startY - row * cellSize.y + 33;
                List<string> stringList = new List<string>()
                {
                    itemData.name,
                    itemData.item1,
                    itemData.item2,
                    itemData.item3
                };
                reward = reward + ProcessString(itemData.name, itemData.desc.ToString());
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
        UpdateNavigationButtonVisibility();
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
        StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
        storyController.LoadStoryDataFromText("", "體力不足，無法作戰。");
    }

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}
