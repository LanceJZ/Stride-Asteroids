using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace Stride_Asteroids
{
    public class Rocks : SyncScript
    {
        List<Rock> rockScriptList = new List<Rock>();

        Prefab rockPrefab;

        public override void Start()
        {
            Main.instance.rockScriptList = rockScriptList;
            rockPrefab = Content.Load<Prefab>("Rock");
            SpawnRocks(Vector3.Zero, Main.RockSize.Large, 8);
        }

        public override void Update()
        {

        }

        void SpawnRocks(Vector3 position, Main.RockSize rockSize, int count)
        {
            for (int i = 0; i < count; i++)
            {
                bool spawnNewRock = true;
                int rockFree = rockScriptList.Count;

                for (int rock = 0; rock < rockFree; rock++)
                {
                    if (!rockScriptList[rock].IsActive())
                    {
                        spawnNewRock = false;
                        rockFree = rock;
                        break;
                    }
                }

                if (spawnNewRock)
                {
                    Entity rockE = rockPrefab.Instantiate().First();
                    SceneSystem.SceneInstance.RootScene.Entities.Add(rockE);
                    rockScriptList.Add(rockE.Components.Get<Rock>());
                    rockScriptList[rockFree].Initialize();
                }

                rockScriptList[rockFree].Spawn(position, rockSize);
            }
        }
    }
}
