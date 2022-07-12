using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICode
{
    public class ChancelDetector : IDetector
    {
        private const float DETECTING_DISTANCE = 15f;

        private AIOffline myAI;

        private Chancel[] chancels;

        private int[] amounts;

        private float[] updatetimings;

        public void Init(AIOffline AI)
        {
            myAI = AI;

            List<PlayerMapInfo> infos = MapInfo.Get().playersinfo;
            chancels = new Chancel[infos.Count];
            amounts = new int[infos.Count];
            updatetimings = new float[infos.Count];
            for(int i = 0; i < chancels.Length;i++)
            {
                chancels[i] = infos[i].chancel;
                amounts[i] = -1;
            }
        }

        public void Update()
        {
            for (int i = 0; i < chancels.Length; i++)
            {
                if (Vector2.Distance(myAI.transform.position, chancels[i].transform.position) <= DETECTING_DISTANCE)
                {
                    amounts[i] = chancels[i].GemAmount();
                    updatetimings[i] = 0;
                }
                else
                {
                    updatetimings[i] += Time.deltaTime;
                    if(updatetimings[i] >= 10f)
                    {
                        amounts[i] = -1;
                    }
                }
            }
        }

        public Chancel[] GetChancels()
        {
            return chancels;
        }

        public int[] GetAmounts()
        {
            return amounts;
        }

        public int GetAmountOfChancelbyID(int id)
        {
            return amounts[id];
        }
        public int GetAmountOfChancel(Chancel chancel)
        {
            for(int i = 0; i < chancels.Length;i++)
            {
                if (chancels[i] == chancel)
                    return amounts[i];
            }
            return 0;
        }
    }
}
