using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private float jumpHeight = 18;
    private float speed = 256;
    Rigidbody2D myBody;
    bool isJump = false;

    [SerializeField] private Button btnLeft;
    [SerializeField] private Button btnRight;
    [SerializeField] private Button btnJump;
    private float borderX;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        borderX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x - 2;
        
        btnLeft.onClick.AddListener(LeftKeyHandle);

        btnRight.onClick.AddListener(RightKeyHandle);

        btnJump.onClick.AddListener(Jump);
    }

    void Update()
    {
        var pos = transform.position;
        if (transform.position.x >= borderX)
        {
            pos.x = borderX;
            transform.position = pos;
        }
        
        if (transform.position.x <= -borderX)
        {
            pos.x = -borderX;
            transform.position = pos;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            LeftKeyHandle();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            RightKeyHandle();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    /// <summary>
    /// handle left key press
    /// </summary>
    private void LeftKeyHandle()
    {
        myBody.velocity = new Vector2(-speed * Time.deltaTime, myBody.velocity.y);
    }

    private void RightKeyHandle()
    {
        myBody.velocity = new Vector2(speed * Time.deltaTime, myBody.velocity.y);
    }

    private void Jump()
    {
        if (!isJump)
        {
            myBody.velocity = new Vector2(myBody.velocity.x, jumpHeight);
            isJump = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
            isJump = false;
    }
}