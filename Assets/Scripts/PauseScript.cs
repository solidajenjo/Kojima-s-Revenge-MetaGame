using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour {

    public Image[] menuPos;
    private int menuPosPointer = 0;
    private MusicScript music;
	// Use this for initialization
	void Start () {
        music = GameObject.FindObjectOfType<MusicScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Player.pause)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (menuPosPointer < menuPos.Length - 1)
                {
                    menuPos[menuPosPointer++].enabled = false;
                    menuPos[menuPosPointer].enabled = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (menuPosPointer > 0)
                {
                    menuPos[menuPosPointer--].enabled = false;
                    menuPos[menuPosPointer].enabled = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                switch (menuPosPointer)
                {
                    case 0:
                        Player.pause = false;
                        Time.timeScale = 1;
                        MusicScript.paused = false;
                        music.audioSource.UnPause();
                        this.gameObject.SetActive(false);
                        break;
                    case 1:
                        Time.timeScale = 1;
                        Scene scene = SceneManager.GetActiveScene();
                        SceneManager.LoadScene(scene.name);
                        break;
                    case 2:
                        Time.timeScale = 1;
                        Application.Quit();
                        break;
                }
            }
        }
	}
}
