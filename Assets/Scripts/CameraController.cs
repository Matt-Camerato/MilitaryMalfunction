using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void Update()
    {
        //make sure player exists
        if(player == null) return;

        //make camera follow player transform
        Vector3 pos = new Vector3(player.position.x, player.position.y, -10);
        transform.position = pos;
    }
}
