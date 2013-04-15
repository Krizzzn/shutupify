
class Html5Player 
  constructor: (@shutupify, @htmlelement) ->
    this.register_events()
    self = this;
    @id = @htmlelement.id || "player_" + Math.floor(Math.random()*100000)
    window.setTimeout () ->
      self.shutupify.send "play", self if self.is_playing()
    , 250
    console.log @id

  is_playing: () ->
  	!@htmlelement.paused

  register_events: () ->
  	self = this
  	@htmlelement.addEventListener "play", ->
  	  self.shutupify.send "play", self
    @htmlelement.addEventListener "pause", ->
  	  self.shutupify.send "pause", self

  play: ->
  	@htmlelement.play()
  	this

  pause: ->
  	@htmlelement.pause()
  	this

  toggle: ->
  	if (this.is_playing())
      this.pause()
    else
      this.play()
    this

  next: ->

  prev: ->


  @find_players = (shutupify) ->
    html_elements = document.querySelectorAll("audio, video");

    for element in html_elements
      do (element) ->
        new Html5Player shutupify, element
  