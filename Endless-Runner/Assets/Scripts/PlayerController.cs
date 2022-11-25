using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SIDE { Left, Mid, Right }

public class PlayerController : MonoBehaviour
{
    public bool jump = false;
    public bool slide = false;

    public SIDE side = SIDE.Mid;
    private float newXPos = 0f;
    [HideInInspector]
    public bool swipeLeft, swipeRight, swipeUp, swipeDown;
    public float xValue;
    public float speedDodge;
    public float jumpPower = 7f;
    private float x;
    private float y;
    public float forwardSpeed = 7f;
    public float maxSpeed = 15f;
    private float colHeight;
    private float colCenterY;

    public Animator anim;
    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        colHeight = controller.height;
        colCenterY = controller.center.y;
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        swipeLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        swipeRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        swipeUp = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        swipeDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);

        // Move horizontally
        if (swipeLeft)
        {
            if(side == SIDE.Mid)
            {
                newXPos = -xValue;
                side = SIDE.Left;
            }
            else if(side == SIDE.Right)
            {
                newXPos = 0;
                side = SIDE.Mid;
            }
        }
        else if (swipeRight)
        {
            if (side == SIDE.Mid)
            {
                newXPos = xValue;
                side = SIDE.Right;
            }
            else if (side == SIDE.Left)
            {
                newXPos = 0;
                side = SIDE.Mid;
            }
        }

        // Increase speed
        if(forwardSpeed < maxSpeed)
        {
            forwardSpeed += 0.1f * Time.deltaTime;
        }

        // Move forward
        Vector3 moveVector = new Vector3(x - transform.position.x, y * Time.deltaTime, forwardSpeed * Time.deltaTime);
        x = Mathf.Lerp(x, newXPos, Time.deltaTime * speedDodge);
        controller.Move(moveVector);

        // Jumping
        Jump();

        // Sliding
        if(swipeDown && !slide)
        {
            StartCoroutine(Slide());
        }
    }
    
    private void Jump()
    {
        if (controller.isGrounded)
        {
            if (swipeUp)
            {
                y = jumpPower;
                jump = true;
                anim.SetBool("isJump", jump);
            }
        }
        else
        {
            y -= jumpPower * 2 * Time.deltaTime;
            jump = false;
            anim.SetBool("isJump", jump);
        }
    }

    private IEnumerator Slide()
    {
        slide = true;
        anim.SetBool("isSlide", slide);
        controller.center = new Vector3(0, colCenterY / 2f, 0);
        controller.height = colHeight / 2f;

        yield return new WaitForSeconds(1.3f);

        controller.center = new Vector3(0, colCenterY, 0);
        controller.height = colHeight;
        slide = false;
        anim.SetBool("isSlide", slide);
    }
}
