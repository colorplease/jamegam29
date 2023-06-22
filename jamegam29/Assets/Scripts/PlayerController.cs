using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]int jumpCount;
    [SerializeField]int maxJumpCount;

    public Camera cam;

    Vector2 movement;
    Vector2 mousePos;
    float fHorizontalVelocity;

    [SerializeField]Transform gun;

    [SerializeField]SpriteRenderer playerSprite;
    [SerializeField]SpriteRenderer gunSprite;


    void Start()
    {
        Application.targetFrameRate = 240;
        rigid = GetComponent<Rigidbody2D>();
	}

	
	void Update()
    {
        Jumping();
        InputGet();
        RotationAim();
    }

    void FixedUpdate()
    {
        Movement();
        Aiming();        
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
        Vector2 lookDir = mousePos - rigid.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 180;
        gun.rotation = Quaternion.Euler(0, 0, angle);
        print(gun.rotation.eulerAngles.z);
    }

    void RotationAim()
    {
        float gunRotation = gun.localEulerAngles.z;
        if(gunRotation > 180)
        {
            gunRotation -= 360;
        }
        if(Mathf.Abs(gunRotation) > 89)
        {
                playerSprite.flipX = true;
                gunSprite.flipY = true;
        }
        else
        {
                playerSprite.flipX = false;
                gunSprite.flipY = false;
        }
    }
}
