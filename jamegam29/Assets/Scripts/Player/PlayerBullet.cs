using System.Collections;
using System.Collections.Generic;
using LevelModule.Scripts;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private GameObject smokeParticleEffect;
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
        if(other.transform.tag == "Wall" || (other.transform.tag.Equals("Enemy")))
        {
            if (other.transform.tag == "Wall")
            {
                // Instantiate the smoke particle effect at the point of collision.
                GameObject smokeEffect = Instantiate(smokeParticleEffect, other.contacts[0].point, Quaternion.identity);
                // Ensure the particle system plays in the correct direction for a 2D game.
                smokeEffect.transform.eulerAngles = new Vector3(-90, 0, 0);
            }

            var enemy =  other.transform.GetComponent<EnemyHealthHandler>();
            //Debug.Log(transform.name + " Collided with " + other.transform.name + "Damage " + bulletDmage);
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
