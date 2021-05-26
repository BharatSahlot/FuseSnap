# Goals
- Grid based map. Designer should be able to set grid size and cell size in the map file.
- Keep these map files on a server, from where they can be downloaded without having to update the app on the play store.
- Define components. Syntax: name, type { config values }, row, column, rotation
    Example:
        name: F1
        type: Fuse { resistance: 10, max-current: 20 }
        row: 5          # row to place this component at
        column: 10      # column to place this component at
        rotation: 23    # 0-360 rotation

        name: B1
        type: Battery { voltage: 10 }
        row: 10
        column: 3
        rotation: 90
- Define wires. Syntax: name[+/-]:name[+/-], leave first empty to connect to ground like :B1-
    Example:
        wires: 
        [
            :B1-
            B1+:F1-,
            C1-:Rs2+
        ]
