using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public Animator animController;
    private Rigidbody rb;
    private RaycastHit hit;
    private Ray ray;
    public float landingDistance, jumpVelocity, speed, landedSpeed, minTargetDist, timeBetweenShots;
    public float particleSpeed;
    public Transform landingWatchDog;
    private Vector3 target;
    public Transform foreArmL, foreArmR, armL, armR, head, headCenter, aimTransform, forwardAimer;
    public Image reticle;
    private float dist, timer;
    private Vector3 lastValidCursorPosition, screenCenter;
    private Camera cam;
    public MuzzleScript muzzle;
    public int amountSimultaneousParticles, gunsDamage;
    public Rigidbody particle;
    private Queue<Rigidbody> particles;
    public Vector3 forward;
    public float shotForce;
    public int katanaDamage, life, totalLife;
    public RawImage lifeBar;
    private float minFill, maxFill;
    private AudioSource gunSound;
    public Transform introCamera;
    public bool dead = false;
    private float deadTimer = 4.0f;
    public static bool pause = false;
    public GameObject pauseMenu;
    private MusicScript music;

    void Start () {
        timer = -1;        
        transform.Translate(new Vector3(0.0f, -20.0f, 0.0f));
        particles = new Queue<Rigidbody>();
        for (int i = 0; i < amountSimultaneousParticles; ++i) particles.Enqueue(Instantiate<Rigidbody>(particle, transform.position, Quaternion.identity));
        transform.Translate(new Vector3(0.0f, 20.0f, 0.0f));
        animController = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);
        Cursor.visible = false;
        cam = Camera.main;
        forward = forwardAimer.position;
        maxFill = lifeBar.rectTransform.sizeDelta.x;
        minFill = 0;
        gunSound = GetComponent<AudioSource>();
        music = GameObject.FindObjectOfType<MusicScript>();
    }

    private void FixedUpdate()
    {
        if (!IntroScript.introFinished) return;
        if (life < 0) return;
            //Movemement        
        if (Input.GetKey(KeyCode.Space) 
            && (animController.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animController.GetCurrentAnimatorStateInfo(0).IsName("Walking")))
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
            animController.SetBool("onAir", true);
        }
        animController.SetBool("walking", false);
        if (!animController.GetCurrentAnimatorStateInfo(0).IsName("Landing")
            && !animController.GetCurrentAnimatorStateInfo(0).IsName("onAir"))
        {
            float speedY = -100;
            if (Input.GetKey(KeyCode.W))
            {
                speedY = rb.velocity.y;
                rb.velocity = new Vector3(rb.velocity.x, speedY, 1.0f * speed);                
                animController.SetBool("walking", true);
            }
            if (Input.GetKey(KeyCode.S))
            {
                speedY = rb.velocity.y;
                rb.velocity = new Vector3(rb.velocity.x, speedY, -1.0f * speed);                
                animController.SetBool("walking", true);
            }
            if (Input.GetKey(KeyCode.D))
            {
                speedY = rb.velocity.y;
                rb.velocity = new Vector3(1.0f * speed, speedY, rb.velocity.z);                
                animController.SetBool("walking", true);
            }
            if (Input.GetKey(KeyCode.A))
            {
                speedY = rb.velocity.y;
                rb.velocity = new Vector3(-1.0f * speed, speedY, rb.velocity.z);
                animController.SetBool("walking", true);
            }
            if (speedY > -100)
            {
                rb.velocity = rb.velocity.normalized * speed;
                rb.velocity = new Vector3(rb.velocity.x, speedY, rb.velocity.z);
            }
        }
    }
    // Update is called once per frame
    void Update () {        
        if (!IntroScript.introFinished) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pause)
            {
                pause = true;
                Time.timeScale = 0;                
                pauseMenu.SetActive(true);
                music.audioSource.Pause();
                MusicScript.paused = true;
            }
            else
            {
                pause = false;
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
                music.audioSource.UnPause();
                MusicScript.paused = false;
            }
        }
        //life Bar
        float currHealth = (float)life;
        float fill = currHealth / (float)totalLife;
        float width = fill * (maxFill - minFill);
        lifeBar.rectTransform.sizeDelta = new Vector2(width, lifeBar.rectTransform.sizeDelta.y);
        if (life < 0 && !dead)
        {
            Time.timeScale = 0.15f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;            
        }
        if (dead)
        {
            deadTimer -= Time.deltaTime;
            if (deadTimer < 0)
            {
                SceneManager.LoadScene("Lose", LoadSceneMode.Single);
            }
        }
        if (life < 0)
        {
            dead = true;
            animController.SetBool("dead", true);
            gunSound.Stop();
            return;
        }
        //orientation                
        screenCenter = cam.WorldToScreenPoint(headCenter.position);        
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            dist = Vector3.Distance(Input.mousePosition, screenCenter);            
            if (dist > minTargetDist && hit.collider.tag != "Player")
            {
                this.transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
                lastValidCursorPosition = Input.mousePosition;
                target = hit.point;
                reticle.transform.position = cam.WorldToScreenPoint(target);
            }            
        }

        //landing control
        if (animController.GetBool("onAir") && rb.velocity.y < 0
        && Physics.Raycast(landingWatchDog.position, landingWatchDog.position + new Vector3(0.0f, -10.0f, 0.0f), out hit))
        {
            if (hit.collider != null)
            {
                float distance = Vector3.Distance(transform.position, hit.point);
                if (distance < Mathf.Abs(rb.velocity.y) * landingDistance) animController.SetBool("onAir", false);
            }
        }

        //shooting
        if (Input.GetMouseButtonDown(0))
        {
            gunSound.Play();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunSound.Stop();
        }
        if (timer <= 0 && Input.GetMouseButton(0))
        {
            muzzle.shoot();
            timer = timeBetweenShots;
            Vector3 rayDirection = -(aimTransform.position - target);
            //Debug.DrawLine(target, target + new Vector3(0.0f, 10.0f, 0.0f), Color.green);
            if (Physics.Raycast(aimTransform.position, rayDirection, out hit)) {
                //Debug.DrawLine(hit.point, hit.point + new Vector3(0.0f, 10.0f, 0.0f), Color.red);
                //Debug.DrawLine(headCenter.position, rayDirection * 100, Color.blue);
                //Debug.Break();
                if (hit.collider.tag != "Ninja" && hit.collider.tag != "NinjaHead" && hit.collider.tag != "KatanaDamager")
                {                    
                    Rigidbody partRb = particles.Dequeue();
                    partRb.transform.position = hit.point;
                    partRb.velocity = hit.normal * particleSpeed;
                    Material mat = hit.collider.GetComponentInChildren<MeshRenderer>().material;
                    partRb.GetComponent<MeshRenderer>().material = mat;
                    particles.Enqueue(partRb);
                }
                if (hit.collider.tag == "Ninja")
                {                    
                    life = (int)Mathf.Clamp(life + ((float)gunsDamage * 0.2f), -10, totalLife);
                    hit.collider.GetComponent<NinjaScript>().recieveDamage(hit.point, rayDirection, gunsDamage);
                }
                else if (hit.collider.tag == "Drone")
                {
                    life = (int)Mathf.Clamp(life + ((float)gunsDamage * 0.2f), -10, totalLife);
                    hit.collider.GetComponent<DroneScript>().recieveDamage(hit.point, rayDirection, gunsDamage);
                }
                else if (hit.collider.tag == "NinjaHead")
                {
                    life = (int)Mathf.Clamp(life + ((float)gunsDamage * 0.5f), -10, totalLife);
                    hit.collider.GetComponentInParent<NinjaScript>().recieveDamage(hit.point, rayDirection, -1);                    
                }
                else if (hit.collider.tag == "OfficeAssets")
                {
                    hit.collider.GetComponent<Rigidbody>().AddExplosionForce(shotForce, hit.point - rayDirection, 10);
                }
                else if (hit.collider.tag == "Compilator")
                {
                    hit.collider.GetComponent<CompilatorScript>().recieveDamage(gunsDamage);
                }
            }
        }
        if (timer > 0) timer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "KatanaDamager")
        {
            life -= katanaDamage;
        }
    }



    private void LateUpdate()
    {
        Vector3 tg = Vector3.zero;
        //aim             
        if (!IntroScript.introFinished)
        {
            tg = introCamera.position;
        }
        else
        {         
            ray = cam.ScreenPointToRay(reticle.rectTransform.position);
            if (Physics.Raycast(ray, out hit))
            {
                tg = hit.point;
            }
        }
        if (life > 0) head.LookAt(tg);
        else tg = new Vector3(tg.x, 2.0f, tg.z);
        armL.LookAt(tg);
        armL.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        armL.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        armR.LookAt(tg);
        armR.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        foreArmL.LookAt(tg);
        foreArmL.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        foreArmL.Rotate(new Vector3(0.0f, -130.0f, 0.0f));
        foreArmR.LookAt(tg);
        foreArmR.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        foreArmR.Rotate(new Vector3(0.0f, -120.0f, 0.0f));
        
    }
}
