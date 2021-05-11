# Fuse Snap

## Intro
This is a two-player turn-based game. Where the goal is to overheat the opponent's Fuses by creating a circuit. Each turn a player selects a terminal and creates a wire of a max length X. The wire can either create a new [terminal](#notes) or join an already created terminal.

## Features
- Online Multiplayer
- Same phone multiplayer
- Electric Circuit Skins
- Many circuit maps

## Circuit Map
A circuit map is a level/map with some electric components placed by the designer. A map must have at least one ground, one battery and one fuse for each player.

## Rounds
Each round will start with selecting a random circuit map. Each turn players will join terminals to overheat the opponents fuse. Every time a fuse is overheated the player gets in-game currency equivalent to the total current through the [circuit](#notes).

## Technical
Circuits are solved using [MNA](#notes). Wires are not an electrical component. They are not 0-volt batteries or 0-ohm resistors. They are just ideal wires, they don't contribute to the actual solving of the circuit.

## Notes
1. A newly created terminal is considered to be a fake ground.
2. This sounds cool but will need to do some testing. Also, this demands skill-based matchmaking.
3. https://lpsa.swarthmore.edu/Systems/Electrical/mna/MNA1.html https://github.com/circuitsim/circuit-simulator https://www.falstad.com/circuit/
