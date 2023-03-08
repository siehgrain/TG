using UnityEngine;

public class SlimeEnemy : MonoBehaviour
{
    // Ajuste a força do salto aqui
    public float jumpForce = 10f;

    // A Layer do chão
    public LayerMask groundLayer;

    // O Rigidbody2D do inimigo
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Define a posição de origem do Raycast
        Vector2 origin = transform.position;
        // Define a direção do Raycast
        Vector2 direction = Vector2.down;
        // Define o comprimento do Raycast
        float distance = 0.5f;

        // Lança um Raycast para baixo para detectar o chão
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, groundLayer.value);

        // Se o Raycast acertar alguma coisa na Layer do chão, dá um pulo para frente
        if (hit.collider != null)
        {
            rb.AddForce(new Vector2(jumpForce, 0f), ForceMode2D.Impulse);
        }
    }
}
