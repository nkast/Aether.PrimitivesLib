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

using Microsoft.Xna.Framework;

namespace tainicom.Aether.PrimitivesLib.Components.Platonic
{
    public class Quad: GeometricPrimitive
    {
        protected override void GenerateGeometry()
        {
            float size = 1f;

            Vector3[] normals =
            {
                new Vector3(0, 1, 0)
            };

            // Create each face in turn.
            foreach (Vector3 normal in normals)
            {
                // Get two vectors perpendicular to the face normal and to each other.
                Vector3 side1 = new Vector3(normal.Y, normal.Z, normal.X);
                Vector3 side2 = Vector3.Cross(normal, side1);

                // Six indices (two triangles) per face.
                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 1);
                AddIndex(CurrentVertex + 2);

                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 2);
                AddIndex(CurrentVertex + 3);

                // Four vertices per face.
                AddVertex((-side1 - side2) * size / 2, normal);
                AddVertex((-side1 + side2) * size / 2, normal);
                AddVertex(( side1 + side2) * size / 2, normal);
                AddVertex(( side1 - side2) * size / 2, normal);
            }
            
            return;
        }
        
        public virtual BoundingBox GetBoundingBox()
        {
            return new BoundingBox();
        }

    }
}
