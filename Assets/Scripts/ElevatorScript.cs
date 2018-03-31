using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour {

    public static GameObject innerLeft, innerRight, outterLeft, outterRight;
    public float totalDisp, speed;
    public GameObject doorBlocker;
    public static bool opening, closing, opened;
    private static float initialX;
	// Use this for initialization
	void Start () {
		
	}

    private void Awake()
    {
        opened = false;
        closing = false;
        opened = false;
        doorBlocker.SetActive(false);
    }
    public static void open()
    {
        opening = true;
        initialX = outterLeft.transform.position.x;
    }

    public static void close()
    {
        closing = true;
        initialX = outterLeft.transform.position.x;
    }
	// Update is called once per frame
	void Update () {
        if (opening)
        {
            if (Mathf.Abs(outterLeft.transform.position.x - initialX) < totalDisp)
            {
                outterLeft.transform.Translate(new Vector3(speed * Time.deltaTime, 0.0f, 0.0f));
                outterRight.transform.Translate(new Vector3(-speed * Time.deltaTime, 0.0f, 0.0f));
                innerLeft.transform.Translate(new Vector3(speed * Time.deltaTime * 0.75f, 0.0f, 0.0f));
                innerRight.transform.Translate(new Vector3(-speed * Time.deltaTime * 0.75f, 0.0f, 0.0f));
            }
            else
            {
                opening = false;
                opened = true;
            }
        }
        if (closing)
        {
            doorBlocker.SetActive(true);
            if (Mathf.Abs(outterLeft.transform.position.x - initialX) < totalDisp)
            {
                outterLeft.transform.Translate(new Vector3(-speed * Time.deltaTime, 0.0f, 0.0f));
                outterRight.transform.Translate(new Vector3(speed * Time.deltaTime, 0.0f, 0.0f));
                innerLeft.transform.Translate(new Vector3(-speed * Time.deltaTime * 0.75f, 0.0f, 0.0f));
                innerRight.transform.Translate(new Vector3(speed * Time.deltaTime * 0.75f, 0.0f, 0.0f));
            }
            else closing = false;
         
        }
	}
}
