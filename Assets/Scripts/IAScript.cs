using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IAScript : MonoBehaviour {

    public float minDistanceBetweenNinjas;
    public float ninjaAttackDistance;
    public float compilationTime;
    private float maxFillComp, minFillComp;
    public RectTransform compilationBar;
    private float timer = 0;
    public ParticleSystem explosion;
    private CompilatorScript[] compilators;
    private HashSet<int> compilatorsUsed;
    private int lastCompilatorActive;
    public static float compilatorsLife;
    public float compilationTotalDamage;
    private float maxFillDamage, minFillDamage;
    public RectTransform damageBar;
    // Use this for initialization
    void Start () {
        NinjaScript.minDistance = minDistanceBetweenNinjas;
        NinjaScript.attackDistance = ninjaAttackDistance;
        NinjaScript.updateNinjasOnScreen();
        NinjaScript.initSpawns();
        DroneScript.initSpawns();
        DroneCannonScript.initRockets();
        DroneCannonScript.timeBetweenRockets = 5.0f;
        RocketScript.rocketSpeed = 2.5f;
        RocketScript.hiddenLocation = new Vector3(100, 100, 100);
        RocketScript.explosionForce = 500.0f;
        RocketScript.explosionRadius = 1.0f;
        RocketScript.initRbs();
        RocketScript.initExplosions(explosion);
        compilators = GameObject.FindObjectsOfType<CompilatorScript>();        
        lastCompilatorActive = Random.Range(0, compilators.Length);
        compilators[lastCompilatorActive].Init();
        compilators[lastCompilatorActive].active = true;
        compilatorsUsed = new HashSet<int>();
        compilatorsUsed.Add(lastCompilatorActive);
        maxFillComp = compilationBar.sizeDelta.x;
        minFillComp = 0;

        maxFillDamage = damageBar.sizeDelta.x;
        minFillDamage = 0;
        compilatorsLife = compilationTotalDamage;
    }
	
	// Update is called once per frame
	void Update () {
        if (!IntroScript.introFinished) return;
        if (compilatorsLife <= 0)
        {
            SceneManager.LoadScene("Win", LoadSceneMode.Single);
        }
        if (timer >= compilationTime)
        {
            SceneManager.LoadScene("Lose", LoadSceneMode.Single);
        }
        timer += Time.deltaTime;

        float fill = timer / compilationTime;
        float width = fill * (maxFillComp - minFillComp);
        compilationBar.sizeDelta = new Vector2(width, compilationBar.sizeDelta.y);        
        fill = compilatorsLife / compilationTotalDamage;
        width = fill * (maxFillDamage - minFillDamage);
        damageBar.sizeDelta = new Vector2(width, damageBar.sizeDelta.y);

        NinjaScript.checkDistances();
        if (!compilators[lastCompilatorActive].active)
        {
            int newCompilator = lastCompilatorActive;
            while (compilatorsUsed.Contains(newCompilator))
            {
                newCompilator = Random.Range(0, compilators.Length);
            }
            compilators[newCompilator].Init();
            compilators[newCompilator].active = true;
            compilatorsUsed.Add(newCompilator);
            lastCompilatorActive = newCompilator;
        }
    }

    public static void instantiateNinja()
    {
        GameObject ninja = (GameObject.FindObjectOfType<NinjaScript>()).gameObject;
        GameObject newNinja = Instantiate(ninja, new Vector3(100,100,100) , Quaternion.identity);
        newNinja.GetComponent<NinjaScript>().dead = true;
        NinjaScript.updateNinjasOnScreen();
        
    }
    public static void instantiateDrone()
    {
        GameObject drone = (GameObject.FindObjectOfType<DroneScript>()).gameObject;
        GameObject newDrone = Instantiate(drone, new Vector3(200, 200, 200), Quaternion.identity);
        newDrone.GetComponent<DroneScript>().dead = true;        
    }
}
