using UnityEngine;
using System.Collections;

public class PushableBlock : MonoBehaviour {
    public Direction pushableDirection = Direction.NORTH;
    public bool stillPushable = true;
    // how much time until it counts as pushing
    public float timeToPush = 0.5f;
    // length of time for tile movement
    public float blockPushLength;
    public int roomNumber;
    public delegate void OnBlockPushed(PushableBlock pushableBlock);
    public OnBlockPushed onBlockPushed;

    enum PushableBlockState {NORMAL, LINK_PUSHING, PUSHED, DONE}
    private PushableBlockState currState = PushableBlockState.NORMAL;
    private float startPushTime = 0.0f;
    private float startMovementTime = 0.0f;
    private Vector3 startPosition;
    private Vector3 endPosition;


	// Use this for initialization
	void Start () {
        SetUpPushableTile();
        

    }

    public void SetUpPushableTile(Direction d, int roomNumber) {
        this.pushableDirection = d;
        this.roomNumber = roomNumber;
        SetUpPushableTile();
    }

    private void SetUpPushableTile() {
        startPosition = transform.position;
        switch (pushableDirection) {
            case Direction.NORTH:
                endPosition.Set(startPosition.x, startPosition.y - 1, startPosition.z);
                break;
            case Direction.EAST:
                endPosition.Set(startPosition.x - 1, startPosition.y, startPosition.z);
                break;
            case Direction.SOUTH:
                endPosition.Set(startPosition.x, startPosition.y + 1, startPosition.z);
                break;
            case Direction.WEST:
                endPosition.Set(startPosition.x + 1, startPosition.y, startPosition.z);
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
        switch (currState) {
            case PushableBlockState.LINK_PUSHING:
                handleLinkPushing();
                break;
            case PushableBlockState.PUSHED:
                handlePushed();
                break;
        }


        
	
	}

    void handleLinkPushing() {
        if((Time.time - startPushTime) < timeToPush) {
            Rigidbody linkRigidBody = PlayerControl.instance.GetComponent<Rigidbody>();
            // if player stops pushing restart time
            switch (pushableDirection) {
                case Direction.NORTH:
                    if (linkRigidBody.velocity.y >= 0) {
                        currState = PushableBlockState.NORMAL;
                    }
                    break;
                case Direction.SOUTH:
                    if (linkRigidBody.velocity.y <= 0) {
                        currState = PushableBlockState.NORMAL;
                    }
                    break;
                case Direction.EAST:
                    if (linkRigidBody.velocity.x >= 0) {
                        currState = PushableBlockState.NORMAL;
                    }
                    break;
                case Direction.WEST:
                    if (linkRigidBody.velocity.y <= 0) {
                        currState = PushableBlockState.NORMAL;
                    }
                    break;
            }
        } else {
            currState = PushableBlockState.PUSHED;
            startMovementTime = Time.time;
        } 
    }

    void handlePushed() {
        float u = (Time.time - startMovementTime) / blockPushLength;
        transform.position = Vector3.Lerp(startPosition, endPosition, u);
        if(u >= 1) {
            currState = PushableBlockState.DONE;
            if(onBlockPushed != null) {
                onBlockPushed(this);
            }
        }
    }

    void OnCollisionStay(Collision other) {
        if(other.gameObject.tag == "Link") {
            if (other.gameObject.tag == "Link") {
                currState = PushableBlockState.LINK_PUSHING;
                startPushTime = Time.time;
            }
        }
    }
}
