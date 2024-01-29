using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConfirmController : MonoBehaviour
{
    public Vector2 contentSize;
    public Vector2 cellSize = new Vector2(230f, 83f);
    public int cellCount = 10;

    public List<GameObject> buttons;

    public List<Sprite> commonImages;
    public AudioSource bgmAudioSource;
    public GameObject canvasObject;
    public GameObject backgroundImage;
    public GameObject messageGO;
    public List<UnityAction> actions;
    public List<string> buttonTitles;

    private void Awake()
    {
        buttons = new List<GameObject>();
        commonImages = DataHolder.commonImages;
        SetupCanvas();
    }

    private void SetupCanvas()
    {
        canvasObject = Utils.CreateCanvas("Confirm", true);
        Button button = canvasObject.AddComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        Vector2 backgroundPosition = new Vector2(0, 0f);

        backgroundImage = Utils.CreateImage(DataHolder.commonImages[((int)UICommon.ConfirmPanel)], backgroundPosition, canvasObject.transform);
        // Create the audio source
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        // Other initialization code...
        canvasObject.SetActive(false);
    }

    public void SetupAction(UnityAction action, string message, List<string> titles){
        if(messageGO == null){
            SetupText();
        }
        foreach(var button in buttons){
            Destroy(button);
        }
        actions = new List<UnityAction>();
        actions.Add(
            () =>
            {
                    ConfirmController confirmController = Camera.main.gameObject.GetComponent<ConfirmController>();
                    confirmController.canvasObject.SetActive(false);
            }
        );
        actions.Add(action);
        buttonTitles = titles;
        SetupButtons();
        buttons[1].GetComponent<Button>().onClick.AddListener(action);
        messageGO.GetComponent<Text>().text = message;
    }

    private void SetupText(){
        messageGO = Utils.CreateText(new Vector2(350,350), new Vector2(0, 38), canvasObject.transform, TextAnchor.MiddleCenter);
    }
    private void SetupButtons()
    {   
        int columnCount = 2;

        float totalWidth = columnCount * cellSize.x;
        float totalHeight = 1 * cellSize.y;

        float startX = -(totalWidth / 2) + (cellSize.x / 2);
        float startY = totalHeight / 2 - (cellSize.y / 2) - 88;


        for (int column = 0; column < columnCount; column++)
        {
            int index = column;

            float xPos = startX + column * cellSize.x;
            float yPos = startY;

            GameObject cell = Utils.CreateTextButton(commonImages[((int)UICommon.LongButton)], new Vector2(xPos, yPos), new Vector2(0, 0), canvasObject.transform, actions[column]);
            cell.name = index.ToString();
            cell.GetComponent<Text>().text = buttonTitles[column];
            buttons.Add(cell);
        }
    }

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}
