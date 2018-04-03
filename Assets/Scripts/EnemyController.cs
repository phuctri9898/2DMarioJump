using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private bool isScore;
    private Transform player;

    private void Start()
    {
        isScore = false;
        player = GameObject.Find(Const.PLAYER_TAG).transform;
    }
    private void Update()
    {
        //move left
        transform.Translate(Vector2.left * Time.deltaTime * GameController.Instante.enemySpeed);
        //check & destroy
        if (transform.position.x < -Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x)
        {
            Destroy(gameObject);
        }
        //check bullet over player
        if (!isScore && transform.position.x < player.position.x)
        {
            GameController.Instante.JumpOverBullet();
            isScore = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == Const.PLAYER_TAG)
            GameController.Instante.GameOver();
    }
}