﻿using System;
using System.Collections.Generic;
using AndrisPhysics.Common;
using AndrisPhysics.Configuration;
using AndrisPhysics.Loop;

namespace AndrisPhysics.Components
{
    public class GameObject
    {
        public delegate void OnCollidedWithGameObjectDelegate(GameObject go);
        public delegate void OnBelowGroundDelegate();

        public OnCollidedWithGameObjectDelegate onCollidedWithGameObject;
        public OnBelowGroundDelegate onBelowGround;

        public GameLoop gameLoop;
        public Transform transform;
        public List<Component> components = new List<Component>();

        public string name;
        public long id;

        public GameObject(float x, float y, string name = "", params Component[] initComponents)
        {
            this.name = name;
            if (string.IsNullOrEmpty(name))
            {
                this.name = GetType().ToString();
            }
            transform = new Transform(x, y, this);
            foreach (var component in initComponents)
            {
                AddComponent(component);
            }
        }

        public void Assign(GameLoop gl)
        {
            this.gameLoop = gl;
        }

        public virtual void Update()
        {
            foreach (var component in components)
            {
                component.Update();
            }
            if (transform.position.y < Settings.belowGround)
            {
                InvokeOnBelowGroundDelegate();
            }
        }

        public GameObject AddComponent(Component component)
        {
            component.Assign(this);
            this.components.Add(component);
            return this;
        }

        public GameObject AddComponent<T>() where T : Component, new()
        {
            T newComponent = new T();
            newComponent.Assign(this);
            this.components.Add(newComponent);
            return this;
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (var x in this.components)
            {
                if (!(x is T)) continue;
                return (T)x;
            }
            return default(T);
        }


        public virtual void Draw()
        {
            try
            {
                BoxCollider boxCollider = GetComponent<BoxCollider>();
                if (boxCollider == null) return;
                Vector2 origin = new Vector2(25, 25);

                Vector2 topLeft = origin + new Vector2((float)Math.Floor(transform.position.x - boxCollider.Radius), (float)Math.Ceiling(-boxCollider.Top.y));
                Console.SetCursorPosition((int)topLeft.x, (int)topLeft.y);
                Console.Write(".");

                Vector2 topRight = origin + new Vector2((float)Math.Floor(transform.position.x + boxCollider.Radius), (float)Math.Ceiling(-boxCollider.Top.y));
                Console.SetCursorPosition((int)topRight.x, (int)topRight.y);
                Console.Write(".");

                Vector2 playerPos = origin + new Vector2((float)Math.Floor(transform.position.x), (float)Math.Ceiling(-transform.position.y));
                Console.SetCursorPosition((int)playerPos.x, (int)playerPos.y);
                Console.Write("O");

                Vector2 bottomRight = origin + new Vector2((float)Math.Floor(transform.position.x + boxCollider.Radius), (float)Math.Ceiling(-boxCollider.Bottom.y));
                Console.SetCursorPosition((int)bottomRight.x, (int)bottomRight.y);
                Console.Write(".");

                Vector2 bottomLeft = origin + new Vector2((float)Math.Floor(transform.position.x - boxCollider.Radius), (float)Math.Ceiling(-boxCollider.Bottom.y));
                Console.SetCursorPosition((int)bottomLeft.x, (int)bottomLeft.y);
                Console.Write(".");
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        public void InvokeCollidedDelegate(GameObject go)
        {
            if (onCollidedWithGameObject != null) onCollidedWithGameObject.Invoke(go);
        }

        public void InvokeOnBelowGroundDelegate()
        {
            if (onBelowGround != null) onBelowGround.Invoke();
        }

        public virtual void Destory()
        {
            gameLoop.Destroy(this);
        }

    }
}
