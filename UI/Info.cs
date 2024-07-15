using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoController : PageIndexController
{    
    public List<Sprite> commonImages;
    public AudioSource bgmAudioSource;
    public GameObject backgroundImage;

    public List<EnemyData> enemyDataList;
    public void Start()
    {
        SetupCanvas();
        base.Start();
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

        Utils.CreateNavigationBar("情報", new Vector2(0f, 206f), canvasObject.transform, () =>
        {
            canvasObject.SetActive(false);
        });
    }

    public string ProcessString(string name, string input)
    {
        if (name.Contains("Delusion"))
            return "delusion" + " (x" + input + ")";
        else if (name.Contains("Greed"))
            return "greed" + " (x" + input + ")";
        else if (name.Contains("Hatred"))
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
        folderName = "Enemies";
        enemyDataList = DataHolder.enemyDataList;
        totalItems = enemyDataList.Count;
        totalPages = Mathf.CeilToInt((float)totalItems / itemsPerPage);
        commonImages = DataHolder.commonImages;
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
                bool isSelected = selectedIndex == column;
                int index = currentPage * itemsPerPage + row * columnCount + column;
                if (index < 0 || index >= enemyDataList.Count) continue;
                EnemyData enemyData = enemyDataList[index];
                
                List<Sprite> icons = new List<Sprite>{
                    GetImage(enemyData.name),
                    GetImage("Heart"),
                    GetImage("Attack")
                };

                float xPos = startX + column * cellSize.x;
                float yPos = startY - row * cellSize.y + 38;
                List<string> stringList = new List<string>()
                {
                    enemyData.name.Replace("Boss", "").Replace("Enemy", ""),
                    enemyData.hp.ToString(),
                    enemyData.damage.ToString(),
                }; 
                int imageIndex = isSelected ? (int)UICommon.NameCardSel : (int)UICommon.NameCard;
                GameObject cell = Utils.CreateAvatarAndRows(DataHolder.commonImages[imageIndex], icons, stringList, new Vector2(xPos, yPos), canvasObject.transform);
                cell.name = index.ToString();
                gameObjects.Add(cell);
                cell.transform.localScale = new Vector3(1,1,1);

                Button button = cell.AddComponent<Button>();
                button.onClick.AddListener(() => OnClickButtonWithParameter(index % itemsPerPage));
                if(isSelected) {
                    rowTextObject = Utils.CreateText(new Vector2(300, 100), new Vector2(0, -178), canvasObject.transform, TextAnchor.MiddleCenter);
                    rowTextObject.GetComponent<Text>().text = ProcessString(enemyData.name, enemyData.reward.ToString());
                    rowTextObject.transform.localScale = new Vector3(1f, 1f, 1f);
                }
            }
        }
        UpdateNavigationButtonVisibility();
    }

    private void OnClickButtonWithParameter(int index)
    {
        if (selectedIndex == index) {
            return;
        }
        selectedIndex = index;
        DisplayPage();
        // StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
        // storyController.LoadStoryDataFromText("", "體力不足，無法作戰。");
    }

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}
