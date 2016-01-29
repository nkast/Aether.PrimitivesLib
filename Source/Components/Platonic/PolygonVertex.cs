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

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace tainicom.Aether.PrimitivesLib.Components
{
    class PolygonVertex
    {
        private Vector3 _position;
        private List<PolygonVertex> _edges;

        public Vector3 Position { get { return _position; } }
        public List<PolygonVertex> Edges { get { return _edges; } }

        public PolygonVertex(Vector3 position)
        {
            this._position = position;
            _edges = new List<PolygonVertex>(3);
        }

        public PolygonVertex(float x, float y, float z)
        {
            this._position = new Vector3(x, y, z);
            _edges = new List<PolygonVertex>(3);
        }
    }

    class PolygonFace
    {
        Plane _plane;
        private Vector3 _normal;
        private List<PolygonVertex> _vertices;

        public Plane Plane { get { return _plane; } }
        public Vector3 Normal { get { return _normal; } }
        public List<PolygonVertex> Vertices { get { return _vertices; } }

        public PolygonFace(Plane plane)
        {
            this._plane = plane;
            _vertices = new List<PolygonVertex>(5);
        }

        public PolygonFace(Vector3 normal)
        {
            this._normal = normal;
            _vertices = new List<PolygonVertex>(5);
        }
        
        internal bool Contains(PolygonVertex vertex)
        {
            if (_vertices.Contains(vertex))
                return true;
            return false;
        }
    }
}
