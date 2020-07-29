using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Games.Time;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Audio;

namespace Stride_Asteroids
{
    public class Explode : Actor
    {
        protected List<Dot> explosionList;

        public override void Start()
        {
            base.Start();

            explosionList = new List<Dot>();
            Prefab dotPrefab;
            dotPrefab = Content.Load<Prefab>("Prefabs/Dot");
            int count = Main.instance.RandomMinMax(25, 50);

            for (int dot = 0; dot < count; dot++)
            {
                Entity dotE = dotPrefab.Instantiate().First();
                SceneSystem.SceneInstance.RootScene.Entities.Add(dotE);
                explosionList.Add(dotE.Components.Get<Dot>());
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public void Spawn(Vector3 position)
        {
            foreach (Dot dot in explosionList)
            {
                dot.Spawn(position + new Vector3(Main.instance.RandomMinMax(-Radius * 0.5f, Radius * 0.5f),
                    Main.instance.RandomMinMax(-Radius * 0.5f, Radius * 0.5f), 0));
            }
        }

    }
}
