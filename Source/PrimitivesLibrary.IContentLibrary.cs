#region License
//   Copyright 2015 Kastellanos Nikolaos
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using nkast.Aether.PrimitivesLib.Components.Platonic;
using nkast.Aether.PrimitivesLib.Materials;
using tainicom.ProtonType.Contracts;

namespace nkast.Aether.PrimitivesLib 
{
    public partial class PrimitivesLibrary : IContentLibrary
    {
        List<LibraryItemDescription> _items = null;
        ReadOnlyCollection<LibraryItemDescription> _readOnlyItems;
        public IList<LibraryItemDescription> Items
        {
            get
            {
                if (_readOnlyItems == null) _readOnlyItems = new ReadOnlyCollection<LibraryItemDescription>(_items);
                return _readOnlyItems;
            }
        }

        private void InitializeContentLibrary()
        {
            _items = new List<LibraryItemDescription>();

            //Platonic solids
            AddLibraryItem(typeof(Tetrahedron)).SetMaterial("BasicMaterial"); // Fire
            AddLibraryItem(typeof(Hexahedron)).SetMaterial("BasicMaterial"); // Earth
            AddLibraryItem(typeof(Octahedron)).SetMaterial("BasicMaterial"); // Air
            AddLibraryItem(typeof(Dodecahedron)).SetMaterial("BasicMaterial"); // Aether?
            //AddLibraryItem(typeof(Icosahedron)).SetMaterial("BasicMaterial"); // Water

            //other
            //AddLibraryItem(typeof(Sphere));
            //AddLibraryItem(typeof(Teapot));
            AddLibraryItem(typeof(Quad)).SetMaterial("BasicMaterial");

            //materials
            AddLibraryItem(typeof(tainicom.Aether.Core.Materials.BasicMaterial)).AddPropertyValue("LightingEnabled", true);
            //.AddPropertyValue("DirectionalLight0.Enabled", true); //TODO: support nested properties
            //.AddPropertyValue("DirectionalLight1.Enabled", true);
            //.AddPropertyValue("DirectionalLight2.Enabled", true);
            
            //cameras
            AddLibraryItem(typeof(tainicom.Aether.Core.Cameras.PerspectiveCamera));

            // Plasmas
            AddLibraryItem(typeof(tainicom.Aether.Core.LeptonPlasma));

            _items.Sort(CompareLibraryItemDescription);
            return;
        }

        private LibraryItem AddLibraryItem(Type type)
        {
            LibraryItem libraryItem = new LibraryItem(type.Name, type);
            _items.Add(libraryItem);
            return libraryItem;
        }

        private static int CompareLibraryItemDescription(LibraryItemDescription x, LibraryItemDescription y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }

    public static class LibraryItemExtensions
    {
        public static void AddPropertyValue(this LibraryItem item, LibraryItemPropertyLibraryItemName propertyValue)
        {
            if (item.PropertiesInitialization == null)
            {
                item.PropertiesInitialization = new[] { propertyValue };
            }
            else
            {
                Array.Resize<object>(ref item.PropertiesInitialization, item.PropertiesInitialization.Length + 1);
                item.PropertiesInitialization[item.PropertiesInitialization.Length-1] = propertyValue;
            }
        }

        public static void AddPropertyValue(this LibraryItem item, LibraryItemPropertyValue propertyValue)
        {
            if (item.PropertiesInitialization == null)
            {
                item.PropertiesInitialization = new[] { propertyValue };
            }
            else
            {
                Array.Resize<object>(ref item.PropertiesInitialization, item.PropertiesInitialization.Length + 1);
                item.PropertiesInitialization[item.PropertiesInitialization.Length - 1] = propertyValue;
            }
        }

        public static LibraryItem AddPropertyValue(this LibraryItem item, String propertyName, String libraryItemName)
        {
            var propertyValue = new LibraryItemPropertyLibraryItemName()
            {
                PropertyName = propertyName,
                LibraryItemName = libraryItemName
            };
            item.AddPropertyValue(propertyValue);
            return item;
        }

        public static LibraryItem AddPropertyValue(this LibraryItem item, string propertyName, object value)
        {
            var propertyValue = new LibraryItemPropertyValue()
            {
                PropertyName = propertyName,
                Value = value
            };
            item.AddPropertyValue(propertyValue);
            return item;
        }

        internal static LibraryItem SetMaterial(this LibraryItem item, String material)
        {
            item.AddPropertyValue("Material", material);
            return item;
        }

        internal static LibraryItem SetTextures(this LibraryItem item, String texture)
        {
            item.AddPropertyValue("Textures", texture);
            return item;
        }
    }
}
