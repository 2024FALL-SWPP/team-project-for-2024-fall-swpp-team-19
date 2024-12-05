using UnityEngine;
using Mirror;

public class GuardianCannonBall : NetworkBehaviour
{
    public float speed = 500f;

    [SyncVar]
    public Vector3 currentPosition;

    private void Update()
    {
        if (isServer)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            currentPosition = transform.localPosition;

            if (transform.localPosition.y > 90f || transform.localPosition.y < -90f)
            {
                NetworkServer.Destroy(gameObject);
            }
        }
        else
        {
            transform.localPosition = currentPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            NetworkServer.Destroy(collision.gameObject);
            NetworkServer.Destroy(gameObject);

            var guardianMinigame = FindObjectOfType<GuardianMinigame>();
            var owner = GetComponent<NetworkIdentity>().connectionToClient.identity.GetComponent<CustomGamePlayer>();
            guardianMinigame.IncrementScore(owner);
        }
    }
}
