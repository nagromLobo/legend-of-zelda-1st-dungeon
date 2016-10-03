using UnityEngine;
using System.Collections;

public class PushableBlock : MonoBehaviour {
    public bool pushable = false;
    public Direction pushableDirection = Direction.NORTH;
    // how much time until it counts as pushing
    public float timeToPush = 0.5f;
    // length of time for tile movement
    public float blockPushLength;
    public int roomNumber;
    public delegate void OnBlockPushed(PushableBlock pushableBlock);
    public OnBlockPushed onBlockPushed;
    public delegate void OnBlackTIleTrigered(GameObject pushableBlock);
    public OnBlackTIleTrigered onBlackTileTrigered;

    public enum PushableBlockState {NORMAL, LINK_PUSHING, PUSHED, DONE}
    public PushableBlockState currState = PushableBlockState.NORMAL;
    private float startPushTime = 0.0f;
    private float startMovementTime = 0.0f;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool pushable_north;
    private bool pushable_east;
    private bool pushable_west;
    private bool pushable_south;



	// Use this for initialization
	void Start () {
        SetUpPushableTile();
        

    }

    public void SetUpPushableTile(bool pushable_north, bool pushable_south, bool pushable_east, bool pushable_west, Vector3 startPosition, int roomNumber, bool pushable) {
        this.pushable_north = pushable_north;
        this.pushable_south = pushable_south;
        this.pushable_east = pushable_east;
        this.pushable_west = pushable_west;
        this.roomNumber = roomNumber;
        transform.position = startPosition;
        this.pushable = pushable;
        currState = PushableBlockState.NORMAL;
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
                    if (linkRigidBody.velocity.x <= 0) {
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

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "BlackTile") {
            onBlackTileTrigered(this.gameObject);
        }
    }

    void OnCollisionStay(Collision other) {
        if(other.gameObject.tag == "Link") {
            if ((currState == PushableBlockState.NORMAL) && pushable) {
                Direction d = UtilityFunctions.reverseDirection(UtilityFunctions.DirectionFromNormal(other.contacts[0].normal));
                switch (d) {
                    case Direction.NORTH:
                        if (!pushable_north) {
                            return;
                        }
                        break;
                    case Direction.EAST:
                        if (!pushable_east) {
                            return;
                        }
                        break;
                    case Direction.SOUTH:
                        if (!pushable_south) {
                            return;
                        }
                        break;
                    case Direction.WEST:
                        if (!pushable_west) {
                            return;
                        }
                        break;
                }
                currState = PushableBlockState.LINK_PUSHING;
                pushableDirection = d;
                startPushTime = Time.time;
                SetUpPushableTile();
            }
        }
    }
}
