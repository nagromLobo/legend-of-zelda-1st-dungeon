using UnityEngine;
using System.Collections;

enum TileType { DOOR, LOCKED, PUSHABLE, NORMAL, SOLID };

public class Tile : MonoBehaviour {
    static Sprite[]         spriteArray;
    public Texture2D        spriteTexture;
	public int				x, y;
	public int				tileNum;
	private BoxCollider		bc;
    private Material        mat;
    private SpriteRenderer rend;

    // tile numvalues in order of increasing x coordinate
    private static int northDoorTileNumLeft = 92;
    private static int northDoorTileNumRight = 93;
    private static int westDoorTileNum = 51;
    private static int eastDoorTileNum = 48;
    private static int northDoorLockedTileNumLeft = 80;
    private static int northDoorLockedTileNumRight = 81;
    //private static int westDoorLockedTileNum = 101;
    private static int eastDoorLockedTileNum = 106;

    private SpriteRenderer  sprend;

	void Awake() {
        if (spriteArray == null) {
            spriteArray = Resources.LoadAll<Sprite>(spriteTexture.name);
        }

		bc = GetComponent<BoxCollider>();
        rend = GetComponent<SpriteRenderer>();

        sprend = GetComponent<SpriteRenderer>();
        //Renderer rend = gameObject.GetComponent<Renderer>();
        //mat = rend.material;
	}

	public void SetTile(int eX, int eY, int eTileNum = -1) {
		if (x == eX && y == eY) return; // Don't move this if you don't have to. - JB

		x = eX;
		y = eY;
		transform.localPosition = new Vector3(x, y, 0);
        gameObject.name = x.ToString("D3")+"x"+y.ToString("D3");

		tileNum = eTileNum;
		if (tileNum == -1 && ShowMapOnCamera.S != null) {
			tileNum = ShowMapOnCamera.MAP[x,y];
			if (tileNum == 0) {
				ShowMapOnCamera.PushTile(this);
			}
		}

        sprend.sprite = spriteArray[tileNum];

		if (ShowMapOnCamera.S != null) SetCollider();
        //TODO: Add something for destructibility - JB

        gameObject.SetActive(true);
		if (ShowMapOnCamera.S != null) {
			if (ShowMapOnCamera.MAP_TILES[x,y] != null) {
				if (ShowMapOnCamera.MAP_TILES[x,y] != this) {
					ShowMapOnCamera.PushTile( ShowMapOnCamera.MAP_TILES[x,y] );
				}
			} else {
				ShowMapOnCamera.MAP_TILES[x,y] = this;
			}
		}
	}


	// Arrange the collider for this tile
	void SetCollider() {
        
        // Collider info from collisionData
        bc.enabled = true;
        char c = ShowMapOnCamera.S.collisionS[tileNum];
        switch (c) {
            case 'S': // Solid
                bc.enabled = true;
                rend.sortingOrder = 0;
                bc.center = Vector3.zero;
                bc.size = Vector3.one;
                bc.isTrigger = false;
                bc.tag = "Tile";
                break;
            case 'T': // Door Threshold
                bc.enabled = true;
                rend.sortingOrder = 0;
                bc.center = Vector3.zero;
                bc.size = Vector3.one;
                bc.size = Vector3.one;
                bc.isTrigger = true;
                bc.tag = "Threshold";
                break;
            case 'P':   // Pushable
                        // have to handle this case
                rend.sortingOrder = 0;
                bc.center = Vector3.zero;
                bc.size = Vector3.one;
                bc.tag = "Tile";
                bc.enabled = true;
                bc.isTrigger = false;
                break;
            case 'D': // Doorway
                bc.enabled = true;
                bc.isTrigger = true;
                bc.center = Vector3.zero;
                bc.size = Vector3.one;
                bc.tag = "Door";
                rend.sortingOrder = 3;
                    break;
            case 'L':
                bc.enabled = true;
                bc.tag = "LockedDoor";
                bc.center = Vector3.zero;
                bc.size = Vector3.one;
                rend.sortingOrder = 1;
                break;
            default:
                setUpDefault(this);
                break;
        }
	}
    
    public void openDoor(Sprite northDoorLeft, Sprite northDoorRight, Sprite eastDoor) {
        if(tileNum == northDoorLockedTileNumLeft) {
            SpriteRenderer rend = GetComponent<SpriteRenderer>();
            rend.sprite = northDoorLeft;
            setUpDefault(this);
            Tile otherTile = ShowMapOnCamera.MAP_TILES[x + 1, y];
            rend = otherTile.GetComponent<SpriteRenderer>();
            rend.sprite = northDoorRight;
            setUpDefault(otherTile);
        } else if(tileNum == northDoorLockedTileNumRight) {
            SpriteRenderer rend = GetComponent<SpriteRenderer>();
            rend.sprite = northDoorRight;
            setUpDefault(this);
            Tile otherTile = ShowMapOnCamera.MAP_TILES[x - 1, y];
            rend = otherTile.GetComponent<SpriteRenderer>();
            rend.sprite = northDoorLeft;
            setUpDefault(otherTile);
            // then we have east  
        } else if(tileNum == eastDoorLockedTileNum) {
            SpriteRenderer rend = GetComponent<SpriteRenderer>();
            rend.sprite = eastDoor;
            setUpDefault(this);
            // else we have west
        } else {
            SetTile(x, y, westDoorTileNum);
        }
    }	

    private void setUpDefault(Tile tile) {
        SpriteRenderer rend = tile.GetComponent<SpriteRenderer>();
        BoxCollider bc = tile.GetComponent<BoxCollider>();
        bc.tag = "Tile";
        rend.sortingOrder = 0;
        bc.enabled = false;
        bc.isTrigger = false;
    }
}