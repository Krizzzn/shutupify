# shutupify
An application to control the playback of your favorite player. Shutupify for windows can control Spotify, VLC (experimental), some HTML elements using a Chrome extension and iTunes.
While running the application listens for events from various sources and controls the music player. 
The application has no GUI - which is intended. I wanted it as uncluttered as possible.

These three triggers control the playback.
* Global hotkeys - play, pause, next and previous track is controlled by CTRL + SHIFT + ALT + cursor-keys.
* pause on lock - the player pauses when you lock your workstation and starts when you come back
* pause on call - play back is pause as soon as you receive a call on lync. 

If multiple players are found at the same time, shutupify will route the commands to the currently playing player. If there are no music players playing the events are routed to the last known player.

## shutupify for Chrome
The chrome extension is built in coffeescript. It can run on any OS that runs Chrome. Besides of communicating to the shutupify windows application, the extension can control the browser playback using a button next to the address bar. The button toggles play and pause commands. 

## build
To build shutupify install ruby and get the `albacore`, `rake`, and `coffeescript` gems.  
Then call `>rake build`. This builds the application and the chrome extension.  
`>rake chrome:build` builds the Chrome extension.  
`>rake win:build_app` builds the Windows application.  

### references
Before building the application you need to place a few dlls into the `Lib` folder. Here's the shopping list:
* __ManagedWinapi.dll__ - used for the hotkey probe
* __Microsoft.Lync.Model.dll__ and __Microsoft.Office.Uc.dll__ - used for the Lync call probe
* __Alchemy.dll__ - used for the web socket player, that controls the  Chrome extension.
* __Interop.iTunesLib.dll__ - control iTunes  
  
* __nunit.framework.dll__ - used by the unit test
* __FluentAssertions.dll__ - used by the unit test
* __Moq.dll__ - used by the unit test

There's no need to supply all of these dlls. The built application will not include the parts that rely on missing dlls.

## configure and run
Configure the application with the `shutupify-settings` file placed in the apps directory. Edit it with you favorite text editor. Changes to the configuration are not reflected in a running instance of shutupify.
 
## License and stuff
Shutupify contains [some code](https://github.com/Krizzzn/shutupify/blob/master/Shutupify/Jukeboxes/Drivers/Spotify.cs) from the toastify project from [codeplex](http://toastify.codeplex.com).

### shutupify
Contributions are welcome!

Copyright (c) 2013 [Christian Peterek](http://twitter.com/krizzzn)

MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.





