using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;



public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;
    public float maxSpeed;
    private int desiredLane = 1;//0:left, 1:middle, 2:right
    public float laneDistance = 2.5f;//The distance between tow lanes
    public float jumpForce;
    public float Gravity = -20;
    public Animator animator;
    public static bool isSliding;
    

    void Start()
    {
        controller = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {   if (!PlayerManager.isGameStarted)
            return;
        animator.SetBool("isGameStarted", true);
        if (forwardSpeed < maxSpeed)
            forwardSpeed += 0.1f * Time.deltaTime;
        direction.z=forwardSpeed;

        direction.y += Gravity * Time.deltaTime;
        if (controller.isGrounded)
        {
            direction.y = -1;
            if (SwipeManager.swipeUp)
            {
                animator.SetBool("isGrounded", false);
                Jump();
            }
        }
        else
        {
            animator.SetBool("isGrounded", true);
            direction.y += Gravity * Time.deltaTime;
        }
        if (SwipeManager.swipeDown)
        {
            Slide();            
       
        }
        if (SwipeManager.swipeRight)
        {
            desiredLane++;
            if (desiredLane == 3)
                desiredLane = 2;
        }
            if (SwipeManager.swipeLeft)
        {
            desiredLane--;
            if (desiredLane == -1)
                desiredLane = 0;
        }
        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (desiredLane == 0)
        {
            targetPosition += Vector3.left * laneDistance;
        }else if (desiredLane == 2)
        {
            targetPosition += Vector3.right * laneDistance;
        }
         //transform.position = targetPosition;
         //controller.center = controller.center;
        if (transform.position == targetPosition)
            return;
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
        if (moveDir.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);
            
    }
    private void FixedUpdate()
    {
        if (!PlayerManager.isGameStarted)
            return;
        controller.Move(direction * Time.fixedDeltaTime);
    }
    private void Jump()
    {
        direction.y = jumpForce;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacle")
        {
            PlayerManager.gameOver = true;
           // FindObjectOfType<AudioManager>().PlaySound("GameOver");
        }
    }
    private IEnumerator Slide()
    {
        isSliding = true;
        animator.SetBool("isSliding", true);
        yield return new WaitForSeconds(1.3f);
        // controller.center = new Vector3(0, -0.5f, 0);
        //controller.height = 1;

        //yield return new WaitForSeconds((slideDuration - 0.25f) / Time.timeScale);

        animator.SetBool("isSliding", false);

        // controller.center = Vector3.zero;
        // controller.height = 2;
        //
        // isSliding = false;
    }
}

