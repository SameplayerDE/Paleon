using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class EntityList : IEnumerable<Entity>, IEnumerable
    {
        public Scene Scene { get { return Layer.Scene; } }
        public Layer Layer { get; private set; }

        private List<Entity> entities;
        private List<Entity> toAdd;
        private List<Entity> toRemove;

        public EntityList(Layer layer)
        {
            Layer = layer;

            entities = new List<Entity>();
            toAdd = new List<Entity>();
            toRemove = new List<Entity>();
        }

        public void Add(Entity entity)
        {
            toAdd.Add(entity);
        }

        public void Remove(Entity entity)
        {
            toRemove.Add(entity);
        }

        public int Count
        {
            get { return entities.Count; }
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Entity[] ToArray()
        {
            return entities.ToArray<Entity>();
        }

        public void UpdateLists()
        {
            if (toAdd.Count > 0)
            {
                for (int i = 0; i < toAdd.Count; i++)
                {
                    Entity entity = toAdd[i];
                    entity.Awake();
                }

                for (int i = 0; i < toAdd.Count; i++)
                {
                    Entity entity = toAdd[i];
                    entities.Add(entity);
                    entity.Begin();
                }

                toAdd.Clear();
            }

            if (toRemove.Count > 0)
            {
                for (int i = 0; i < toRemove.Count; i++)
                {
                    Entity entity = toRemove[i];
                    entities.Remove(entity);
                }

                toRemove.Clear();
            }
        }

        public void Update()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                if (entity.Active)
                    entity.Update();
            }
        }

        public void Render()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                if (entity.Visible)
                    entity.Render();
            }
        }
    }
}
