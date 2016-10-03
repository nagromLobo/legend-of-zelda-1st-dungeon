using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
//using System.Diagnostics;


public class StateMachine
{
	private State _current_state;
	
	public void ChangeState(State new_state)
	{
		if(_current_state != null)
		{
			_current_state.OnFinish();
		}

		_current_state = new_state;
		// States sometimes need to reset their machine. 
		// This reference makes that possible.
		//MonoBehaviour.print ("I am in the change state");
		_current_state.state_machine = this;
		_current_state.OnStart();
	}
	
	public void Reset()
	{
		if(_current_state != null)
			_current_state.OnFinish();
		_current_state = null;
	}
	
	public void Update()
	{
		if(_current_state != null)
		{
			float time_delta_fraction = Time.deltaTime / (1.0f / Application.targetFrameRate);
			_current_state.OnUpdate(time_delta_fraction);
		}
	}

	public bool IsFinished()
	{
		return _current_state == null;
	}
}

// A State is merely a bundle of behavior listening to specific events, such as...
// OnUpdate -- Fired every frame of the game.
// OnStart -- Fired once when the state is transitioned to.
// OnFinish -- Fired as the state concludes.
// State Constructors often store data that will be used during the execution of the State.
public class State
{
	// A reference to the State Machine processing the state.
	public StateMachine state_machine;
	
	public virtual void OnStart() {}
	public virtual void OnUpdate(float time_delta_fraction) {} // time_delta_fraction is a float near 1.0 indicating how much more / less time this frame took than expected.
	public virtual void OnFinish() {}
	
	// States may call ConcludeState on themselves to end their processing.
	public void ConcludeState() { state_machine.Reset(); }
}

// A State that takes a renderer and a sprite, and implements idling behavior.
// The state is capable of transitioning to a walking state upon key press.
public class StateIdleWithSprite : State
{
	PlayerControl pc;
	SpriteRenderer renderer;
	Sprite sprite;

	public StateIdleWithSprite(PlayerControl pc, SpriteRenderer renderer, Sprite sprite)
	{
		this.pc = pc;
		this.renderer = renderer;
		this.sprite = sprite;
		//MonoBehaviour.print ("idle make");
	}
	
	public override void OnStart()
	{
		//MonoBehaviour.print ("idle now");
		renderer.sprite = sprite;
	}
	
	public override void OnUpdate(float time_delta_fraction)
	{
		//MonoBehaviour.print ("before I do anything");
		if(pc.current_state == EntityState.ATTACKING)
			return;

		// Transition to walking animations on key press.
		if (Input.GetKeyDown (KeyCode.DownArrow))
			state_machine.ChangeState (new StatePlayAnimationForHeldKey (pc, renderer, pc.link_run_down, 6, KeyCode.DownArrow));
		else if (Input.GetKeyDown (KeyCode.UpArrow) && !Input.GetKeyDown (KeyCode.DownArrow))
			state_machine.ChangeState (new StatePlayAnimationForHeldKey (pc, renderer, pc.link_run_up, 6, KeyCode.UpArrow));
		else if (Input.GetKeyDown (KeyCode.RightArrow))
			state_machine.ChangeState (new StatePlayAnimationForHeldKey (pc, renderer, pc.link_run_right, 6, KeyCode.RightArrow));
		else if (Input.GetKeyDown (KeyCode.LeftArrow))
			state_machine.ChangeState (new StatePlayAnimationForHeldKey (pc, renderer, pc.link_run_left, 6, KeyCode.LeftArrow));
		//don't do anything if 2 arrow keys are pressed
	}
}

// A State for playing an animation until a particular key is released.
// Good for animations such as walking.
public class StatePlayAnimationForHeldKey : State
{
	PlayerControl pc;
	SpriteRenderer renderer;
	KeyCode key;
	Sprite[] animation;
	int animation_length;
	float animation_progression;
	float animation_start_time;
	int fps;
	
	public StatePlayAnimationForHeldKey(PlayerControl pc, SpriteRenderer renderer, Sprite[] animation, int fps, KeyCode key)
	{
		this.pc = pc;
		this.renderer = renderer;
		this.key = key;
		this.animation = animation;
		this.animation_length = animation.Length;
		this.fps = fps;
		//MonoBehaviour.print ("Play animation created!");
		
		if(this.animation_length <= 0)
			Debug.LogError("Empty animation submitted to state machine!");
	}
	
	public override void OnStart()
	{
		animation_start_time = Time.time;
	}
	
	public override void OnUpdate(float time_delta_fraction)
	{
		//MonoBehaviour.print ("attempting to move");
		if(pc.current_state == EntityState.ATTACKING)
			return;

		if(this.animation_length <= 0)
		{
			Debug.LogError("Empty animation submitted to state machine!");
			return;
		}
		
		// Modulus is necessary so we don't overshoot the length of the animation.
		int current_frame_index = ((int)((Time.time - animation_start_time) / (1.0 / fps)) % animation_length);
		renderer.sprite = animation[current_frame_index];
		
		// If another key is pressed, we need to transition to a different walking animation.
		if (Input.GetKeyDown (KeyCode.DownArrow) && !Input.GetKeyDown (KeyCode.UpArrow) && !Input.GetKeyDown (KeyCode.RightArrow) && !Input.GetKeyDown (KeyCode.LeftArrow))
			state_machine.ChangeState (new StatePlayAnimationForHeldKey (pc, renderer, pc.link_run_down, 6, KeyCode.DownArrow));
		else if (Input.GetKeyDown (KeyCode.UpArrow) && !Input.GetKeyDown (KeyCode.DownArrow)  && !Input.GetKeyDown (KeyCode.RightArrow) && !Input.GetKeyDown (KeyCode.LeftArrow))
			state_machine.ChangeState (new StatePlayAnimationForHeldKey (pc, renderer, pc.link_run_up, 6, KeyCode.UpArrow));
		else if (Input.GetKeyDown (KeyCode.RightArrow) && !Input.GetKeyDown (KeyCode.DownArrow)  && !Input.GetKeyDown (KeyCode.UpArrow) && !Input.GetKeyDown (KeyCode.LeftArrow))
			state_machine.ChangeState (new StatePlayAnimationForHeldKey (pc, renderer, pc.link_run_right, 6, KeyCode.RightArrow));
		else if (Input.GetKeyDown (KeyCode.LeftArrow))
			state_machine.ChangeState (new StatePlayAnimationForHeldKey (pc, renderer, pc.link_run_left, 6, KeyCode.LeftArrow));
		// If we detect the specified key has been released, return to the idle state.
		else if(!Input.GetKey(key))
			state_machine.ChangeState(new StateIdleWithSprite(pc, renderer, animation[1]));
		else {
			//run in current direction but don't move don't know how to do that
		}
	}
}

public class StateLinkDoorMovementAnimation : State {
    private PlayerControl pc;
    private SpriteRenderer renderer;
    private Sprite[] animation;
    private int fps;
    private int animation_length;
    private float animation_progression;
    private float animation_start_time;
    private float length;
    private Sprite idleSprite;
    public StateLinkDoorMovementAnimation(PlayerControl pc, SpriteRenderer renderer, Sprite[] animation, int fps, float length) {
        this.pc = pc;
        this.renderer = renderer;
        this.animation = animation;
        this.animation_length = animation.Length;
        this.fps = fps;
        this.length = length;
        MonoBehaviour.print("Play animation created!");

        if (this.animation_length <= 0)
            Debug.LogError("Empty animation submitted to state machine!");
    }

    public override void OnStart() {
        animation_start_time = Time.time;
    }

    public override void OnUpdate(float time_delta_fraction) {
        if (this.animation_length <= 0) {
            Debug.LogError("Empty animation submitted to state machine!");
            return;
        }
        if (Time.time >= animation_start_time + length) {
            state_machine.ChangeState(new StateIdleWithSprite(pc, renderer, animation[0]));
        }

        // Modulus is necessary so we don't overshoot the length of the animation.
        int current_frame_index = ((int)((Time.time - animation_start_time) / (1.0 / fps)) % animation_length);
        renderer.sprite = animation[current_frame_index];

    }
}

public class StateLinkAttack : State {
    PlayerControl pc;
    GameObject weapon_prefab;
    GameObject weapon_instance;
	float duration = 0.0f;
    float coolDown = 0.0f;
	GameObject BowInstance;

    public StateLinkAttack(PlayerControl pc, GameObject weapon_prefab, float coolDown) {
        this.pc = pc;
        this.weapon_prefab = weapon_prefab;
        this.coolDown = coolDown;
		this.duration = coolDown;

    }

    public override void OnStart() {
		//if bomb is used decrement bomb
		//if bow is used decrement arrows
        pc.current_state = EntityState.ATTACKING;
        pc.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //weapon_instance = MonoBehaviour.Instantiate(weapon_prefab, pc.transform.position, Quaternion.identity) as GameObject;

		if (weapon_prefab.name == "Boomerang") {
            //weapon_instance = (MonoBehaviour.Instantiate(weapon_prefab, PlayerControl.instance.transform.position, Quaternion.identity) as GameObject);
			//weapon_prefab.GetComponent<Boomerang> ().Position (pc.transform.position);
			weapon_prefab.GetComponent<Boomerang> ().Instantiate ();
			weapon_instance = GameObject.FindWithTag ("Boomerang").GetComponent<Boomerang> ().weapon_instance;

		} else if (weapon_prefab.name == "Arrow") {

			weapon_prefab.GetComponent<Arrow> ().Instantiate ();
			weapon_instance = GameObject.FindWithTag ("Arrow").GetComponent<Arrow> ().weapon_instance;
			BowInstance = GameObject.FindWithTag ("Arrow").GetComponent<Arrow> ().BowInstance;

		} else {

			weapon_prefab.GetComponent<WoodenSword> ().Instantiate ();
			weapon_instance = GameObject.FindWithTag ("Sword").GetComponent<WoodenSword> ().weapon_instance;
		}

        Vector3 direction_offset = Vector3.zero;
        Vector3 direction_eulerangle = Vector3.zero;

        if (pc.current_direction == Direction.NORTH) {
            direction_offset = new Vector3(0, 1, 0);
            direction_eulerangle = new Vector3(0, 0, 90);
        } else if (pc.current_direction == Direction.EAST) {
            direction_offset = new Vector3(1, 0, 0);
            direction_eulerangle = new Vector3(0, 0, 0);
        } else if (pc.current_direction == Direction.SOUTH) {
            direction_offset = new Vector3(0, -1, 0);
            direction_eulerangle = new Vector3(0, 0, 270);
        } else if (pc.current_direction == Direction.WEST) {
            direction_offset = new Vector3(-1, 0, 0);
            direction_eulerangle = new Vector3(0, 0, 180);
        }
			
        // move and rotate weapon
		//MonoBehaviour.print("before " + weapon_instance.transform.position);
        weapon_instance.transform.position += direction_offset;
        Quaternion new_weapon_rotation = new Quaternion();
        new_weapon_rotation.eulerAngles = direction_eulerangle;
        weapon_instance.transform.rotation = new_weapon_rotation;

		if (weapon_instance.tag == "Arrow") {

			BowInstance.transform.position += direction_offset;
			BowInstance.transform.rotation = new_weapon_rotation;
		}

		if (weapon_instance.tag == "Boomerang") {

			MonoBehaviour.print ("in Boomerang");
			weapon_instance.GetComponent<Boomerang> ().DirectionGo (pc.current_direction);

			weapon_instance.GetComponent<Boomerang> ().ReleaseBoomerang ();
			weapon_instance.tag = "BoomerangReleased";
			GameObject.FindWithTag ("BoomerangReleased").GetComponent<Boomerang> ().released = true;
		}

		//MonoBehaviour.print("after " + weapon_instance.transform.position);
		//GameObject.FindWithTag("Sword").GetComponent<WoodenSword>().pos = weapon_instance.transform.position;
    }

    public override void OnUpdate(float time_delta_fraction) {
		//if (weapon_instance.tag == "Boomerang")
			//weapon_instance.GetComponent<Boomerang> ().Position (pc.transform.position);
        coolDown -= time_delta_fraction;
        if (coolDown <= 0) {
            ConcludeState();
        }
    }

    public override void OnFinish() {
        pc.current_state = EntityState.NORMAL;

		if (weapon_prefab.name == "Arrow") {
			GameObject.FindWithTag ("Arrow").GetComponent<Arrow> ().ReleaseArrow ();
			GameObject.FindWithTag ("Arrow").GetComponent<Arrow> ().released = true;
			pc.rupee_count -= 1;
			Hud.UpdateRupees ();
			MonoBehaviour.print ("decrement rupee");
            MonoBehaviour.Destroy(BowInstance);
        } else if (weapon_prefab.name == "Wooden Sword") {

			GameObject.FindWithTag ("Sword").GetComponent<WoodenSword> ().ReleaseSword ();
			GameObject.FindWithTag ("Sword").GetComponent<WoodenSword> ().released = true;

			if (pc.half_heart_count != pc.max_half_heart_count) {
				//MonoBehaviour.print ("if full you should not be here");
				MonoBehaviour.Destroy (weapon_instance);
                pc.OnAttack(AttackType.MAGIC_SWORD);
			} else {
                pc.OnAttack(AttackType.SWORD);
            }
		} else {
			MonoBehaviour.print ("Arrow should not be in here");
			// MonoBehaviour.Destroy (weapon_instance);
		}

		MonoBehaviour.print ("conclude the damn state"); 
			
    }
}

public class StateLinkBombAttack: State {
	PlayerControl pc;
	GameObject weapon_prefab;
	GameObject weapon_instance;
	float coolDown = 0.0f;
	//public delegate void Bombdropped(GameObject bomb);
	//public Bombdropped bombDropped;

	//Bomb bomb;

	public StateLinkBombAttack(PlayerControl pc, GameObject weapon_prefab, float coolDown) {
		this.pc = pc;
		this.weapon_prefab = weapon_prefab;
		this.coolDown = coolDown;
	}

	public override void OnStart() {
		//if bomb is used decrement bomb
		//if bow is used decrement arrows
		pc.current_state = EntityState.ATTACKING;
		pc.GetComponent<Rigidbody>().velocity = Vector3.zero;


		Vector3 direction_offset = Vector3.zero;
		//weapon_instance = MonoBehaviour.Instantiate(weapon_prefab, PlayerControl.instance.transform.position, Quaternion.identity) as GameObject;

		//weapon_instance.AddComponent<Bomb>().ReleaseBomb ();
		//		GameObject go = GameObject.Find("somegameobjectname");
		//		ScriptB other = (ScriptB) go.GetComponent(typeof(ScriptB));
		//		other.DoSomething();

		//weapon_prefab.GetComponent<Bomb> ().ReleaseBomb ();
		weapon_prefab.GetComponent<Bomb> ().Initiate ();
		weapon_instance = GameObject.FindWithTag("Bomb").GetComponent<Bomb>().weapon_instance;

		GameObject.FindWithTag("Bomb").GetComponent<Bomb> ().ReleaseBomb ();

		if (pc.current_direction == Direction.NORTH) {
			direction_offset = new Vector3(0, 1, 0);
		} else if (pc.current_direction == Direction.EAST) {
			direction_offset = new Vector3(1, 0, 0);
		} else if (pc.current_direction == Direction.SOUTH) {
			direction_offset = new Vector3(0, -1, 0);
		} else if (pc.current_direction == Direction.WEST) {
			direction_offset = new Vector3(-1, 0, 0);
		}

		//weapon_instance.ReleaseBomb ();

		// move and rotate weapon

		weapon_instance.transform.position += direction_offset;
		//		weapon_instance.GetComponent<BoxCollider>().isTrigger = false;
		weapon_instance.tag = "BombReleased";
		//GameObject.FindWithTag("Bomb").GetComponent<Bomb>().weapon_instance = weapon_instance;
		pc.bomb_count -= 1;
		Hud.UpdateBombs ();
	}

	public override void OnUpdate(float time_delta_fraction) {
		//		bomb.Update(time_delta_fraction);
		coolDown -= time_delta_fraction;
		if (coolDown <= 0) {
			ConcludeState();
		}
	}

	public override void OnFinish() {
		pc.current_state = EntityState.NORMAL;
	}

}

public class StateLinkStunnedMovement : State {
    private PlayerControl pc;
    private float coolDown;
    private float startTime;
    private Vector3 pushBackNormal; // should link be thrown back on stun

    public StateLinkStunnedMovement(PlayerControl pc, float coolDown, Vector3 pushBackNoraml) {
        this.pc = pc;
        this.coolDown = coolDown;
        this.pushBackNormal = pushBackNoraml;
    }

    public override void OnStart() {
        startTime = Time.time;
        if (pushBackNormal != Vector3.zero) {
            pc.GetComponent<Rigidbody>().velocity =
                pc.GetComponent<Rigidbody>().velocity = pushBackNormal * pc.walkingVelocity;
        }
    }

    public override void OnUpdate(float time_delta_fraction) {
        if((Time.time - startTime) > coolDown) {
            state_machine.ChangeState(new StateLinkNormalMovement(pc));
        }
    }

    public override void OnFinish() {
        pc.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}

public class StateLinkStunnedSprite : State {
    PlayerControl pc;
    SpriteRenderer renderer;
    Sprite sprite;
    float coolDown;
    float startTime;

    public StateLinkStunnedSprite(PlayerControl pc, SpriteRenderer renderer, Sprite sprite, float coolDown) {
        this.pc = pc;
        this.renderer = renderer;
        this.sprite = sprite;
        this.coolDown = coolDown;
        //MonoBehaviour.print("idle make");
    }

    public override void OnStart() {
        //MonoBehaviour.print("idle now");
        renderer.sprite = sprite;
        startTime = Time.time;
    }

    public override void OnUpdate(float time_delta_fraction) {
       if((Time.time - startTime) >= coolDown) {
            state_machine.ChangeState(new StateIdleWithSprite(pc, renderer, sprite));
        }
    }
}

    // Additional recommended states:
    // StateDeath
    // StateDamaged
    // StateWeaponSwing
    // StateVictory
    //
    // Additional control states:
    // LinkNormalMovement.
    // LinkStunnedState.


 public class StateLinkNormalMovement : State {
    PlayerControl pc;

    public StateLinkNormalMovement(PlayerControl pc) {
        this.pc = pc;
    }

    public override void OnUpdate(float time_delta_fraction) {
        float horizontal_input = Input.GetAxis("Horizontal");
        float vertical_input = Input.GetAxis("Vertical");
        if (horizontal_input != 0.0f) {
            vertical_input = 0.0f;
        }

        Direction prevDirection = pc.current_direction;
        //Decide the current direction
        if (horizontal_input > 0.0f)
            pc.current_direction = Direction.EAST;
        else if (horizontal_input < 0.0f)
            pc.current_direction = Direction.WEST;
        else if (vertical_input < 0.0f)
            pc.current_direction = Direction.NORTH;
        else if (vertical_input > 0.0f)
            pc.current_direction = Direction.SOUTH;
        pc.GetComponent<Rigidbody>().velocity = new Vector3(horizontal_input, -vertical_input, 0)
            * pc.walkingVelocity
            * time_delta_fraction;

        pc.transform.position = UtilityFunctions.fixToGrid(pc.transform.position, pc.current_direction, prevDirection);

        //link attack
		if (Input.GetKeyDown(KeyCode.A) && !Hud.DisableWeapons) {
            state_machine.ChangeState(new StateLinkAttack(pc, pc.Sword_prefab, 15));
        }
        //handle no weapon selection
		if (Input.GetKeyDown(KeyCode.S) && !Hud.DisableWeapons) {
			if(pc.selected_weapon_prefab != null) {
                if ((pc.selected_weapon_prefab.name == "Bomb") && (pc.bomb_count > 0)) {
                    pc.OnAttack(AttackType.BOMB);
                    state_machine.ChangeState(new StateLinkBombAttack(pc, pc.selected_weapon_prefab, 6));
                } else if (pc.selected_weapon_prefab.name == "Boomerang") {
                    state_machine.ChangeState(new StateLinkAttack(pc, pc.selected_weapon_prefab, 30));
                    pc.OnAttack(AttackType.PROJECTILE);
                } else if (pc.selected_weapon_prefab.name == "Arrow" && pc.rupee_count > 0) {
                    MonoBehaviour.print("hello???");
                    state_machine.ChangeState(new StateLinkAttack(pc, pc.selected_weapon_prefab, 30));
                    pc.OnAttack(AttackType.PROJECTILE);
                }
			}
        }

    }

    public override void OnFinish() {
        pc.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}

public class StateEnemyMovementAnimation : State {
    private SpriteRenderer renderer;
    private Sprite[] animation;
    private int fps;
    private int animation_length;
    float animation_progression;
    float animation_start_time;

    public StateEnemyMovementAnimation(SpriteRenderer renderer, Sprite[] animation, int fps) {
        this.renderer = renderer;
        this.animation = animation;
        this.animation_length = animation.Length;
        this.fps = fps;
        //MonoBehaviour.print("Play animation created!");

        if (this.animation_length <= 0)
            Debug.LogError("Empty animation submitted to state machine!");
    }

    public override void OnStart() {
        animation_start_time = Time.time;
    }

    public override void OnUpdate(float time_delta_fraction) {
        if (this.animation_length <= 0) {
            Debug.LogError("Empty animation submitted to state machine!");
            return;
        }

        // Modulus is necessary so we don't overshoot the length of the animation.
        int current_frame_index = ((int)((Time.time - animation_start_time) / (1.0 / fps)) % animation_length);
        renderer.sprite = animation[current_frame_index];

    }
}

public class StateBladeTrapMovement : State {
    BladeTrap bladeTrap;
    float velocity;
    Direction direction;

    public StateBladeTrapMovement(BladeTrap bt, float velocity, Direction d) {
        this.bladeTrap = bt;
        this.velocity = velocity;
        this.direction = d;
    }

    public override void OnStart() {
        Vector3 velocityVector = Vector3.zero;
        switch(direction){
            case Direction.NORTH:
                velocityVector = new Vector3(0, 1, 0);
                break;
            case Direction.EAST:
                velocityVector = new Vector3(1, 0, 0);
                break;
            case Direction.SOUTH:
                velocityVector = new Vector3(0, -1, 0);
                break;
            case Direction.WEST:
                velocityVector = new Vector3(-1, 0, 0);
                break;
        }
        bladeTrap.GetComponent<Rigidbody>().velocity = velocityVector * velocity;
    }

    public override void OnFinish() {
        bladeTrap.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

}

public class StateKeeseMovement : StateEnemyMovement {
    float pauseProbability;
    float timeToPause;
    float pauseSlowdownTime;
    float maxFlyRadius;
    float flyRadius;
    Vector3 directionVector;

    public StateKeeseMovement(Enemy enemy, float timeToCrossTile, Direction direction, float turnProbability,
                             float pauseProbability, float timeToPause, float pauseSlowdownTime,
                             float maxFlyRadius, Vector3 directionVector)
        : base(enemy, timeToCrossTile, direction, turnProbability) {
        this.pauseProbability = pauseProbability;
        this.timeToPause = timeToPause;
        this.maxFlyRadius = maxFlyRadius;
        this.pauseSlowdownTime = pauseSlowdownTime;
        this.directionVector = Vector3.Normalize(directionVector);


    }
    public override void OnStart() {
        setTileLastAndNext();
    }
    protected override bool shouldEnemyWait() {
        if (Random.value < pauseProbability) {
            return true;
        }
        return false;
    }

    //protected override void pauseEnemy() {
        
    //}

    protected override bool MoveEnemy() {
        float u = (Time.time - timeLastTile) / (timeToCrossTile * flyRadius);
        if (u > 1) {
            return true;
        }
        // Adjust u by adding an easing curve based on a Sine wave
        //u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));

        // Intepolate the two linear interpolation points
        //enemy.transform.position = ((1 - u) * posLastTile + u * posNextTile);
        enemy.transform.position = Vector3.Lerp(posLastTile, posNextTile, u);
        return false;
    }

    protected override void turnEnemy() {
        state_machine.ChangeState(new StateKeeseMovement(enemy, timeToCrossTile, direction,
                                                         turnProbability, pauseProbability, timeToPause,
                                                         pauseSlowdownTime, maxFlyRadius, Vector3.zero));
    }

    protected override void setTileLastAndNext() {
        timeLastTile = Time.time;
        posLastTile = enemy.transform.position;
        flyRadius = Random.Range(0, maxFlyRadius);
        if(directionVector == Vector3.zero) {
            Vector2 xyChoordsNextTile = (Random.insideUnitCircle * flyRadius);
            posNextTile = new Vector3(posLastTile.x + xyChoordsNextTile.x, posLastTile.y + xyChoordsNextTile.y, posLastTile.z);
        } else {
            posNextTile = posLastTile + (directionVector * flyRadius);
        }
        
    }
}

public class StateGelMovement : StateEnemyMovement {
    protected float pauseProbability = 0.8f;
    protected float timeToPause = 1.0f;
    public StateGelMovement(Enemy enemy, float timeToCrossTile, Direction direction, float turnProbability, float pauseProbability, float timeToPause)
        : base(enemy, timeToCrossTile, direction, turnProbability){
        this.pauseProbability = pauseProbability;
        this.timeToPause = timeToPause;
    }

    protected override bool shouldEnemyWait() {
        if(Random.value < pauseProbability) {
            return true;
        }
        return false;
    }

    protected override void pauseEnemy() {
        state_machine.ChangeState(new StateEnemyStunned(enemy, direction, turnProbability, timeToPause));
    }
}

public class StateGoriyaMovement : StateEnemyMovement {
    float throwBoomerangProbability = 0.2f;
    float boomerangCooldown = 0.0f;
    public StateGoriyaMovement(Enemy enemy, float timeToCrossTile, Direction direction, float turnProbability, float throwBoomerangProbability, float boomerangCooldown)
        :base(enemy, timeToCrossTile, direction, turnProbability){
        this.throwBoomerangProbability = throwBoomerangProbability;
        this.boomerangCooldown = boomerangCooldown;
    }

    protected override bool shouldEnemyAttack() {
        if(Random.value < throwBoomerangProbability) {
            return true;
        }
        return false;
    }

    protected override void enemyAttack() {
        // instantiate boomerang
        // throw boomerang
        state_machine.ChangeState(new StateEnemyStunned(enemy, direction, turnProbability, boomerangCooldown));
        enemy.OnEnemyAttack();
    }
}

public class StateEnemyStunned : State {
    Enemy enemy;
    Direction direction;
    float turnProbability;
    float stunCooldown;
    float timeStart;

    public StateEnemyStunned(Enemy enemy, Direction direction, float turnProbability, float stunCooldown) {
        this.enemy = enemy;
        this.direction = direction;
        this.turnProbability = turnProbability;
        this.stunCooldown = stunCooldown;
    }

    public override void OnStart() {
		MonoBehaviour.print ("I am stunned");
        timeStart = Time.time;
       
    }

    public override void OnUpdate(float time_delta_fraction) {
        // if we are done
        if((Time.time - timeStart) > stunCooldown) {
            enemy.StartEnemyMovement(direction);
        }
    }
}


public class StateEnemyDamaged : State {
    Enemy enemy;
    Direction direction;
    float turnProbability;
    float cooldown;
    float timeStart;
    Vector3 startPosition;
    Vector3 pushback;
    Vector3 endPosition;

    public StateEnemyDamaged(Enemy enemy, Direction direction, float turnProbability, float cooldown, float distancePushback, Vector3 pushback) {
        this.enemy = enemy;
        this.direction = direction;
        this.turnProbability = turnProbability;
        this.cooldown = cooldown;
        this.pushback = pushback;
        this.startPosition = enemy.transform.position;
        endPosition = startPosition + (pushback * distancePushback);
    }

    public override void OnStart() {
        timeStart = Time.time;

    }

    public override void OnUpdate(float time_delta_fraction) {
        // if we are done
        if ((Time.time - timeStart) > cooldown) {
            enemy.StartEnemyMovement(direction);
        } else if (pushback != Vector3.zero) {
            float u = (Time.time - timeStart) / cooldown;
            if (u < 1) {
                enemy.transform.position = Vector3.Lerp(startPosition, endPosition, u);
            }
        }
    }
}

public class StateAquamentusMovement : StateEnemyMovement {
    float attackProbability;
    public StateAquamentusMovement(Enemy enemy, float timeToCrossTile, Direction direction, float turnProbability, float attackProbability) 
        :base(enemy, timeToCrossTile, direction, turnProbability){
        this.attackProbability = attackProbability;
    }

    

    protected override void turnEnemy() {
        Direction prevDir = direction;
        Direction newDir = UtilityFunctions.reverseDirection(direction);
        enemy.OnEnemyTurned(newDir);
        state_machine.ChangeState(new StateAquamentusMovement(enemy, timeToCrossTile, newDir, turnProbability, attackProbability));
    }

    protected override bool shouldEnemyAttack() {
        return Random.value < attackProbability;
    }

    protected override void enemyAttack() {
        // throw fireballs
        enemy.OnEnemyAttack();
    }
}

// assumes enemy starts on grid
public class StateEnemyMovement : State {
    protected Enemy enemy;
    protected Direction direction;
    protected float timeToCrossTile;
    protected float turnProbability; // probability of turning for each tile reached
    //Vector3 lastTilePosition;
    //float lastRoundPos = -1.0f; // so we don't check for turning multiple times for the same tile
    protected float timeLastTile;
    protected Vector3 posLastTile;
    protected Vector3 posNextTile;

    public StateEnemyMovement(Enemy enemy, float timeToCrossTile, Direction direction, float turnProbability) {
        this.enemy = enemy;
        this.direction = direction;
        this.timeToCrossTile = timeToCrossTile;
        this.turnProbability = turnProbability;
    }

    public override void OnStart() {
        enemy.OnEnemyTurned(direction);
        setTileLastAndNext();
        Rigidbody rigidBody = enemy.GetComponent<Rigidbody>();
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        return;
    }



    public override void OnUpdate(float time_delta_fraction) {
        bool onMainGrid = MoveEnemy();
        if (onMainGrid) {
            if (shouldEnemyTurn()) {
                turnEnemy();
            }
            if (shouldEnemyWait()) {
                pauseEnemy();
            }
            if (shouldEnemyAttack()) {
                enemyAttack();
            }
            setTileLastAndNext();
        }
        return;
    }

    // returns true if we are back on the main grid
    protected virtual bool MoveEnemy() {
        Vector3 currEnemyPosition = enemy.transform.position;
        float u = (Time.time - timeLastTile) / timeToCrossTile;
        if(u > 1.0f) {
            return true;
        }
        // if horizontial
        if (direction == Direction.NORTH || direction == Direction.SOUTH) {
            float newYValue = Mathf.Lerp(posLastTile.y, posNextTile.y, u);
            currEnemyPosition.Set(currEnemyPosition.x, newYValue, currEnemyPosition.z);
          // else is vertical
        } else {
            float newXValue = Mathf.Lerp(posLastTile.x, posNextTile.x, u);
            currEnemyPosition.Set(newXValue, currEnemyPosition.y, currEnemyPosition.z);
        }
        enemy.transform.position = currEnemyPosition;
        return false;
    }

    protected virtual bool shouldEnemyTurn() {
        return (Random.value < turnProbability);
    }

    protected virtual void turnEnemy() {
        Direction prevDir = direction;
        Direction newDir = UtilityFunctions.randomDirection(direction);
        enemy.transform.position = UtilityFunctions.fixToGrid(enemy.transform.position, newDir, prevDir);
        state_machine.ChangeState(new StateEnemyMovement(enemy, timeToCrossTile, UtilityFunctions.randomDirection(direction), turnProbability));
    }

    protected virtual bool shouldEnemyWait() {
        return false;
    }

    protected virtual void pauseEnemy() {}

    protected virtual bool shouldEnemyAttack() {
        return false;
    }

    protected virtual void enemyAttack() {}


    // sets the last tile to the current position and the next tiel
    // to the correct value relevant to the current position and direction
    protected virtual void setTileLastAndNext() {
        posLastTile = enemy.transform.position;
        switch (direction) {
            case Direction.NORTH:
                posNextTile = new Vector3(posLastTile.x, posLastTile.y + 1.0f, posLastTile.z);
                break;
            case Direction.EAST:
                posNextTile = new Vector3(posLastTile.x + 1.0f, posLastTile.y, posLastTile.z);
                break;
            case Direction.SOUTH:
                posNextTile = new Vector3(posLastTile.x, posLastTile.y - 1.0f, posLastTile.z);
                break;
            case Direction.WEST:
                posNextTile = new Vector3(posLastTile.x - 1.0f, posLastTile.y, posLastTile.z);
                break;
        }
        timeLastTile = Time.time;
    }

    public override void OnFinish() {
        enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}


