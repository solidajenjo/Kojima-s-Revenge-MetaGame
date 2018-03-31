using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NinjaScript : MonoBehaviour {

    public bool dead = false;
    public float incrementOfSlice, deadExpansion;
    public GameObject bloodParticles;
    private static Queue<GameObject> bloodFonts;
    public int bloodFontsAmount, lifeAmount;
    private int life;
    private float slice = 0;
    private Player player;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    private Animator animController;

    public bool jumping = false, falling = false;
    private Vector3 startSize;
    private bool jumpToPlayer = false;

    public SkinnedMeshRenderer[] materials;

    //public static float waitingTime;
    public static float minDistance;
    public static float attackDistance;
    public static NinjaScript[] ninjas;
    public static Vector3[] spawns;

    public static void initSpawns()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("NinjaSpawn");
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
        while(spawnOnScreen.x >= 0 && spawnOnScreen.x <= 1 && spawnOnScreen.y >= 0 && spawnOnScreen.y <= 1)
        {
            spawn = spawns[Random.Range(0, spawns.Length)];
            spawnOnScreen = Camera.main.WorldToViewportPoint(spawn);
        }
        return spawn;
    }

    public static void updateNinjasOnScreen()
    {
        ninjas = FindObjectsOfType<NinjaScript>();
    }
	// Use this for initialization
	void Start () {   
        startSize = transform.localScale;
        life = lifeAmount;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animController = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<Player>();
        bloodFonts = new Queue<GameObject>();
        for (int i = 0; i < bloodFontsAmount; ++i)
        {
            bloodFonts.Enqueue(Instantiate<GameObject>(bloodParticles, Vector3.zero, Quaternion.identity));
        }
	}

    private void FixedUpdate()
    {
        if (!IntroScript.introFinished) return;
        if (animController.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Armature|PrepareJump" && !jumping && !falling && !dead && !player.dead)
        {
            if (!jumpToPlayer) rb.velocity = new Vector3(Random.Range(-5.0f, 5.0f), 5.0f, Random.Range(-5.0f, 5.0f));
            else
            {
                transform.LookAt(player.transform.position);
                rb.velocity = new Vector3(transform.forward.x * 5.0f, 5.0f, transform.forward.z * 5.0f);
                
            }
            jumpToPlayer = false;
            if (navMeshAgent.isOnNavMesh) navMeshAgent.enabled = false;
            jumping = true;            
        }
        if (Vector3.Distance(transform.position, player.transform.position) < attackDistance && !player.dead)
        {
            transform.LookAt(player.transform.position);
            if (navMeshAgent.isOnNavMesh) navMeshAgent.isStopped = true;
            transform.LookAt(player.transform);
            animController.SetTrigger("Attack");
        }
        if (transform.position.y < -3.936235f)
        {
            dead = true;
        }
    }
    // Update is called once per frame
    void Update () {
        if (!IntroScript.introFinished) return;
        if (rb.velocity.magnitude > 50) rb.velocity = Vector3.zero;

        if (life <= 0 || player.dead)
        {
            dead = true;
            animController.SetBool("Dead", true);
            animController.SetBool("Walking", false);
            if (navMeshAgent.isOnNavMesh) navMeshAgent.isStopped = true;
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;                
            }
        }
        else
        {
            if (!jumping && !falling) 
            {
                if (navMeshAgent.isOnNavMesh)
                {
                    navMeshAgent.SetDestination(player.transform.position);                    
                    transform.LookAt(navMeshAgent.destination);
                    animController.SetBool("Walking", true);
                    navMeshAgent.isStopped = false;
                    if (player.transform.position.y > 0.5f /*&& Vector3.Distance(transform.position, player.transform.position) < 3.0f*/)
                    {                        
                        wait();
                        jumpToPlayer = true;
                    }                    
                }
            }            
        }

        if (dead)
        {
            die();
            slice += incrementOfSlice;
        }
        if (slice == 1.0f) dead = false;
        if (jumping && rb.velocity.y <= 0)
        {
            falling = true;
            jumping = false;            
            rb.velocity = new Vector3(rb.velocity.x, -0.5f, rb.velocity.z);
        }
        if (falling && Mathf.Abs(rb.velocity.y) < 0.1f)
        {            
            falling = false;
            navMeshAgent.enabled = true;
            if (navMeshAgent.isOnNavMesh)
            {                
                animController.SetTrigger("JumpEnd");
            }
            else
            {
                animController.SetTrigger("Jump");
            }
        }
    }

    public void die()
    {
        foreach(SkinnedMeshRenderer ren in materials){
            foreach (Material mat in ren.materials)
            {
                mat.SetFloat(Shader.PropertyToID("_SliceAmount"), slice);
            }
        }
        if (slice > 1 && !player.dead)
        {            
            transform.position = getRandomSpawnPos();
            slice = 0.0f;
            foreach (SkinnedMeshRenderer ren in materials)
            {
                foreach (Material mat in ren.materials)
                {
                    mat.SetFloat(Shader.PropertyToID("_SliceAmount"), slice);
                }
            }
            life = lifeAmount;

            transform.localScale = startSize;
            dead = false;
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;                
            }
            animController.SetBool("Walking", true);
            animController.SetBool("Dead", false);
            return;
        }
        transform.localScale = transform.localScale * deadExpansion;
    }

    public void recieveDamage(Vector3 hitPos, Vector3 hitDirection, int damage)
    {
        if (dead) return;
        if (damage < 0) life = 0;
        else life -= damage;
        GameObject bloodPart = bloodFonts.Dequeue();
        bloodPart.transform.position = hitPos;
        bloodPart.transform.LookAt(hitPos + hitDirection);
        bloodPart.GetComponent<ParticleSystem>().Play();
        bloodFonts.Enqueue(bloodPart);
    }

    public static void checkDistances()
    {
        for (int i = 0; i < NinjaScript.ninjas.Length; ++i)
        {
            for (int j = i + 1; j < NinjaScript.ninjas.Length; ++j)
            {
                if (Vector3.Distance(NinjaScript.ninjas[i].transform.position, NinjaScript.ninjas[j].transform.position) < minDistance
                    && !NinjaScript.ninjas[i].jumping)
                {                    
                    NinjaScript.ninjas[i].wait();
                }
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "TableSeparator")
        {
            wait();           
        }
    }
    public void wait()
    {
        animController.SetTrigger("Jump");
        if (navMeshAgent.isOnNavMesh) navMeshAgent.isStopped = true;

    }
}
