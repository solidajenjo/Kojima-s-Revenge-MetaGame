using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneScript : MonoBehaviour {

    public float minDistance;
    public float speed, incrementOfSlice;
    public int lifeAmount;
    public int life;
    public bool dead = false;
    private Player player;
    private Rigidbody rb;
    public float slice = 0;
    public MeshRenderer[] materials;
    public static Vector3[] spawns;

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody>();
        life = lifeAmount;
    }

    public static void initSpawns()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("DroneSpawn");
        spawns = new Vector3[gos.Length];
        int i = 0;
        foreach (GameObject go in gos)
        {
            spawns[i++] = go.transform.position;
        }
    }
    public static Vector3 getRandomSpawnPos()
    {
        Vector3 spawn = spawns[Random.Range(0, spawns.Length)];
        Vector3 spawnOnScreen = Camera.main.WorldToViewportPoint(spawn);
        while (spawnOnScreen.x >= 0 && spawnOnScreen.x <= 1 && spawnOnScreen.y >= 0 && spawnOnScreen.y <= 1)
        {
            spawn = spawns[Random.Range(0, spawns.Length)];
            spawnOnScreen = Camera.main.WorldToViewportPoint(spawn);
        }
        return spawn;
    }

    private void FixedUpdate()
    {
        if (!IntroScript.introFinished) return;
        if (Vector3.Distance(transform.position, player.transform.position) > minDistance && !dead && !player.dead)
        {
            rb.velocity = transform.forward * speed;
        }
        else rb.velocity = Vector3.zero;
    }
    // Update is called once per frame
    void Update () {
        if (!IntroScript.introFinished) return;
        if ((life < 0 && !dead) || player.dead)
        {
            dead = true;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;
            rb.AddExplosionForce(Random.Range(1,500), transform.position + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)), 100);
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
        }
        if (dead)
        {            
            slice += incrementOfSlice;
            die();            
        }
        else
        {
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        }
	}
    public void recieveDamage(Vector3 hitPos, Vector3 hitDirection, int damage)
    {        
        if (dead) return;
        if (damage < 0) life = 0;
        else life -= damage;       
    }
    public void die()
    {
        foreach (MeshRenderer ren in materials)
        {
            foreach (Material mat in ren.materials)
            {
                mat.SetFloat(Shader.PropertyToID("_SliceAmount"), slice);
            }
        }
        if (slice > 1  && !player.dead)
        {
            transform.position = getRandomSpawnPos();
            slice = 0.0f;
            foreach (MeshRenderer ren in materials)
            {
                foreach (Material mat in ren.materials)
                {
                    mat.SetFloat(Shader.PropertyToID("_SliceAmount"), slice);
                }
            }
            life = lifeAmount;            
            dead = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }
            return;
        }
    }
}
