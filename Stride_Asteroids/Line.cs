using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games.Time;
using Stride.Graphics;
using Stride.Rendering;

namespace Stride_Asteroids
{
    public class Line : Actor
    {
        Entity line;
        ModelComponent lineMesh;
        float timerAmount = 0;
        TimerTick lifeTimer = new TimerTick();

        public override void Start()
        {
            base.Start();
            float size = Main.instance.RandomMinMax(0.003666f, 0.00666f);
            // VertexPositionNormalTexture is the layout that the engine uses in the shaders
            var vBuffer = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice, new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(0, -size, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Top Left.
                 new VertexPositionNormalTexture(new Vector3(0, size, 0), new Vector3(0, 1, 1), new Vector2(0, 0)) //Bottom right.
            });

            MeshDraw meshDraw = new MeshDraw
            {
                PrimitiveType = PrimitiveType.LineStrip, // Tell the GPU that this is a line.
                VertexBuffers = new[] { new VertexBufferBinding(vBuffer, VertexPositionNormalTexture.Layout, vBuffer.ElementCount) },
                DrawCount = vBuffer.ElementCount
            };

            Mesh mesh = new Mesh();
            mesh.Draw = meshDraw;

            Model model = new Model();
            model.Add(mesh);
            lineMesh = new ModelComponent(model);
            line = new Entity();
            line.Add(lineMesh);
            Entity.AddChild(line);
            Destroy();
        }

        public override void Update()
        {
            if (IsActive())
            {
                base.Update();
                lifeTimer.Tick();

                if (lifeTimer.TotalTime.TotalSeconds > timerAmount)
                {
                    Destroy();
                }
            }
        }

        public void Spawn(Vector3 position)
        {
            this.position = position;
            lifeTimer.Reset();
            lineMesh.Enabled = true;
            rotation = Main.instance.RandomRadian();
            rotationVelocity = Main.instance.RandomMinMax(-1.5f, 1.5f);
            timerAmount = Main.instance.RandomMinMax(0.5f, 6.75f);
            SetVelocity(Main.instance.RandomMinMax(1.5f, 3.5f));
            UpdatePR();
        }

        public bool IsActive()
        {
            if (lineMesh != null)
                return lineMesh.Enabled;
            else
                return false;
        }

        void Destroy()
        {
            lineMesh.Enabled = false;
        }

    }
}
