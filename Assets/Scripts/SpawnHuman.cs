using UnityEngine;

public class SpawnHuman : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectToSpawn.SetActive(true);
            Destroy(gameObject);
        }
    }
}
