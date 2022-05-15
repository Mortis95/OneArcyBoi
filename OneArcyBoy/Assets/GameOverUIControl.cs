using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIControl : MonoBehaviour
{
    public GameObject gameOver;
    public GameObject parentCanvas;

    public bool isVisible;


    private void Awake(){
        setVisible(false);
    }
    public void setVisible(bool v){
        isVisible = v;
        if (isVisible){
            gameOver.transform.position = parentCanvas.transform.position;
        } else {
            gameOver.transform.position = new Vector3(5000,5000,0);
        }
    }

    public void setMessage(string s){
        Text gameOverText = gameOver.GetComponent<Text>();
        gameOverText.text = s;
    }
}
