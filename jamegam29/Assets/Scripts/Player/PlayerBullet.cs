using System.Collections;
using System.Collections.Generic;
using LevelModule.Scripts;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField]bool hasFallOff;
    [SerializeField]float minBulletFallOff;
   [SerializeField]float maxBulletFallOff;
   [SerializeField]float killDrag;

   private int bulletDmage;
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    public void InitiializeBullet(float damage)
    {
        bulletDmage = (int)damage;
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Bullet Collided with " + other.transform.name);
        
        if(other.transform.tag == "Wall" || (other.transform.tag.Equals("Enemy")))
        {
            var enemy =  other.transform.GetComponent<EnemyHealthHandler>();
            if (enemy)
            {
                enemy.TakeDamage(bulletDmage);
            }
           
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        if(hasFallOff)
        {
            StartCoroutine(bulletFalloff());
        }
    }

    IEnumerator bulletFalloff()
    {
        yield return new WaitForSeconds(Random.Range(minBulletFallOff, maxBulletFallOff));
        gameObject.GetComponent<Rigidbody2D>().drag = killDrag;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        Destroy(gameObject, 1f);
    }
}
