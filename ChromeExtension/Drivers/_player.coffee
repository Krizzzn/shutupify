# the leading underscore in the filename needs to stay.
# the RAKEFILE will compile this baseclass first.

class Player
 constructor: (@shutupify, @htmlelement) ->
    this.register_events()
    self = this;
    @id = @htmlelement.id || "player_" + Math.floor(Math.random()*100000)
    window.setTimeout () ->
      self.shutupify.send self if self.is_playing()
    , 250
    self.shutupify.add_player this
    console.log @id

  is_playing: () ->
    false

  register_events: () ->
    self = this

  play: ->
    console.log "play() is not implemented!"
    this

  pause: ->
    console.log "pause() is not implemented!"
    this

  next: ->
    console.log "next() is not implemented!"
    this

  prev: ->
    console.log "prev() is not implemented!"
    this

  toggle: ->
    if (this.is_playing())
      this.pause()
    else
      this.play()
    this