using System;
using AndrisPhysics.Common;

namespace AndrisPhysics.Components
{
    class BoxCollider : Component
    {
        public float Radius { get; }
        public float Height { get; }
        public float HalfHeight => Height / 2;
        public float Width => Radius * 2f;

        public Vector2 Origin => new Vector2(Bottom.x, Bottom.y + HalfHeight);

        public Vector2 Top => new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + HalfHeight);

        public Vector2 Bottom => new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - HalfHeight);

        public Vector2 Left => new Vector2(gameObject.transform.position.x - Radius, gameObject.transform.position.y);

        public Vector2 Right => new Vector2(gameObject.transform.position.x + Radius, gameObject.transform.position.y);

        public BoxCollider(float height, float radius)
        {
            Radius = radius;
            Height = height;
        }

        public Vector2[] GetCorners()
        {
            return new[]
            {
                new Vector2(gameObject.transform.position.x - Radius, gameObject.transform.position.y - HalfHeight),
                new Vector2(gameObject.transform.position.x - Radius, gameObject.transform.position.y + HalfHeight),
                new Vector2(gameObject.transform.position.x + Radius, gameObject.transform.position.y + HalfHeight),
                new Vector2(gameObject.transform.position.x + Radius, gameObject.transform.position.y - HalfHeight)
            };
        }

        public Direction CollidesWith(BoxCollider col)
        {
            Direction hit = Direction.Null;

            float w = 0.5f * (Width + col.Width);
            float h = 0.5f * (Height + col.Height);
            float dx = gameObject.transform.position.x - col.gameObject.transform.position.x;
            float dy = gameObject.transform.position.y - col.gameObject.transform.position.y;

            if (Math.Abs(dx) <= w && Math.Abs(dy) <= h)
            {
                float wy = w * dy;
                float hx = h * dx;

                if (wy > hx)
                {
                    hit = wy > -hx ? Direction.Up : Direction.Left;
                }
                else
                {
                    hit = wy > -hx ? Direction.Right : Direction.Down;
                }
            }
            return hit;
        }
    }
}
