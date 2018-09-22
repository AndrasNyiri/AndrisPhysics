using AndrisPhysics.Components;

namespace AndrisPhysics.Common
{
    class RaycastHit
    {
        public GameObject go;
        public Vector2 point;
        public Vector2 normal;

        public RaycastHit(GameObject go, Vector2 point, Vector2 normal)
        {
            this.go = go;
            this.point = point;
            this.normal = normal;
        }
    }
}
