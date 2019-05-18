using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class PlayerVoice : MonoBehaviour {

    /*
     * THE REASON THIS DOESN'T WORK IS BECAUSE THE CLIP NEEDS TO RECORD THE WHOLE MESSAGe
     * HAVE A GOOGLE OF "UNITY OPUS AND TRY TO GET THAT TO WORK"
     **/

    public Text audioText;

    private AudioSource source;
    private AudioClip clip;

    private int maxPos = 0;
    private int currentPos = 0;

    private float[] currentData;

    void Start() {
        source = GetComponent<AudioSource>();
        clip = AudioClip.Create("PlayerMic", 160000, 1, 16000, false);
        //clip.loadType = AudioClipLoadType.Streaming;
        //clip.
        currentData = new float[clip.samples * clip.channels];
        clip.GetData(currentData, 0);

        source.clip = clip;
        source.loop = true;

        source.Play();
        source.Pause();
    }

    public void AddData(float[] data) {
        // Update clips data to the one we just received via packets
        //Debug.Log(currentData.Length + " | " + startingPosition);
        for (int x = 0; x < data.Length; x++) {
            currentData[maxPos + x] = data[x];
        }
 
        // Set the data back to the clip
        clip.SetData(currentData, 0);

        maxPos = maxPos + data.Length;
    }

    void Update() {
        currentPos = source.timeSamples;

        int difference = maxPos - currentPos;


        if (difference > 5000) {
            source.UnPause();
        }
        else if (difference < 0) {
            source.UnPause();
        }
        else if (difference <= 2000) {
            source.Pause();
        }
        

        audioText.text = "Time: " + source.timeSamples;
    }
}
