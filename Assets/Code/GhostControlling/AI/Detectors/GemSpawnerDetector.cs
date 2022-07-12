using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICode
{
    public class GemSpawnerDetector : IDetector
    {
        private const float DETECTING_DISTANCE = 15f;

        private AIOffline myAI;

        private GemSpawner[] gemSpawners;

        private int[] spawnersamounts;

        public void Init(AIOffline AI)
        {
            myAI = AI;
            gemSpawners = new GemSpawner[MapInfo.Get().gemSpawners.Count];
            spawnersamounts = new int[MapInfo.Get().gemSpawners.Count];
            for (int i = 0; i < MapInfo.Get().gemSpawners.Count; i++)
            {
                gemSpawners[i] = MapInfo.Get().gemSpawners[i];
                spawnersamounts[i] = MapInfo.Get().gemSpawners[i].GemAmount();
            }
        }

        public void Update()
        {
            for (int i = 0; i < gemSpawners.Length; i++)
            {
                if (Vector2.Distance(myAI.transform.position, gemSpawners[i].transform.position) <= DETECTING_DISTANCE)
                {
                    spawnersamounts[i] = gemSpawners[i].GemAmount();
                }
            }
        }

        public GemSpawner[] GetGemSpawners()
        {
            return gemSpawners;
        }
            

        public int[] GetGemSpawnersAmounts()
        {
            return spawnersamounts;
        }
    }
}
