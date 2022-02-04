using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    void Start()
    {
        CreateController();
    }

    private void CreateController()
    {
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), 3, Random.Range(minZ, maxZ));
        playerPrefab = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
    }

    public void Die()
    {
        PhotonNetwork.Destroy(playerPrefab);
        CreateController();
    }
}
