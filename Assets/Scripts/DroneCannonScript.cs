using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCannonScript : MonoBehaviour {

    private Player player;
    public static float timeBetweenRockets;
    public static Queue<RocketScript> rockets;
    public static RocketScript rocket;
    private DroneScript drone;
    public Transform rocketLauncher;
    private float timer = 0;

    public static void initRockets()
    {
        rocket = FindObjectOfType<RocketScript>();
        Vector3 spawnPos = new Vector3(2000.0f, -50.0f, 2000.0f);
        rockets = new Queue<RocketScript>();
        for (int i = 0; i < 10; ++i) rockets.Enqueue(Instantiate<RocketScript>(rocket, spawnPos, Quaternion.identity));
        
    }
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<Player>();
        drone = GetComponentInParent<DroneScript>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!IntroScript.introFinished) return;
        transform.LookAt(player.transform.position);
        timer -= Time.deltaTime;
        if (timer <= 0 && !drone.dead && !player.dead)
        {
            RocketScript roc = rockets.Dequeue();
            roc.shoot(rocketLauncher.position, player.transform.position);            
            timer = timeBetweenRockets;
            rockets.Enqueue(roc);
        }
	}
}
