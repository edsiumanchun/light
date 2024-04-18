using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
     void Start()
    {
        this.gameObject.AddComponent<VideoPlayer>();
    }

    public void StartVideo(string videoPath)
    {
        
        VideoPlayer videoPlayer = this.gameObject.GetComponent<VideoPlayer>();
        // Attach the VideoPlayer component to the GameObject
        
        // Set the video source
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = "file://" + Application.dataPath + "/Resources/Videos/" + videoPath + ".mp4";
        
        // Set other properties as needed
        videoPlayer.playOnAwake = true;
        videoPlayer.isLooping = false;
        
        // Play the video
        videoPlayer.Play();
    }
}
