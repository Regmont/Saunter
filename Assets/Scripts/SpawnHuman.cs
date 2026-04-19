using UnityEngine;

public class SpawnHuman : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioClip audio = Resources.Load<AudioClip>("SFX/granade");
            AudioManager.Instance.PlaySFX(audio);

            objectToSpawn.SetActive(true);
            Destroy(gameObject);
        }
    }
}
