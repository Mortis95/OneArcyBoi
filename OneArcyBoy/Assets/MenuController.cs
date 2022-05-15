using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{

    public Transform menu;
    public Transform parent;
    public bool isVisible;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void switchVisibility(){
        isVisible = !isVisible;
        if (isVisible){
            menu.position = new Vector3(0,0,0);
        } else {
            menu.position = new Vector3(5000,5000,0);
        }
    }

    public void setVisible(bool b){
        isVisible = b;
    }

    public
    // Update is called once per frame
    void Update()
    {
        if (isVisible){
            menu.position = parent.position;
        } else {
            menu.position = new Vector3(5000,5000,0);
        }
    }
}
