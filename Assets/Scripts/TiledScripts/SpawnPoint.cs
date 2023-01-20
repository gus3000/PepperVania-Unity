using UnityEngine;

namespace TiledScripts
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private float yDelta = 0f;
        private void Start()
        {
            var player = GameObject.FindWithTag("Player");
            player.transform.position = transform.position + Vector3.up * yDelta;
        }
    }
}