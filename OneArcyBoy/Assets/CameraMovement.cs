using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
   public Transform player;
   public Vector3 offset;
   //How far should the camera pan out to the mouse? Default = 2 (halfway)
   public float scale;
   //Smoothness: 0 = Camera doesn't follow at all
   //Smoothness: 1 = Camera snaps instantly to target
   //Recommended: 0.05
   public float smoothness;
   public int maxXDistance;
   public int maxYDistance;

   //Late Update runs immediately after update - Make sure player has finished moving before we try to adjust camera every frame
   void LateUpdate(){
       if (player != null){
            /* Vector3 screenPosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector3 cameraPos = (player.position + worldPosition) / 2;

                float xToPlayer = cameraPos.x - player.position.x;
                float yToPlayer = cameraPos.y - player.position.y; 

            if (xToPlayer > maxXDistance){
                cameraPos.x = player.position.x + maxXDistance;
            } else if (xToPlayer < -maxXDistance){
                cameraPos.x = player.position.x - maxXDistance;
            }
            if(yToPlayer > maxYDistance){
                cameraPos.y = player.position.y + maxYDistance;
            } else if(yToPlayer < -maxYDistance){
                cameraPos.y = player.position.y - maxYDistance;
            }
            
            Vector3 desiredPosition = cameraPos + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothness);
            transform.position = smoothedPosition; */

            transform.position = player.position + offset;
        }
       
   }
}
