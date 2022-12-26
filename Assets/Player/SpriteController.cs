 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class SpriteController : MonoBehaviour {
 
    private Animator playerAnim;
    private int direction = 0;
 
    private Rigidbody2D rigidbody;

    // Use this for initialization
    void Start () {
 
        playerAnim = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
    }
 
    // Update is called once per frame
    void Update () {
   
        print(rigidbody.velocity);

        if (rigidbody.velocity.x > 0.01) {
            playerAnim.Play("right", 0);
        }
        else if (rigidbody.velocity.x < -0.01)
        {
            playerAnim.Play("left", 0);
        }
        else if (rigidbody.velocity.y > 0.01)
        {
            playerAnim.Play("up", 0);
        }
        else if (rigidbody.velocity.y < -0.01)
        {
            playerAnim.Play("down", 0);
        }



        // if (Input.GetKey(KeyCode.W))
        // {
        //     playerAnim.Play("up", 0);
        //     direction = 1;
        // }
 
        // if (Input.GetKey(KeyCode.A))
        // {
        //     playerAnim.Play("left", 0);
        //     direction = 2;
        // }
 
        // if (Input.GetKey(KeyCode.S))
        // {
        //     playerAnim.Play("down", 0);
        //     direction = 3;
        // }
 
        // if (Input.GetKey(KeyCode.D))
        // {
        //     playerAnim.Play("right", 0);
        //     direction = 4;
        // }
 
        // if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        // {
        //     if(direction == 1){playerAnim.Play("Player_Face_Up", 0, 0f);}
        //     if(direction == 2){playerAnim.Play("Player_Face_Left", 0, 0f); }
        //     if(direction == 3){playerAnim.Play("Player_Face_Down", 0, 0f); }
        //     if(direction == 4){playerAnim.Play("Player_Face_Right", 0, 0f); }
        // }
 
    }
}