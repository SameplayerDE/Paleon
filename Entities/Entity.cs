using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Paleon
{
    public class Entity
    {
        public bool Active = true;
        public bool Visible = true;
        public Vector2 Position;

        public Scene Scene { get; private set; }

        public ComponentList Components { get; private set; }

        public float X
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public Layer Layer
        {
            get; internal set;
        }

        public Entity(Scene scene)
        {
            Scene = scene;

            Components = new ComponentList(this);
        }

        public void Awake()
        {
            Components.Awake();
        }

        public void Begin()
        {
            Components.Begin();
        }

        public void Update()
        {
            Components.Update();
        }

        public void Render()
        {
            Components.Render();
        }

        public void Add(Component component)
        {
            Components.Add(component);
        }

        public void Add(params Component[] components)
        {
            Components.Add(components);
        }

        public void Remove(Component component)
        {
            Components.Remove(component);
        }

        public void Remove(params Component[] components)
        {
            Components.Remove(components);
        }

        public void RemoveSelf()
        {
            Layer.Remove(this);

            Layer = null;
            Scene = null;
        }

        public T SceneAs<T>() where T : Scene
        {
            return Scene as T;
        }

        public T Get<T>() where T : Component
        {
            return Components.Get<T>();
        }

        public bool Has<T>() where T : Component
        {
            if (Components.Get<T>() != null)
                return true;

            return false;
        }
    }
}
