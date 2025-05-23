using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EssentialObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectsPrefab;

    private void Awake()
    {
        var existingObjects = FindObjectsOfType<EssentialObjects>();
        if (existingObjects.Length == 0)
        {
            var spawnPos = new Vector3 (0, 0, 0);

            var grid = FindObjectOfType<Grid>();
            if (grid != null)
            {
                spawnPos = grid.transform.position;
            }
            Instantiate(essentialObjectsPrefab, transform.position +  spawnPos, Quaternion.identity);
        }
    }
}
