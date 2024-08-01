# SolidMesh
## Data structure for global mesh operations

This repo contains an implementation of the SolidMesh data structure introduced on [EUROGRAPHICS 2024](https://diglib.eg.org/server/api/core/bitstreams/a9715c76-57a6-4fbb-bd69-0052eeafc86c/content) and [CAD 2024](https://www.cad-conference.net/files/CAD24/CAD24_174-178.pdf) conferences. The implementation is created in C# using the [Unity Real-Time Development Platform](https://unity.com/).  

## How to load the test scene?
1. Download ZIP or clone the repository.

2. Create a new Unity Project. Choose 3D Bulit-in Render Pipeline. I've used `Unity 2022.3.15f1` for testing, make sure you are using a compatible version of Unity.
<img width="799" alt="Creating a new project" src="https://github.com/user-attachments/assets/c5fd5abe-dcc2-494c-afe0-68d48e376442">

4. Move the downloaded files to your project folder (overwriting `Assets`, `Packages`, and `ProjectSettings` folders).

5. Open `Test Scene` from the `Scenes` folder.
<img width="982" alt="Loading the test scene" src="https://github.com/user-attachments/assets/89f27f90-856d-4c20-b724-b6252f537e18">

## How to use SolidMeshBehaviour?
### Import options
In the Test Scene you can see a camera, a light, and a 3rd GameObject containing a `Solid Mesh Behaviour` component. You can select the input mesh by clicking the bullet beside the Input Mesh field. 
<img width="978" alt="Loading a new input mesh" src="https://github.com/user-attachments/assets/bda0d428-26e3-4c47-8153-f4f46b2e6716">

**Note:** This GameObject also contains a `Mesh Filter` and a `Mesh Renderer` component. These components are handled by the `Solid Mesh Behaviour`, please do not change them.

You can use the following options by importing:
* Swap the Y and Z coordinates
* Transform mesh to the unit sphere (translate the center of gravity to the origin, rescale the mesh to fit inside the unit sphere).
* Move mesh to the ground (translate along the Y-axis to set the minimum of the Y coordinates of the vertices to zero).
* Flip normal vectors of triangles (and vertices).

<img width="335" alt="SolidMeshBehaviour options" src="https://github.com/user-attachments/assets/7739e7e5-ecb4-45e5-a96b-bea706a89a7c">

### Laplacian smoothing
Smooth the mesh by clicking the **Laplacian Smoothing** button.
<img width="962" alt="Laplacian smoothing" src="https://github.com/user-attachments/assets/11de3ceb-5de0-49e2-a0af-a7e7c0598205">

**Note:** When running algorithms, we measure their execution time. Please check the console window for the measurement results.

### Loop subdivision
Subdivide the mesh by clicking the **Loop Subdivision** button.
<img width="956" alt="Loop subdivision" src="https://github.com/user-attachments/assets/18f5fb01-e114-475b-85cd-5f0ce20f4330">

**Note:** While testing I used a 16-bit index buffer, which supports at most 65536 vertices in a mesh. Therefore, if the number of vertices of the subdivided mesh exceeds this bound, the operation will be ignored. Comment out the `return` statement of `Subdivide()` function in `SolidMeshBehaviour.cs` to eliminate this protection.

### Reimport input mesh
Use the Reimport button to reload the original input mesh. 
<img width="959" alt="Reimport input mesh" src="https://github.com/user-attachments/assets/12455fd3-8d96-4718-97d6-c4f250326567">

### Testing with your own meshes
You can use custom meshes for testing. We used simple OBJ files imported to Unity. We strongly recommend disabling all of Unity's built-in mesh compression/optimization options, since using these, the topology of the mesh may change. 

**Note:** SolidMesh data structure is created to store and manipulate a special class of surfaces. The limitations are the following.
* The mesh has to be a topological 2-manifold.
*  Only triangle meshes are allowed, quad meshes are not supported (yet).
* Boundaries are not allowed (yet), i.e. the surface represented by the mesh has to be closed.
<img width="377" alt="Import settings" src="https://github.com/user-attachments/assets/7a311352-6fc1-47dd-bb84-41df2b3fec2c">
