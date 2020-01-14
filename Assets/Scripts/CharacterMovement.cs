using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 450f;        
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;           
    float horizontalMove = 0f;
    public float runSpeed = 40f;
    public float dashSpeed = 50f;
    public int cooldown = 4;
    private bool jump = false;
    private bool dash = false;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;
    private Vector3 m_Velocity = Vector3.zero;
    private bool isGrounded;
    private bool isWalled;
    public Animator animator;
    private Vector2 screenBounds;
    private bool canDash = true;
    private bool grabbed = false;
    int col = 0;

    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

    }

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("speed", Mathf.Abs(horizontalMove));
        
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || isWalled) {
                jump = true;
            }
            

        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (canDash)
            {
                dash = true;
            }
        }
        if (m_Rigidbody2D.velocity.y < 0)
        {
            if (isWalled)
            {
                m_Rigidbody2D.velocity -= Vector2.up * Physics2D.gravity.y * 0.3f * Time.deltaTime;
            }
            else
            {
                m_Rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (5f) * Time.deltaTime;

            }
        }
    }

    private void FixedUpdate()
    {
        Move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }
    public void Move(float move, bool jump)
    {
             
        Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        if (move > 0 && !m_FacingRight)
        {
            Flip();
        }
        else if (move < 0 && m_FacingRight)
        {
            Flip();
        }

        if (jump)
        {

            if (isGrounded)
            {
                animator.SetTrigger("jump");
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));

            }
            else if (isWalled)
            {

                if (col > 0)
                {
                    m_Rigidbody2D.velocity = new Vector2(20f, 10);
                    //m_Rigidbody2D.AddForce(new Vector2(-runSpeed * 20, m_JumpForce * 1.2f));

                }
                else if (col < 0)
                {
                    m_Rigidbody2D.velocity = new Vector2(-20f, 10);
                }
            }
        }
        if (dash)
        {
            if (m_FacingRight)
            {
                m_Rigidbody2D.velocity = new Vector2(dashSpeed, 0);
            }else if (!m_FacingRight)
            {
                m_Rigidbody2D.velocity = new Vector2(-dashSpeed, 0);
            }
            dash = false;
            canDash = false;
            
        }
    }


    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("ground"))
        {
            isGrounded = true;
            canDash = true;
        }
        else if (collision.collider.CompareTag("walls"))
        {
            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 center = collision.collider.bounds.center;

            if (contactPoint.x > center.x)
            {
                col = 1;
            }
            else if (contactPoint.x < center.x)
            {
                col = -1;
            }
            isWalled = true;
        }
        else if (collision.collider.CompareTag("enemy"))
        {
            m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

            animator.SetTrigger("dead");
            StartCoroutine("Dead");
        }
        else if (collision.collider.CompareTag("grabbable"))
        {
            grabbed = true;
            m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY;
            transform.SetParent(collision.gameObject.transform, true);

            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("ground"))
        {
            isGrounded = false;
        }
        else if (collision.collider.CompareTag("walls"))
        {
            isWalled = false;
            col = 0;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "grabbable")
        {
            grabbed = true;
            m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY;
            transform.SetParent(collision.gameObject.transform, true);


        }
    }

    IEnumerator Dead()
    {
        float waitTime = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
