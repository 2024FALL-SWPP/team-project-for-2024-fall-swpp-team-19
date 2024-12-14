using UnityEngine;
using Mirror;

public class GalagaEnemy : NetworkBehaviour
{
    public float speed = 100f;

    [SyncVar]
    public Vector3 currentPosition;

    private void Update()
    {
        if (isServer)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            currentPosition = transform.localPosition;

            if (transform.localPosition.y < -90f)
            {
                NetworkServer.Destroy(gameObject);
            }
        }
        else
        {
            transform.localPosition = currentPosition;
        }
    }
}
