using AndrisPhysics.Common;
using AndrisPhysics.Configuration;

namespace AndrisPhysics.Components
{
    public class Rigidbody : Component
    {
        public float gravityModifier = 3.5f;
        public float mass;
        public bool isKinematic;
        public bool ignoreRaycast;
        public float drag = 1.2f;
        public Vector2 velocity = Vector2.zero;
        public bool isOnGround;

        private float _targetDrag;


        public Rigidbody(float mass, bool isKinematic, bool ignoreRaycast)
        {
            this.mass = mass;
            this.isKinematic = isKinematic;
            this.ignoreRaycast = ignoreRaycast;
        }

        public override void Update()
        {
            if (isKinematic) return;
            ApplyGravity();
            ApplyVelocity();
            _targetDrag = isOnGround ? drag * 3f : drag;
            ApplyDrag();
        }

        public void AddForce(Vector2 force)
        {
            this.velocity += force;
        }

        private void ApplyDrag()
        {
            velocity -= velocity * _targetDrag * gameObject.gameLoop.DeltaTime;
        }

        private void ApplyHeavyDrag()
        {
            velocity -= velocity * 20f * gameObject.gameLoop.DeltaTime;
        }

        private void ApplyVelocity()
        {
            if (velocity.Magnitude > Settings.velocityLimit)
            {
                ApplyHeavyDrag();
            }
            gameObject.transform.position += velocity * gameObject.gameLoop.DeltaTime;
        }

        private void ApplyGravity()
        {
            velocity += new Vector2(0, Settings.gravity) * gravityModifier * gameObject.gameLoop.DeltaTime;
        }
    }
}
