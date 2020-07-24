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
        int largeRockAmount = 2;

        public override void Start()
        {
            Main.instance.rockScriptList = rockScriptList;
            Main.instance.rocksScript = this;
            rockPrefab = Content.Load<Prefab>("Prefabs/Rock");
            NewWaveSpawn();
        }

        public override void Update()
        {

        }

        public void ResetRocks()
        {
            DisableRocks();
            largeRockAmount = 2;
            NewWaveSpawn();
        }

        public void RockHit()
        {
            foreach (Rock rock in rockScriptList)
            {
                if (rock.IsActive())
                {
                    if (rock.Hit)
                    {
                        switch (rock.Size)
                        {
                            case Main.RockSize.Large:
                                SpawnRocks(rock.Position, Main.RockSize.Medium, 2);
                                break;
                            case Main.RockSize.Medium:
                                SpawnRocks(rock.Position, Main.RockSize.Small, 2);
                                break;
                        }
                        break;
                    }
                }
            }

            int rockCount = 0;

            foreach (Rock rock in rockScriptList)
            {
                if (rock.IsActive() && !rock.Hit)
                {
                    rockCount++;
                }
            }

            if (rockCount < 1)
            {
                NewWaveSpawn();
            }
        }

        void NewWaveSpawn()
        {
            if (largeRockAmount < 12)
            {
                largeRockAmount += 2;
            }

            SpawnRocks(Vector3.Zero, Main.RockSize.Large, largeRockAmount);
            Main.instance.Wave++;
        }

        void DisableRocks()
        {
            foreach(Rock rock in rockScriptList)
            {
                rock.Disable();
            }
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
