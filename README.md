# Niantic Configurator

This project allows users to create simple primitive objects and place them in the real world space, using the [Lightship ARDK](https://lightship.dev/docs/ardk/)

You can change each object's materials on the fly, and move them around the scene using your phone as the controller and physics.

Many features in this project are created using a combination of the Lightship ARDK and Unity's ARFoundation package.

## Installation
### Clone the Repository with Git LFS
- [Download Git LFS from the website](https://git-lfs.com/)
  - Alternatively, you can install it on macOS using Homebrew: `brew install git-lfs` or MacPorts: `port install git-lfs`
- Run `git lfs install` in your Git command line to set up Git LFS for your user account. You only need to run this once per user account.
- Clone the repository normally, and it should pull in the Git LFS assets.
  - If you had previously cloned the repository, you can run `git lfs pull` to pull in the Git LFS assets.

### Enable the Niantic Lightship ARDK Plugin
In order to run the project and build to a device you'll need to enable the Niantic Lightship SDK.
- In Unity, select the menu `Lightship -> XR Plug-in Management`
- For iOS check the box for `Niantic Lightship SDK + Apple ARKit`
- For Android check the box `Niantic Lightship SDK + GOogle ARCore`
- 
### Add a Lightship API key
An API key needs to be added to the Lightship Settings in order to build and run the project.
- Sign up at [the lightship dev website] (https://lightship.dev/)
- Create a new project
- Copy the generated API key
- In Unity, select the menu `Lightship -> Settings` and paste your key into the `API Key` field

### Sign and build

For iOS you'll need to add a signing identity. This can be done in Unity's project settings, or within Xcode.
You can add a signing identity with `Project Settings -> Player -> Other Settings`, or within XCode under `Unity-iPhone -> Signing & Capabilities`.

## Features
### Meshing
As soon as the app starts, it will immediately start scanning the room and generating a mesh. The settings for this can be found in the `Sample Scene`.
In the scene hierarchy, expand the `XR Origin` and select the `Mesh Manager`. Here you can find the `ARMeshManager` component and the `Lightship Meshing Extension`.
A prefab is assigned in the ARMeshManager that sets up the default mesh, and materials that will be used to generate the mesh.
Keep in mind the `UIController` component will change the material assigned to the mesh at runtime.

#### Semantic Mesh Filtering
This project uses [Semantic Mesh Filtering](https://lightship.dev/docs/ardk/how-to/ar/semantic_mesh_filtering/) to prevent any people in room from being included in mesh generation.
Within the `Lightship Meshing Extension` mesh filtering and the block list are enabled, with the string `person` added to the block list.

### Light approximation
Using Unity's AR Foundation, light from the real world is estimated and calculated values are applied to the light in the scene.
You can choose what values are generated within the `AR Camera Manager` component, and adjust how the values are applied within the `BasicLightEstimation` component.

### Move Objects with Physics
The `PlayerController` script is responsible for handling input from the Player. If the player creates or picks up 
an object this script will move the object towards a target position and rotation using Physics forces. 
This allows the object to collide and interact with the dynamic generated mesh, while still allowing the player to 
control it's position by moving their phone.

### UIToolkit
The runtime UI is created using Unity's UIToolkit. Look through the `UIController` component to see how this is connected to the scene.

### Triplanar shading
[An example triplanar shader](https://github.com/keijiro/StandardTriplanar) is included that can texture the dynamic generated mesh without the need for generating UVs. 
