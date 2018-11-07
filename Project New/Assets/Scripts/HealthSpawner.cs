using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSpawner : MonoBehaviour {

    [SerializeField]
    private float spawnTime;

    static List<GameObject> gameObjects;

    public GameObject hpPrefab;

    private bool waitingToRespawn;

    private void Awake()
    {
        gameObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update () {
        gameObjects.Remove(null);

        if (!waitingToRespawn && !gameObjects.Find(x => x.gameObject.name == "Healthpack(Clone)"))
        {
            StartCoroutine(SpawnHealthPack());
        }
	}

    IEnumerator SpawnHealthPack()
    {
        waitingToRespawn = true;
        Transform hpTransform = transform;

        hpTransform.position = new Vector3(Random.Range(-8, 9), 5, 0);
        yield return new WaitForSeconds(spawnTime);
        GameObject HealthPack = Instantiate(hpPrefab, hpTransform) as GameObject;
        gameObjects.Add(HealthPack);

        waitingToRespawn = false;
    }
}
