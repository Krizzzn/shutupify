
shutupify = 
  initialize: -> 
    console.log "initialize"
    audio_elements = document.getElementsByTagName "audio"
    return if audio_elements.length = 0
    console.log "found targets"
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
      current_player = document.getElementById splitted[1]
      switch splitted[0]
        when "PLAY!" then current_player.play()
        when "PAUSE!" then current_player.pause()
        when "PLAYING?" then console.log current_player.paused
      console.log player, splitted[1], message

document.addEventListener "DOMContentLoaded", ->
  shutupify.initialize()

shutupify.initialize()


#port = chrome.runtime.connect(name: "knockknock")
#port.postMessage joke: "Knock knock"
#port.onMessage.addListener (msg) ->
#  if msg.question is "Who's there?"
#    port.postMessage answer: "Madame"
#  else port.postMessage answer: "Madame... Bovary"  if msg.question is "Madame who?"
