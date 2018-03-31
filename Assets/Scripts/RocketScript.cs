using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour {

    private Rigidbody rb;
    public static float rocketSpeed, explosionForce, explosionRadius;
    public static Vector3 hiddenLocation;
    private bool shot = false;
    public static Queue<ParticleSystem> explosions;
    public float rocketDamage;
    private Player player;
    private Vector3 target;
    private AudioSource explosionSound;

    public static Rigidbody[] rbs;

    public static void initExplosions(ParticleSystem explosion)
    {
        explosions = new Queue<ParticleSystem>();
        for (int i = 0; i < 10; ++i) explosions.Enqueue(Instantiate<ParticleSystem>(explosion, new Vector3(500,500,500), Quaternion.identity));
    }
    public static void initRbs()
    {
        rbs = GameObject.FindObjectsOfType<Rigidbody>();
    }
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<Player>();
        explosionSound = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!IntroScript.introFinished) return;
        if (shot)
        {
            transform.LookAt(target);
            if (Vector3.Distance(transform.position,target) < 0.1f) explode();
            rb.velocity = transform.forward * rocketSpeed;
        }        
    }

    public void shoot(Vector3 position, Vector3 target)
    {
        this.transform.position = position;
        this.target = target + new Vector3(0.0f, 0.5f, 0.0f);        
        shot = true;
    }
    private void explode()
    {
        shot = false;
        ParticleSystem explosion = explosions.Dequeue();
        explosion.transform.position = transform.position;
        explosion.Play();
        explosionSound.Play();
        explosions.Enqueue(explosion);
        foreach (Rigidbody rb in rbs)
        {
            
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            if (rb.tag == "Player")
            {
                int damage = (int)(rocketDamage / Vector3.Distance(transform.position, rb.transform.position));
                if (damage > rocketDamage) player.life -= damage;                
            }
        }
        rb.transform.position = hiddenLocation;
        rb.velocity = Vector3.zero;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!IntroScript.introFinished) return;
        if (collision.collider.tag != "Drone")
        {
            explode();
        }
    }
}
