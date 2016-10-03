using UnityEngine;
using System.Collections;

public class SlideableBlocks : MonoBehaviour {
    public enum BlockPosition { LEFT, RIGHT};
    public enum SlideableBlockState { CLOSED, OPENING, CLOSING, OPENED}
    public SlideableBlockState current_state;
    public BlockPosition relativePosition;
    public float moveTime = 1.0f;

    private SlideableBlocks linkedBlock;
    private Vector3 startPosition;

    private float startTime = 0.0f;
    private Vector3 endPosition;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        switch (current_state) {
            case SlideableBlockState.OPENING:
                float u = ((Time.time - startTime) / moveTime);
                if(u < 1) {
                    transform.position = Vector3.Lerp(startPosition, endPosition, u);
                } else {
                    current_state = SlideableBlockState.OPENED;
                }
                break;
            case SlideableBlockState.CLOSING:
                float t = ((Time.time - startTime) / moveTime);
                if(t < 1) {
                    transform.position = Vector3.Lerp(endPosition, startPosition, t);
                } else {
                    current_state = SlideableBlockState.CLOSED;
                }
                break;
        }
	
	}

    public void InitBlocks(Vector3 pos) {
        transform.position = pos;
        startPosition = pos;
        endPosition = new Vector3(startPosition.x - 1, startPosition.y, startPosition.z);
        // make linked block one over to the right
        linkedBlock = (Instantiate(this.gameObject, new Vector3(pos.x + 1, pos.y, pos.z), Quaternion.identity) as GameObject).GetComponent<SlideableBlocks>();
        linkedBlock.InitDependentBlock(new Vector3(pos.x + 1, pos.y, pos.z));
        this.relativePosition = BlockPosition.LEFT;
    }

    public void InitDependentBlock(Vector3 pos) {
        relativePosition = BlockPosition.RIGHT;
        this.transform.position = pos;
        startPosition = pos;
        endPosition = new Vector3(pos.x + 1, pos.y, pos.z);
    }

    public void MoveBlocks() {
        if(current_state == SlideableBlockState.CLOSED) {
            startTime = Time.time;
            current_state = SlideableBlockState.OPENING;
            if(relativePosition == BlockPosition.LEFT) {
                linkedBlock.MoveBlocks();
            }
        }
    }

    public void CloseBlocks() {
        if(current_state == SlideableBlockState.OPENED) {
            startTime = Time.time;
            current_state = SlideableBlockState.CLOSING;
            if(relativePosition == BlockPosition.LEFT) {
                linkedBlock.CloseBlocks();
            }
        }
    }

    public void RestBlocksToOpen() {
        transform.position = endPosition;
        current_state = SlideableBlockState.OPENED;
        if(relativePosition == BlockPosition.LEFT) {
            linkedBlock.RestBlocksToOpen();
        }

    }
}
