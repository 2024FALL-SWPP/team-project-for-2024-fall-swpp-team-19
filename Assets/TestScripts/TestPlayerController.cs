using UnityEngine;
using Mirror;
public class TestPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private void Update()
    {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.Translate(move);
    }
}