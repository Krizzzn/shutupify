
shutupify = 

  initialize: -> 
    @players = []
    this.initialize_all_players()
    console.log @players
    
    shutupify.initialize_events()
    this

#  register_event: (audio) ->
#    audio.addEventListener "play", ->
#      chrome.runtime.sendMessage {"playback": "started", "player_id": this.id}
#    audio.addEventListener "pause", ->
#      chrome.runtime.sendMessage {"playback": "paused", "player_id": this.id}

  initialize_events: ->
    self = this
    chrome.runtime.onMessage.addListener (message, sender, sendResponse) ->
      console.log self.current_player, message
      switch message
        when "PLAY!" then self.current_player.play()
        when "PAUSE!" then self.current_player.pause()
        when "TOGGLE!" then self.current_player.toggle()
        when "PLAYING?"
          console.log self.current_player.is_playing(), sendResponse
          sendResponse(foo: self.current_player.is_playing())

  send: (msg, player) ->
    @current_player = player
    playing = (player.is_playing()) ? "started" : "paused"
    chrome.runtime.sendMessage {"playback": playing }
    console.log msg, player

  initialize_all_players: ->
    @players = @players.concat Html5Player.find_players(this)

shutupify.initialize()


#port = chrome.runtime.connect(name: "knockknock")
#port.postMessage joke: "Knock knock"
#port.onMessage.addListener (msg) ->
#  if msg.question is "Who's there?"
#    port.postMessage answer: "Madame"
#  else port.postMessage answer: "Madame... Bovary"  if msg.question is "Madame who?"
