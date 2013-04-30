
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
        when "PLAYING?" then self.send self.current_player

  send: (player) ->
    @current_player = player
    playing = if player.is_playing()
      "started"
    else
      "paused"
    chrome.runtime.sendMessage {"playback": (playing=="started"), "player_id": player.id }
    console.log "playing=#{playing}", player

  initialize_all_players: ->
    Html5Player.find_players(this)
    YoutubePlayer.find_players(this)
    SoundcloudPlayer.find_players(this)

shutupify.initialize()
