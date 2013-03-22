# shutupify

An application to control the playback of your favorite player (currently only Spotify). 

Three triggers control the Spotify playback.
* Global hotkeys - play, pause, next and previous track is controlled by CTRL + SHIFT + ALT + cursorkeys
* pause on lock - the player pauses when you lock your screen and starts when you come back
* pause on call - play back is pause as soon as you receive a call on lync. 

---------

The project is a quick hack to make my own life more easy.
There are following issues:
* In it's current state it will not compile without the lync SDK installed.  
* There are no configuration possibilities (yet?).  
* It doesn't even contain the reference dlls (yet?).
* playback doesn't start automatically after a lync call ended (some threading problem)
* works only for incoming calls

--------

Shutupify contains code from the toastify project (Spotify.cs) from codeplex. (Please don't hurt me).



