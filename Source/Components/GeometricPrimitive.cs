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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Elementary;
using tainicom.Aether.Elementary.Leptons;
using tainicom.Aether.Elementary.Photons;
using tainicom.Aether.Elementary.Serialization;
using tainicom.Aether.Engine;
using tainicom.Aether.MonoGame;

namespace tainicom.Aether.PrimitivesLib.Components
{
    abstract public class GeometricPrimitive: IInitializable, IPhoton, ILepton, IWorldTransform, IWorldTransformUpdateable, IBoundingBox, IAetherSerialization
    {
        // During the process of constructing a primitive model, vertex
        // and index data is stored on the CPU in these managed lists.
        protected List<VertexPositionNormal> vertices = new List<VertexPositionNormal>();
        List<ushort> indices = new List<ushort>();

        // Once all the geometry has been specified, the InitializePrimitive
        // method copies the vertex and index data into these buffers, which
        // store it on the GPU ready for efficient rendering.
        protected VertexBuffer vertexBuffer;
        protected IndexBuffer indexBuffer;

        BoundingBox _bb;

        public GeometricPrimitive()
        {
            Textures = new ITexture[] { null };
        }

        public void Initialize(AetherEngine engine)
        {
            var device = AetherContextMG.GetDevice(engine);
            GenerateGeometry();
            Vector3 v = vertices[0].Position; 
            _bb = new BoundingBox(v,v);
            for(int i=1; i<vertices.Count; i++)//
            {
                v = vertices[i].Position;
                BoundingBox bb2 = new BoundingBox(v,v);
                BoundingBox.CreateMerged(ref _bb, ref bb2, out _bb);
            }
            InitializePrimitive(device);
        }


        abstract protected void GenerateGeometry();


        /// <summary>
        /// Adds a new vertex to the primitive model. This should only be called
        /// during the initialization process, before InitializePrimitive.
        /// </summary>
        protected void AddVertex(Vector3 position, Vector3 normal)
        {
            vertices.Add(new VertexPositionNormal(position, normal));
        }


        /// <summary>
        /// Adds a new index to the primitive model. This should only be called
        /// during the initialization process, before InitializePrimitive.
        /// </summary>
        protected void AddIndex(int index)
        {
            if (index > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("index");

            indices.Add((ushort)index);
        }


        /// <summary>
        /// Queries the index of the current vertex. This starts at
        /// zero, and increments every time AddVertex is called.
        /// </summary>
        protected int CurrentVertex
        {
            get { return vertices.Count; }
        }


        /// <summary>
        /// Once all the geometry has been specified by calling AddVertex and AddIndex,
        /// this method copies the vertex and index data into GPU format buffers, ready
        /// for efficient rendering.
        protected void InitializePrimitive(GraphicsDevice graphicsDevice)
        {
            // Create a vertex buffer, and copy our vertex data into it.
            vertexBuffer = new VertexBuffer(graphicsDevice,
                                            typeof(VertexPositionNormal),
                                            vertices.Count, BufferUsage.None);

            vertexBuffer.SetData(vertices.ToArray());

            // Create an index buffer, and copy our index data into it.
            indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort),
                                          indices.Count, BufferUsage.None);

            indexBuffer.SetData(indices.ToArray());
                       
        }
        

        #region Implement IPhoton

        public void Accept(IGeometryVisitor geometryVisitor)
        {
            geometryVisitor.SetVertices(this, 
                vertexBuffer, 0, 0, vertexBuffer.VertexCount,
                indexBuffer, 0, indexBuffer.IndexCount / 3);
        }

        public IMaterial Material { get; set; }
        public ITexture[] Textures { get; set; }

        #endregion


        #region Implement ILepton
        protected Matrix _localTransform = Matrix.Identity;

        Vector3 _position;
        Vector3 _scale = Vector3.One;
        Quaternion _rotation = Quaternion.Identity;

        public Matrix LocalTransform { get { return _localTransform; } } 

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; UpdateLocalTransform(); }
        }

        public Quaternion Rotation
        {
            get { return _rotation; }
            set { _rotation = value; UpdateLocalTransform(); }
        }

        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; UpdateLocalTransform(); }
        }
                
        protected void UpdateLocalTransform()
        {
            _localTransform = Matrix.CreateScale(_scale)
                            * Matrix.CreateFromQuaternion(_rotation) 
                            * Matrix.CreateTranslation(_position);

            _worldTransform = _localTransform * _parentWorldTransform;
        }
        #endregion
        

        #region  Implement IWorldTransform
        Matrix _parentWorldTransform = Matrix.Identity;
        Matrix _worldTransform = Matrix.Identity;
        public void UpdateWorldTransform(IWorldTransform parentWorldTransform) 
        {
            _parentWorldTransform = parentWorldTransform.WorldTransform;
            _worldTransform = _localTransform * _parentWorldTransform;
        }

        public Matrix WorldTransform { get { return _worldTransform; } }

        #endregion


        public virtual BoundingBox GetBoundingBox()
        {
            return _bb;
        }


        #region Implement IAetherSerialization
        public void Save(IAetherWriter writer)
        {
            writer.WriteInt32("Version", 1);
            writer.WriteVector3("Position", this._position);
            writer.WriteQuaternion("Rotation", _rotation);
            writer.WriteVector3("Scale", _scale);
            writer.WriteParticle("Material", (IAether)Material);
        }

        public void Load(IAetherReader reader)
        {
            int version;
            IAether p;
            reader.ReadInt32("Version", out version);
            reader.ReadVector3("Position", out _position);
            reader.ReadQuaternion("Rotation", out _rotation);
            reader.ReadVector3("Scale", out _scale);
            reader.ReadParticle("Material", out p); Material = p as IMaterial;
            UpdateLocalTransform();
        }
        #endregion

    }
}
