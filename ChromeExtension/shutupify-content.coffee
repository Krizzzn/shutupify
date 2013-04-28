
shutupify = 

  initialize: -> 
    @players = []
    this.initialize_all_players()
    this.initialize_events()
    this

  add_player: (player) ->
    @players.push player

  initialize_events: ->
    self = this
    chrome.runtime.onMessage.addListener (message, sender) ->
      console.log self.current_player, message
      switch message
        when "PLAY!" then self.current_player.play()
        when "PAUSE!" then self.current_player.pause()
        when "TOGGLE!" then self.current_player.toggle()
        when "PLAYING?" then self.send "PLAYING", self.current_player

  send: (msg, player) ->
    @current_player = player
    playing = (player.is_playing()) ? "started" : "paused"
    chrome.runtime.sendMessage {"playback": playing, "player_id": player.id }
    console.log msg, player

  initialize_all_players: ->
    Html5Player.find_players(this)
    YoutubePlayer.find_players(this)

shutupify.initialize()
