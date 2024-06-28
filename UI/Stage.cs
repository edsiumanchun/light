using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{
    List<string> stages = new List<string>(){"Grave","Clinic","Palace","Jetavana"};
    public Vector2 contentSize;
    public Vector2 cellSize = new Vector2(88f, 88f);
    public int cellCount = 10;
    public int tempStage = 0;

    public List<GameObject> buttons;

    public List<Sprite> commonImages;
    public AudioSource bgmAudioSource;
    public GameObject canvasObject;
    public GameObject selectionBarObject;
    public GameObject backgroundImage;

    private void Start()
    {
        buttons = new List<GameObject>();
        commonImages = DataHolder.commonImages;
        SetupCanvas();
        SetupBar();
        SetupButtons();
    }

    private void SetupCanvas()
    {
        canvasObject = Utils.CreateCanvas("Stage", true);
        Button button = canvasObject.AddComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        Vector2 backgroundPosition = new Vector2(0, 0f);

        backgroundImage = Utils.CreateImage(DataHolder.commonImages[((int)UICommon.Panel)], backgroundPosition, canvasObject.transform);
        // Create the audio source
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        // Other initialization code...
        canvasObject.SetActive(false);
    }
    private void SetupBar(){
        tempStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        Utils.CreateNavigationBar("地圖", new Vector2(0f, 198f), canvasObject.transform, () =>
        {
            canvasObject.SetActive(false);
        });
        selectionBarObject = Utils.CreateSelectionBar(stages[tempStage], new Vector2(0f, -178f), canvasObject.transform, () =>
        {
            tempStage = Mathf.Max(tempStage - 1, 0);
            selectionBarObject.GetComponent<Text>().text = stages[tempStage];
            // foreach(var button in buttons){
            //     Destroy(button);
            // }
            // SetupButtons();
            updateButtonImages();
        }, () =>
        {
            tempStage = Mathf.Min(tempStage + 1, stages.Count - 1);
            selectionBarObject.GetComponent<Text>().text = stages[tempStage];
            // foreach(var button in buttons){
            //     Destroy(button);
            // }
            // SetupButtons();
            updateButtonImages();
        });
    }

    private void SetupButtons()
    {
        int rowCount = 2;
        int columnCount = 5;

        float totalWidth = columnCount * cellSize.x;
        float totalHeight = rowCount * cellSize.y;

        float startX = -(totalWidth / 2) + (cellSize.x / 2);
        float startY = totalHeight / 2 - (cellSize.y / 2);

        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                int index = row * columnCount + column;

                float xPos = startX + column * cellSize.x;
                float yPos = startY - row * cellSize.y;

                GameObject cell = Utils.CreateTextButton(null, new Vector2(xPos, yPos + 30f), new Vector2(0, 0), canvasObject.transform, () =>
                {
                    CacheManager.SetCache(Cache.CurrentStage.ToString(), tempStage);
                    CacheManager.SetCache(Cache.Exit.ToString(), index == 0 ? 2 : (AssetData.goUp ? 2 : 0));
                    MainController.LoadAssetsFromJson(tempStage, index, "HomeScene");
        updateButtonImages();
                    canvasObject.SetActive(false);
                });

                cell.name = (index + 1).ToString();
                cell.GetComponent<Text>().text = (index + 1).ToString();
                buttons.Add(cell.transform.parent.gameObject);
                cell.transform.parent.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        updateButtonImages();
    }

    void updateButtonImages() {
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        Debug.Log("???" + currentFloor);
        int i = 0;
        foreach(var button in buttons){
            var image = currentFloor == i ? commonImages[((int)UICommon.SelCircleButton)] : commonImages[((int)UICommon.CircleButton)];
            if(tempStage != CacheManager.GetCache(Cache.CurrentStage.ToString())){
                image = commonImages[((int)UICommon.CircleButton)];
            }
            button.GetComponent<Image>().sprite = image;
            i++;
        }
    }

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}
