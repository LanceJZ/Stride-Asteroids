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
using Stride.Audio;

namespace Stride_Asteroids
{
    public class Shot : Actor
    {
        Entity shot;
        ModelComponent shotMesh;
        TimerTick timer = new TimerTick();
        float timerAmount = 0;

        public override void Start()
        {
            radius = 0.001f;

            // VertexPositionNormalTexture is the layout that the engine uses in the shaders
            var vBuffer = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice, new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(-radius, radius, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Top Left.
                 new VertexPositionNormalTexture(new Vector3(radius, -radius, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Bottom right.
                 new VertexPositionNormalTexture(new Vector3(radius, radius, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Bottom right.
                 new VertexPositionNormalTexture(new Vector3(-radius, -radius, 0), new Vector3(0, 1, 1), new Vector2(0, 0)) //Top Left.
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
            shotMesh = new ModelComponent(model);

            shot = new Entity();
            shot.Add(shotMesh);
            Entity.AddChild(shot);
            Disable();
        }

        public override void Update()
        {
            if (shotMesh.Enabled)
            {
                base.Update();
                CheckForEdge();

                if (timer.TotalTime.TotalSeconds > timerAmount)
                {
                    Disable();
                }

                timer.Tick();
            }
        }

        public void Spawn(Vector3 position, Vector3 velocity, float timer)
        {
            base.position = position;
            base.velocity = velocity;
            this.timer.Reset();
            timerAmount = timer;
            shotMesh.Enabled = true;
            UpdatePR();
        }

        public void Disable()
        {
            shotMesh.Enabled = false;
        }

        public bool IsActive()
        {
            if (shotMesh != null)
                return shotMesh.Enabled;
            else
                return false;
        }
    }
}
