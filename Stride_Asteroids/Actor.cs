using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace Stride_Asteroids
{
    public class Actor : SyncScript
    {
        protected bool hit;
        protected bool active;
        protected float radius = 0;
        protected float rotation = 0;
        protected float rotationVelocity = 0;
        protected Vector3 velocity = new Vector3();
        protected Vector3 position = new Vector3();
        protected Vector2 edge = new Vector2(0.607f, 0.496f);

        public bool Hit { get => hit; }
        public bool Active { get => active; }
        public float Radius { get => radius; }
        public Vector3 Velocity { get => velocity; }
        public Vector3 Position { get => position; }

        public override void Start()
        {

        }

        public override void Update()
        {
            //Calculate movement this frame according to velocity and acceleration.
            float elapsed = (float)Game.UpdateTime.Elapsed.TotalSeconds;

            position += velocity * elapsed;
            rotation += rotationVelocity * elapsed;

            if (rotation < 0)
                rotation = MathUtil.TwoPi;

            if (rotation > MathUtil.TwoPi)
                rotation = 0;

            UpdatePR();
        }

        public void UpdatePR()
        {
            Entity.Transform.Position = position;
            Entity.Transform.RotationEulerXYZ = new Vector3(0, 0, rotation);
        }


        protected bool CirclesIntersect(Vector3 Target, float TargetRadius)
        {
            float dx = Target.X - position.X;
            float dy = Target.Y - position.Y;
            float rad = radius + TargetRadius;

            if ((dx * dx) + (dy * dy) < rad * rad)
                return true;

            return false;
        }

        protected void CheckForEdge()
        {
            if (position.X > edge.X)
            {
                position.X = -edge.X;
                UpdatePR();
            }

            if (position.X < -edge.X)
            {
                position.X = edge.X;
                UpdatePR();
            }

            if (position.Y > edge.Y)
            {
                position.Y = -edge.Y;
                UpdatePR();
            }

            if (position.Y < -edge.Y)
            {
                position.Y = edge.Y;
                UpdatePR();
            }
        }

        protected float RandomHieght()//For UFO and Rock only
        {
            return Main.instance.RandomMinMax(-edge.Y, edge.Y);
        }


    }
}
