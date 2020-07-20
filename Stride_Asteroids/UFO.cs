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
    public class UFO : Actor
    {
        float adj = 0.0666f;
        Entity UFOentity;
        ModelComponent UFOMesh;
        ModelComponent UFOTIMesh;
        ModelComponent UFOBIMesh;

        public override void Start()
        {
            radius = 0.446f * adj;
            ModelCreation();
        }

        public override void Update()
        {
            base.Update();
            CheckForEdge();
            velocity.X = 0.1f;
            position.Y = 0.2f;
        }

        public bool IsActive()
        {
            if (UFOMesh != null)
                return UFOMesh.Enabled;
            else
                return false;
        }

        void Disable()
        {
            UFOMesh.Enabled = false;
            UFOTIMesh.Enabled = false;
            UFOBIMesh.Enabled = false;
            hit = false;

        }

        void ModelCreation()
        {
            // VertexPositionNormalTexture is the layout that the engine uses in the shaders
            var vBuffer = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice, new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(-0.07f * adj, 0.25f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Top Left
                 new VertexPositionNormalTexture(new Vector3(-0.164f * adj, 0.07f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Upper Left
                 new VertexPositionNormalTexture(new Vector3(-0.446f * adj, -0.094f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Mid Left
                 new VertexPositionNormalTexture(new Vector3(-0.188f * adj, -0.258f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Bottom Left
                 new VertexPositionNormalTexture(new Vector3(0.188f * adj, -0.258f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Bottom Right
                 new VertexPositionNormalTexture(new Vector3(0.446f * adj, -0.094f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Mid Right
                 new VertexPositionNormalTexture(new Vector3(0.164f * adj, 0.07f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Upper Right
                 new VertexPositionNormalTexture(new Vector3(0.07f * adj, 0.25f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Top Right
                 new VertexPositionNormalTexture(new Vector3(-0.07f * adj, 0.25f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Top left
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
            UFOMesh = new ModelComponent(model);

            UFOentity = new Entity();
            UFOentity.Add(UFOMesh);
            Entity.AddChild(UFOentity);
            // Top inside lines for UFO
            var vBufferti = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice, new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(0.164f * adj, 0.07f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Top inside line left
                 new VertexPositionNormalTexture(new Vector3(-0.164f * adj, 0.07f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Top inside line right
            });

            MeshDraw meshDrawti = new MeshDraw
            {
                PrimitiveType = PrimitiveType.LineStrip, // Tell the GPU that this is a line.
                VertexBuffers = new[] { new VertexBufferBinding(vBufferti, VertexPositionNormalTexture.Layout, vBufferti.ElementCount) },
                DrawCount = vBufferti.ElementCount
            };

            Mesh meshti = new Mesh();
            meshti.Draw = meshDrawti;

            Model modelti = new Model();
            modelti.Add(meshti);
            UFOTIMesh = new ModelComponent(modelti);
            Entity UFOti = new Entity();
            UFOti.Add(UFOTIMesh);
            UFOentity.AddChild(UFOti);

            // Bottom inside lines for UFO
            var vBufferbi = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice, new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(0.446f * adj, -0.094f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Bottom inside line left
                 new VertexPositionNormalTexture(new Vector3(-0.446f * adj, -0.094f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)) // Bottom inside line right
            });

            MeshDraw meshDrawbi = new MeshDraw
            {
                PrimitiveType = PrimitiveType.LineStrip, // Tell the GPU that this is a line.
                VertexBuffers = new[] { new VertexBufferBinding(vBufferbi, VertexPositionNormalTexture.Layout, vBufferbi.ElementCount) },
                DrawCount = vBufferbi.ElementCount
            };

            Mesh meshbi = new Mesh();
            meshbi.Draw = meshDrawbi;

            Model modelbi = new Model();
            modelbi.Add(meshbi);
            UFOBIMesh = new ModelComponent(modelbi);
            Entity UFObi = new Entity();
            UFObi.Add(UFOBIMesh);
            UFOentity.AddChild(UFObi);
        }
    }
}
