# Physics
The code used in the examples is available at the Following Git Repository.

## Creating a Physics Collider
A Collider in the Engine needs a Shape and a Layer.
The Shape is Created according to the bepuphysics implementation
```csharp
Entity boxShape = new Box(
                Vector3.Zero, //Position
                2f, //Width
                2f, //Height
                2f, //Depth
                1f); //Mass
```
The Layer is part of a Layer System, each layer that is going to be used has to be registered beforehand.
```csharp
int physicsLayerID = LayerManager.RegisterLayer( //Registering the Layer
						"physics", //The Name of the layer
						new Layer(1, 1));//The Layer Data(Group, Subgroup)
```
The Group is used to define general groups(e.g. such as "physics"), Subgroups are used to further divide the Objects into groups that can have enabled or disabled collisions(e.g. such as "bulletPlayer" and "bulletEnemy")

The Collider Component is constructed from the Shape and the Layer.
```csharp
//Creating A physics layer to be able to control which objects are meant to collide with each other
int physicsLayerID = LayerManager.RegisterLayer("physics", new Layer(1, 1));

//Creating the Components for the Physics Engine
//Note: There are different ways to get the LayerID than storing it.
Collider boxCollider = new Collider(boxShape, physicsLayerID);
Collider groundCollider = new Collider(groundShape, "physics");
Collider kineticCollider = new Collider(kineticShape, LayerManager.LayerToName(physicsLayerID));
```

## Kinematic Colliders
To Create Kinematic Colliders, which have infinite mass and infinite inertia, you can simply call the apropriate function in the PhysicsCollider.
```csharp
kineticCollider.PhysicsCollider.BecomeKinematic();
```
In fact you can also revert the process by calling
```csharp
kineticCollider.PhysicsCollider.BecomeDynamic();
```
## Static Colliders
A static Collider is an immovable object that can not be made dynamic nor kinetic.
It can never be moved, but it is computationally cheap which makes it attractive for map collisions.
```csharp
Entity boxShape = new Box(
                Vector3.Zero, //Position
                2f, //Width
                2f, //Height
                2f); //Depth
```