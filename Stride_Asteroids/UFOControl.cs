using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Engine;
using Stride.Core.Mathematics;
using Stride.Games.Time;

namespace Stride_Asteroids
{
    public class UFOControl : SyncScript
    {
        public UFO UFOScript;
        Prefab UFOPrefab;
        float spawnAmount = 10.15f;
        float spawnAdj = 0;
        TimerTick spawnTimer = new TimerTick();
        int spawnCount = 0;

        public override void Start()
        {
            Main.instance.UFOScript = this;
            UFOPrefab = Content.Load<Prefab>("Prefabs/UFO");
            Entity ufo = UFOPrefab.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(ufo);
            UFOScript = ufo.Components.Get<UFO>();
        }

        public override void Update()
        {
            spawnTimer.Tick();

            if (spawnTimer.TotalTime.Seconds > spawnAmount + spawnAdj)
            {
                ResetTimer();

                if (UFOScript.IsActive())
                {
                    return;
                }

                SpawnUFO();
            }
        }

        public void ResetTimer()
        {
            spawnTimer.Reset();
            float adj = Main.instance.Wave * 0.1f;
            spawnAdj = Main.instance.RandomMinMax(-adj, adj * 0.5f);
        }

        void SpawnUFO()
        {
            if (!UFOScript.IsActive())
            {
                UFOScript.Spawn(spawnCount);
            }
        }
    }
}
