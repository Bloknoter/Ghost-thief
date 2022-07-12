using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Graphs;

namespace AICode
{
    public class SimpleStartStrategy : IStrategy
    {
        private AIOffline myAI;

        private GemSpawner nearestgemSpawner;
        private int nearestGemSpawnerID = 0;

        public void Init(AIOffline AI)
        {
            myAI = AI;
            Path leastpath = null;
            for(int i = 0; i < myAI.GemsSpawnerVertexes.Length;i++)
            {
                Path currpath = AI.pathData.path.FindPath(AI.MySpawnPointVertexID, myAI.GemsSpawnerVertexes[i]);
                if(leastpath == null || currpath.Length < leastpath.Length)
                {
                    leastpath = currpath;
                    nearestgemSpawner = MapInfo.Get().gemSpawners[myAI.pathData.path.GetVertexbyID(myAI.GemsSpawnerVertexes[i]).ObjectID];
                    nearestGemSpawnerID = myAI.GemsSpawnerVertexes[i];
                }
            }
            myAI.SetPath(leastpath);
        }

        public void Update()
        {

            if (myAI.CurrentPath == null)
            {
                if (myAI.CurrentVertexID == nearestGemSpawnerID && !myAI.IsMovingBetweenVertexes)
                {
                    if (nearestgemSpawner.GemAmount() > 0)
                    {
                        nearestgemSpawner.GetGem();
                        myAI.AddGem();
                        myAI.SetDestinationToMyChancel();
                    }
                    else
                    {
                        myAI.SetStrategy(new SimpleStealingStrategy());
                    }
                }
                if (myAI.CurrentVertexID != nearestGemSpawnerID)
                {
                    if (nearestgemSpawner.GemAmount() > 0)
                    {
                        myAI.SetDestination(nearestGemSpawnerID);
                    }
                    else
                    {
                        myAI.SetStrategy(new SimpleStealingStrategy());
                    }
                }
            }

        }
    }
}
