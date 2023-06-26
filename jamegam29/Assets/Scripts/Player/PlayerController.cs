using System;
using System.Collections;
using System.Collections.Generic;
using LevelModule.Scripts.AbilityData;
using UnityEngine;
using Random = UnityEngine.Random;


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
    [SerializeField]Transform testFire; //used for sniper ability to check whether enemy is in line of sight //point where raycast fires
    [SerializeField]LayerMask sniperAbilityMask;
    [SerializeField]List<float> enemiesicanshoot;
    [SerializeField]List<float> enemiesicanshootDistance;
    float angleSniperFire;
    [SerializeField]float sexyFreezeDuration;
    [SerializeField]GameObject sexyFlash;
    [SerializeField]UnityEngine.Rendering.Universal.Light2D sexyLight;
    Coroutine crossHairCoroutine;
    Coroutine trailCoroutine;


    [Header("Ability Data")] 
    [SerializeField] private AbilityUIHandler _abilityUIHandler;
    [SerializeField] private AbilityData gun1AbilityData;
    [SerializeField] private AbilityData gun2AbilityData;
    [SerializeField] private AbilityData gun3AbilityData;
    
    private bool _isFrozen;
    private bool _isGun1OnCooldown;
    private bool _isGun2OnCooldown;
    private bool _isGun3OnCooldown;

    private int gun1AbilityChargeCount;
    private int gun2AbilityChargeCount;
    private int gun3AbilityChargeCount;

    [Header("Music Stuff Idk")]
    public AudioSource mainAudio;
    public AudioClip[] audioClips;
    public float SFXVolume;
    public float MusicVolume;
    
    public enum GunType
    {
        Gun1,
        Gun2,
        Gun3
    }

    void Start()
    {
        Application.targetFrameRate = 240;
        rigid = GetComponent<Rigidbody2D>();
        //sets the current gun the player is holding to the pistol
        gunNumber = 0;
        firePoint.localPosition = new Vector3(guns[gunNumber].firePointLocation.x, guns[gunNumber].firePointLocation.y, firePoint.position.z);
        gun.localScale = new Vector3(guns[gunNumber].gunSize.x, guns[gunNumber].gunSize.y, firePoint.position.z);
        gun.GetComponent<SpriteRenderer>().sprite = guns[gunNumber].gunSprite;
        
        // Set up ability
        gun1AbilityChargeCount = gun1AbilityData.abilityCharges;
        gun2AbilityChargeCount = gun2AbilityData.abilityCharges;
        gun3AbilityChargeCount = gun3AbilityData.abilityCharges;
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

    public int GetIndexOfLowestValue(float[] arr)
    {
        float value = float.PositiveInfinity;
        int index = -1;
        for(int i = 0; i < arr.Length; i++)
        {
            if(arr[i] < value)
            {
                index = i;
                value = arr[i];
            }
        }
        return index;
    }

    void Ability()
    {

        
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            //shotgun
            if(gunNumber == 0 && !_isGun1OnCooldown)
            {
                //turns on crosshair animation, visible to screen
                crosshair.SetBool("byebye", true);
                crossHairCoroutine = StartCoroutine(crossHair());
                heymaImdoingmyAbilityHere = true;
            }
            
            //sniper
            if(gunNumber == 1 && !_isGun2OnCooldown)
            {
                enemiesicanshoot.Clear();
                enemiesicanshootDistance.Clear();
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                for(int i = 0; i<enemies.Length; i++)
                {
                    Vector2 whereisEnemy = new Vector2(enemies[i].transform.position.x, enemies[i].transform.position.y) - rigid.position;
                    angleSniperFire = Mathf.Atan2(whereisEnemy.y, whereisEnemy.x) * Mathf.Rad2Deg - 90;
                    testFire.rotation = Quaternion.Euler(testFire.rotation.x, testFire.rotation.y, angleSniperFire);
                    RaycastHit2D hit = Physics2D.Raycast(testFire.position, whereisEnemy, 99999, sniperAbilityMask);
                    if(hit.collider.tag == "Enemy")
                    {
                        enemiesicanshoot.Add(angleSniperFire);
                        enemiesicanshootDistance.Add(Vector3.Distance(transform.position, hit.transform.position));
                        
                    }
                    
                }
                float[] enemiesicanshootDistanceArray = enemiesicanshootDistance.ToArray();
                float[] enemiesicanshootArray = enemiesicanshoot.ToArray();
                if(enemiesicanshootDistanceArray.Length > 0)
                {
                    gun.rotation = Quaternion.Euler(0, 0, enemiesicanshoot[GetIndexOfLowestValue(enemiesicanshootDistanceArray)] - 90);
                    GameObject bullet = Instantiate(guns[gunNumber].bulletType, firePoint.position, firePoint.rotation);
                    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                    bullet.GetComponent<PlayerBullet>().InitiializeBullet(guns[2].gunDamage);
                    rb.AddForce(-firePoint.right * guns[2].bulletForce, ForceMode2D.Impulse);
                    screenShake.ShakeShake(guns[2].shakeDuration, guns[2].curve, guns[2].intensifier);
                    sexyLight.intensity = 0;
                    sexyFlash.SetActive(true);
                    Time.timeScale = 0;
                    StartCoroutine(sexyFreeze());   
                }
                else
                {
                    Debug.Log("ha");
                }

            }
        }
        if(Input.GetKey(KeyCode.Mouse1))
        {
            if(gunNumber == 0 && !_isGun1OnCooldown)
            {
                //follows mouse position and activates trail renderer
                crosshair.transform.position = new Vector3(mousePos.x, mousePos.y, crosshair.transform.position.z);
                TrailRenderer wheredidigoTrail = GetComponent<TrailRenderer>();
                wheredidigoTrail.enabled = true;
            }
            
        }
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            if(gunNumber == 0 && heymaImdoingmyAbilityHere == true && !_isGun1OnCooldown)
            {
                //stops the crosshair coroutine for the bullet time, removes the crosshair, resets the player velocity, and resumes normal time
                StopCoroutine(crossHairCoroutine);
                trailCoroutine = StartCoroutine(trail());
                crosshair.SetBool("byebye", false);
                rigid.velocity = Vector2.zero;
                transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
                Time.timeScale = 1f;
                gun1AbilityChargeCount--;

                if (gun1AbilityChargeCount == 0)
                {
                    StartCoroutine(Co_GunAbilityCooldown(GunType.Gun2));
                    heymaImdoingmyAbilityHere = false;
                }
            }
            if(gunNumber == 1 && heymaImdoingmyAbilityHere == true && !_isGun2OnCooldown)
            {
                gun2AbilityChargeCount--;
                
                if (gun2AbilityChargeCount == 0)
                {
                    StartCoroutine(Co_GunAbilityCooldown(GunType.Gun3));
                    heymaImdoingmyAbilityHere = false;
                }

            }
            
           
        }
    }

    IEnumerator sexyFreeze()
    {
        yield return new WaitForSecondsRealtime(sexyFreezeDuration);
        Time.timeScale = 1;
        sexyLight.intensity = 1;
        sexyFlash.SetActive(false);
        mainAudio.PlayOneShot(audioClips[0], SFXVolume);

        heymaImdoingmyAbilityHere = true;
    }

    private IEnumerator Co_GunAbilityCooldown(GunType gunType)
    {
        float coolDownTime = 0f;

        switch (gunType)
        {
            case GunType.Gun2:
                coolDownTime = gun2AbilityData.abilityCooldown;
                _isGun2OnCooldown = true;
                break;
            case GunType.Gun1:
                coolDownTime = gun1AbilityData.abilityCooldown;
                _isGun3OnCooldown = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gunType), gunType, null);
        }

        while (coolDownTime > 0)
        {
            coolDownTime -= Time.deltaTime;
            Debug.Log("Cooldown time: " + coolDownTime);
            _abilityUIHandler.UpdateAbilityIcon(coolDownTime, gunType);
            yield return null;
        }


        _abilityUIHandler.ResetIcon(gunType);
        switch (gunType)
        {
            case GunType.Gun2:
                _isGun2OnCooldown = false;
                gun2AbilityChargeCount = gun2AbilityData.abilityCharges;
                break;
            case GunType.Gun3:
                _isGun3OnCooldown = false;
                gun1AbilityChargeCount = gun1AbilityData.abilityCharges;
                break;
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
            if(gunNumber < guns.Length - 2)
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
            if(trailCoroutine != null)
            {
                StopCoroutine(trailCoroutine);
            }
            if(crossHairCoroutine != null)
            {
                StopCoroutine(crossHairCoroutine);
            }   
                TrailRenderer wheredidigoTrail = GetComponent<TrailRenderer>();
                wheredidigoTrail.enabled = false;
                crosshair.SetBool("byebye", false);
                Time.timeScale = 1f;
                heymaImdoingmyAbilityHere = false;
        }
    }

    void Shooting()
    {
        if(Input.GetButtonDown("Fire1") && gunNumber != 0 && Time.time >= nextTimeToFire)
        {
            if(gunNumber == 0 && heymaImdoingmyAbilityHere == true)
            {
                nextTimeToFire = Time.time;
                print("ses");
            }
            else
            {
                nextTimeToFire = Time.time + 1f/guns[gunNumber].fireRate;
            }
            GameObject bullet = Instantiate(guns[gunNumber].bulletType, firePoint.position, firePoint.rotation);
            bullet.GetComponent<PlayerBullet>().InitiializeBullet(guns[gunNumber].gunDamage);
            
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(-firePoint.right * guns[gunNumber].bulletForce, ForceMode2D.Impulse);
            
            screenShake.ShakeShake(guns[gunNumber].shakeDuration, guns[gunNumber].curve, guns[gunNumber].intensifier);
        }
        if(Input.GetButtonDown("Fire1") && gunNumber == 0 && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f/guns[gunNumber].fireRate;
            for(int i = 0; i<Random.Range(10,15);i++)
            {
                GameObject bullet = Instantiate(guns[gunNumber].bulletType, firePoint.position, firePoint.rotation);
                bullet.GetComponent<PlayerBullet>().InitiializeBullet(guns[gunNumber].gunDamage);
                
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
            lookDir = mousePos - rigid.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 180;
            gun.rotation = Quaternion.Euler(0, 0, angle);
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
       // StopAllCoroutines();
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

