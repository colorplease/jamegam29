using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class guns
{
    public Vector2 gunSize;
    public Vector2 firePointLocation;
    public float gunDamage;
    public float bulletForce;
    public float fireRate;
    public Sprite gunSprite;
}

public class PlayerController : MonoBehaviour
{
    [Header("Movement Stuff idk")]
     [SerializeField]
    LayerMask lmWalls;
    [SerializeField]
    float fJumpVelocity = 5;

    Rigidbody2D rigid;

    float fJumpPressedRemember = 0;
    [SerializeField]
    float fJumpPressedRememberTime = 0.2f;

    float fGroundedRemember = 0;
    [SerializeField]
    float fGroundedRememberTime = 0.25f;

    [SerializeField]
    float fHorizontalAcceleration = 1;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingBasic = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenStopping = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenTurning = 0.5f;

    [SerializeField]
    [Range(0, 1)]
    float fCutJumpHeight = 0.5f;
    [Header("jumping stuff idk")]
    [SerializeField]int jumpCount;
    [SerializeField]int maxJumpCount;

    [Header("Aiming stuff idk")]

    public Camera cam;

    Vector2 movement;
    Vector2 mousePos;
    float fHorizontalVelocity;

    [SerializeField]Transform gun;

    [SerializeField]SpriteRenderer playerSprite;
    [SerializeField]SpriteRenderer gunSprite;
    Vector2 lookDir;
    float gunRotation;
    [Header("shooting Stuff idk")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject bulletPrefab2;
    public guns[] guns;
    public int gunNumber;
    float nextTimeToFire = 0f;


    void Start()
    {
        Application.targetFrameRate = 240;
        rigid = GetComponent<Rigidbody2D>();
        gunNumber = 0;
        firePoint.localPosition = new Vector3(guns[gunNumber].firePointLocation.x, guns[gunNumber].firePointLocation.y, firePoint.position.z);
        gun.localScale = new Vector3(guns[gunNumber].gunSize.x, guns[gunNumber].gunSize.y, firePoint.position.z);
        gun.GetComponent<SpriteRenderer>().sprite = guns[gunNumber].gunSprite;
	}

	
	void Update()
    {
        Jumping();
        InputGet();
        RotationAim();
        Shooting();
        Switching();
    }

    void FixedUpdate()
    {
        Movement();
        Aiming();        
    }

    void Ability()
    {
        
    }

    void Switching()
    {
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
        }
    }

    void Shooting()
    {
        if(Input.GetButtonDown("Fire1") && gunNumber != 1 && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f/guns[gunNumber].fireRate;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(-firePoint.right * guns[gunNumber].bulletForce, ForceMode2D.Impulse);
        }
        if(Input.GetButtonDown("Fire1") && gunNumber == 1 && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f/guns[gunNumber].fireRate;
            for(int i = 0; i<Random.Range(10,15);i++)
            {
                GameObject bullet = Instantiate(bulletPrefab2, firePoint.position, firePoint.rotation);
                float variation = Random.Range(-20,20);
                var x = firePoint.position.x - transform.position.x;
                var y = firePoint.position.y - transform.position.y;
                float rotateAngle = variation + (Mathf.Atan2(y,x) * Mathf.Rad2Deg - 180);
                var MovementDirection = new Vector2(Mathf.Cos(rotateAngle * Mathf.Deg2Rad), Mathf.Sin(rotateAngle*Mathf.Deg2Rad)).normalized;
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(-MovementDirection * guns[gunNumber].bulletForce, ForceMode2D.Impulse);
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
}

