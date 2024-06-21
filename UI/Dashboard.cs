// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections.Generic;
// using System.IO;

// public class DashboardController : MonoBehaviour
// {
//     public List<Sprite> keyImages;

//     /// ////////
//     public GameObject canvasObject;
//     public AudioSource bgmAudioSource;
//     public GameObject backgroundImage;
//     public List<GameObject> keyTextImages;
//     public Vector2 screenSize;

//     private void Start()
//     {
//         keyImages = DataHolder.keyImages;
//         // canvas
//         SetupCanvas();
//         SetupInfo();
//         SetText();
//     }

//     private void SetupCanvas()
//     {
//         canvasObject = Utils.CreateCanvas("Dashboard", false);
//         screenSize = Utils.GetCanvasSize(canvasObject);
//         Button button = canvasObject.AddComponent<Button>();
//         button.onClick.AddListener(OnClickButton);
//         Vector2 backgroundPosition = new Vector2(118 - screenSize.x / 2, screenSize.y / 2 - 313);

//         backgroundImage = Utils.CreateImage(DataHolder.dashboardBackgroundImages[0], backgroundPosition, canvasObject.transform);
//         // Create the audio source
//         bgmAudioSource = gameObject.AddComponent<AudioSource>();
//         // Other initialization code...
//         canvasObject.SetActive(false);
//     }


//     private void SetupInfo()
//     {
//         keyTextImages = new List<GameObject>();
//         for(int i = 0; i < 6; i++){
//             Utils.CreateImage(keyImages[i], new Vector2(-26, 113 - (i * 44)), backgroundImage.transform);
//             keyTextImages.Add(Utils.CreateText(new Vector2(100,50), new Vector2(54 , 113 - (i * 44)), backgroundImage.transform, TextAnchor.MiddleLeft));
//         }

//         // for(int i = 0; i < 4; i++){
//         //     Utils.CreateImage(keyImages[i+4], new Vector2(60, 110 - (i * 44)), backgroundImage.transform);
//         //     keyTextImages.Add(Utils.CreateText(new Vector2(100,50), new Vector2(140 , 110 - (i * 44)), backgroundImage.transform, TextAnchor.MiddleLeft));
//         // }
//     }

//     public void SetText()
//     {
//         int copperKeyCount = CacheManager.GetCache(Cache.Copper.ToString());
//         int silverKeyCount = CacheManager.GetCache(Cache.Silver.ToString());
//         int goldKeyCount = CacheManager.GetCache(Cache.Gold.ToString());
//         // int flowerKeyCount = CacheManager.GetCache(Cache.Flower.ToString());

//         int greedCount = CacheManager.GetCache(Cache.Greed.ToString());
//         int sillyCount = CacheManager.GetCache(Cache.Silly.ToString());
//         int angryCount = CacheManager.GetCache(Cache.Angry.ToString());
//         // int damageCount = CacheManager.GetCache(Cache.Damage.ToString());
//         // int defendCount = CacheManager.GetCache(Cache.Defend.ToString());
//         // Update the key count texts
//         keyTextImages[0].GetComponent<Text>().text = greedCount.ToString();
//         keyTextImages[1].GetComponent<Text>().text = angryCount.ToString();
//         keyTextImages[2].GetComponent<Text>().text = sillyCount.ToString();
//         // keyTextImages[3].GetComponent<Text>().text = coinKeyCount.ToString();

//         keyTextImages[3].GetComponent<Text>().text = copperKeyCount.ToString();
//         keyTextImages[4].GetComponent<Text>().text = silverKeyCount.ToString();
//         keyTextImages[5].GetComponent<Text>().text = goldKeyCount.ToString();
//         // keyTextImages[7].GetComponent<Text>().text = flowerKeyCount.ToString();

//     }

//     void OnClickButton()
//     {
//         // Set the active state of the game object to false
//         canvasObject.SetActive(false);
//     }
// }
