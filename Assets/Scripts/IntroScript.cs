using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroScript : MonoBehaviour {

    public float imageTime;
    public float cameraSpeed;
    public float cameraPanDuration;
    public Image[] images;
    public Image[] textImages;
    private int textImagePointer = 0;
    public GameObject codecOverlay;
    private int status;
    private float timer;
    private RectTransform briefRt;
    private AudioSource briefMusic;
    public static bool introFinished;
    private Vector3 textInitial, textFinal, initialIntroCameraPos;
    public RawImage background;
    public Camera mainCamera, introCamera;
    public AudioSource imBack, codecSound;
    public GameObject innerLeft, innerRight, outterLeft, outterRight, HUD;
    private float startCameraMoveTime;
    private Player player;
    private bool skipped;

    // Use this for initialization
    void Start () {
        introFinished = false;
        player = GameObject.FindObjectOfType<Player>();
        ElevatorScript.innerLeft = innerLeft;
        ElevatorScript.innerRight = innerRight;
        ElevatorScript.outterLeft = outterLeft;
        ElevatorScript.outterRight = outterRight;
        status = 0;
        timer = imageTime;
        briefMusic = GetComponent<AudioSource>();
        briefRt = images[2].GetComponent<RectTransform>();
        textInitial = briefRt.position;
        float y = Screen.height / 1080.0f;
        textFinal = new Vector3(textInitial.x, 1330.0f * y, textInitial.z);
        HUD.SetActive(false);
        skipped = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) && !introFinished && !skipped)
        {
            status = 3;
            skipped = true;
            briefMusic.Stop();
            images[0].enabled = false;
            images[1].enabled = false;
            background.enabled = false;
            images[2].enabled = false;
        }
		switch (status)
        {
            case 0:
                timer -= Time.deltaTime;
                Camera.SetupCurrent(introCamera);
                if (timer < 0)
                {
                    status = 1;
                    images[0].enabled = false;
                    images[1].enabled = true;
                    timer = imageTime;
                }
                break;
            case 1:
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    status = 2;
                    images[1].enabled = false;
                    images[2].enabled = true;                    
                    briefMusic.Play();
                }
                break;
            case 2:
                float t = briefMusic.time / briefMusic.clip.length;
                Debug.Log(t + " " + Time.time);
                briefRt.position = Vector3.Lerp(textInitial, textFinal, t);
                if (!briefMusic.isPlaying)
                {                    
                    background.enabled = false;
                    images[2].enabled = false;
                    status = 3;
                }
                break;
            case 3:
                if (!ElevatorScript.opening && !ElevatorScript.opened) ElevatorScript.open();
                if (ElevatorScript.opened) status = 4;
                break;
            case 4:
                imBack.Play();
                status = 5;
                break;
            case 5:
                introCamera.transform.Translate(introCamera.transform.forward * cameraSpeed * Time.deltaTime);
                if (!imBack.isPlaying)
                {
                    status = 6;
                    startCameraMoveTime = Time.time;
                    initialIntroCameraPos = Camera.current.transform.position;
                    player.animController.SetBool("walking", true);
                }
                break;
            case 6:
                float f = (Time.time - startCameraMoveTime) / cameraPanDuration;
                Camera.current.transform.position = Vector3.Slerp(initialIntroCameraPos, mainCamera.transform.position, f);
                player.gameObject.transform.position += player.gameObject.transform.forward * 3 * Time.deltaTime;
                Camera.current.transform.LookAt(player.transform.position);

                if (f > 0.95) status = 7;
                break;
            case 7:                
                ElevatorScript.close();
                player.animController.SetBool("walking", false);
                status = 8;
                codecSound.Play();
                break;
            case 8:
                if (!codecSound.isPlaying) status = 9;
                break;
            case 9:
                codecOverlay.SetActive(true);
                textImages[textImagePointer].gameObject.SetActive(true);
                textImages[textImagePointer].enabled = true;
                status = 10;
                break;
            case 10:
                if (Input.anyKeyDown)
                {
                    textImages[textImagePointer].gameObject.SetActive(false);
                    textImages[textImagePointer++].enabled = false;
                    if (textImagePointer == textImages.Length)
                    {
                        status = 1000;
                        break;
                    }
                    textImages[textImagePointer].gameObject.SetActive(true);
                    textImages[textImagePointer].enabled = true;
                    
                }
                break;
            case 1000:
                foreach (Image i in images) i.enabled = false;
                foreach (Image i in textImages) i.enabled = false;
                background.enabled = false;
                codecOverlay.SetActive(false);
                introFinished = true;
                introCamera.enabled = false;
                MusicScript.start = true;
                HUD.SetActive(true);
                break;
        }
	}
}
