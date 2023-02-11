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
using Microsoft.Xna.Framework;

namespace nkast.Aether.PrimitivesLib.Components.Platonic
{
    // Hexahedron (Air) / Cube
    class Octahedron : GeometricPrimitive
    {   
        protected override void GenerateGeometry()
        {
            float sqrtOf2 = (float)Math.Sqrt(2);

            float size = 1f / sqrtOf2;

            Vector3[] sizes2 =
            {
                new Vector3(+1,  0,  0) * size,
                new Vector3(-1,  0,  0) * size,                
            };

            Vector3[] sizes =
            {
                new Vector3( 0, +1,  0) * size,
                new Vector3( 0,  0, +1) * size,
                new Vector3( 0, -1,  0) * size,
                new Vector3( 0,  0, -1) * size,                

                new Vector3(-1, +1, 0) * size,
            };

            for (int i = 0; i < 2; i++)
            {   
                for (int k = 0; k < 4; k++)
                {
                    AddIndex(CurrentVertex + 0);
                    AddIndex(CurrentVertex + 1);
                    AddIndex(CurrentVertex + 2);

                    Vector3 v0 = sizes[k];
                    Vector3 v1 = sizes2[i];
                    Vector3 v2 = sizes[(k+1)%4];
                    
                    Vector3 normal = -Vector3.Cross(v0-v1,v0-v2);
                    normal.Normalize();

                    if (Vector3.Dot(v0, normal) < 0)
                    {
                        normal = -normal;
                        Vector3 tmp = v1;
                        v1 = v2;
                        v2 = tmp;
                    }

                    AddVertex(v0, normal);
                    AddVertex(v1, normal);
                    AddVertex(v2, normal);
                }            
            }
            return;
        }
        
    }
}
