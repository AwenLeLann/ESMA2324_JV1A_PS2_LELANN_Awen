using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image healthBar;

    public AudioClip sfx1, sfx2, sfx3, sfx4, sfx5;
    public AudioSource src;

    [SerializeField]
    private Transform respawnPoint;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float respawnTime;

    private float respawnTimeStart;
    private bool respawn;

    private CinemachineVirtualCamera CVC;

    private PlayerStat ps;

    private void Start(){
        CVC = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
        
    }

    private void Update(){
        CheckRepawn();
    }

    public void Respawn(){
        respawnTimeStart = Time.time;
        respawn = true;
    }

    private void CheckRepawn(){
        if(Time.time >= respawnTimeStart + respawnTime && respawn){
            var playerTemp = Instantiate(player, respawnPoint);
            CVC.m_Follow = playerTemp.transform;

            playerTemp.GetComponent<PlayerStat>().healthBar = healthBar;
            playerTemp.GetComponent<PlayerMouvement>().src = src;
            playerTemp.GetComponent<PlayerStat>().src = src;
            playerTemp.GetComponent<Combat>().src = src;

            playerTemp.GetComponent<PlayerMouvement>().sfx1 = sfx1;
            playerTemp.GetComponent<PlayerMouvement>().sfx2 = sfx2;
            playerTemp.GetComponent<PlayerStat>().sfx1 = sfx3;
            playerTemp.GetComponent<Combat>().sfx1 = sfx4;
            playerTemp.GetComponent<Combat>().sfx2 = sfx5;

            respawn = false;
        }
    }
}
