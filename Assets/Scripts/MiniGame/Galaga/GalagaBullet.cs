using UnityEngine;
using Mirror;

public class GalagaBullet : MonoBehaviour
{
    public float speed = 500f;

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.localPosition.y > 90f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);

            var galagaMinigame = FindObjectOfType<GalagaMinigame>();
            galagaMinigame.IncrementScore();
        }
    }
}
