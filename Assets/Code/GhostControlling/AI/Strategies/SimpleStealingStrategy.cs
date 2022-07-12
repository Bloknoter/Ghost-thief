using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AICode
{
    public class SimpleStealingStrategy : IStrategy
    {
        private AIOffline myAI;

        private ChancelDetector chancelDetector;

        private int first_nearestchancelvertexid;

        private int second_nearestchancelvertexid;

        private int currenttargetchancelvertexid = -1;

        public void Init(AIOffline AI)
        {
            myAI = AI;

            chancelDetector = (ChancelDetector)myAI.GetDetector<ChancelDetector>();

            int[] chancelvertexesid = myAI.pathData.path.GetVertexesIDbyType(Graphs.Vertex.VertexType.Chancel);

            first_nearestchancelvertexid = -1;
            float first_shortestpath = -1;

            second_nearestchancelvertexid = -1;
            float second_shortestpath = -1;

            //Debug.Log($"Finding two nearest chancels for AI number {myAI.ID}");

            for (int i = 0; i < chancelvertexesid.Length;i++)
            {
                if (myAI.ID != myAI.pathData.path.GetVertexbyID(chancelvertexesid[i]).ObjectID)
                {
                    float currentpathlength = myAI.pathData.path.FindPath(myAI.MyChancelVertexID, chancelvertexesid[i]).Length;
                    //Debug.Log($"Current chancel vertex: {chancelvertexesid[i]}, total path length: {currentpathlength}");
                    /*if(first_nearestchancelvertexid != -1 && second_nearestchancelvertexid != -1)
                    {
                        Debug.Log($"First nearest chancel: {first_nearestchancelvertexid}, path length: {first_shortestpath}");
                        Debug.Log($"Second nearest chancel: {second_nearestchancelvertexid}, path length: {second_shortestpath}");
                    }*/
                    if (first_nearestchancelvertexid == -1)
                    {
                        //Debug.Log($"First nearest chancel == -1, new is {chancelvertexesid[i]}");
                        first_nearestchancelvertexid = chancelvertexesid[i];
                        first_shortestpath = currentpathlength;
                    }
                    else if (second_nearestchancelvertexid == -1)
                    {
                        //Debug.Log($"Second nearest chancel == -1, new is {chancelvertexesid[i]}");
                        second_nearestchancelvertexid = chancelvertexesid[i];
                        second_shortestpath = currentpathlength;
                    }
                    else if(currentpathlength < first_shortestpath)
                    {
                        if (first_shortestpath > second_shortestpath)
                        {
                            first_nearestchancelvertexid = chancelvertexesid[i];
                            first_shortestpath = currentpathlength;
                        }
                        else
                        {
                            second_nearestchancelvertexid = chancelvertexesid[i];
                            second_shortestpath = currentpathlength;
                        }
                    }
                    else if(currentpathlength < second_shortestpath)
                    {
                        if (first_shortestpath < second_shortestpath)
                        {
                            second_nearestchancelvertexid = chancelvertexesid[i];
                            second_shortestpath = currentpathlength;
                        }
                        else
                        {
                            first_nearestchancelvertexid = chancelvertexesid[i];
                            first_shortestpath = currentpathlength;
                        }
                    }
                }
            }

            //Debug.Log($"AI number {myAI.ID} found two nearest chancels: {first_nearestchancelvertexid} and {second_nearestchancelvertexid}");

            SetRandomTarget();
        }
        private bool ischoosingrandom = false;
        private void SetRandomTarget()
        {
            if (!ischoosingrandom)
            {
                int[] amounts = chancelDetector.GetAmounts();
                if (amounts[myAI.pathData.path.GetVertexbyID(first_nearestchancelvertexid).ObjectID] <
                    amounts[myAI.pathData.path.GetVertexbyID(second_nearestchancelvertexid).ObjectID])
                {
                    currenttargetchancelvertexid = second_nearestchancelvertexid;
                }
                else
                {
                    currenttargetchancelvertexid = first_nearestchancelvertexid;
                }
                ischoosingrandom = true;
            }
            else
            {
                int r = Random.Range(1, 3);
                switch(r)
                {
                    case 1:
                        currenttargetchancelvertexid = first_nearestchancelvertexid;
                        break;
                    case 2:
                        currenttargetchancelvertexid = second_nearestchancelvertexid;
                        break;
                }
            }
        }

        public void Update()
        {
            if(myAI.CurrentPath == null)
            {
                if(myAI.GemAmount() == 0)
                {
                    if (myAI.CurrentVertexID == currenttargetchancelvertexid && !myAI.IsMovingBetweenVertexes)
                    {
                        int chancelamount = chancelDetector.GetAmountOfChancelbyID(myAI.pathData.path.GetVertexbyID(currenttargetchancelvertexid).ObjectID);
                        if (chancelamount > 0)
                        {
                            chancelDetector.GetChancels()[myAI.pathData.path.GetVertexbyID(currenttargetchancelvertexid).ObjectID].GetGem();
                            myAI.AddGem();
                            myAI.SetDestinationToMyChancel();
                        }
                        else
                        {
                            SetRandomTarget();
                            myAI.SetDestination(currenttargetchancelvertexid);
                        }
                    }
                    else
                    {
                        SetRandomTarget();
                        myAI.SetDestination(currenttargetchancelvertexid);
                    }
                }
                
            }
            else
            {
                if(myAI.DestinationVertexID == currenttargetchancelvertexid)
                {
                    if(chancelDetector.GetAmountOfChancelbyID(myAI.pathData.path.GetVertexbyID(currenttargetchancelvertexid).ObjectID) == 0)
                    {
                        if(currenttargetchancelvertexid == first_nearestchancelvertexid)
                        {
                            currenttargetchancelvertexid = second_nearestchancelvertexid;
                            myAI.SetDestination(currenttargetchancelvertexid);
                        }
                        else
                        {
                            currenttargetchancelvertexid = first_nearestchancelvertexid;
                            myAI.SetDestination(currenttargetchancelvertexid);
                        }
                    }
                }
            }
        }
    }
}
