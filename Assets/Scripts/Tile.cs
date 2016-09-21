using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
    static Sprite[]         spriteArray;

    public Texture2D        spriteTexture;
	public int				x, y;
	public int				tileNum;
	private BoxCollider		bc;
    private Material        mat;
    private SpriteRenderer rend;

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
            rend.sortingOrder = 0;
            bc.center = Vector3.zero;
            bc.size = Vector3.one;
            break;
        case 'P':   // Pushable
                    // have to handle this case
            rend.sortingOrder = 0;
            break;
        case 'D': // Doorway
            rend.sortingOrder = 0;
            bc.enabled = true;
            bc.isTrigger = true;
            bc.tag = "Door";
            break;
        case 'A': // Doorarch
            rend.sortingOrder = 3;
            bc.enabled = false;
            break;
        default:
            rend.sortingOrder = 0;
            bc.enabled = false;
            break;
        }
	}	
}