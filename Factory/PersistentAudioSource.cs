using UnityEngine;

public class PersistentAudioSource : MonoBehaviour
{
    private int AudioSourceIndex = -1;
    private static PersistentAudioSource instance;

    public static PersistentAudioSource Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PersistentAudioSource>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("PersistentAudioSource");
                    instance = obj.AddComponent<PersistentAudioSource>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void PlayAudio(int sourceIndex)
    {
        if(AudioSourceIndex != sourceIndex) {
            AudioSourceIndex = sourceIndex;
            audioSource.volume = 0.5f;
            audioSource.clip = DataHolder.audios[sourceIndex];
            audioSource.Play();
        }
    }
    
}