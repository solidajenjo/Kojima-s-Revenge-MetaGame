using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompilatorScript : MonoBehaviour {

    private Material screenMat;
    private float timer;
    private Vector2 texOffset;    
    public float minTime, maxTime, texMovement;
    public int life, totalLife;
    private float yScale;
    public GameObject lifeBar;
    private bool destroyed = false;
    public bool active = false;
    public ParticleSystem parts;
    public GameObject ligth;
    // Use this for initialization

    private void Start()
    {
        ligth.SetActive(false);
    }
    public void Init () {
        timer = 0.0f;
        texOffset = Vector2.zero;
        yScale = lifeBar.transform.localScale.z;
        screenMat = (GetComponent<MeshRenderer>()).materials[1];
        life = totalLife;
        ligth.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        ligth.SetActive(false);
        if (destroyed || !active) return;
        ligth.SetActive(true);
        timer -= Time.deltaTime;
		if (timer <= 0.0f)
        {
            timer = Random.Range(minTime, maxTime);
            texOffset -= new Vector2(0.0f, texMovement);
            screenMat.SetTextureOffset("_MainTex", texOffset);            
        }
	}

    public void recieveDamage(int damage)
    {
        if (destroyed || !active) return;
        IAScript.compilatorsLife -= damage;
        life -= damage;
        if (life <= 0)
        {
            destroyed = true;
            active = false;
            parts.Play();
            ligth.SetActive(false);
        }
        float newLife = ((float)life * yScale) / (float)totalLife;
        lifeBar.transform.localScale = new Vector3(lifeBar.transform.localScale.x, lifeBar.transform.localScale.y, newLife);
    }
}
