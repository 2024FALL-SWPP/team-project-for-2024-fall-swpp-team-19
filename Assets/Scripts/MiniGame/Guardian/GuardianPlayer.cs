using UnityEngine;
using Mirror;

public class GuardianPlayer : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 부모 동기화
        if (isServer)
        {
            RpcSetParent(gameObject, "GuardianCanvas");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 클라이언트에서 호출할 수 있도록 Rpc 메서드를 정의
    [ClientRpc]
    void RpcSetParent(GameObject guardian, string parentName)
    {
        Transform canvas = GameObject.Find(parentName).transform;
        if (canvas != null)
        {
            guardian.transform.SetParent(canvas, false); // false keeps the local transform
        }
        else
        {
            Debug.LogError("Canvas not found in the scene!");
        }
    }
}
