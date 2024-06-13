using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public List<Sprite> keyImages;
    public Vector2 contentSize;
    public Vector2 cellSize = new Vector2(188f, 200f);
    public int cellCount = 10;

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
        // Other initialization code...
        canvasObject.SetActive(false);
    }

    private void SetupButtons()
    {
        Utils.CreateNavigationBar("商品資訊", new Vector2(0f, 193f), canvasObject.transform, () =>
        {
            canvasObject.SetActive(false);
        });
        int rowCount = 2;
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
                List<Sprite> icons = new List<Sprite>{
                    products[index],
                    keyImages[2]
                };
                float xPos = startX + column * cellSize.x;
                float yPos = startY - row * cellSize.y - 33;
                ProductData productData = productsDataList[index];
                List<string> stringList = new List<string>()
                {
                    productData.name,
                    productData.coin.ToString()
                };
                GameObject cell = Utils.CreateAvatarAndRows(keyImages[7], icons, stringList, new Vector2(xPos, yPos), canvasObject.transform);
                cell.name = index.ToString();
                gameObjects.Add(cell);

                Button button = cell.AddComponent<Button>();
                button.onClick.AddListener(() => OnClickButtonWithParameter(index));
            }
        }
    }

    private void OnClickButtonWithParameter(int index)
    {
        ProductData productData = productsDataList[index];
        bool result = CacheManager.ChangeCoin(productData.coin);
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

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}
