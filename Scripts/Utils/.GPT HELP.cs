// Create a new mesh object
Mesh mesh = new Mesh();

// Create lists to hold the vertices, triangles, and normals
List<Vector3> vertices = new List<Vector3>();
List<int> triangles = new List<int>();
List<Vector3> normals = new List<Vector3>();

// Iterate through the voxel grid
for (int x = 0; x < grid.width; x++) {
  for (int y = 0; y < grid.height; y++) {
    for (int z = 0; z < grid.depth; z++) {
      
      // Check if the voxel is active
      if (grid.GetVoxel(x, y, z) == VoxelState.Active) {
        
        // Create the vertices for the voxel
        vertices.Add(new Vector3(x, y, z));
        vertices.Add(new Vector3(x + 1, y, z));
        vertices.Add(new Vector3(x, y, z + 1));
        vertices.Add(new Vector3(x + 1, y, z + 1));
        vertices.Add(new Vector3(x + 1, y + 1, z));
        vertices.Add(new Vector3(x + 1, y + 1, z + 1));
        vertices.Add(new Vector3(x, y + 1, z + 1));
        vertices.Add(new Vector3(x, y + 1, z));

        // Create the triangles for the voxel
        triangles.AddRange(new int[] {
          vertices.Count - 8, vertices.Count - 7, vertices.Count - 6,
          vertices.Count - 8, vertices.Count - 6, vertices.Count - 5,
          vertices.Count - 7, vertices.Count - 4, vertices.Count - 3,
          vertices.Count - 7, vertices.Count - 3, vertices.Count - 6,
          vertices.Count - 4, vertices.Count - 1, vertices.Count - 2,
          vertices.Count - 4, vertices.Count - 2, vertices.Count - 3,
          vertices.Count - 1, vertices.Count - 8, vertices.Count - 5,
          vertices.Count - 1, vertices.Count - 5, vertices.Count - 2
        });
        
        // Create the normals for the voxel
        for (int i = 0; i < 8; i++) {
          normals.Add(Vector3.up);
        }
      }
    }
  }
}

// Set the mesh's vertices, triangles, and normals
mesh.vertices = vertices.ToArray();
mesh.triangles = triangles.ToArray();
mesh.normals = normals.ToArray();
