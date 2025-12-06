using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject leafPrefab;

        public float minSpawnTime = 1f;
        public float maxSpawnTime = 10f;

        void Start()
        {
            StartCoroutine(SpawnLoop());
        }

        IEnumerator SpawnLoop()
        {
            while (true)
            {
                // Wait random time
                float delay = Random.Range(minSpawnTime, maxSpawnTime);
                float offest = Random.Range(0f,3f);
                yield return new WaitForSeconds(delay);

                // Spawn leaf
                Instantiate(leafPrefab, transform.position + (Vector3.right * offest), Quaternion.identity);
            }
        }
    }
