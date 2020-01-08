# Maze

Implementation of several maze solving algorithms with visual updates.

<b>Maze Constraints</b>
- Top contains only one entrance indicated by white
- Bottom containts only one exit indicated by white

See Maze/mazes for example files

## MazeCSharp [![Build status](https://ci.appveyor.com/api/projects/status/32r7s2skrgm9ubva/branch/master?svg=true)](https://ci.appveyor.com/project/sLill/maze)
- Provide a live visualization of each algorithm's progression through the maze
- Exports a maze image file with a highlighted solution
- Uses memory mapped files to prevent thrashing on large mazes. Saves a signifigant amount of time
- Multi-threading and asynchronous processing done where possible but could always be improved
