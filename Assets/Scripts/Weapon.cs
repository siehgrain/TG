using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletForce;
    public void Fire(bool isFacingRight) //método para atirar
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation); //instancia a bala
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>(); //pega o Rigidbody2D da bala
        SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
         if (isFacingRight) //verifica a direção do jogador e adiciona a força de acordo
        {
            rb.AddForce(transform.right * bulletForce, ForceMode2D.Impulse);
            spriteRenderer.flipX = false; //não flipa o sprite
        }
        else
        {
            rb.AddForce(transform.right * -bulletForce, ForceMode2D.Impulse);
            spriteRenderer.flipX = true; //não flipa o sprite
        }
    }
    
}