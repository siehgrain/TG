using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
     public GameObject explosionPrefab;
     public float explosionLifetime = 1.0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject); //destrói a bala
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity); //instancia a animação de explosão
        Destroy(explosion, explosionLifetime); //destrói a animação após um tempo de vida
    }
}
