using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace Stride_Asteroids
{
    public class UFOControl : SyncScript
    {
        UFO UFOScript;
        Prefab UFOPrefab;

        public override void Start()
        {
            Main.instance.UFOScript = UFOScript;
            UFOPrefab = Content.Load<Prefab>("UFO");
            Entity ufo = UFOPrefab.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(ufo);
            UFOScript = ufo.Components.Get<UFO>();

        }

        public override void Update()
        {

        }
    }
}
