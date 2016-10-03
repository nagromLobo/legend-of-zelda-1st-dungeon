using UnityEngine;
using System.Collections;

public class BladeTrap : Enemy {
    public enum BladeTrapState { FOREWARDS, BACKWARDS, PAUSED }
    public GameObject bladeTrapTriggerPrefab;
    public float trigerRatioToTravel = 0.5f; // how much of the trigger to travel over
    private RoomSizedTrigger[] triggers = new RoomSizedTrigger[2];
    public BladeTrapState bladeTrapState = BladeTrapState.PAUSED;
    float vertTriggerSize = 0.0f;
    float horizTriggerSize = 0.0f;
    Vector3 startPos;

    public delegate void OnBlackTileTriger(GameObject bladeTrap);
    public OnBlackTileTriger onBlackTileTriger;

    protected override void Start() {
        base.Start();
        triggers[0] = (Instantiate(bladeTrapTriggerPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<RoomSizedTrigger>();
        triggers[1] = (Instantiate(bladeTrapTriggerPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<RoomSizedTrigger>();
        triggers[0].SetTriggerType(RoomSizedTrigger.TriggerType.HORIZONTIAL);
        triggers[1].SetTriggerType(RoomSizedTrigger.TriggerType.VERTICAL);
        triggers[0].OnTriggered += OnTriggered;
        triggers[1].OnTriggered += OnTriggered;
        horizTriggerSize = (((float)triggers[0].horizontialSize) * trigerRatioToTravel);
        vertTriggerSize = (((float)triggers[1].verticalSize) * trigerRatioToTravel);
        startPos = transform.position;
    }

    protected override void Update() {
        base.Update();
        bool turn = false;
        // cut foreward movement at half of the triggersize
        if(bladeTrapState == BladeTrapState.FOREWARDS) {
            switch (currDirection) {
                case Direction.NORTH:
                    if(transform.position.y >=  startPos.y + (vertTriggerSize / 2)) {
                        turn = true;
                    }
                    break;
                case Direction.EAST:
                    if (transform.position.x >= startPos.x + (horizTriggerSize / 2)) {
                        turn = true;
                    }
                    break;
                case Direction.SOUTH:
                    if (transform.position.y <= startPos.y - (vertTriggerSize / 2)) {
                        turn = true;
                    }
                    break;
                case Direction.WEST:
                    if (transform.position.x <= startPos.x - (horizTriggerSize / 2)) {
                        turn = true;
                    }
                    break;
            }
            if (turn) {
                bladeTrapState = BladeTrapState.BACKWARDS;
                StartEnemyMovementReverse();
            }
        }
    }

    protected override void OnCollisionEnter(Collision other) {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        Direction collisionDirection = UtilityFunctions.DirectionFromNormal(other.contacts[0].normal);
        adjustBackToGrid(UtilityFunctions.reverseDirection(collisionDirection), this.transform.position);
        if (other.gameObject.tag == "Enemy" ||
           other.gameObject.tag == "Tile" ||
           other.gameObject.tag == "LockedDoor" ||
           other.gameObject.tag == "Pushable" ||
           other.gameObject.tag == "Link") {
            switch (bladeTrapState) {
                case BladeTrapState.FOREWARDS:
                    if((Mathf.Abs(transform.position.x - startPos.x) > 0.5) || ((Mathf.Abs(transform.position.y - startPos.y)) > 0.5)){
                        bladeTrapState = BladeTrapState.BACKWARDS;
                    }
                     // this.transform.position = adjustBackToGrid(currDirection, this.transform.position); 
                    StartEnemyMovementReverse();
                    break;
                case BladeTrapState.BACKWARDS:
                    bladeTrapState = BladeTrapState.PAUSED;
                    switch (currDirection) {
                        case Direction.NORTH:
                            this.transform.position = adjustBackToGrid(Direction.SOUTH, this.transform.position);
                            break;
                        case Direction.SOUTH:
                            this.transform.position = adjustBackToGrid(Direction.NORTH, this.transform.position);
                            break;
                        case Direction.EAST:
                            this.transform.position = adjustBackToGrid(Direction.WEST, this.transform.position);
                            break;
                        case Direction.WEST:
                            this.transform.position = adjustBackToGrid(Direction.EAST, this.transform.position);
                            break;
                    }
                    triggers[0].transform.position = transform.position;
                    triggers[1].transform.position = transform.position;
                    startPos = transform.position;
                    control_statemachine.Reset();
                    break;
            }
        }
    }

    protected void StartEnemyMovementReverse() {
        switch (currDirection) {
            case Direction.NORTH:
                StartEnemyMovement(Direction.SOUTH);
                break;
            case Direction.EAST:
                StartEnemyMovement(Direction.WEST);
                break;
            case Direction.SOUTH:
                StartEnemyMovement(Direction.NORTH);
                break;
            case Direction.WEST:
                StartEnemyMovement(Direction.EAST);
                break;
        }
    }

    //// called when the blade trap enters the blade trap trigger 
    //protected override void OnTriggerEnter(Collider other) {
    //    base.OnTriggerEnter(other);
    //    if(other.gameObject.tag == "BladeTrapTrigger") {
    //        BladeTrapTrigger trigger = other.GetComponent<BladeTrapTrigger>();
    //        trigger.OnTriggered += OnTriggered;
    //    }
    //}

    private void OnTriggered(Collider other) {
        // only handle the trigge from the trigger in the case that 
        if(bladeTrapState == BladeTrapState.PAUSED) {
            Vector3 otherPos = other.transform.position;
            if (otherPos.y > (transform.position.y + 1.0f)) {
                currDirection = Direction.NORTH;
            } else if(otherPos.x > (transform.position.x + 1.0f)) {
                currDirection = Direction.EAST;
            } else if(otherPos.y < (transform.position.y - 1.0f)) {
                currDirection = Direction.SOUTH;
            } else if(otherPos.x < (transform.position.x - 1.0f)) {
                currDirection = Direction.WEST;
            }
            bladeTrapState = BladeTrapState.FOREWARDS;
            StartEnemyMovement(currDirection);
        }
    }

    public override void StartEnemyMovement(bool disallowCurrentDirection) { }

    // start movement in a given direction
    public override void StartEnemyMovement(Direction d) {
        control_statemachine.ChangeState(new StateBladeTrapMovement(this, timeToCrossTile, d));
    }

    public override void StartEnemyAnimation(Direction d) {}

    public override void OnEnemyTurned(Direction d) {}

    public override void EnemyDamaged(Collider other) {
        // link can't damage blade traps, right?

        //    //int damageHalfHearts = w.damage;
        //    int damage = 1;
        //    normalColor = spriteRenderer.color;
        //    current_state = EntityState.DAMAGED;
        //    damageStartTime = Time.time;
        //    heartCount -= damage;
        //    if (heartCount <= 0) {
        //        // update room state (enemy destroyed)
        //        OnEnemyDestroyed(this.gameObject);
        //        Destroy(this.gameObject);
        //        return;
        //    }
        //    // if not destroyed animate enemy
        //}
    }

    protected override void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);
        if(other.gameObject.tag == "BlackTile") {
            onBlackTileTriger(this.gameObject);
        }
    }

    void OnDestroy() {
        Destroy(triggers[0].gameObject);
        Destroy(triggers[1].gameObject);
    }
}
