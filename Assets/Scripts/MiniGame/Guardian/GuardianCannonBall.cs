using UnityEngine;
using Mirror;

public class GuardianCannonBall : MonoBehaviour
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

            var guardianMinigame = FindObjectOfType<GuardianMinigame>();
            guardianMinigame.IncrementScore();
        }
    }
}
