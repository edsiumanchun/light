# Castle level JSON guide

This guide explains the numeric values used by the level files in
`Resources/Json/Game/<stage>/<floor>.json`.

Each stage also has a `Config.json`. The arrays inside `Config.json` translate
numeric level values into asset names. Consequently, the same number can refer
to a different image or prefab in another stage.

## File structure

```json
{
  "layout": {
    "columns": 5,
    "rows": 4,
    "cellWidth": 131,
    "cellHeight": 170
  },
  "camera": {
    "visibleRows": 4,
    "followPlayerX": true,
    "followPlayerY": true,
    "horizontalDeadZone": 55,
    "verticalDeadZone": 55
  },
  "assets0": []
}
```

`assets0`, `assets1`, and `assets2` are horizontal level sections. Godot places
the sections next to one another from left to right.

### `layout`

| Key | Meaning |
|---|---|
| `columns` | Fallback number of playable columns when the grids are empty. |
| `rows` | Fallback number of playable rows when the grids are empty. |
| `cellWidth` | Horizontal distance, in pixels, between adjacent grid columns. |
| `cellHeight` | Vertical distance, in pixels, between adjacent floor boundaries. |

The current Godot builder detects the actual row and column count from the
grids. Therefore, the grids are authoritative when they are not empty.
`columns` and `rows` are primarily fallbacks and documentation.

### `camera`

| Key | Meaning |
|---|---|
| `visibleRows` | Approximate number of rows visible at once. This determines gameplay zoom. |
| `followPlayerX` | Whether the camera follows the player horizontally. |
| `followPlayerY` | Whether the camera follows the player vertically. |
| `horizontalDeadZone` | Horizontal movement allowed before the camera starts following. |
| `verticalDeadZone` | Vertical movement allowed before the camera starts following. |

## Grid formats

The preferred format is a nested `grid` array:

```json
{
  "name": "Object",
  "grid": [
    [0, 1, 0],
    [21, 0, 12]
  ]
}
```

Legacy files may use separate row keys. These two examples are equivalent:

```json
{
  "name": "Object",
  "gridArray0": [0, 1, 0],
  "gridArray1": [21, 0, 12]
}
```

Rows are read from top to bottom. Values within a row are read from left to
right.

For every spatial grid, `0` means **empty: create nothing in this position**.

## `PlateH`: horizontal floors and ceilings

`PlateH` values are **one-based indexes** into `paths.plateH` in the stage's
`Config.json`:

```text
asset index = abs(value) - 1
```

For Stage 0:

```json
"plateH": ["City", "GoUp", "GoDown", "Space"]
```

| JSON value | Config index | Stage 0 result |
|---:|---:|---|
| `0` | — | Empty |
| `1` | `0` | `City` platform |
| `2` | `1` | `GoUp` exit |
| `3` | `2` | `GoDown` exit |
| `4` | `3` | `Space` platform |

`PlateH` describes boundary lines, not room centers. A level with four room
rows normally has five `PlateH` rows: the top boundary, three internal
boundaries, and the bottom boundary.

Each horizontal value occupies one column width. Consecutive non-zero values
place horizontal plates next to each other.

## `PlateV`: vertical walls and gates

`PlateV` values are also **one-based indexes** into `paths.plateV`:

```text
asset index = abs(value) - 1
```

For Stage 0:

```json
"plateV": [
  "City",
  "GateCopper",
  "GateSilver",
  "GateGold",
  "GateExit",
  "Space"
]
```

| JSON value | Config index | Stage 0 result |
|---:|---:|---|
| `0` | — | Empty |
| `1` | `0` | `City` wall |
| `2` | `1` | Copper gate |
| `3` | `2` | Silver gate |
| `4` | `3` | Gold gate |
| `5` | `4` | Exit gate |
| `6` | `5` | `Space` wall |

A room grid with five columns normally has six `PlateV` columns because the
values represent the left boundary, four internal boundaries, and the right
boundary. Each vertical plate is positioned halfway between the horizontal
boundary above and the horizontal boundary below.

## `Object`: characters, enemies, items, and scenery

`Object` uses encoded numeric ranges. The loader first calculates:

```text
encoded index = abs(value) - 1
list index    = encoded index % 10
```

| JSON values | Meaning | Config list |
|---:|---|---|
| `0` | Empty | — |
| `1` | Player spawn fallback | `characters[0]` conceptually |
| `2`–`10` | Story character | `characters[value - 1]` |
| `11`–`20` | Enemy | `enemies[(value - 1) % 10]` |
| `21`–`30` | Item/key | `items[(value - 1) % 10]` |
| `31`–`40` | Solid obstacle | `obstacles[(value - 1) % 10]` |
| `41`–`50` | Non-solid decoration | `decorations[(value - 1) % 10]` |
| `51`–`60` | Trigger or potion pickup | `triggers[(value - 1) % 10]` |
| `91`–`100` | Boss | `bosses[(value - 1) % 10]` |
| `1001` and above | Legacy special enemy/gem code | Currently rendered as the `Greed` enemy in Godot |

Values ending in `0` select list index `9`, so they are valid only if that list
has at least ten entries. Invalid indexes are ignored.

### Stage 0 object examples

Stage 0 has these relevant lists:

```json
"enemies": ["EnemyBone", "EnemyBat", "Enemy1"],
"items": ["KeyCopper", "KeySilver", "KeyGold"],
"obstacles": ["Dark"],
"triggers": [
  "TriggerLight",
  "TriggerDark",
  "PotionGreen",
  "PotionBlue",
  "PotionRed"
]
```

| Value | Result |
|---:|---|
| `1` | Player spawn |
| `11` | `EnemyBone` |
| `12` | `EnemyBat` |
| `13` | `Enemy1` |
| `21` | Copper key |
| `22` | Silver key |
| `23` | Gold key |
| `31` | `Dark` obstacle |
| `51` | `TriggerLight` |
| `52` | `TriggerDark` |
| `53` | Green potion |
| `54` | Blue potion |
| `55` | Red/HP potion |
| `91` | `BossBone` |
| `2222`, `4444`, etc. | Legacy special code; currently `Greed` in Godot |

Object coordinates identify the bottom-center floor position of the cell.
Characters stand on that point. Items and decorations are moved upward by half
their visible height so their bottom edge rests on the floor.

## Signed values

Most plate and object lookups use `abs(value)`, so `12` and `-12` select the
same enemy asset in the current Godot port.

In the original Unity implementation, the sign of an `Object` value also
controlled facing direction. That facing behavior has not yet been ported to
Godot. Do not rely on positive/negative object values for direction until it is
implemented.

`Event` is an exception: its sign has a special meaning described below.

## `Door` and `Event`

These grids use their values as floor or story identifiers rather than asset
indexes.

### `Door`

| Value | Meaning |
|---:|---|
| `0` | No door |
| Any non-zero integer | Door leading to `abs(value)` floor number |

Example: `3` creates a door that calls `GameState.change_floor(3)`.

### `Event`

| Value | Meaning |
|---:|---|
| `0` | No event |
| Positive integer | An interaction that reads `Resources/Json/Story/<stage>/<floor>-<value>.json` |
| Negative integer | A story trigger using `abs(value)` as the story floor/id |

Example: `-4` starts the story data for story floor/id `4` when the player
enters the trigger.

In Stage 0/Floor 0, put `1` in the teacher's cell to load `0-1.json` and
`2` in the bed's cell to load `0-2.json`. The player presses Interact
within range of the cell. Each numbered event can be completed once per room load.

`layout.lockedEvents: [2]` starts event 2 locked. In a story's `event` block,
add `"unlocknext"` to unlock the following event number or `"unlock:5"` to
unlock event 5 directly. Omit the command when the story should unlock nothing.
Events omitted from `lockedEvents` are available from the start.

`layout.sleepEvent: 2` uses event 2's floor position for the sleep animation.
Set door destinations in the separate `Door` grid.

## Non-spatial asset values

These entries usually use only `gridArray0` and are not normal placement grids.

| Name | Number meaning | Index base | Current Godot status |
|---|---|---:|---|
| `Background` | Index in `paths.backgrounds` | Zero-based | Supported when no tiled `walls` list is used |
| `Audio` | Index in `paths.audios` | Zero-based | Supported |
| `Exit` | Legacy destination/exit value | Direct value | Retained in old JSON; ignored by the current builder |
| `Exit2` | Secondary legacy exit value | Direct value | Ignored by the current builder |
| `Teleport` | Legacy teleport mode/value | Direct value | Ignored by the current builder |
| `TeleportExit` | Legacy teleport-exit mode/value | Direct value | Ignored by the current builder |
| `WallSkip` | Row numbers where Unity should not generate an automatic wall | Direct row values | Unity-only; ignored by the current builder |
| `Effect` | Effect indexes | Direct list of values | Unity-only; ignored by the current builder |
| `ShowStory` | Legacy story visibility flag | Value not used | Ignored by the current builder |
| `GoUp` | Legacy upward-exit flag | Value not used | Ignored by the current builder |

Unlike plates and objects, `Background` and `Audio` are zero-based. For example,
with:

```json
"audios": ["elf", "bgm"]
```

`"gridArray0": [0]` selects `elf.mp3`, while `[1]` selects `bgm.mp3`.

## Asset paths and numbering

The stage's `Config.json` is the source of truth for every numbered asset. For
example:

```json
{
  "paths": {
    "plateH": ["City", "GoUp", "GoDown", "Space"],
    "items": ["KeyCopper", "KeySilver", "KeyGold"]
  }
}
```

Remember the two numbering conventions:

- `Background` and `Audio`: use the Config index directly, starting at `0`.
- `PlateH`, `PlateV`, and encoded `Object` categories: add `1` to the desired
  Config index, plus the category prefix for objects.

For an object list index `i`, use:

```text
enemy     = 11 + i
item      = 21 + i
obstacle  = 31 + i
decoration = 41 + i
trigger   = 51 + i
boss      = 91 + i
```

## Worked example

```json
{
  "name": "Object",
  "grid": [
    [0, 1, 0, 0, 23],
    [53, 21, 0, 12, 0]
  ]
}
```

- Row 0, column 1: fallback player spawn (`1`).
- Row 0, column 4: gold key (`23`).
- Row 1, column 0: green potion (`53`).
- Row 1, column 1: copper key (`21`).
- Row 1, column 3: the second enemy in the stage config (`12`).

Every other cell is empty because its value is `0`.
