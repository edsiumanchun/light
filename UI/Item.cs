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
    public GameObject dashboard, dashboard2;
    public void Start()
    {
        folderName = "UI/Items";
        itemDataList = DataHolder.itemDataList;
        totalItems = itemDataList.Count;
        SetupCanvas();
        base.Start();
        cellSize = new Vector2(203f, 200f);
        commonImages = DataHolder.commonImages;
        DisplayPage();
    }

    private void SetupCanvas()
    {
        canvasObject = Utils.CreateCanvas("Item", true);
        Button button = canvasObject.AddComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        Vector2 backgroundPosition = new Vector2(0, 0f);

        backgroundImage = Utils.CreateImage(DataHolder.commonImages[((int)UICommon.Panel2)], backgroundPosition, canvasObject.transform);
        // Create the audio source
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        // Other initialization code...
        canvasObject.SetActive(false);

        Utils.CreateNavigationBar("角色資訊", new Vector2(0f, 206f), canvasObject.transform, () => // profile
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
        int rowCount = 1;
        int columnCount = GetItemsLeft();

        float totalWidth = columnCount * cellSize.x;
        float totalHeight = rowCount * cellSize.y;

        float startX = -(totalWidth / 2) + (cellSize.x / 2);
        float startY = totalHeight / 2 - (cellSize.y / 2) ;
        string reward = "獎勵\n";
        // Debug.Log("abc" + totalItems);
        // Debug.Log("abc" + totalPages);
        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                int index = currentPage * itemsPerPage + row * columnCount + column;
                ItemData itemData = itemDataList[index];
                List<Sprite> icons = new List<Sprite>{
                    null,
                    GetImage(itemData.item1),
                    GetImage(itemData.item2),
                    GetImage(itemData.item3)
                };

                float xPos = startX + column * cellSize.x;
                float yPos = startY - row * cellSize.y - 130;
                List<int> textList = GetTextList(column);
                List<string> stringList = new List<string>()
                {
                    itemData.name,
                    textList[0].ToString(),
                    textList[1].ToString(),
                    textList[2].ToString()
                };
                reward = reward + ProcessString(itemData.name, itemData.desc.ToString());
                GameObject cell = Utils.CreateAvatarAndRows(DataHolder.commonImages[((int)UICommon.NameCard2)], icons, stringList, new Vector2(xPos, yPos), canvasObject.transform);
                cell.name = index.ToString();
                gameObjects.Add(cell);
                cell.transform.localScale = new Vector3(1,1,1);

                Button button = cell.AddComponent<Button>();
                button.onClick.AddListener(() => OnClickButtonWithParameter(index));
            }
        }
        rowTextObject = Utils.CreateText(new Vector2(300, 100), new Vector2(0, -178), canvasObject.transform, TextAnchor.MiddleCenter);
        // rowTextObject.GetComponent<Text>().text = reward;
        rowTextObject.GetComponent<Text>().text = "";
        rowTextObject.transform.localScale = new Vector3(1f, 1f, 1f);
        UpdateNavigationButtonVisibility();
        DisplayDashboard();
        DisplayDashboard2();
    }
    private void DisplayDashboard(){
        if(dashboard != null) {
            Destroy(dashboard);
        }
        List<string> titles = new List<string> { "每回輸出", "每回耗魔", "傷害減低" };
        List<string> values = new List<string>
        {
            CacheManager.GetDamage().ToString(),
            CacheManager.GetConsume().ToString(),
            CacheManager.GetDefend().ToString(),
        };
        dashboard = Utils.CreateRows(titles, values, new Vector2(-120, 130), canvasObject.transform);
        dashboard.transform.localScale = new Vector3(1f, 1f, 1f);
        dashboard.name = "Dashboard";
    }

    private void DisplayDashboard2(){
        if(dashboard2 != null) {
            Destroy(dashboard2);
        }
        List<string> titles = new List<string> { "生命回復", "收穫加成" };
        List<string> values = new List<string>
        {
            CacheManager.GetCache(Cache.HPRecover.ToString()).ToString(),
            CacheManager.GetCache(Cache.Reward.ToString()).ToString()
        };
        dashboard2 = Utils.CreateRows(titles, values, new Vector2(120, 130), canvasObject.transform);
        dashboard2.transform.localScale = new Vector3(1f, 1f, 1f);
        dashboard2.name = "Dashboard2";
    }

    public List<int> GetTextList(int column)
    {   
        List<int> values = new List<int>();
        if (column == 2)
        {
            values.Add(CacheManager.GetCache(Cache.Copper.ToString()));
            values.Add(CacheManager.GetCache(Cache.Silver.ToString()));
            values.Add(CacheManager.GetCache(Cache.Gold.ToString()));
        }
        else if (column == 1)
        {
            values.Add(CacheManager.GetCache(Cache.Greed.ToString()));
            values.Add(CacheManager.GetCache(Cache.Hatred.ToString()));
            values.Add(CacheManager.GetCache(Cache.Delusion.ToString()));
        } else {
            values.Add(CacheManager.GetCache(Cache.Ethics.ToString()));
            values.Add(CacheManager.GetCache(Cache.Concentration.ToString()));
            values.Add(CacheManager.GetCache(Cache.Wisdom.ToString()));
        }
        return values;
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
        // StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
        // storyController.LoadStoryDataFromText("", "體力不足，無法作戰。");
    }

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}
