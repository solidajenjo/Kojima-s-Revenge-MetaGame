using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour {

    public float incrementPerLoop;
    public float maxPitch;
    public static bool start = false;
    public static bool paused = false;
    public AudioSource audioSource;


	void Start () {        
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!IntroScript.introFinished) return;
        if (start && !paused && !audioSource.isPlaying)
        {
            audioSource.pitch = Mathf.Clamp(audioSource.pitch + incrementPerLoop, 1.0f, maxPitch);
            audioSource.Play();
            IAScript.instantiateDrone();
            IAScript.instantiateNinja();
        }
	}
}
