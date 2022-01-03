using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class InventoryCmp : Component
    {

        private Item clothing;
        private Item tool;
        private Item cargo;

        public Item Clothing 
        { 
            get { return clothing; }
            set
            {
                if (clothing == value)
                    return;

                clothing = value;

                cbOnClothingChanged?.Invoke(value);
            }
        }

        public Item Tool
        {
            get { return tool; }
            set
            {
                if (tool == value)
                    return;

                tool = value;

                cbOnToolChanged?.Invoke(value);
            }
        }

        public Item Cargo
        {
            get { return cargo; }
            set
            {
                if (cargo == value)
                    return;

                cargo = value;

                cbOnCargoChanged?.Invoke(value);
            }
        }

        private Action<Item> cbOnClothingChanged;
        private Action<Item> cbOnToolChanged;
        private Action<Item> cbOnCargoChanged;

        public InventoryCmp() : base(false, false)
        {

        }

        public void RegisterOnClothingChangedCallback(Action<Item> callback)
        {
            cbOnClothingChanged += callback;
        }

        public void RegisterOnToolChangedCallback(Action<Item> callback)
        {
            cbOnToolChanged += callback;
        }

        public void RegisterOnCargoChangedCallback(Action<Item> callback)
        {
            cbOnCargoChanged += callback;
        }

        public void UnregisterOnClothingChangedCallback(Action<Item> callback)
        {
            cbOnClothingChanged -= callback;
        }

        public void UnregisterOnToolChangedCallback(Action<Item> callback)
        {
            cbOnToolChanged -= callback;
        }

        public void UnregisterOnCargoChangedCallback(Action<Item> callback)
        {
            cbOnCargoChanged -= callback;
        }

    }
}
