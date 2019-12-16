using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;        
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;           
    float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool jump = false;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;
    private Vector3 m_Velocity = Vector3.zero;
    private bool isGrounded;
    private bool isWalled;
    public Animator animator;
    private Vector2 screenBounds;

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
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));

            }else if (isWalled)
            {
                m_Rigidbody2D.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * runSpeed * 10, m_JumpForce));
            }

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
        }
        else if (collision.collider.CompareTag("walls"))
        {
            isWalled = true;
        }
        else if (collision.collider.CompareTag("enemy"))
        {
            m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            
            animator.SetBool("dead", true);
            StartCoroutine("Dead");
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
        }
        
    }

    IEnumerator Dead()
    {
        float waitTime = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
