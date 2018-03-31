using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SceneManager.LoadScene("Office", LoadSceneMode.Single);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
