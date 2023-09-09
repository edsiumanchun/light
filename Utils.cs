// // using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Networking;

// using System.Globalization;
// using System.Net;
// using System.Security.Cryptography;
// using System.Text;
// using UnityEditor;

// public class Utils : MonoBehaviour
// {

//     // public static void MoveCamera(bool isCameraHardCode){
//     //     FloorScript floorScript = GameObject.Find("Floor").GetComponent<FloorScript>();
//     //     floorScript.isCameraHardCode = isCameraHardCode;
//     // }

//     public static Sprite LoadAssetSprite(string url)
//     {
//         return Resources.Load<Sprite>(url);
//     }

//     public static GameObject LoadAssetPrefab(string url)
//     {
//         return Instantiate(Resources.Load(url) as GameObject);
//     }

//     // public static void SetClickSound(GameObject obj, string clipName = "Audio/Sound/click"){
//     //     AudioSource playerAudio = obj.AddComponent <AudioSource> ();
//     //     playerAudio.clip = Resources.Load<AudioClip>(clipName);
//     //     playerAudio.volume = DefaultScript.sound / 10.0f;
//     //     playerAudio.playOnAwake = false;
//     // }
// #pragma popup Canvas adn gui
//     // public static GUIStyle GetGUIStyle(string name){

//     //     var style = GUI.skin.GetStyle(name);
//     //     style.alignment = TextAnchor.MiddleCenter;
//     //     style.fontSize =  Mathf.RoundToInt(16 * DefaultScript.scaleFactor);
//     //     return style;
//     // }

//     // public static void SetupCanvas(GameObject GO){
//     //     Canvas myCanvas = GO.AddComponent<Canvas> ();
//     //     myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
//     //     myCanvas.worldCamera = GO.GetComponent<Camera>();
//     //     myCanvas.transform.position = new Vector3(0,0,0);
//     //     // canvasGO.SetActive(false);

//     //     CanvasScaler canvasScaler = GO.AddComponent<CanvasScaler> ();
//     //     canvasScaler.scaleFactor = DefaultScript.scaleFactor;
//     //     GO.AddComponent<GraphicRaycaster>();
//     // }

//     // public static Vector2 WorldToGuiPoint(Vector3 GOposition)
//     // {
//     //     var guiPosition = Camera.main.WorldToScreenPoint(GOposition);
//     //     guiPosition.y = DefaultScript.ScreenHeight - guiPosition.y;
//     //     return guiPosition;
//     // }
//     // public static Rect GetGUIRect(){
//     //     float guiHeight =  (260 + 70)* DefaultScript.scaleFactor;

//     //     float guiWidth = 230 * DefaultScript.scaleFactor;
//     //     return new Rect ((DefaultScript.ScreenWidth - guiWidth)/2,  (DefaultScript.ScreenHeight - guiHeight)/2, guiWidth, guiHeight);
//     // }

//     // public static Rect BackgroundRect(){
//     //     return new Rect (0,0,DefaultScript.ScreenWidth,DefaultScript.ScreenHeight);
//     // }

//     // public static Rect ImageRect(){
//     //     Rect windowRect = GetGUIRect();
//     //     float width = (windowRect.width);
//     //     float height = 60 * DefaultScript.scaleFactor;
//     //     float pY = 10 * DefaultScript.scaleFactor;
//     //     return new Rect(width / 2 - height / 2,pY, height, height);
//     // }

//     // public static Rect AlertRect(int idx, int split = 3){
//     //     Rect windowRect = GetGUIRect();
//     //     float width = (windowRect.width);
//     //     float height = (((windowRect.height)  - 70  * DefaultScript.scaleFactor)) / split;
//     //     float top = 64 * DefaultScript.scaleFactor;
//     //     float pY = 3 * DefaultScript.scaleFactor;
//     //     float pX = 5;
//     //     return new Rect(pX,top + pY + height * idx, width  - pX * 2, height - pY * 2);
//     // }
// #pragma popup Message
//     // public static void SetupMessage2(GameObject GO,string msg, int amount = 0, bool isCritical = false, bool isPoison = false){
//     //     ToolScript.SetupMessage(GO,msg,new Color(0.8f,0.8f,0.8f), amount, isCritical, isPoison);
//     // }
//     // public static void SetupMessage(GameObject GO,string msg,Color color, int amount = 0, bool isCritical = false, bool isPoison = false){
//     //     Color _color = color;
//     //     float padding = 0.06f * GO.transform.localScale.y;

//     //     Vector2 parentSize = ToolScript.ScaleSize(GO);
//     //     GameObject damageGO = new GameObject("DMG");
//     //     damageGO.transform.SetParent(GO.transform);
//     //     damageGO.transform.localScale = new Vector3(0.16f, 0.16f,130f);
//     //     damageGO.transform.localPosition =  new Vector3(0,parentSize.y / 2 + padding + 0.015f,0);
//     //     DamagePopup popup = damageGO.AddComponent<DamagePopup>();
//     //     if(msg == ""){
//     //         popup.Setup(amount, isCritical, isPoison);   
//     //     } else {
//     //         popup.SetupMessage(msg, color);
//     //     }
//     //     damageGO.gameObject.AddComponent<Outline>();
//     // }

// #pragma position


//     // public static float PanelWidth(){
//     //     return DefaultScript.ScreenWidth / DefaultScript.scaleFactor;
//     // }

//     // public static float PanelHeight(){
//     //     return DefaultScript.ScreenHeight / DefaultScript.scaleFactor;
//     // }

//     // public static void SetAllScale(GameObject parent){
//     //     foreach(Transform child in parent.transform)
//     //     {
//     //         child.localScale = new Vector3(1,1,1);
//     //     }
//     // }

// #pragma Effect
//     // public static void SetupEffect(GameObject GO, EnemyData enemyData){
//     //     // if(gameObject.name == "skeletonPrincess"){
//     //     //     return;
//     //     // }
//     //     Vector2 goSize = ToolScript.ScaleSize(GO);
//     //     float yoffset = goSize.y/2;
//     //     var p = Resources.Load("Effect/" + enemyData.effect) as GameObject;
//     //     GameObject effectGO = (GameObject)Instantiate(p);
//     //     Debug.Log("Effect/" + enemyData.effect);
//     //     // effectGO = GameObject.Instantiate(enemyData.effect); //  強行EFFECT
//     //     effectGO.transform.SetParent(GO.transform);  
//     //     effectGO.transform.position = new Vector3(0,yoffset,DefaultScript.camZ);
//     //     // effectGO.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
//     //     effectGO.tag = "Effect";
//     // }


// #pragma GameObject UI

// //     public static GameObject PickupSomething(GameObject parent, string objectName, Vector2 position, int sortingOrder = 23888){
// //         GameObject objectGO;
// //         objectGO = new GameObject("objectGO");
// //         objectGO.transform.SetParent(parent.transform);
        
// //         SpriteRenderer image = objectGO.AddComponent<SpriteRenderer>();
// //         image.sortingOrder = sortingOrder;
// //         image.sprite = ToolScript.LoadAssetSprite("Sprites/" + objectName);

// //         objectGO.transform.localPosition = position; 
// //         objectGO.transform.localScale = new Vector2(1,1); 
// //         // image.size = size;
// //         Animation animation = objectGO.GetComponent<Animation>();
// //         if(animation == null){
// //             animation = objectGO.AddComponent<Animation>();
// //         }
// //         objectGO.AddComponent<Animation>().FadeIn();
// //         return objectGO;
// //     }

// //     public static void Show(GameObject GO) {
// //         // toggles the visibility of this gameobject an
// //         Renderer[] renderers = GO.GetComponentsInChildren<Renderer>();
// //         for (int i = 0, ni = renderers.Length; i < ni; i++){
// //             renderers[i].enabled = true;
// //         }
// //     }

// //     public static void Hide(GameObject GO) {
// //         // toggles the visibility of this gameobject an
// //         Renderer[] renderers = GO.GetComponentsInChildren<Renderer>();
// //         for (int i = 0, ni = renderers.Length; i < ni; i++){
// //             renderers[i].enabled = false;
// //         }
// //     }

// //     public static void ChangeImage(GameObject GO, string url, ImageType imageType = ImageType.head){
// //         SpriteRenderer spriteRenderer = GO.GetComponent<SpriteRenderer>();
// //         if(spriteRenderer == null){
// //             Renderer[] renderers = GO.GetComponentsInChildren<Renderer>();
// //             if(renderers == null || renderers.Length == 0){
// //                 return ;
// //             }
// //             for (int i = 1, ni = renderers.Length; i < ni; i++)
// //             {
// //                 if(imageType == ImageType.head){
// //                     if(renderers[i].name == "head"){
// //                         SpriteRenderer r = (SpriteRenderer)renderers[i];
// //                         // r.sprite = (GameObject)Instantiate(Resources.Load("Sprites/Widget/" + url));
// //                     }
// //                 } else if(imageType == ImageType.body){ 
// //                     if(renderers[i].name == "body"){
// //                         SpriteRenderer r = (SpriteRenderer)renderers[i];
// //                         r.sprite = ToolScript.LoadAssetSprite("Sprites/Widget/" + url);
// //                     }
// //                 } else if(imageType == ImageType.weapon){ 
// //                     if(renderers[i].name.Contains("wea")){
// //                         SpriteRenderer r = (SpriteRenderer)renderers[i];
// //                         r.sprite = ToolScript.LoadAssetSprite("Sprites/Widget/" + url);
// //                     }
// //                 }
// //             }
// //         } else {
// //             spriteRenderer.color = new Color(1f, 0.2f, 0f, 1f);
// //         }
// //     }

// //     public static void ChangeSpriteColor(GameObject GO,  Color color,bool isHead = true){

// //         SpriteRenderer spriteRenderer = GO.GetComponent<SpriteRenderer>();
// //         if(spriteRenderer == null){
// //             Renderer[] renderers = GO.GetComponentsInChildren<Renderer>();
// //             if(renderers == null || renderers.Length == 0){
// //                 return ;
// //             }
// //             for (int i = 1, ni = renderers.Length; i < ni; i++)
// //             {
// //                 // print(renderers[i].name);
// //                 if(renderers[i].name == "BarSprite" || renderers[i].name == "Background" || renderers[i].name == "Border" || renderers[i].name == "TMP_Text"){
// //                     continue;
// //                 }
// //                 if(isHead == true){
// //                     if(renderers[i].name == "head"){
// //                         if(renderers[i].GetType() == typeof(SpriteRenderer)){
// //                             SpriteRenderer r = (SpriteRenderer)renderers[i];
// //                             r.color = color;
// //                         }
// //                     } 
// //                 } else {
// //                     if(renderers[i].GetType() == typeof(SpriteRenderer)){
// //                         SpriteRenderer r = (SpriteRenderer)renderers[i];
// //                         r.color = color;
// //                     }
// //                 }
// //             }
// //         } else {
// //             spriteRenderer.color = new Color(1f, 0.2f, 0f, 1f);
// //         }
// //     }

// //     public static void NoHead(GameObject GO, bool isHead = true){
// //         SpriteRenderer spriteRenderer = GO.GetComponent<SpriteRenderer>();
// //         if(spriteRenderer == null){
// //             Renderer[] renderers = GO.GetComponentsInChildren<Renderer>();
// //             if(renderers == null || renderers.Length == 0){
// //                 return ;
// //             }
// //             for (int i = 1, ni = renderers.Length; i < ni; i++)
// //             {
// //                 if(isHead == true){
// //                     if(renderers[i].name == "head"){
// //                         if(renderers[i].GetType() == typeof(SpriteRenderer)){
// //                             SpriteRenderer r = (SpriteRenderer)renderers[i];
// //                             r.enabled = false;
// //                         }
// //                     } 
// //                 } else {
// //                     if(renderers[i].GetType() == typeof(SpriteRenderer)){
// //                         SpriteRenderer r = (SpriteRenderer)renderers[i];
// //                             r.enabled = false;
// //                     }
// //                 }
// //             }
// //         } else {
// //             spriteRenderer.color = new Color(1f, 0.2f, 0f, 1f);
// //         }
// //     }


// //     public static Vector2 ScaleSize(GameObject GO, bool needScale = false) // must create sprite first, collider later
// //     // child scale by parent, so cannot use for trap, just simply get spriterenderer size 
// //     {
// //         if(GO == null){
// //             return Vector2.zero;
// //         }
// //         if(GO.GetComponent<BoxCollider2D>() != null && GO.GetComponent<BoxCollider2D>().size.x != 1 && GO.GetComponent<BoxCollider2D>().size.y != 1){// box collider 原汁原味唔計

// //             if(needScale){
// //                 return new Vector2(GO.GetComponent<BoxCollider2D>().size.x * GO.transform.localScale.x, GO.GetComponent<BoxCollider2D>().size.y * GO.transform.localScale.y);
// //             } else {
// //                 return GO.GetComponent<BoxCollider2D>().size;
// //             }
// //         }
// //         SpriteRenderer spriteRenderer = GO.GetComponent<SpriteRenderer>();
// //         if(spriteRenderer == null){
// //             Renderer[] renderers = GO.GetComponentsInChildren<Renderer>();
// //             if(renderers == null || renderers.Length == 0){
// //                 return Vector2.zero;
// //             }
// //             Bounds bounds = renderers[0].bounds;
// //             for (int i = 1, ni = renderers.Length; i < ni; i++)
// //             {
// //                 bounds.Encapsulate(renderers[i].bounds);
// //             }
// //             if(needScale){
// //                 // print(bounds.size.x);
// //                 return new Vector2(bounds.size.x / GO.gameObject.transform.localScale.x, bounds.size.y / GO.gameObject.transform.localScale.y); // special
// //             } else {
// //                 return new Vector2(bounds.size.x, bounds.size.y); // special
// //             }
// //         } 
// //         // final !!

// //         if(needScale){
// //             return new Vector2(spriteRenderer.sprite.bounds.size.x * GO.transform.localScale.x, spriteRenderer.sprite.bounds.size.y * GO.transform.localScale.y);
// //         } else {
// //             return new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
// //         }
// //     }

// //     public static float GetCanvasHeight(GameObject GO, float width){
// //         return GO.GetComponent<RectTransform>().rect.height * width / GO.GetComponent<RectTransform>().rect.width;
// //     }
// //     // Start is called before the first frame update
// // #pragma game status
// //     public static bool PageIsActive(GameObject GO){
// //         if(GO == null){
// //             return false;
// //         } else if(GO.activeSelf == true){
// //             return true;
// //         } else {
// //             return false;
// //         }
// //     }

// //     public static bool InGame(){
// //         if(PageIsActive(InAppPurchaseScript._inAppPurchase) || PageIsActive(BookScript._book) || PageIsActive(StoryScript._story) || PageIsActive(SettingScript._setting) ){
// //             return false;
// //         } else {
// //             return true;
// //         }
// //     }
// // #pragma Apple

// //     public static string GenerateRandomString(int length)
// //     {
// //         if (length <= 0)
// //         {
// //             return "..";
// //             // throw new Exception("Expected nonce to have positive length");
// //         }

// //         const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
// //         var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
// //         var result = string.Empty;
// //         var remainingLength = length;

// //         var randomNumberHolder = new byte[1];
// //         while (remainingLength > 0)
// //         {
// //             var randomNumbers = new List<int>(16);
// //             for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
// //             {
// //                 cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
// //                 randomNumbers.Add(randomNumberHolder[0]);
// //             }

// //             for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
// //             {
// //                 if (remainingLength == 0)
// //                 {
// //                     break;
// //                 }

// //                 var randomNumber = randomNumbers[randomNumberIndex];
// //                 if (randomNumber < charset.Length)
// //                 {
// //                     result += charset[randomNumber];
// //                     remainingLength--;
// //                 }
// //             }
// //         }

// //         return result;
// //     }

// //     public static string GenerateSHA256NonceFromRawNonce(string rawNonce)
// //     {
// //         var sha = new SHA256Managed();
// //         var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
// //         var hash = sha.ComputeHash(utf8RawNonce);

// //         var result = string.Empty;
// //         for (var i = 0; i < hash.Length; i++)
// //         {
// //             result += hash[i].ToString("x2");
// //         }

// //         return result;
// //     }

// // #pragma collider



// //     public static GameObject SetupRigidColliderPos(GameObject GO){ // used 3 times
// //         Rigidbody2D rigidBody = GO.GetComponent<Rigidbody2D>();
// //         if(rigidBody == null){
// //             rigidBody =  GO.AddComponent<Rigidbody2D>();
// //         }
// //         BoxCollider2D boxCollider2D =  GO.GetComponent<BoxCollider2D>();
// //         if(boxCollider2D == null){
// //             boxCollider2D =  GO.AddComponent<BoxCollider2D>();
// //         }
// //         return GO;
// //     }

// //     public static GameObject SetupColliderPrefabLogic(GameObject GO, GameObject parent = null){ // for prefab, SET POS BY YOURSELF
// //         SetupRigidColliderPos(GO);
// //         if(parent != null){
// //             GO.transform.SetParent(parent.transform);
// //         }
// //         BoxCollider2D col =  GO.GetComponent<BoxCollider2D>();
// //         col.size = ToolScript.ScaleSize(GO, false);
// //         col.offset = new Vector2(0, col.size.y / 2);
// //         PhysicsMaterial2D newMat = Resources.Load("NoFriction", typeof(PhysicsMaterial2D)) as PhysicsMaterial2D;
// //         GO.GetComponent<Rigidbody2D>().sharedMaterial = newMat;
// //         // GO.transform.localScale =new Vector3(1 ,1,0);   // magic
// //         return GO;
// //     }

// //     public static GameObject SetupColliderPrefab(string name, GameObject parent = null){ // for prefab, SET POS BY YOURSELF
// //         Debug.Log(name);
// //         GameObject GO = LoadAssetPrefab(name);
// //         SetupColliderPrefabLogic(GO,parent);
// //         return GO;
// //     }

// //     public static void SetupPosColliderImage(GameObject GO, string name, Vector2 positionScaled, Vector2 sizeScaled, GameObject parent = null, bool isTile = false) //FOR WALL AND ENEMY, DECOR WALL, USE GETPOS IN FLOORSCRIPT
// //     {
// //         SpriteRenderer image = GO.GetComponent<SpriteRenderer>();
// //         if(parent != null){
// //             // GO.transform.localScale =new Vector3(1 ,1,0);   // magic
// //             GO.transform.SetParent(parent.transform);
// //             if(image == null){
// //                 // GO.transform.localScale =new Vector3(1 ,1,0);   // if not prefab, already applied scale
// //             }
// //         }
// //         SetupRigidColliderPos(GO);
// //         Rigidbody2D rigidBody = GO.GetComponent<Rigidbody2D>(); // for image
// //         rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
// //         rigidBody.isKinematic=true;
// //         if(image == null){
// //             image = GO.AddComponent<SpriteRenderer>();
// //         }
// //         if(name != ""){ // wall may hv no image
// //             image.sprite = ToolScript.LoadAssetSprite(name);   
// //         }
// //         if(isTile){
// //             image.drawMode = SpriteDrawMode.Tiled;   
// //             image.size = new Vector2(sizeScaled.x , sizeScaled.y );
// //         }

// //         BoxCollider2D boxCollider2D =  GO.GetComponent<BoxCollider2D>();
// //         if(boxCollider2D == null){
// //             boxCollider2D =  GO.AddComponent<BoxCollider2D>();
// //         }
// //         boxCollider2D.size = new Vector2 (image.size.x , image.size.y );
// //         Vector3 vector = parent != null ? parent.transform.localScale : new Vector3(1,1,1);
// //         GO.transform.localPosition = new Vector3(positionScaled.x / vector.x, positionScaled.y / vector.y, DefaultScript.camZ - 0.03f); 
// //     }

// // // DIFFERENT
// //     public static void SetupPosImage(GameObject GO, string name, Vector2 positionScaled, Vector2 sizeScaled,GameObject parent = null, bool isTile = false) //FOR DECORATION, BACKGROUND, USE GETPOS IN FLOORSCRIPT
// //     {
// //         Debug.Log(name);
                
// //         if(parent != null){
// //             GO.transform.SetParent(parent.transform);
// //             // GO.transform.localScale =new Vector3(1 ,1,0);   // if not prefab, already applied scale
// //         }
// //         SpriteRenderer image = GO.GetComponent<SpriteRenderer>();
// //         if(image == null){
// //             image = GO.AddComponent<SpriteRenderer>();
// //         }
// //         if(name != ""){ // no image name then quit
// //             image.sprite = ToolScript.LoadAssetSprite(name);
// //         }
// //         Vector3 vector = parent != null ? parent.transform.localScale : new Vector3(1,1,1);
// //         if(isTile){
// //             image.drawMode = SpriteDrawMode.Tiled;   
// //             image.size = new Vector2(sizeScaled.x, sizeScaled.y); // magic for backgroundGO
// //                 GO.transform.localPosition = new Vector3(positionScaled.x / vector.x, positionScaled.y / vector.y, DefaultScript.camZ - 0.03f); 
// //         } else {
// //             if(sizeScaled != Vector2.zero){ // only if size has value
// //                 image.size = new Vector2(sizeScaled.x, sizeScaled.y);
// //                 GO.transform.localPosition = new Vector3(positionScaled.x / vector.x, positionScaled.y / vector.y, DefaultScript.camZ - 0.03f); 
// //             } 
// //         }
// //     }

// //     public static void ignoreCollision(GameObject GO,  GameObject GO2){
// //         BoxCollider2D collider1 = GO.GetComponent<BoxCollider2D>();
// //         BoxCollider2D collider2 = GO2.GetComponent<BoxCollider2D>();
// //         if(collider1 == null){
// //             return;
// //         }
// //         if(collider2 == null){
// //             return;
// //         }
// //         Physics2D.IgnoreCollision(collider1, collider2);

// //     }

// //     public static void AddPS(string url, string name, GameObject parent, float rotate){
// //         var p = Resources.Load(url);
// //         GameObject GO = (GameObject)Instantiate(p);
// //         // GameObject GO = GameObject.Instantiate(Camera.main.GetComponent<ParticleEffectScript>().CFX4_Hit_B_Orange);
// //         GO.name = name;
// //         GO.AddComponent<ParticaleAnimator>();
// //         GO.transform.SetParent(parent.transform);
// //         // GO.transform.SetParent(ammoGO.transform);  
// //         GO.transform.localScale = new Vector3(1f,1f,1f);
// //         GO.transform.localRotation = Quaternion.Euler(0, rotate, 0);
// //         // GO.transform.SetParent(null);
// //         // GO.transform.localPosition = new Vector3(0,0,0);
// //         return;
// //     }
// }
