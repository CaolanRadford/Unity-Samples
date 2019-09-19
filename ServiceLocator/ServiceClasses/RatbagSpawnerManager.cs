using AbstractClasses;
using Controllers;
using UnityEngine;

//keeps track of, and serves, the spawner objects for the player that are found in the scene
namespace Services
{
    public class RatbagSpawnerManager : Service
    {
        //fields
        private RatbagSpawner[] spawners;

        void OnEnable()
        {
            spawners = FindObjectsOfType<RatbagSpawner>();
        }

        public RatbagSpawner GetRandomSpawner()
        {
            int randomIndex = Random.Range(0, spawners.Length);

            return spawners[randomIndex];
        }
    }
}
