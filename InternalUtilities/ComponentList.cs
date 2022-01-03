using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class ComponentList
    {
        public Entity Entity { get; internal set; }

        private List<Component> components;

        internal ComponentList(Entity entity)
        {
            Entity = entity;

            components = new List<Component>();
        }

        public void Add(Component component)
        {
            components.Add(component);
            component.Entity = Entity;
        }

        public void Add(params Component[] components)
        {
            for (int i = 0; i < components.Length; i++)
                Add(components[i]);
        }

        public void Remove(Component component)
        {
            components.Remove(component);
            component.Entity = null;
        }

        public void Remove(params Component[] components)
        {
            for (int i = 0; i < components.Length; i++)
                Remove(components[i]);
        }

        public int Count
        {
            get { return components.Count; }
        }

        internal void Awake()
        {
            for (int i = 0; i < components.Count; i++)
                components[i].Awake();
        }

        internal void Begin()
        {
            for (int i = 0; i < components.Count; i++)
                components[i].Begin();
        }

        internal void Update()
        {
            for(int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component.Active)
                    component.Update();
            }
        }

        internal void Render()
        {
            for (int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component.Visible)
                    component.Render();
            }
        }

        public Component this[int index]
        {
            get
            {
                if (index < 0 || index >= components.Count)
                    throw new IndexOutOfRangeException();
                else
                    return components[index];
            }
        }

        public T Get<T>() where T : Component
        {
            for(int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component is T)
                    return component as T;
            }
            return null;
        }

        public IEnumerable<T> GetAll<T>() where T : Component
        {
            for(int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component is T)
                    yield return component as T;
            }
        }

        public Component[] ToArray()
        {
            return components.ToArray<Component>();
        }

    }
}
