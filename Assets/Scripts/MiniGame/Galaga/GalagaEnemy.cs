using UnityEngine;
using Mirror;

public class GalagaEnemy : MonoBehaviour
{
    public float speed = 100f;

    public Vector3 currentPosition;

    private void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        currentPosition = transform.localPosition;

        if (transform.localPosition.y < -90f)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
