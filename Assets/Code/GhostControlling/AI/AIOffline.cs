using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Graphs;

namespace AICode
{
    public class AIOffline : BasePlayer
    {
        [SerializeField]
        private Rigidbody2D rigid;

        [SerializeField]
        private Transform mytransform;

        [SerializeField]
        [HideInInspector]
        public PathData pathData;

        public int MySpawnPointVertexID { get; private set; }

        public int MyChancelVertexID { get; private set; }

        public int CurrentVertexID { get; private set; } // Current position of AI

        public int CurrentVertexIDinPath { get; private set; } // Current vertex of AI in CurrentPath.path

        public int NextVertexID { get; private set; } // The next vertex AI is moving to

        public int DestinationVertexID
        {
            get
            {
                if (CurrentPath != null)
                {
                    return CurrentPath.path[CurrentPath.path.Length - 1];
                }
                else
                {
                    return -1;
                }
            }
        } // The main destination AI is moving to. the end of the path

        public bool IsMovingBetweenVertexes { get; private set; } // True - the AI is moving between to vertexes along the edge,
                                                                  // False - the AI is situated on some vertex

        public bool IsDraggingGemToChancel
        {
            get
            {
                return DestinationVertexID == MyChancelVertexID && CurrentVertexID != DestinationVertexID && gems > 0;
            }
        }

        public Path CurrentPath { get; private set; } // The whole path between Start Vertex and Destination Vertex


        protected override bool isBot()
        {
            return true;
        }

        public void SetID(int newID)
        {
            if(newID >= 0)
            {
                ID = newID;
            }
        }

        protected override void onEvent(EventData photonEvent)
        {

        }


        public List<IDetector> detectors { get; private set; } = new List<IDetector>();

        private IStrategy strategy;

        public int[] ChancelVertexes { get; private set; }

        public int[] GemsSpawnerVertexes { get; private set; }

        override protected void Start()
        {
            base.Start();
            BecomeInvisible();

            IDetector detector = new GemSpawnerDetector();
            detector.Init(this);
            detectors.Add(detector);

            detector = new ChancelDetector();
            detector.Init(this);
            detectors.Add(detector);

            ChancelVertexes = pathData.path.GetVertexesIDbyType(Vertex.VertexType.Chancel);
            GemsSpawnerVertexes = pathData.path.GetVertexesIDbyType(Vertex.VertexType.GemSpawner);

            MyChancelVertexID = pathData.path.GetVertexIDbyType(Vertex.VertexType.Chancel, ID);
            MyChancel = MapInfo.Get().playersinfo[ID].chancel;

            MySpawnPointVertexID = pathData.path.GetVertexIDbyType(Vertex.VertexType.SpawnerPoint, ID);

            Vertex spawnVertex = pathData.path.GetVertexbyID(MySpawnPointVertexID);
            mytransform.position = new Vector3(spawnVertex.position.x, spawnVertex.position.y, 0);
            CurrentVertexID = MySpawnPointVertexID;
            NextVertexID = 0;
            CurrentVertexIDinPath = -1;

            SetRandomStartStrategy();
        }

        override protected void Update()
        {
            if (IsPlaying)
            {
                base.Update();
                if (CurrentVertexID == MyChancelVertexID && gems > 0)
                {
                    MyChancel.AddGem();
                    GetGem();
                    CurrentPath = null;
                }
                for (int i = 0; i < detectors.Count; i++)
                {
                    detectors[i].Update();
                }
                strategy.Update();   
            }
        }

        private void FixedUpdate()
        {
            if (IsPlaying)
            {
                if (CurrentPath != null)
                {
                    if (IsMovingBetweenVertexes)
                    {
                        Vertex nextvertex = pathData.path.GetVertexbyID(NextVertexID);
                        float dist = Vector2.Distance(mytransform.position, nextvertex.position);
                        float time = dist / (speed * SPEED_MULTIPLIER);
                        Vector2 deltamoving = (nextvertex.position - (Vector2)mytransform.position) / time;
                        rigid.MovePosition((Vector2)mytransform.position + deltamoving);
                        if (Vector2.Distance(mytransform.position, nextvertex.position) < 0.1f)
                        {
                            IsMovingBetweenVertexes = false;
                            CurrentVertexID = NextVertexID;
                        }
                    }
                    else
                    {
                        if (CurrentVertexID == DestinationVertexID)
                        {
                            CurrentPath = null;
                            CurrentVertexIDinPath = 0;
                            IsMovingBetweenVertexes = false;
                        }
                        else
                        {
                            CurrentVertexIDinPath++;
                            NextVertexID = CurrentPath.path[CurrentVertexIDinPath + 1];
                            IsMovingBetweenVertexes = true;
                        }
                    }
                }
            }
        }

        public void SetDestination(int destinationID)
        {
            //Debug.Log($"New destination of AI number {ID}: {destinationID}, current vertex: {CurrentVertexID}");
            if (IsMovingBetweenVertexes)
            {
                NextVertexID = CurrentVertexID;
            }
            CurrentPath = pathData.path.FindPath(CurrentVertexID, destinationID);
            CurrentVertexIDinPath = -1;            
        }

        public void SetDestinationToMyChancel()
        {
            SetDestination(MyChancelVertexID);
        }

        public void SetPath(Path newpath)
        {
            CurrentPath = newpath;
        }

        public IDetector GetDetector<T>() where T : IDetector
        {
            for (int i = 0; i < detectors.Count; i++)
            {
                if (detectors[i] is T)
                {
                    return detectors[i];
                }
            }
            return null;
        }

        private void SetRandomStartStrategy()
        {
            int r = Random.Range(0, 1);
            switch (r)
            {
                case 0:
                    SetStrategy(new SimpleStartStrategy());
                    break;
            }
        }

        public void SetStrategy(IStrategy newstrategy)
        {
            if (newstrategy != null)
            {
                strategy = newstrategy;
                strategy.Init(this);
            }
        }

        protected override void OnBeingHitbyBomb()
        {
            CurrentVertexID = MySpawnPointVertexID;
            IsMovingBetweenVertexes = false;
            CurrentPath = pathData.path.FindPath(CurrentVertexID, DestinationVertexID);
            CurrentVertexIDinPath = -1;
        }
    }
}
