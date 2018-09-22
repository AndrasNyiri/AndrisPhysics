using AndrisPhysics.Common;

namespace AndrisPhysics.Components
{
    class Transform
    {
        public GameObject gameObject;
        public Vector2 position;

        public Transform(float x, float y, GameObject gameObject)
        {
            this.gameObject = gameObject;
            position = new Vector2(x, y);
        }

        public Transform(Vector2 position)
        {
            this.position = position;
        }

    }
}
