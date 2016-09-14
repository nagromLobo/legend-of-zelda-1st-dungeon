using UnityEngine;
using System.Collections;

public class player_control : MonoBehaviour {

    public float walkingVelocity = 1.0f;
    public int rupee_count = 0;

    public static player_control instance;

    // FIXME --> Connect all the sprites to this array (and the speed) in the inspector
    public Sprite[] link_run_down;
    public Sprite[] link_run_up;
    public Sprite[] link_run_right;
    public Sprite[] link_run_left;

    StateMachine animation_state_machine;

	// Use this for initialization
	void Start () {
        if(instance != null) {
            Debug.LogError("Mutiple Link objects detected:");
        }
        instance = this;

        // Launch Idle State
        animation_state_machine = new StateMachine();
    }
	
	// Update is called once per frame
	void Update () {
        float horizontal_input = Input.GetAxis("Horizontal");
        float vertical_input = Input.GetAxis("Vertical");
        if(horizontal_input != 0.0f) {
            vertical_input = 0.0f;
        }

        GetComponent<Rigidbody>().velocity = new Vector3(horizontal_input, -vertical_input, 0) * walkingVelocity;

	}

    void OnTriggerEnter(Collider coll) {
        if(coll.gameObject.tag == "Rupee"){
            Destroy(coll.gameObject);
            rupee_count++;
        } else if(coll.gameObject.tag == "heart"){
            // Whateva
        }
    }
}
