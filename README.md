# Summer2018CRCSProject
## Summary
The culmination of the VR/AR/PC/RC teamwork done by Saam, Ben, Justin, Albert, Stan, and Kelvin. For this project our goal was cross reality collaboration within a synchronized enviroment. We made a demo that shows a VR player using the Vive, PC person using Windows/Mac, and an RC car that is used by a PC person all interacting with one another.  
## How To Run the Demo
### Connecting as a PC Master without RC Car Use
1. Run `git clone https://github.com/UWB-ARSandbox/Summer2018CRCSProject.git` to clone the repository
2. Open the `Summer2018CRCSProject/Assets/ASL/Scenes/CrossRealityDemoPC.unity` Master scene for a PC player
3. Ensure the `Master Client` checkbox is selected in the `NetworkManager` GameObject in the hierarchy 
4. Press play and you should appear in the starting room
5. Move with W-A-S-D and the Mouse (enable gravity `G` enable clipping `C`)
6. Ensure all Clients are connected
7. Activate portals by activating the 'Portals' GameObject in the hierarchy (click 'Portals' in hierarchy then check the empty checkbox in the Inspector)
### Connecting as a PC Master with RC Car Use
1. Run `git clone https://github.com/UWB-ARSandbox/Summer2018CRCSProject.git` to clone the repository
2. Use the ASL RC Car Tutorial at https://drive.google.com/open?id=1R2MzQxzncyRmByJ1abZf69f4Thv8fwrT to power the car, connect to the car, start the cameras, calibrate the sensor, and start listening for TCP connections 
3. Open the `Summer2018CRCSProject/Assets/ASL/Scenes/CrossRealityDemo.unity` Master scene for a PC player with RC car use
4. Ensure the `Master Client` checkbox is selected in the `NetworkManager` GameObject in the heirarchy
5. Press play and you should appear in the starting room
6. Move with W-A-S-D and look with the Mouse (enable gravity `G` enable clipping `C`)
7. Ensure all Clients are connected
8. Activate portals by activating the 'Portals' GameObject in the hierarchy (click 'Portals' in hierarchy then check the empty checkbox in the Inspector)
9. Navigate to the hallway (6, 2, 10)
10. Click on the blue car to take control of the car and recieve first person view with the car's cameras
11. Use the up, down, left, and right arrow keys to drive the car
12. Press the 'E' key to close the TCP connection to the car
### Connecting as a PC Client
1. Run `git clone https://github.com/UWB-ARSandbox/Summer2018CRCSProject.git` to clone the repository
2. Open the `Summer2018CRCSProject/Assets/ASL/Scenes/CrossRealityDemoClient` Client scene for a PC player
3. Ensure the Master is connected
4. Ensure the `Master Client` checkbox is deselected in the `NetworkManager` prefab
5. Ensure the Port and Room name in the `NetworkManager` prefab are the same as the Master's
6. Press play and you should appear in the starting room
7. Move with W-A-S-D and the Mouse (enable gravity `G` enable clipping `C`)
### Connecting as a VR Client
1. Run `git clone https://github.com/UWB-ARSandbox/Summer2018CRCSProject.git` to clone the repository
2. Open the `Summer2018CRCSProject/Assets/ASL/Scenes/CrossRealityDemoVRClient` Client scene for a VR player
3. Ensure the Master is connected
4. Ensure the `Master Client` checkbox is deselected in the `NetworkManager` prefab
5. Ensure the Port and Room name in the `NetworkManager` prefab are the same as the Master's
6. Navigate to the GameObject `VRTK_SDKManager` in the hierarchy, ensure the field for device to use reflect your VR device
7. Press play and you should appear in the starting room
8. Move by holding the trigger button to project a beam that indicates the destination to teleport to.
## Known Issues
- Portals teleport PC players to random locations occasionally
- Orientation when exiting a portal is not consistent between player types and portal angles
- Portals must be deactivated prior to launch by master and reactivated when all players connect to the room
- RC car's localization is limited to battery life, rear tire grab and non constant acceleration
- Player identification tags (billboardtext) not working consistently betweeen different player types.
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
- Adding more control options for VR players
## Future Applications
- Search and Rescue mission
- Real Estate, touring homes virtually
- Safety training
- Tour of a site (residential, commercial, university, shopping complex)
