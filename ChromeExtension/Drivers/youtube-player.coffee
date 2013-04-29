#this might only work on thesd youtube site

class YoutubePlayer extends Player

  is_playing: () ->
    @htmlelement.getPlayerState() == 1

  register_events: () ->
    self = this
    last_known_state = null
    youtube_watch = window.setInterval () ->
      self.shutupify.send self if self.is_playing() isnt last_known_state
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
    element = document.getElementById("movie_player") || document.getElementById("video-player")
    player = new YoutubePlayer(shutupify, element) if element?
    shutupify
  