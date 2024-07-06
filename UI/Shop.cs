using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : PageIndexController
{
    public int cellCount = 10;
    public int selectedIndex = 0;

    public List<Sprite> commonImages;
    public AudioSource bgmAudioSource;
    public GameObject backgroundImage;
    public static List<ProductData> productsDataList;
    public void Start()
    {
        folderName = "UI/Items";
        productsDataList = DataHolder.productsDataList;
        totalItems = productsDataList.Count;
        SetupCanvas();
        base.Start();
        commonImages = DataHolder.commonImages;
        DisplayPage();
    }

    private void SetupCanvas()
    {
        canvasObject = Utils.CreateCanvas("Shop", true);
        Button button = canvasObject.AddComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        Vector2 backgroundPosition = new Vector2(0, 0f);

        backgroundImage = Utils.CreateImage(DataHolder.commonImages[((int)UICommon.Panel)], backgroundPosition, canvasObject.transform);
        // Create the audio source
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        Utils.CreateNavigationBar("商店", new Vector2(0f, 198f), canvasObject.transform, () =>
        {
            canvasObject.SetActive(false);
        });
        // Other initialization code...
        canvasObject.SetActive(false);
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

        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                int index = currentPage * itemsPerPage + row * columnCount + column;
                float xPos = startX + column * cellSize.x;
                float yPos = startY - row * cellSize.y + 33;
                ProductData productData = productsDataList[index];

                List<Sprite> icons = new List<Sprite>{
                    GetImage(productData.name)
                };
                List<string> stringList = new List<string>()
                {
                    productData.name
                };
                if(productData.greed > 0) {
                    icons.Add(GetImage("Greed"));
                    stringList.Add(productData.greed.ToString());
                }
                if(productData.hatred > 0) {
                    icons.Add(GetImage("Hatred"));
                    stringList.Add(productData.hatred.ToString());
                }
                if(productData.delusion > 0) {
                    icons.Add(GetImage("Delusion"));
                    stringList.Add(productData.delusion.ToString());
                }
                GameObject cell = Utils.CreateAvatarAndRows(DataHolder.commonImages[((int)UICommon.NameCard)], icons, stringList, new Vector2(xPos, yPos), canvasObject.transform, false);
                cell.name = index.ToString();
                gameObjects.Add(cell);

                Button button = cell.AddComponent<Button>();
                button.onClick.AddListener(() => OnClickButtonWithParameter(index));
            }
        }
        rowTextObject = Utils.CreateText(new Vector2(300, 100), new Vector2(-100, -178), canvasObject.transform, TextAnchor.MiddleLeft);
        rowTextObject.GetComponent<Text>().text = productsDataList[selectedIndex].desc;
        rowTextObject.transform.localScale = new Vector3(1f, 1f, 1f);

        GameObject buyButton = Utils.CreateTextButton(DataHolder.commonImages[((int)UICommon.ShortButton)], new Vector2(300, 0), new Vector2(0, 0), rowTextObject.transform, () =>
        {
            Buy();
        });
        buyButton.transform.parent.gameObject.transform.localScale = new Vector3(1f,1f,1f);
        buyButton.name = "Button";
        buyButton.GetComponent<Text>().text = "買入";
    }

    private void Buy(){
        int index = selectedIndex;
        ProductData productData = productsDataList[index];
        Cache type = productData.name == "Delusion" ? Cache.Delusion : 
        productData.name == "Greed" ? Cache.Greed : Cache.Hatred;
        bool result = CacheManager.ChangeCoin(productData.delusion, type);
        if (result) {
            switch (index)
            {
                case 0:
                    int hpMax = CacheManager.GetCache(Cache.HP.ToString());
                    CacheManager.ChangeHPMP(hpMax, false);
                    break;
                case 1:
                    CacheManager.ChangeCache(Cache.Damage.ToString(), 1, 9999999);
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        } else {

        }
    }

    private void OnClickButtonWithParameter(int index)
    {
        
    }

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}
