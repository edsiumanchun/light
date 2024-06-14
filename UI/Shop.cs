using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public List<Sprite> keyImages;
    public Vector2 contentSize;
    public Vector2 cellSize = new Vector2(188f, 200f);
    public int cellCount = 10;
    public int selectedIndex = 0;

    public GameObject rowTextObject;
    public List<GameObject> gameObjects;

    public List<Sprite> commonImages;
    public AudioSource bgmAudioSource;
    public GameObject canvasObject;
    public GameObject backgroundImage;
    public static List<ProductData> productsDataList;
    public List<Sprite> products;
    private void Start()
    {
        keyImages = DataHolder.keyImages;
        products = DataHolder.products;
        productsDataList = DataHolder.productsDataList;

        gameObjects = new List<GameObject>();
        commonImages = DataHolder.commonImages;
        SetupCanvas();
        SetupButtons();
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

    private void SetupButtons()
    {
        foreach(GameObject gameObject in gameObjects){
            Destroy(gameObject);
        }
        if(rowTextObject != null) {
            Destroy(rowTextObject);
        }
        int rowCount = 1;
        int columnCount = 3;

        float totalWidth = columnCount * cellSize.x;
        float totalHeight = rowCount * cellSize.y;

        float startX = -(totalWidth / 2) + (cellSize.x / 2);
        float startY = totalHeight / 2 - (cellSize.y / 2) ;

        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                int index = row * columnCount + column;
                float xPos = startX + column * cellSize.x;
                float yPos = startY - row * cellSize.y + 33;
                ProductData productData = productsDataList[index];

                List<Sprite> icons = new List<Sprite>{
                    products[index]
                };
                List<string> stringList = new List<string>()
                {
                    productData.name
                };
                if(productData.greed > 0) {
                    icons.Add(keyImages[0]);
                    stringList.Add(productData.greed.ToString());
                }
                if(productData.angry > 0) {
                    icons.Add(keyImages[1]);
                    stringList.Add(productData.angry.ToString());
                }
                if(productData.silly > 0) {
                    icons.Add(keyImages[2]);
                    stringList.Add(productData.silly.ToString());
                }
                GameObject cell = Utils.CreateAvatarAndRows(keyImages[7], icons, stringList, new Vector2(xPos, yPos), canvasObject.transform);
                cell.name = index.ToString();
                gameObjects.Add(cell);

                Button button = cell.AddComponent<Button>();
                button.onClick.AddListener(() => OnClickButtonWithParameter(index));
            }
        }
        rowTextObject = Utils.CreateText(new Vector2(300, 100), new Vector2(-100, -178), canvasObject.transform, TextAnchor.MiddleLeft);
        rowTextObject.GetComponent<Text>().text = productsDataList[selectedIndex].desc;
        rowTextObject.transform.localScale = new Vector3(1f, 1f, 1f);

        GameObject buyButton = Utils.CreateTextButton(DataHolder.commonImages[((int)UICommon.LongButton)], new Vector2(300, 0), new Vector2(0, 0), rowTextObject.transform, () =>
        {
            
        });
        buyButton.transform.parent.gameObject.transform.localScale = new Vector3(0.68f,1f,1f);
        buyButton.name = "Button";
        buyButton.GetComponent<Text>().text = "買入";
    }

    private void Buy(){
        int index = selectedIndex;
        ProductData productData = productsDataList[index];
        bool result = CacheManager.ChangeCoin(productData.silly);
        if (result) {
            switch (index)
            {
                case 0:
                    int hpMax = CacheManager.GetCache(Cache.HP.ToString());
                    CacheManager.ChangeHP(hpMax);
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
