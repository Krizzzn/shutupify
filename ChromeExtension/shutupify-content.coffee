
shutupify = 


  initialize: -> 
  	audio_elements = document.getElementsByTagName "audio"
  	return if audio_elements.length == 0
  	shutupify.register_event audio_element for audio_element in audio_elements
  	shutupify.initialize_events()

  register_event: (audio) ->
  	audio.addEventListener "play", ->
  	  chrome.runtime.sendMessage {"playback": "started", "player_id": this.id}
  	audio.addEventListener "pause", ->
  	  chrome.runtime.sendMessage {"playback": "paused", "player_id": this.id}

  initialize_events: ->
  	chrome.runtime.onMessage.addListener (message, sender) ->
  	  splitted = message.split ":"
  	  return unless splitted[0] is "PLAY"
  	  player = document.getElementById splitted[1]
  	  console.log player, splitted[1], message
  	  player.play()


document.addEventListener "DOMContentLoaded", ->
  shutupify.initialize()

shutupify.initialize()


#port = chrome.runtime.connect(name: "knockknock")
#port.postMessage joke: "Knock knock"
#port.onMessage.addListener (msg) ->
#  if msg.question is "Who's there?"
#    port.postMessage answer: "Madame"
#  else port.postMessage answer: "Madame... Bovary"  if msg.question is "Madame who?"
