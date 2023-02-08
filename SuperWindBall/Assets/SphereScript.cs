using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SphereScript : MonoBehaviour
{

    private int Score = 0;
    private Rigidbody rigid;
    [SerializeField] private Text scoreText;

    private void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    void respawnBall()
    {
        transform.localPosition = new Vector3(-2.0f, 0.75f, -2.0f);
        rigid.velocity = rigid.angularVelocity = Vector3.zero;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Goal"))
        {
            Score++;
            scoreText.text = Score.ToString();
            respawnBall();
        }
        else if (other.tag.Equals("Death"))
        {
            respawnBall();
        }
    }

}
