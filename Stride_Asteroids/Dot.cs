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
    public class Dot : Actor
    {
        Entity dot;
        ModelComponent dotMesh;
        TimerTick aliveTimer = new TimerTick();
        float timerAmount = 0;

        public override void Start()
        {
            radius = 0.0005f;
            // VertexPositionNormalTexture is the layout that the engine uses in the shaders
            var vBuffer = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice, new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(radius, radius, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Top Left.
                 new VertexPositionNormalTexture(new Vector3(-radius, -radius, 0), new Vector3(0, 1, 1), new Vector2(0, 0)) //Bottom right.
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
            dotMesh = new ModelComponent(model);

            dot = new Entity();
            dot.Add(dotMesh);
            Entity.AddChild(dot);
            Destroy();
        }

        public override void Update()
        {
            if (IsActive())
            {
                base.Update();
                aliveTimer.Tick();

                if (aliveTimer.TotalTime.TotalSeconds > timerAmount)
                {
                    Destroy();
                }
            }
        }

        public void Spawn(Vector3 position)
        {
            base.position = position;
            float timer = Main.instance.RandomMinMax(0.1f, 1.25f);
            float speed = Main.instance.RandomMinMax(0.001f, 0.125f);
            aliveTimer.Reset();
            timerAmount = timer;
            dotMesh.Enabled = true;
            velocity = SetVelocity(speed);
            UpdatePR();
        }

        public bool IsActive()
        {
            if (dotMesh != null)
                return dotMesh.Enabled;
            else
                return false;
        }

        void Destroy()
        {
            dotMesh.Enabled = false;
        }
    }
}
