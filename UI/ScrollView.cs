//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class ScrollViewController : MonoBehaviour
//{
//    public Vector2 contentSize;
//    public Vector2 cellSize;
//    public int cellCount = 10;

//    public List<Sprite> commonImages;
//    public List<Sprite> scrollViewImages;

//    public AudioSource bgmAudioSource;
//    public GameObject canvasObject;
//    public GameObject backgroundImage;
//    public GameObject closeButton;

//    private void Start()
//    {
//        commonImages = DataHolder.commonImages;
//        scrollViewImages = DataHolder.scrollViewImages;
//        SetupCanvas();
//        SetupScrollView();
//    }

//    private void SetupCanvas()
//    {
//        canvasObject = Utils.CreateCanvas("ScrollView", true);
//        Button button = canvasObject.AddComponent<Button>();
//        button.onClick.AddListener(OnClickButton);
//        Vector2 backgroundPosition = new Vector2(0, 0f);

//        backgroundImage = Utils.CreateImage(DataHolder.scrollViewImages[0], backgroundPosition, canvasObject.transform);
//        // Create the audio source
//        bgmAudioSource = gameObject.AddComponent<AudioSource>();
//        // Other initialization code...
//        canvasObject.SetActive(false);
//    }

//    private void SetupScrollView()
//    {
//        for (int i = 0; i < cellCount; i++)
//        {
//            GameObject cell = Utils.CreateTextButton(DataHolder.scrollViewImages[1], new Vector2(0, -i * cellSize.y), new Vector2(0,0), canvasObject.transform, () =>
//             {

//             });
//            cell.name = "ScrollView" + i.ToString();
//            cell.GetComponent<Text>().text = i.ToString();

//            // Add background image to the cell (optional)
//            //if (backgroundImage != null)
//            //{
//            //    Image cellBackground = cell.AddComponent<Image>();
//            //    //cellBackground.sprite = backgroundImage;
//            //    cellBackground.rectTransform.anchorMin = Vector2.zero;
//            //    cellBackground.rectTransform.anchorMax = Vector2.one;
//            //    cellBackground.rectTransform.offsetMin = Vector2.zero;
//            //    cellBackground.rectTransform.offsetMax = Vector2.zero;
//            //}
//        }
//    }


//    void OnClickButton()
//    {
//        // Set the active state of the game object to false
//        canvasObject.SetActive(false);
//    }
//}
