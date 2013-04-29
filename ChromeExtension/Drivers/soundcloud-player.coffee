
class SoundcloudPlayer extends Player

  is_playing: () ->
    @htmlelement.className.indexOf("sc-button-pause") >= 0 or @htmlelement.className.indexOf("playButton__playing") >= 0

  register_events: () ->
    self = this

    @htmlelement.addEventListener "click", ->
      self.shutupify.send self
    this

  play: ->
    this.toggle() unless this.is_playing()
    this

  pause: ->
    this.toggle() if this.is_playing()
    this

  toggle: ->
    event = window.document.createEvent "Event"
    event.initEvent "click", true, true
    @htmlelement.dispatchEvent event
    this

  @find_soundcloud_players = (shutupify) ->
    elements = document.querySelectorAll(".sc-button-play, .playButton")
    for element in elements
      if element.className.indexOf("shutupify") < 0
        element.className += " shutupify"
        new SoundcloudPlayer shutupify, element

  @find_players = (shutupify) ->
    return unless location.host.toLowerCase() is "soundcloud.com" or document.getElementById("widget")?
    
    console.log "looking for SoundcloudPlayer"

    window.setTimeout ->
      SoundcloudPlayer.find_soundcloud_players shutupify
    , 4 * 1000
    window.setInterval ->
      SoundcloudPlayer.find_soundcloud_players shutupify
    , 8 * 1000 

    shutupify
  