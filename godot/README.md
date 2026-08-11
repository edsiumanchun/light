# Castle Godot port

This is a Godot 4 migration scaffold. It reuses the original JSON floors, PNG/JPG sprites and MP3 audio while replacing Unity-specific runtime systems with GDScript.

Implemented foundations:

- JSON-driven stage and floor loading
- Generated backgrounds, platforms, walls and objects
- 2D player movement and double jump
- Interaction detector
- Basic enemies and combat
- Gates, keys, pickups and floor exits
- JSON save data in `user://castle-save.json`
- HUD for stage, floor, HP, MP and keys
- JSON dialogue loading and an interactive dialogue panel

Unity `.prefab`, `.unity`, `.controller`, `.spriteLib` and `.psb` files are not used by Godot. Export character artwork to PNG/sprite sheets or supply GLB/FBX models before replacing the placeholder player/enemy visuals.

Open the repository root containing `project.godot` in Godot 4 and run the project.

Controls: use A/D or arrows to move, Space to jump, E/Enter or the on-screen button to interact, and left-click/tap/the button to advance dialogue. Story characters can also be clicked directly.
