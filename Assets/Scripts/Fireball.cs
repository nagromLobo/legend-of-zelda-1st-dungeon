using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour {
    public Vector3 normalizedPath;
    public float distance = 9.0f;
    public float timeToTravel = 2.0f;

    private float startTime = 0.0f;
    private Vector3 endPosition;
    private Vector3 startPosition;

	// Use this for initialization
	void Start () {
	}

    public void StartFireball(Vector3 normalizedPath, float distance, float timeToTravel) {
        this.normalizedPath = normalizedPath;
        this.distance = distance;
        this.timeToTravel = timeToTravel;
        this.startTime = Time.time;
        endPosition = transform.position + (normalizedPath * distance);
        startPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        float u = ((Time.time - startTime) / timeToTravel);
        transform.position = Vector3.Lerp(startPosition, endPosition, u);
        if(u >= 1) {
            Destroy(this.gameObject);
        }
	}
}
