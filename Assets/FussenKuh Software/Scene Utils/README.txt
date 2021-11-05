------------------
Scene Utils
------------------

------------------
Purpose
------------------
A simple way to load scenes.


------------------
Scripts
------------------
SceneUtils.cs -- Provides a handful of static functions to load/reload Unity scenes and retrieve the current scene's name. Also, provides a utility function to process the 'Escape' key as a 'quit' command.

SceneUtilsVisuals.cs -- Attach this to a Canvas with a panel element. This script provides static functions to load/reload Unity scenes while gracefully fading the display out and in.

------------------
Prefabs
------------------
Canvas - Loading Fader -- A prefab pre-configured with the SceneUtilsVisuals.cs script. Simply drag this Canvas into your scene and make calls to SceneUtilsVisuals.LoadScene() and SceneUtilsVisuals.ReloadScene()

------------------
Demo Scene
------------------
SceneUtils.scene -- A simple scene that shows the 'Canvas - Loading Fader' prefab in action.
