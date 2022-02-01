# SONUS

# How to build
In unity, go to file -> build settings -> make sure that the scenes are in the right order and the build settings are set to webgl and development build is checked, then you can build or build and run. If you build and run, it will build and run the project in a locally hosted server on your default web browser. If you just build, it will put the build in the specified location. **The location of the build should have no spaces in the path otherwise it will fail for some reason (eg .../unity builds/... will fail but .../unityBuilds/... will not).** If you choose to build but not run, you will have to start a local server to host the webpage, simply opening it from index won't work. To do that run the following command in bash, in the directory of the build ``` python3 -m http.server --cgi 8360 ``` then open a browser and go to localhost:8360 (you need python 3 installed for this to work).
If you have set up the build settings once, you can just use **ctrl+b** to build and run. 

# How to run the game so far

The game should start on the first scene (Loading Scene) and automatically transition to a lobby scene. To create a game input a keyword into the text field and press create, this should lead to the basic map with a controllable player. To join a game that is already created you need a second instance of the game, in the lobby scene input the word used to create the game and press join.

Note that you cannot open two instances of the game from the "play button" from unity, so you have to build the project and open it outside of unity if you want to test the multiplayer part.

# Commands

Use WASD or arrow keys to move around, SPACE to jump, Left-Click to shoot.
