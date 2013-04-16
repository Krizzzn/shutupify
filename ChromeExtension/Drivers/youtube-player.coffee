#this might only work on the official youtube site

class YoutubePlayer extends Player

  is_playing: () ->
    @htmlelement.getPlayerState() == 1

  register_events: () ->
    self = this
    last_known_state = null
    youtube_watch = window.setInterval () ->
      if self.is_playing() != last_known_state
        self.shutupify.send "play", self if self.is_playing()
        self.shutupify.send "pause", self unless self.is_playing()
      last_known_state = self.is_playing()
    , 5000
    this

  play: ->
    @htmlelement.playVideo()
    this

  pause: ->
    @htmlelement.pauseVideo()
    this

  @find_players = (shutupify) ->
    element = document.getElementById("movie_player")
    player = new YoutubePlayer(shutupify, element) if element?
    if player?
      [player]
    else
      []
  