using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyLaserEnemy : MonoBehaviour
{
    public SpriteRenderer spr;
    public LineRenderer lr;
    public float turnSpeed;
    public float currentAngle;
    public int reflectionCount;
    public LayerMask lm;
    private void Awake(){
        spr = gameObject.GetComponent<SpriteRenderer>();
        lr = gameObject.GetComponent<LineRenderer>();
    }

    void Update(){}
    void FixedUpdate()
    {
        currentAngle += turnSpeed * Time.fixedDeltaTime;
        currentAngle = currentAngle % 360;
        Vector2 direction = RotatePointAroundPivot(Vector2.up, transform.position, new Vector3(0,0,currentAngle)) - transform.position;
        //Debug.DrawRay(transform.position, direction, Color.magenta);
        raycastStuff(reflectionCount,direction);
        transform.Rotate(new Vector3(0,0,turnSpeed * Time.fixedDeltaTime));

    }
    
    void raycastStuff(int reflections, Vector2 direction){
        Vector2 currentPos = transform.position;
        while(reflections > 0){
            //Debug.DrawRay(currentPos, direction, Color.magenta);
            RaycastHit2D hit = Physics2D.Raycast(currentPos, direction, float.MaxValue,lm);
            if(hit){
                direction = Vector2.Reflect(direction, hit.normal);
                Debug.DrawLine(currentPos, hit.point, Color.magenta);
                currentPos = hit.point + direction * 0.001f;
                
            } else {
                Debug.DrawRay(currentPos, direction, Color.magenta);
                break;
            }
            reflections -= 1;
        }
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
 }
}
