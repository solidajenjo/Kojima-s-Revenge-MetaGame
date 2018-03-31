using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public Transform playerTransform;
    private Vector3 dispOffset;

	void Start () {
        dispOffset = playerTransform.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 transformPosition = playerTransform.position - dispOffset;
        transform.position = new Vector3(Mathf.Clamp(transformPosition.x, -20.3f, 19.4f),
            transformPosition.y, Mathf.Clamp(transformPosition.z, -22.8f, 21.4f));
        if (transformPosition.x == transform.position.x && transformPosition.z == transform.position.z) transform.LookAt(playerTransform);        
	}
}
