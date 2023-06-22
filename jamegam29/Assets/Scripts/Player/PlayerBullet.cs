using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{

    void Start()
    {
        Destroy(gameObject, 5f);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.transform.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
