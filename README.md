# Summer2018CRCSProject
## Summary
The culmination of the VR/AR/PC/RC teamwork done by Saam, Ben, Justin, Albert, Stan, and Kelvin. For this project our goal was cross reality collaboration within a syncronized enviroment. We made a demo that shows a VR player using the Vive, PC person using Windows/Mac, and an RC car that is used by a PC person all interacting with one another.  
## How To Run the Demo
### Connecting as a PC Master
1. Run `git clone https://github.com/UWB-ARSandbox/Summer2018CRCSProject.git` to clone the repository
2. Open the `Summer2018CRCSProject/Assets/ASL/Scenes/CrossRealityDemoPC` Master scene for a PC player
3. Ensure the `Master` checkbox is selected in the `NetworkManager` prefab 
4. Press play and you should appear in the starting room
5. Move with W-A-S-D and the Mouse (enable gravity `G` enable clipping `C`)
6. Ensure all Clients are connected
7. Activate portals by selecting the empty check box at the top of the `Portals` prefab
### Connecting as a RC Master
1.
2.
3.
4.
### Connecting as a PC Client
1. Run `git clone https://github.com/UWB-ARSandbox/Summer2018CRCSProject.git` to clone the repository
2. Open the `Summer2018CRCSProject/Assets/ASL/Scenes/CrossRealityDemoClient` Client scene for a PC player
3. Ensure the Master is connected
4. Ensure the `Master` checkbox is deselected in the `NetworkManager` prefab
5. Ensure the Port and Room name in the `NetworkManager` prefab are the same as the Master's
6. Press play and you should appear in the starting room
7. Move with W-A-S-D and the Mouse (enable gravity `G` enable clipping `C`)
### Connecting as a VR Client
1.
2.
3.
4.
## Known Issues
- Portals teleport PC players to random locations occasionally
- Portals must be deactivated prior to launch by master and reactivated when all players connect to the room
- VR player must re-sync body parts at run-time after joining ASL (in order for others to view VR player)
- RC car's localization is limited to battery life, rear tire grab and non constant acceleration
## Future Enhancements
- Allowing VR player to control RC car
- Scanning more campus locations
- Adding more interaction for players
- Adding video streams to in game TV's
- Expanding on QR scanning capabilities
- Changing RC cars rear wheels
- Changing RC cars rechargable battery pack
- Adding AR to the Demo (Tango or Hololens)
- Adding path lines to lead players to correct location
## Future Applications
- Search and Rescue mission
- Real Estate, touring homes virtually
- Safety training
- Tour of a site (residential, commercial, university, shopping complex)

<details>
<Summary> Test </Summary>
This is a test.
</details>
