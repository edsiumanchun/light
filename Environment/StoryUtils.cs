using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

// VictoryStoryEvent.cs
public interface IStoryEvent
{
    IEnumerator Play(DialogBox dialogBox);
}

public class WelcomeStoryEvent : IStoryEvent
{
    public IEnumerator Play(DialogBox dialogBox)
    {
        dialogBox.Show("Character 1: Welcome to the world of floating plates!");
        yield return new WaitForSeconds(2.0f);
        dialogBox.Show("Character 2: It's so beautiful and peaceful here.");
        yield return new WaitForSeconds(5.0f);
        dialogBox.Hide();
    }
}

public class VictoryStoryEvent : IStoryEvent
{
    public IEnumerator Play(DialogBox dialogBox)
    {
        dialogBox.Show("Character 1: We did it! We defeated the boss!");
        yield return new WaitForSeconds(2.0f);
        dialogBox.Show("Character 2: Yes, and now we can enjoy the peace once again.");
        yield return new WaitForSeconds(5.0f);
        dialogBox.Hide();
    }
}

public class DialogBox : MonoBehaviour
{
    public Text messageText;
    public GameObject dialogBox;

    public void Show(string message)
    {
        messageText.text = message;
        dialogBox.SetActive(true);
    }

    public void Hide()
    {
        dialogBox.SetActive(false);
    }
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
}


public class Story : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
