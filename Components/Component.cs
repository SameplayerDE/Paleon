using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Component
    {
        public virtual Entity Entity { get; internal set; }
        public bool Active;
        public bool Visible;

        public Component(bool active, bool visible)
        {
            Active = active;
            Visible = visible;
        }

        public virtual void Awake()
        {

        }

        public virtual void Begin()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Render()
        {

        }

        public void RemoveSelf()
        {
            if (Entity != null)
                Entity.Remove(this);
        }

        public T SceneAs<T>() where T : Scene
        {
            return Scene as T;
        }

        public T EntityAs<T>() where T : Entity
        {
            return Entity as T;
        }

        public Scene Scene
        {
            get { return Entity?.Scene; }
        }
    }
}
