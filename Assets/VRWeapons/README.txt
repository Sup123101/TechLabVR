Thank you for buying my VRWeapons asset pack!

Before starting:
Ensure that you have SteamVR (or if using VRTK, make sure you have VRTK) in your project.

To get started:

(Optional)	1: If using VRTK, go to the "Interaction System Packs" folder and unpack the VRW_VRTK_InteractionSystem.unitypackage.
			2: Go to the Window/VRWeapons/ menu and open up the appropriate weapon builder window
(SteamVR)	3: Press the "Set Up Controller Scripts" button to add the appropriate controller scripts. These scripts fire events that enable controller actions.
			4: Follow the weapon builder menu step by step to build your weapon!
			
			Tricky parts:
			---
			On the base Weapon script, there are two object fields for colliders (Weapon Body Collider and Second Hand Grip Collider). These must be assigned to make sure the weapon functions 
				correctly (For one-handed weapons, only the Weapon Body Collider needs to be assigned). The builder will make a best-guess at what goes where, but make sure you double check to make sure you've 
				placed colliders in the correct position. The Weapon Body Collider is the main collider for the weapon - for my examples I used a convex mesh collider for this. The Second Hand Grip Collider is 
				the collider that will be used for two-handed gripping of the weapon, I used a box collider on the top parent of each weapon for this.
			---
			The base weapon object also has an Object Pool component attached to it. This pool is used for spent bullet shells, and will use the bolt's "Chambered Round Snap T" transform to position them, when 
				firing.
			---
			When setting up magazine and bullet drop zones (to feed new mags or rounds), don't forget to set up tags! Drop zones may not function correctly if tags aren't set properly.

If you are having any issues, there's a tutorial video up here: https://youtu.be/SnPY38CZ9UU Or you can reach me at slayd7@gmail.com or on Twitter @Slayd7. Also, we have a Discord now: https://discord.gg/SPG8fch 
	Come stop by, ask questions, chat! I'm usually online, and it's the easiest way to get real-time help.

If you have the time, leaving me an honest review would really help me out. The only thing I ask is that if you're having an issue with VRW, please contact me before leaving a negative review, and give me a 
	chance to make it right!

Thanks, and enjoy!
-Brad

----------------------------------------------
	Random things of note
----------------------------------------------

Weapons chamber accurately, depending on how quickly your bolt makes a full cycle. So, for example: If you have an automatic Weapon with a 0.1 fire rate (10 rounds per second) but your bolt's slide speed is slower
	than that (5 slide speed = 5 frames back, 5 frames forward, 10 frames total, running at 90fps would make that 10/90 seconds to chamber a new round, or 0.1111 seconds) the Weapon may fail to fire - because it
	isn't cycling the bolt fast enough! So either adjust your slide speed or your fire rate. Same goes for Autofire Proj and Burst fire.