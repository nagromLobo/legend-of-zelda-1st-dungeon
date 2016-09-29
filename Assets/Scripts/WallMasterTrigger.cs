using UnityEngine;
using System.Collections;

public class WallMasterTrigger : RoomSizedTrigger {
    public Direction direction;
    public delegate void onTriggeredWithDirection(Direction direction, Collider other);
    public onTriggeredWithDirection OnTriggeredWithDirection;

    public void SetDirection(Direction d) {
        this.direction = d;
    }

    protected override void OnTriggerEnter(Collider other) {
        if (other.tag == "Link") {
            OnTriggeredWithDirection(direction, other);
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.tag == "Link") {
            OnTriggeredWithDirection(direction, other);
        }
    }

}
