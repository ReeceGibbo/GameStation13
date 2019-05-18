using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio;
using NAudio.Wave;
using System;
using UnityEngine.UI;
using System.IO;
using System.Runtime.InteropServices;
using UnityOpus;

public class NAudioTest : MonoBehaviour {

    public Text audioText;

    public AudioSource source;
    private AudioClip clip;

    WaveIn sourceStream = null;

    int previousPoint = 0;

    bool addNewData = false;
    float[] newData;

    // Opus Codec
    Encoder encoder;
    Decoder decoder;

    void Start() {
        // Opus Codec
        encoder = new Encoder(SamplingFrequency.Frequency_24000, NumChannels.Mono, OpusApplication.VoIP);
        encoder.Bitrate = 24000;
        encoder.Complexity = 10;
        encoder.Signal = OpusSignal.Voice;

        decoder = new Decoder(SamplingFrequency.Frequency_24000, NumChannels.Mono);

        // Create SOURCE CLIP STUFF
        clip = AudioClip.Create("MyMic", 24000, 1, 24000, false);
        source.loop = true;
        source.clip = clip;
        source.Play();

        // Create MICROPHONE INPUT
        sourceStream = new WaveIn();
        sourceStream.WaveFormat = new WaveFormat(24000, 1);

        sourceStream.DataAvailable += new EventHandler<WaveInEventArgs>(sourceStream_DataAvaliable);

        sourceStream.StartRecording();
    }

    void sourceStream_DataAvaliable(object sender, WaveInEventArgs e) {
        newData = new float[e.BytesRecorded / 2];

        // Convert to samples - 16bit
        int newDataIndex = 0;
        for (int index = 0; index < e.BytesRecorded; index += 2) {
            short sample = (short) ((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);

            float sample32 = sample / 32768f;

            newData[newDataIndex] = sample32;
            newDataIndex++;
        }

        addNewData = true;
    }

    private void OnApplicationPause(bool pause) {
        if (pause) {
            sourceStream.StopRecording();
            source.Pause();
        } else {
            if (sourceStream != null) {
                sourceStream.StartRecording();
                source.UnPause();
            }
        }
    }

    private void OnApplicationQuit() {
        sourceStream.StopRecording();
        sourceStream.Dispose();
    }

    void Update() {
        
        if (addNewData) {
            addNewData = false;

            float[] clipData = new float[clip.channels * clip.samples];
            clip.GetData(clipData, 0);

            // CONVERSTION CODEC STUFF
            byte[] OPUS = ConvertWAVtoOPUS(newData);

            float[] BACK = ConvertOPUStoWAV(OPUS);

            Debug.Log("WAV: " + (newData.Length * 4) + " | OPUS: " + OPUS.Length + " | WAV AGAIN: " + (BACK.Length * 4));

            newData = BACK;

            for (int x = 0; x < newData.Length; x++) {
                if ((x + previousPoint) >= clipData.Length) {
                    // We need to do a new thingy CLIP
                    previousPoint = -x;
                }

                clipData[x + previousPoint] = newData[x];
            }

            clip.SetData(clipData, 0);

            previousPoint += newData.Length;





            



        }

        audioText.text = "Source Time: " + source.timeSamples + "\n" + "Previous Point: " + previousPoint;

        int difference = previousPoint - source.timeSamples;

        if (difference > 200) {
            source.UnPause();
        }
        else if (difference < 0) {
            source.UnPause();
        }
        else if (difference < 100) {
            source.Pause();
        }
        
    }

    
    
    public byte[] ConvertWAVtoOPUS(float[] wav) {
        byte[] opus = new byte[wav.Length * 4];
        int length = encoder.Encode(wav, opus);

        byte[] finalOpus = new byte[length];
        for (int x = 0; x < length; x++) {
            finalOpus[x] = opus[x];
        }

        return finalOpus;
    }

    public float[] ConvertOPUStoWAV(byte[] opus) {
        float[] wav = new float[20000];
        int length = decoder.Decode(opus, opus.Length, wav);

        float[] newWav = new float[length];
        for (int x = 0; x < length; x++) {
            newWav[x] = wav[x];
        }

        return newWav;
    }
    
    

}
