
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
public class EventController : MonoBehaviour
{
    public static Vector3 GetVector(string target,Vector3 vector3) { 
        GameObject gameObject = GameObject.Find(target);
        return new Vector3(gameObject.transform.position.x - vector3.x, gameObject.transform.position.y - vector3.y, vector3.z);
    }

    public static void TurnAround(GameObject gameObject){
        Transform transform = gameObject.transform;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public static float ShowEvent(string name)
    { 
        GameObject player = GameObject.Find("Player");
        GameObject story = GameObject.Find("Story");
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        GameObject bossGem = GameObject.Find("BossGem");
        GameObject rosia = GameObject.Find("Rosia");
        GameObject music = GameObject.Find("Music");
        int currentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        Vector3 vector3;
        GameObject gameObject;
        switch(name)
        {
            case "tohospital":
                CacheManager.SetCache(Cache.ExitH.ToString(), 4); 
                CacheManager.SetCache(Cache.ExitV.ToString(), -1); 
                MainController.LoadAssetsFromJsonWithIsSave(1, 0, "HomeScene", false);
                return 1f;
            case "firework":
                List<string> paths = new List<string>((new[] { "Firework2", "Firework3"}));
                List<Object> fireworks = Utils.LoadPrefabs("Effect", paths);
                int j = 0;
                foreach(var firework in fireworks) {
                    Camera.main.GetComponent<HomeController>().SpawnFirework((GameObject)firework, j == 1);
                    j++;
                }
                return 0.1f;
            case "nextscene":
                CacheManager.SetCache(Cache.ExitH.ToString(), 4); 
                CacheManager.SetCache(Cache.ExitV.ToString(), -1);
                MainController.LoadAssetsFromJsonWithIsSave(currentStage + 1, 0, "HomeScene", false);
                return 1f;
            case "savescene":
                CacheManager.SetCache(Cache.ExitH.ToString(), 4); 
                CacheManager.SetCache(Cache.ExitV.ToString(), -1);
                CacheManager.SetCache(Cache.CurrentFloor.ToString(), currentFloor);
                return 1f;
            case "completeTutor":
                CacheManager.SetCache(Cache.ShowTutor.ToString(), 1);
                StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
                storyController.SetIntervative(true);
                return 0.1f;
            // disappear
            case "storyleave":
                TurnAround(story);
                vector3 = GetVector("36_PlateV_Space", new Vector3(0.8f,0f,1f));
                vector3 = new Vector3(vector3.x, story.transform.position.y, vector3.z);
                story.transform.DOMove(vector3, 1.8f);
                // story.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Move);
                return 3;
            case "nursedisappear":
                //TurnAround(story);
                gameObject = GameObject.Find("Nurse");
                Utils.Fade(gameObject, true);
                // story.transform.DOMove(GetVector("Player", new Vector3(0.8f,0f,1f)), 1);
                return 1f;
            case "bossgoldkey":
                gameObject = new GameObject("GoldKey");
                SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = (Sprite)DataHolder.items[2];
                gameObject.AddComponent<KeyController>();
                gameObject.transform.localPosition = new Vector3(boss.transform.localPosition.x,boss.transform.localPosition.y + 0.5f, -0.3f);
                AssetData.SetParent(gameObject);
                return 1f;
            case "bossdisappear":
                boss.GetComponent<EnemyController>().Die(false);
                return 1f;
            case "bossgemdisappear":
                bossGem.GetComponent<EnemyController>().Die(false);
                return 1f;
            case "storydisappear":
                Utils.Fade(story, true);
                return 0.1f;
            // shop
            case "musicdisappear":
                Utils.Fade(music, true);
                return 1.8f;
            // shop
            case "shop":
                ShopController shopController = Camera.main.gameObject.GetComponent<ShopController>();
                shopController.canvasObject.SetActive(true);
                return 0.1f;
            // animation
            case "playersay":
                // player.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Down);
                return 1;
            case "playerdown":
                // player.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Down);
                return 1;
            case "playerhurt":
                // player.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Hurt);
                return 1;
            case "playerpoint":
                // player.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Point);
                return 0.3f;
            case "rosiaturn":
                TurnAround(rosia);
                return 0.1f;
            case "playerturn":
                TurnAround(player);
                return 0.1f;
            case "storyturn":
                TurnAround(story);
                return 0.1f;
            case "playeridle":
                player.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Idle);
                return 1;
            case "playerkneel":
                //player.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Kneel);
                return 1;
            case "rosiawake":
                //rosia.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Wake);
                return 1f;
            case "storywake":
                // story.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Wake);
                return 1f;
            case "playerwake":
                // player.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Wake);
                return 1f;
            case "playerdrink":
                // story.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Wake);
                return 1f;
            case "rosiadown":
                // rosia.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Down);
                return 1f;
            case "rosiacry":
                // rosia.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Cry);
                return 1f;
                
            // move

            case "musicappear":
                vector3 = GetVector("Player",new Vector3(1.8f,0f,1f));
                gameObject = Utils.LoadPrefabAndInstantObject("Characters", "Music", vector3);
                gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
                Utils.Fade(gameObject, false);
                AssetData.SetParent(gameObject);
                return 1.8f;
            case "playerattack":
                player.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Attack);
                Utils.AddEffect(boss, "purify");
                // Utils.AddEffect(player, "purify");
                return 1.8f;
            case "rosiacome":
                vector3 = GetVector("Player",new Vector3(0.8f,0.4f,1f));
                rosia = Utils.LoadPrefabAndInstantObject("Enemies", "Enemy0", vector3);
                rosia.transform.DOMove(GetVector("Player", new Vector3(0.8f,0f,1f)), 1);
                AssetData.SetParent(rosia);
                return 1.3f;
            case "nursecome":
                vector3 = GetVector("Player",new Vector3(-5f,0f,1f));
                gameObject = Utils.LoadPrefabAndInstantObject("Characters", "Nurse", vector3);
                gameObject.name = "Nurse";
                Utils.Fade(gameObject, false);
                // nurse.transform.DOMove(GetVector("Story", new Vector3(-0.8f,04f,1f)), 2);
                // nurse.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Move);
                AssetData.SetParent(gameObject);
                return 2f;
            case "rosiacometoplayer": 
                vector3 = new Vector3(GetVector("Player",new Vector3(-1f,0.4f,1f)).x, player.transform.localPosition.y, 1f);
                rosia.transform.DOMove(vector3, 2);
                player.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Move);
                return 2f;
            case "playercometostory": // unused
                vector3 = new Vector3(GetVector("Story",new Vector3(1.3f,0.4f,1f)).x, player.transform.localPosition.y, 1f);
                player.transform.DOMove(vector3, 2);
                player.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Move);
                return 2f;
            case "mariascare":
                return 1f;
            case "ghostseat":
                return 1f;
                
                // ghost mum son
            case "ghostsleave":
                gameObject = GameObject.Find("GhostKid");
                Utils.Fade(story, true);
                Utils.Fade(gameObject, true);
                return 1f;
            case "ghostmumcry":
                return 1f;
            case "ghostmumcome":
                story.transform.DOMove(GetVector("GhostKid", new Vector3(0.8f,0f,1f)), 2f);
                return 2f;
            case "ghostkidshow":
                vector3 = GetVector("Player",new Vector3(-3.3f,0f,1f));
                gameObject = Utils.LoadPrefabAndInstantObject("Characters", "StoryGhostKid", vector3);
                gameObject.name = "GhostKid";
                Utils.Fade(gameObject, false);
                return 1.8f;
            case "ghostshow":
                vector3 = GetVector("Player",new Vector3(1.3f,0.4f,1f));
                gameObject = Utils.LoadPrefabAndInstantObject("Enemies", "Enemy0", vector3);
                gameObject.AddComponent<EnemyController>();
                gameObject.tag = "Boss";
                gameObject.transform.DOMove(GetVector("Player", new Vector3(1.3f,0f,1f)), 1.8f);
                AssetData.SetParent(gameObject);
                return 1.8f;
                // chef and cow
            case "chefcome":
                // story.transform.DOMove(GetVector("Cow", new Vector3(0.8f,0f,1f)), 1);
                // story.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Move);
                // AssetData.SetParent(story);
                return 1f;
            case "knifedrop":
                return 0.1f;
            case "checkcow":
                return 0.1f;
            case "savecows":
                return 0.1f;
            case "musicplaybgm":
                Utils.ChangeAudioClipByName(0);
                return 0.1f;
            case "musicplay":
                Utils.ChangeAudioClipByName(1);
                return 0.1f;
                Utils.ChangeAudioClipByName(2);
            case "musicplay2":
                return 0.1f;
            
        }
        return 0.1f;
    }
}
