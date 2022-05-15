using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public float moveSpeed = 5f;

    public Rigidbody2D rb;

    public Vector2 movement;

    public SpriteRenderer mySpriteRenderer;

    public Sprite left1;
    public Sprite left2;
    public Sprite up1;
    public Sprite up2;
    public Sprite down1;
    public Sprite down2;




    public int animationCycle = 0;
    public bool animationFrame = true;

    // Update is called once per frame
    void Update()
    {
        //Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate(){
        //Movement
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        animationCycle += 1;
        if (animationCycle >= 20){
            animationFrame = !animationFrame;
            animationCycle = 0;
        } 

        if(movement.x > 0){
            if (animationFrame){
                mySpriteRenderer.sprite = left1;
                mySpriteRenderer.flipX = true;
            } else {
                mySpriteRenderer.sprite = left2;
                mySpriteRenderer.flipX = true;
            }
        } else if(movement.x < 0){
            if (animationFrame){
                mySpriteRenderer.sprite = left1;
                mySpriteRenderer.flipX = false;
            } else{
                mySpriteRenderer.sprite = left2;
                mySpriteRenderer.flipX = false;
            }
        }else if(movement.y > 0){
            if(animationFrame){
                mySpriteRenderer.sprite = up1;
                mySpriteRenderer.flipX = false;
            } else{
                mySpriteRenderer.sprite = up2;
                mySpriteRenderer.flipX = false;
            }
        } else if(movement.y < 0){
            if (animationFrame){
                mySpriteRenderer.sprite = down1;
                mySpriteRenderer.flipX = false;
            } else{
                mySpriteRenderer.sprite = down2;
                mySpriteRenderer.flipX = false;
            }
        }

    }
}
