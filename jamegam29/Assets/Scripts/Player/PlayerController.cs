using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class guns
{
    public string gunName;
    public Vector2 gunSize; //scale of the gun that looks decent on the player
    public Vector2 firePointLocation; //position of the firepoint to adjust for the scale change
    public float gunDamage; //you know what this does.
    public float bulletForce; //affects bullet speed
    public float fireRate; //you know what this does.
    public Sprite gunSprite; //you know what this does.
    public float shakeDuration; //passes to the screenshake script
    public AnimationCurve curve; //use to customize the screenshake per gun
    public float intensifier; //multiplier for animation curve because they a lil too strong
    public GameObject bulletType; //check prefabs folder bro
}

public class PlayerController : MonoBehaviour
{
    [Header("Movement Stuff idk")]
     [SerializeField]
    LayerMask lmWalls; //basically "what is jumpable?"
    [SerializeField]
    float fJumpVelocity = 5; //jump height

    Rigidbody2D rigid;

    float fJumpPressedRemember = 0; 
    [SerializeField]
    float fJumpPressedRememberTime = 0.2f; //when player walks off platform, still register a jump as long as they press the jump button in this amount of time (called coyote time)

    float fGroundedRemember = 0;
    [SerializeField]
    float fGroundedRememberTime = 0.25f; //when player hits the jump button when still in midair, still register jump as long as they hit the ground in this amount of time (called jump buffering)

    [SerializeField]
    float fHorizontalAcceleration = 1; //speed
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingBasic = 0.5f; //max speed check inspector
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenStopping = 0.5f; //turn around time check inspector
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenTurning = 0.5f;//turn around time check inspector

    [SerializeField]
    [Range(0, 1)]
    float fCutJumpHeight = 0.5f; //idk what this does lmao
    [Header("jumping stuff idk")]
    [SerializeField]int jumpCount; //current amount of times the player has jumped midair
    [SerializeField]int maxJumpCount; //max amount of times player is allowed to jump midair

    [Header("Aiming stuff idk")]

    public Camera cam; //main camera

    Vector2 movement;
    Vector2 mousePos;
    float fHorizontalVelocity;

    [SerializeField]Transform gun;

    [SerializeField]SpriteRenderer playerSprite;
    [SerializeField]SpriteRenderer gunSprite;
    Vector2 lookDir;
    float gunRotation;
    [Header("shooting Stuff idk")]
    public Transform firePoint; //attatched to gun parent
    public guns[] guns;
    public int gunNumber; //current gun that the player is holding
    float nextTimeToFire = 0f;
    [SerializeField]Animator crosshair; //drag in "crosshair hi guys" from the hierarchy
    [SerializeField]ScreenShake screenShake; //check the Main Camera and drag in ScreenShake
    [SerializeField]bool heymaImdoingmyAbilityHere; //check when player is mid ability
    [SerializeField]Transform testFire; //used for sniper ability to check whether enemy is in line of sight
    [SerializeField]Transform testFireTip; //point where raycast fires
    bool lockedOn; //used to resume aiming if enemy is out of line of sight


    private bool _isFrozen;

    void Start()
    {
        Application.targetFrameRate = 240;
        rigid = GetComponent<Rigidbody2D>();
        //sets the current gun the player is holding to the pistol
        gunNumber = 0;
        firePoint.localPosition = new Vector3(guns[gunNumber].firePointLocation.x, guns[gunNumber].firePointLocation.y, firePoint.position.z);
        gun.localScale = new Vector3(guns[gunNumber].gunSize.x, guns[gunNumber].gunSize.y, firePoint.position.z);
        gun.GetComponent<SpriteRenderer>().sprite = guns[gunNumber].gunSprite;
	}

	
	void Update()
    {
        if (_isFrozen)
            return;
        
        Jumping();
        InputGet();
        RotationAim();
        Shooting();
        Switching();
        Ability();
    }

    void FixedUpdate()
    {
        if (_isFrozen)
            return;
        
        Movement();
        Aiming();        
    }

    void Ability()
    {

        
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            //sniper


            //shotgun
            if(gunNumber == 1)
            {
                //turns on crosshair animation, visible to screen
                crosshair.SetBool("byebye", true);
                StartCoroutine(crossHair());
            }
            heymaImdoingmyAbilityHere = true;
            
        }
        if(Input.GetKey(KeyCode.Mouse1))
        {
            if(gunNumber == 1)
            {
                //follows mouse position and activates trail renderer
                crosshair.transform.position = new Vector3(mousePos.x, mousePos.y, crosshair.transform.position.z);
                TrailRenderer wheredidigoTrail = GetComponent<TrailRenderer>();
                wheredidigoTrail.enabled = true;
            }
            if(gunNumber == 2)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                for(int i = 0; i<enemies.Length; i++)
                {
                    Vector2 whereisEnemy = new Vector2(enemies[i].transform.position.x, enemies[i].transform.position.y) - rigid.position;
                    float angle = Mathf.Atan2(whereisEnemy.y, whereisEnemy.x) * Mathf.Rad2Deg - 90;
                    testFire.rotation = Quaternion.Euler(testFire.rotation.x, testFire.rotation.y, angle);
                    RaycastHit2D hit = Physics2D.Raycast(testFire.position, whereisEnemy);
                    Debug.DrawRay(testFireTip.position, whereisEnemy, Color.red);
                    if(hit.collider.tag == "Enemy")
                    {
                        gun.rotation = Quaternion.Euler(0, 0, angle - 90);
                        lockedOn = true;
                    }
                    else
                    {
                        lockedOn = false;
                    }
                    
                }
            }
        }
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            if(gunNumber == 1 && heymaImdoingmyAbilityHere == true)
            {
                //stops the crosshair coroutine for the bullet time, removes the crosshair, resets the player velocity, and resumes normal time
                StopAllCoroutines();
                StartCoroutine(trail());
                crosshair.SetBool("byebye", false);
                rigid.velocity = Vector2.zero;
                transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
                Time.timeScale = 1f;
            }
            if(gunNumber == 2 && heymaImdoingmyAbilityHere == true)
            {
                lockedOn = false;
                
            }
            heymaImdoingmyAbilityHere = false;
        }
    }

    IEnumerator crossHair()
    {
        yield return new WaitForSeconds(0.1875f);
        Time.timeScale = 0.1f;
    }

    IEnumerator trail()
    {
        yield return new WaitForSeconds(0.3f);
        TrailRenderer wheredidigoTrail = GetComponent<TrailRenderer>();
        wheredidigoTrail.enabled = false;
    }

    void Switching()
    {
        //switches gun number and edits the gun object on the player to match using the gun[]
        if(Input.GetKeyDown(KeyCode.W))
        {
            if(gunNumber < guns.Length - 1)
            {
                gunNumber++;
            }
            else
            {
                gunNumber = 0;
            }
            firePoint.localPosition  = new Vector3(guns[gunNumber].firePointLocation.x, guns[gunNumber].firePointLocation.y, firePoint.position.z);
            gun.localScale = new Vector3(guns[gunNumber].gunSize.x, guns[gunNumber].gunSize.y, firePoint.position.z);
            gun.GetComponent<SpriteRenderer>().sprite = guns[gunNumber].gunSprite; 
            nextTimeToFire = Time.time;

            //stops shotgun ability
            //stops the crosshair coroutine for the bullet time, removes the crosshair, resets the player velocity, and resumes normal time
                StopAllCoroutines();
                TrailRenderer wheredidigoTrail = GetComponent<TrailRenderer>();
                wheredidigoTrail.enabled = false;
                crosshair.SetBool("byebye", false);
                Time.timeScale = 1f;
                heymaImdoingmyAbilityHere = false;
        }
    }

    void Shooting()
    {
        if(Input.GetButtonDown("Fire1") && gunNumber != 1 && Time.time >= nextTimeToFire)
        {
            if(gunNumber == 1 && heymaImdoingmyAbilityHere == true)
            {
                nextTimeToFire = Time.time;
                print("ses");
            }
            else
            {
                nextTimeToFire = Time.time + 1f/guns[gunNumber].fireRate;
            }
            GameObject bullet = Instantiate(guns[gunNumber].bulletType, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(-firePoint.right * guns[gunNumber].bulletForce, ForceMode2D.Impulse);
            screenShake.ShakeShake(guns[gunNumber].shakeDuration, guns[gunNumber].curve, guns[gunNumber].intensifier);
        }
        if(Input.GetButtonDown("Fire1") && gunNumber == 1 && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f/guns[gunNumber].fireRate;
            for(int i = 0; i<Random.Range(10,15);i++)
            {
                GameObject bullet = Instantiate(guns[gunNumber].bulletType, firePoint.position, firePoint.rotation);
                float variation = Random.Range(-20,20);
                var x = firePoint.position.x - transform.position.x;
                var y = firePoint.position.y - transform.position.y;
                float rotateAngle = variation + (Mathf.Atan2(y,x) * Mathf.Rad2Deg - 180);
                var MovementDirection = new Vector2(Mathf.Cos(rotateAngle * Mathf.Deg2Rad), Mathf.Sin(rotateAngle*Mathf.Deg2Rad)).normalized;
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(-MovementDirection * guns[gunNumber].bulletForce, ForceMode2D.Impulse);
                screenShake.ShakeShake(guns[gunNumber].shakeDuration, guns[gunNumber].curve, guns[gunNumber].intensifier);
            }
        }
    }

    void InputGet()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        float fHorizontalVelocity = rigid.velocity.x;
    }

    void Movement()
    {
            fHorizontalVelocity += movement.x;
            if (Mathf.Abs(movement.x) < 0.01f)
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);
            else if (Mathf.Sign(movement.x) != Mathf.Sign(fHorizontalVelocity))
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime * 10f);
            else
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);
            

        rigid.velocity = new Vector2(fHorizontalVelocity, rigid.velocity.y);
    }

    void Jumping()
    {
        Vector2 v2GroundedBoxCheckPosition = (Vector2)transform.position + new Vector2(0, -0.68f);
        Vector2 v2GroundedBoxCheckScale = (Vector2)transform.localScale + new Vector2(-0.02f, 0);
        bool bGrounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmWalls);

        fGroundedRemember -= Time.deltaTime;
        if (bGrounded)
        {
            fGroundedRemember = fGroundedRememberTime;
            jumpCount = 0;
        }

        fJumpPressedRemember -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!bGrounded)
            {
                if(jumpCount < maxJumpCount)
                {
                    rigid.velocity = new Vector2(rigid.velocity.x, fJumpVelocity);
                    jumpCount++;
                }
            }
            else
            {
                fJumpPressedRemember = fJumpPressedRememberTime;
            }
        }
        

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (rigid.velocity.y > 0)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * fCutJumpHeight);
            }
        }

        if ((fJumpPressedRemember > 0) && (fGroundedRemember > 0))
        {
            fJumpPressedRemember = 0;
            fGroundedRemember = 0;
            rigid.velocity = new Vector2(rigid.velocity.x, fJumpVelocity);
        }
    }

    void Aiming()
    {
        if(gunNumber != 2 || lockedOn == false)
        {
            lookDir = mousePos - rigid.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 180;
            gun.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void RotationAim()
    {
        gunRotation = gun.localEulerAngles.z;
        if(gunRotation > 180)
        {
            gunRotation -= 360;
        }
        if(Mathf.Abs(gunRotation) > 89)
        {
                playerSprite.flipX = true;
                gun.localScale = new Vector3(gun.localScale.x, -Mathf.Abs(gun.localScale.y), gun.localScale.z);
        }
        else if (Mathf.Abs(gunRotation) < 89)
        {
                playerSprite.flipX = false;
                gun.localScale = new Vector3(gun.localScale.x, Mathf.Abs(gun.localScale.y), gun.localScale.z);
        }
    }
    
    public void FreezePlayer(bool isFrozen)
    {
        StopAllCoroutines();
        Time.timeScale = 1;
        _isFrozen = isFrozen;
        rigid.isKinematic = isFrozen;

        if (isFrozen)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
       
    }
}

