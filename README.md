# 3D Graphics Demo 
:ok_hand:OLD RELEASE [TRY](https://github.com/iolanta/3D-Graphics/releases/download/v0.6/3D_graphics.exe):ok_hand:

This demo shows simple, from the bottom, storage, transform and render of simple 3D objects using basic drawing tools of Windows Forms powered by CPU.
## Storage
3D objects are stored as list of points and and list of polygons of any amount of edges. Any objectes that are genereted, loaded or transformed in demo can be saved to simple txt format simial to .OBJ format. Any custom models in the same format as well as special ROTATION format can be loaded at run-time.
## Transforms
3D objects can be subjected to any kind of basic aphine transforms as well as one few complex ones including:
- offset
- rotation around axes
- scale around axes
- rotation and scale(mirroring) around object's center
- rotation around set line
All transfroms are implemented using matrixes.
## Render
This demo utiliases Backface Culling and W-buffer algortithm to render Triangle and Quad polygons, which are rasterized by linear interpolation. 
## Lightning
Lighting is Lambertian model rasterized linearly by Guro method. 
## Textures
Simple texture application is also inlcuded, texture can only be a single  ARGB image applyed to every visible polygon POST rasterization. 
## Camera
Objects are viewed using camera controlled by keyboard WASD/QE/RF. Camera is implemented trough View and Projection transforms, and includes FPS counter.
### ~~Perspectives:~~
- ~~Central(with fixed focus)~~
- ~~Orthographic by any axis~~
- ~~Isomentric~~
## Objects
The demo contains preset of some basic objects and curves
- Cube
- Tetrahedron
- Octahedron
- Icosahedron
- Torus
- Some 3D curves

