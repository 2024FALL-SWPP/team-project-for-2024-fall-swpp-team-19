using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarHunterDuck : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ground에 닿으면 점프 가능 상태로 변경
        if (collision.gameObject.CompareTag("StarHunterGround"))
        {
            var starHunterMinigame = FindObjectOfType<StarHunterMinigame>();
            starHunterMinigame.duckIsGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("StarHunterStar"))
        {
            Destroy(collision.gameObject);

            var starHunterMinigame = FindObjectOfType<StarHunterMinigame>();
            starHunterMinigame.IncrementScore();
        }
    }
}
