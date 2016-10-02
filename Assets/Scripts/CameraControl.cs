using UnityEngine;
using System.Collections;

enum CameraState { NORMAL, ROOM_TRANSITION, RESET }
public class CameraControl : MonoBehaviour {
    // Singleton accessor
    public static CameraControl S;
    public int roomWidth = 15;
    public int roomHeight = 13;
    public float transitionTime;
    private Vector3 cameraStartPos;
    private Vector3 cameraEndPos;
    private float cameraMoveStart;
    private Direction transitionDir;
    private Vector3 cameraInitialPos;
    private CameraState current_state = CameraState.NORMAL;


    public delegate void CameraMoved(Direction d, float transitionTime);
    public CameraMoved cameraMovedDelegate;
	public delegate void CameraMoveComplete (Vector3 pos);
	public CameraMoveComplete cameraMoveCompleteDelegate;


    void Awake() {
        if (S != null) {
            Debug.LogError("Mutiple Link objects detected:");
        }
        S = this;

    }

    void Start() {
        cameraInitialPos = transform.position;
    }

    // Update is called once per frame
    void Update() {
        Vector3 currCameraPos = this.gameObject.transform.position;
        if (current_state == CameraState.ROOM_TRANSITION) {
            float u = (Time.time - cameraMoveStart) / transitionTime;
            if (u <= 1) {
                // if horizontial
                if (transitionDir == Direction.NORTH || transitionDir == Direction.SOUTH) {
                    float newYValue = Mathf.Lerp(cameraStartPos.y, cameraEndPos.y, u);
                    currCameraPos.Set(currCameraPos.x, newYValue, currCameraPos.z);
                    // else is vertical
                } else {
                    float newXValue = Mathf.Lerp(cameraStartPos.x, cameraEndPos.x, u);
                    currCameraPos.Set(newXValue, currCameraPos.y, currCameraPos.z);
                }
                transform.position = currCameraPos;
            } else {
                current_state = CameraState.NORMAL;
                if(cameraMoveCompleteDelegate != null) {
                    cameraMoveCompleteDelegate(transform.position);
                }
            }
        }

    }

    public void MoveCamera(Direction d) {
        if(cameraMovedDelegate != null) {
            cameraMovedDelegate(d, transitionTime);
        }
        cameraStartPos = this.gameObject.transform.position;
        transitionDir = d;
        cameraMoveStart = Time.time;
        current_state = CameraState.ROOM_TRANSITION;
        switch (d) {
            case Direction.SOUTH:
                cameraEndPos = new Vector3(cameraStartPos.x, cameraStartPos.y - roomHeight, cameraStartPos.z);
                break;
            case Direction.EAST:
                cameraEndPos = new Vector3(cameraStartPos.x + roomWidth, cameraStartPos.y, cameraStartPos.z);
                break;
            case Direction.NORTH:
                cameraEndPos = new Vector3(cameraStartPos.x, cameraStartPos.y + roomHeight, cameraStartPos.z);
                break;
            case Direction.WEST:
                cameraEndPos = new Vector3(cameraStartPos.x - roomWidth, cameraStartPos.y, cameraStartPos.z);
                break;
        }
    }

    public void ReturnToStart() {
        current_state = CameraState.RESET;
        transform.position = cameraInitialPos;
        PlayerControl.instance.OnReturnToStart();
        current_state = CameraState.NORMAL;
    }
}