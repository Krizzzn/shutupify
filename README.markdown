# shutupify

An application to control the playback of your favorite player (currently only Spotify). While running the application listens for events and controls the music player. 
The application has no GUI - which is intended. I wanted it as uncluttered as possible.

These three triggers control the playback.
* Global hotkeys - play, pause, next and previous track is controlled by CTRL + SHIFT + ALT + cursorkeys.
* pause on lock - the player pauses when you lock your workstation and starts when you come back
* pause on call - play back is pause as soon as you receive a call on lync. 

---------

The project is a quick hack to make my own life more easy.  
There are the following issues (and more):
* In it's current state it will not compile without the lync SDK installed.  
* There are no configuration possibilities (yet?).  
* It doesn't even contain the reference dlls (yet?).
* playback doesn't start automatically after a lync call ended (some threading problem)
* works only for incoming calls

-------

Following references are needed but not included:  
* ManagedWinapi.dll
* Microsoft.Lync.Model.dll (desktop version)

--------

Shutupify contains code from the toastify project (Spotify.cs) from [codeplex](http://toastify.codeplex.com). (Please don't hurt me).



