
class Html5Player extends Player

  is_playing: () ->
  	!@htmlelement.paused

  register_events: () ->
  	self = this
  	@htmlelement.addEventListener "play", ->
  	  self.shutupify.send self
    @htmlelement.addEventListener "pause", ->
  	  self.shutupify.send self

  play: ->
  	@htmlelement.play()
  	this

  pause: ->
  	@htmlelement.pause()
  	this

  @find_players = (shutupify) ->
    html_elements = document.querySelectorAll("audio, video");

    for element in html_elements
      new Html5Player shutupify, element

    shutupify
  