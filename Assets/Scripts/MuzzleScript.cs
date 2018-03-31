using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleScript : MonoBehaviour {

    public GameObject muzzleLeft, muzzleRight;
    private float timer;
    public float duration;
	// Use this for initialization
	void Start () {
        timer = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (timer > 0)
        {
            muzzleLeft.gameObject.SetActive(true);
            muzzleRight.gameObject.SetActive(true);
            timer -= Time.deltaTime;
        }
        else
        {
            muzzleLeft.gameObject.SetActive(false);
            muzzleRight.gameObject.SetActive(false);
        }
    }

    public void shoot()
    {
        timer = duration;
    }
}
