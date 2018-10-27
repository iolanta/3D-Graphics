# 3D Graphics Demo
This demo shows simple, from the bottom, storage, transform and render of simple 3D objects using basic drawing tools of Windows Forms.
## Storage
3D objects are stored as list of points and and list of polygons of any amount of edges. Any objectes that are genereted, loaded or transformed in demo can be saved to simple txt format. Any custom models in the same format as well as special ROTATION format can be loaded at run-time.
## Transforms
3D objects can be subjected to any kind of basic aphine transforms as well as one few complex ones including:
- offset
- rotation around axes
- scale around axes
- rotation and scale(mirroring) around object's center
- rotation around set line
All transfroms are implemented using matrixes.
## Render
So far demo renders objects naive way.Invisible sides are NOT ommited. 
### Perspectives:
- Central(with fixed focus)
- Orthographic by any axis
- Isomentric
## Objects
The demo contains preset of some basic objects and curves
- Cube
- Tetrahedron
- Octahedron
- Icosahedron
- Some 3D curves

