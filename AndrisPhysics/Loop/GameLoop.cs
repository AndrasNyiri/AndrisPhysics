using System;
using System.Collections.Generic;
using System.Linq;
using AndrisPhysics.Common;
using AndrisPhysics.Components;

namespace AndrisPhysics.Loop
{
    public class GameLoop
    {
        public bool shouldDrawToConsole = false;
        public long time;
        public readonly List<GameObject> activeObjects = new List<GameObject>();

        private long _deltaTime;
        private readonly long _startTime;

        private readonly List<Invokable> _invokables = new List<Invokable>();
        private readonly List<GameObject> _gameObjectsToRemove = new List<GameObject>();

        private const int FRAME_CAP = 500;
        private const float MILLIS_PER_SECOND = 1000f / FRAME_CAP;

        public float DeltaTime => _deltaTime / 1000f;
        public float Time => time / 1000f;

        private int _frames;
        private float _lastTime;

        public int Fps { get; private set; }

        private long _idCounter = 1;


        public GameLoop()
        {
            _startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        private void CalculateTime()
        {
            long nowMilis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long newTime = nowMilis - _startTime;
            _deltaTime = newTime - time;
            time = newTime;
        }


        public void Update()
        {
            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startTime - time < Math.Floor(MILLIS_PER_SECOND)) return;
            CalculateTime();

            if (_lastTime < Time)
            {
                _lastTime = Time + 1;
                Fps = _frames;
                _frames = 0;
            }
            else
            {
                _frames++;
            }


            for (int i = _invokables.Count - 1; i > -1; i--)
            {
                _invokables[i].Update(_deltaTime);
            }

            foreach (var gameObject in activeObjects)
            {
                gameObject.Update();
            }

            CheckCollision();

            if (shouldDrawToConsole)
            {
                Console.Clear();
                foreach (var gameObject in activeObjects)
                {
                    gameObject.Draw();
                }
            }

            foreach (var gameObject in _gameObjectsToRemove)
            {
                activeObjects.Remove(gameObject);
            }

            _gameObjectsToRemove.Clear();
        }

        public void CheckCollision(GameObject gameObject, bool invokeOnCollided = true)
        {
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            if (rigidbody.isKinematic) return;
            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
            rigidbody.isOnGround = false;
            foreach (var go in activeObjects)
            {
                BoxCollider goBoxCollider = go.GetComponent<BoxCollider>();
                Rigidbody goRigidbody = go.GetComponent<Rigidbody>();
                if (gameObject == go || boxCollider.CollidesWith(goBoxCollider) == Direction.Null) continue;
                if (invokeOnCollided)
                {
                    gameObject.InvokeCollidedDelegate(go);
                    go.InvokeCollidedDelegate(gameObject);
                }

                Vector2 x2 = go.transform.position;
                Direction collisionDir = boxCollider.CollidesWith(goBoxCollider);
                switch (collisionDir)
                {
                    case Direction.Up:
                        x2 = boxCollider.Bottom;
                        rigidbody.isOnGround = true;
                        gameObject.transform.position = new Vector2(gameObject.transform.position.x, go.transform.position.y + (goBoxCollider.HalfHeight + boxCollider.HalfHeight));
                        break;
                    case Direction.Left:
                        x2 = boxCollider.Right;
                        gameObject.transform.position = new Vector2(go.transform.position.x - (goBoxCollider.Radius + boxCollider.Radius), gameObject.transform.position.y);
                        break;
                    case Direction.Right:
                        x2 = boxCollider.Left;
                        gameObject.transform.position = new Vector2(go.transform.position.x + (goBoxCollider.Radius + boxCollider.Radius), gameObject.transform.position.y);
                        break;
                    case Direction.Down:
                        x2 = boxCollider.Top;
                        gameObject.transform.position = new Vector2(gameObject.transform.position.x, go.transform.position.y - (goBoxCollider.HalfHeight + boxCollider.HalfHeight));
                        break;
                }


                Vector2 v1 = rigidbody.velocity;
                Vector2 v2 = goRigidbody.velocity;
                Vector2 x1 = gameObject.transform.position;

                float m1 = rigidbody.mass;
                float m2 = goRigidbody.mass;

                float mass1 = 2 * m2 / (m1 + m2);
                float mass2 = 2 * m1 / (m1 + m2);

                var f1 = v1 - mass1 * (v1 - v2).Dot(x1 - x2) / (float)Math.Pow((x1 - x2).Magnitude, 2) * (x1 - x2);
                var f2 = v2 - mass2 * (v2 - v1).Dot(x2 - x1) / (float)Math.Pow((x2 - x1).Magnitude, 2) * (x2 - x1);

                rigidbody.velocity = f1;
                if (!goRigidbody.isKinematic) goRigidbody.velocity = f2;

            }
        }

        public void CheckCollision()
        {
            foreach (var gameObject in activeObjects)
            {
                CheckCollision(gameObject);
            }
        }

        public bool Raycast(GameObject from, GameObject to, bool onlyKinematic, out RaycastHit hit)
        {
            return Raycast(from.transform.position, to.transform.position, out hit, onlyKinematic);
        }

        public bool Raycast(Vector2 from, Vector2 to, out RaycastHit hit, bool onlyKinematic, params GameObject[] gameObjectsToIgnore)
        {
            var hits = new List<RaycastHit>();
            foreach (var go in activeObjects)
            {
                Rigidbody goRigidbody = go.GetComponent<Rigidbody>();
                BoxCollider goBoxCollider = go.GetComponent<BoxCollider>();
                if (goRigidbody.ignoreRaycast || onlyKinematic && !goRigidbody.isKinematic || gameObjectsToIgnore.Contains(go)) continue;
                var corners = goBoxCollider.GetCorners();
                for (var i = 0; i < corners.Length; i++)
                {
                    var prev = i > 0 ? corners[i - 1] : corners[corners.Length - 1];
                    var next = i < corners.Length - 1 ? corners[i + 1] : corners[0];
                    var point = LineIntersect(corners[i], next, from, to);
                    if (point != null)
                    {
                        hits.Add(new RaycastHit(go, point, (corners[i] - prev).UnitVector));
                    }
                }
            }

            if (hits.Count > 0)
            {
                hits.Sort((a, b) => (a.point - from).Magnitude.CompareTo((b.point - from).Magnitude));
                hit = hits[0];
                return true;
            }

            hit = null;
            return false;
        }


        public bool IsObjectBetween(GameObject go1, GameObject go2, bool onlyKinematic = true)
        {
            Vector2 v1 = go1.transform.position;
            Vector2 v2 = go2.transform.position;

            foreach (var go in activeObjects)
            {
                Rigidbody goRigidbody = go.GetComponent<Rigidbody>();
                BoxCollider goBoxCollider = go.GetComponent<BoxCollider>();
                if (go == go1 || go == go2 || onlyKinematic && !goRigidbody.isKinematic) continue;
                var col = goBoxCollider;
                var xOffset = go1.GetComponent<BoxCollider>().Width;
                if (v1.y > col.Top.y && v2.y < col.Bottom.y && v1.x < col.Right.x + xOffset && v1.x > col.Left.x - xOffset && v2.x < col.Right.x + xOffset && v2.x > col.Left.x - xOffset)
                {
                    return true;
                }
                if (v2.y > col.Top.y && v1.y < col.Bottom.y && v1.x < col.Right.x + xOffset && v1.x > col.Left.x - xOffset && v2.x < col.Right.x + xOffset && v2.x > col.Left.x - xOffset)
                {
                    return true;
                }
            }

            return false;
        }

        public Vector2 LineIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            Vector2 r = b - a;
            Vector2 s = d - c;

            var di = r.x * s.y - r.y * s.x;
            var u = ((c.x - a.x) * r.y - (c.y - a.y) * r.x) / di;
            var t = ((c.x - a.x) * s.y - (c.y - a.y) * s.x) / di;

            if (0 <= u && u <= 1 && 0 <= t && t <= 1)
            {
                return a + t * r;
            }
            return null;
        }

        public List<GameObject> GetInOverlapSphere(Vector2 pos, float radius)
        {
            List<GameObject> gos = new List<GameObject>();
            foreach (var go in activeObjects)
            {
                float distance = Vector2.Distance(go.transform.position, pos);
                if (distance <= radius)
                {
                    gos.Add(go);
                }
            }

            return gos;
        }

        public void RegisterGameObject(GameObject go)
        {
            go.id = _idCounter++;
            activeObjects.Add(go);
        }

        public void Destroy(GameObject go)
        {
            _gameObjectsToRemove.Add(go);
        }

        public Invokable AddInvokable(Invokable invokable)
        {
            _invokables.Add(invokable);
            invokable.SetGameLoop(this);
            return invokable;
        }

        public void RemoveInvokeable(Invokable invokable)
        {
            _invokables.Remove(invokable);
        }


    }
}
