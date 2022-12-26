using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;
    private Vector2 movement;
    private Rigidbody2D rbody;
    private Animator playerAnim;

    readonly float friction = 0.00001f;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Movement();
        Animation();
    }

    void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
        // print(movement.x);
        // print(movement.y);

        currentDirection = movement.PrimaryDirection();
        if (currentDirection != null) {
            lastDirection = currentDirection.Value;
        }
    }

     private void Movement() 
     {
        //movement = movement * friction;

        //Vector2 currentPos = rbody.position;
        //Vector2 adjustedMovement = movement * movementSpeed;
        //Vector2 newPos = currentPos + adjustedMovement * Time.fixedDeltaTime;
        //rbody.MovePosition(newPos);

        rbody.AddForce(movement * movementSpeed);
        //ForceMode.VelocityChange = ForceMode.VelocityChange;

    }

    private Vector2Extensions.Direction? currentDirection;
    private Vector2Extensions.Direction lastDirection;

    private void Animation() {


        if (currentDirection is Vector2Extensions.Direction direction)
        {
            switch (direction)
            {
                case Vector2Extensions.Direction.up:
                    playerAnim.Play("up", 0);
                    break;
                case Vector2Extensions.Direction.down:
                    playerAnim.Play("down", 0);
                    break;
                case Vector2Extensions.Direction.left:
                    playerAnim.Play("left", 0);
                    break;
                case Vector2Extensions.Direction.right:
                    playerAnim.Play("right", 0);
                    break;
            }
        } else {
            switch (lastDirection)
            {
                case Vector2Extensions.Direction.up:
                    playerAnim.Play("idle_up", 0);
                    break;
                case Vector2Extensions.Direction.down:
                    playerAnim.Play("idle_down", 0);
                    break;
                case Vector2Extensions.Direction.left:
                    playerAnim.Play("idle_left", 0);
                    break;
                case Vector2Extensions.Direction.right:
                    playerAnim.Play("idle_right", 0);
                    break;
            }
        }

        //if (movement.x > 0.01) {
        //    playerAnim.Play("right", 0);
        //}
        //else if (movement.x < -0.01)
        //{
        //    playerAnim.Play("left", 0);
        //}
        //else if (movement.y > 0.01)
        //{
        //    playerAnim.Play("up", 0);
        //}
        //else if (movement.y < -0.01)
        //{
        //    playerAnim.Play("down", 0);
        //}
    }
}

// namespace ExtensionMethods
// {
public static class Vector2Extensions
{
    public enum Direction
    {
        up, down, left, right
    }

    public static Direction? PrimaryDirection(this Vector2 vec)
    {
        if (vec.x > 0.01)
        {
            return Direction.right;
        }
        else if (vec.x < -0.01)
        {
            return Direction.left;
        }
        else if (vec.y > 0.01)
        {
            return Direction.up;
        }
        else if (vec.y < -0.01)
        {
            return Direction.down;
        }

        return null;
    }
}
// }