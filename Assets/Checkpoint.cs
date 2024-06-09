using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    GameManager gameManager;

    private void Awake(){
        gameManager = GameObject.FindGameObjectWithTag("Player").GetComponent<GameManager>();

    }
    //private void OnTriggerEnter2D(Collider2D collision){
    //    if(collision.CompareTag("Player")){
    //       gameManager.Check(transform.position);
    //    }
   // }
}
