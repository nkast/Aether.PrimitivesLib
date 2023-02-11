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
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace nkast.Aether.PrimitivesLib.Components.Platonic
{
    // Dodecahedron (Air) / Cube
    class Dodecahedron : GeometricPrimitive
    {           
        // The following Cartesian coordinates define the vertices of a dodecahedron 
        // centered at the origin:
        // (±1, ±1, ±1) 
        // (0, ±1/φ, ±φ) 
        // (±1/φ, ±φ, 0) 
        // (±φ, 0, ±1/φ)
        // φ is the golden ratio 1.618
        
        const float f = 1.618f;

        protected override void GenerateGeometry()
        {
            float size = 1f / f;
            var tmpVertices = GenerateVertices(size);
            FindEdges(tmpVertices, ( (f*size) / 2f));
            List<PolygonFace> faces = FindFaces(tmpVertices);

            foreach (var face in faces)
            {
                AddFace(face);
            }
            return;
        }

        private void AddFace(PolygonFace face)
        {
            int i0 = this.CurrentVertex;


            Vector3 v0 = face.Vertices[0].Position;
            Vector3 normal = face.Normal;

            if (Vector3.Dot(v0, normal) < 0)
            {
                AddVertex(face.Vertices[4].Position, -face.Normal);
                AddVertex(face.Vertices[3].Position, -face.Normal);
                AddVertex(face.Vertices[2].Position, -face.Normal);
                AddVertex(face.Vertices[1].Position, -face.Normal);
                AddVertex(face.Vertices[0].Position, -face.Normal);
            }
            else
            {
                AddVertex(face.Vertices[0].Position, face.Normal);
                AddVertex(face.Vertices[1].Position, face.Normal);
                AddVertex(face.Vertices[2].Position, face.Normal);
                AddVertex(face.Vertices[3].Position, face.Normal);
                AddVertex(face.Vertices[4].Position, face.Normal);
            }

            var ia = i0 + 1;
            for(int i=2;i<face.Vertices.Count;i++)
            {
                int ib = ia + 1;
                AddIndex(i0);
                AddIndex(ia);
                AddIndex(ib);
                ia = ib;
            }

        }

        private List<PolygonFace> FindFaces(List<PolygonVertex> vertices)
        {
            List<PolygonFace> faces = new List<PolygonFace>(20);
            foreach (var pv in vertices)
            {
                AddFaceFrom(faces, pv.Edges[0], pv, pv.Edges[1]);
                AddFaceFrom(faces, pv.Edges[0], pv, pv.Edges[2]);
                AddFaceFrom(faces, pv.Edges[1], pv, pv.Edges[0]);
                AddFaceFrom(faces, pv.Edges[1], pv, pv.Edges[2]);
                AddFaceFrom(faces, pv.Edges[2], pv, pv.Edges[0]);
                AddFaceFrom(faces, pv.Edges[2], pv, pv.Edges[1]);
            }

            Debug.Assert(faces.Count == 12);
            return faces;
        }

        /// <remarks>v0 is connected to v1 and v1 is connected to v2</remarks>
        private void AddFaceFrom(List<PolygonFace> faces, PolygonVertex v0, PolygonVertex v1, PolygonVertex v2)
        {            
            //search face 
            foreach (var face in faces)
            {
                if (face.Contains(v0) && face.Contains(v1) && face.Contains(v2))
                    return;
            }
            
            var normal = GetVertexNormal(v0, v1, v2);

            for (int i = 0; i < 3; i++)
            {
                var v3 = v2.Edges[i];
                if (v3 == v1) continue; // don't walk back
                var normal2 = GetVertexNormal(v1, v2, v3);
                var dot = Vector3.Dot(normal, normal2);
                if (dot < 0.85f) continue;
                for (int k = 0; k < 3; k++)
                {
                    var v4 = v3.Edges[k];
                    if (v4 == v2) continue; // don't walk back
                    var normal3 = GetVertexNormal(v2, v3, v4);
                    dot = Vector3.Dot(normal, normal3);
                    if (dot < 0.85f) continue;
                    for (int j = 0; j < 3; j++)
                    {
                        if (v0 == v4.Edges[j])
                        {
                            var face = new PolygonFace(normal);
                            face.Vertices.Add(v0);
                            face.Vertices.Add(v1);
                            face.Vertices.Add(v2);
                            face.Vertices.Add(v3);
                            face.Vertices.Add(v4);
                            faces.Add(face);
                        }
                    }
                }
            }

            return;
        }

        /// <remarks>v0 is connected to v1 and v1 is connected to v2</remarks>
        private static Vector3 GetVertexNormal(PolygonVertex v0, PolygonVertex v1, PolygonVertex v2)
        {
            var ba = v0.Position - v1.Position;
            var bc = v2.Position - v1.Position;
            ba.Normalize();
            bc.Normalize();
            return Vector3.Cross(ba, bc);
        }


        private List<PolygonVertex> GenerateVertices(float r)
        {
            List<PolygonVertex> vertices = new List<PolygonVertex>(20);

            // Calculate constants that will be used to generate vertices
            var phi = (float)(Math.Sqrt(5) - 1) / 2; // The golden ratio

            var a = (float)(1f / Math.Sqrt(3));
            var b = a / phi;
            var c = a * phi;

            // Generate each vertex
            foreach (var i in new[] { -1, 1 })
            {
                foreach (var j in new[] { -1, 1 })
                {
                    vertices.Add(new PolygonVertex(
                                        0,
                                        i * c * r,
                                        j * b * r));
                    vertices.Add(new PolygonVertex(
                                        i * c * r,
                                        j * b * r,
                                        0));
                    vertices.Add(new PolygonVertex(
                                        i * b * r,
                                        0,
                                        j * c * r));

                    foreach (var k in new[] { -1, 1 })
                        vertices.Add(new PolygonVertex(
                                            i * a * r,
                                            j * a * r,
                                            k * a * r));
                }
            }

            Debug.Assert(vertices.Count == 20);
            return vertices;
        }

        private void FindEdges(List<PolygonVertex> polygonVertices, float maxDistance)
        {
            float maxDistanceSquared = maxDistance * maxDistance;

            for (int i = 0; i < polygonVertices.Count; i++)
            {
                int c = 0;
                for (int k = i+1; k < polygonVertices.Count; k++)
                {
                    if (k == i) continue;
                    if (polygonVertices[i].Edges.Contains(polygonVertices[k])) continue;
                    float sqdist = Vector3.DistanceSquared(polygonVertices[i].Position, polygonVertices[k].Position);                    
                    if (sqdist - 0.001f <= maxDistanceSquared)
                    {
                        polygonVertices[i].Edges.Add(polygonVertices[k]);
                        polygonVertices[k].Edges.Add(polygonVertices[i]);
                    }
                }
            }
            foreach (var pv in polygonVertices)
            {
                Debug.Assert(pv.Edges.Count == 3);
            }
            return;
        }

    }
}
