
shutupify =
  player: null
  socket: null,
  open_socket: ->
    unless (shutupify.socket?)
      this.connect()
  connect: ->
  	this.socket = new WebSocket("ws://localhost:9971/shutupify") 

  	this.socket.onopen = ->
  	  console.log "shutupify connected"

  	this.socket.onclose = ->
  	  console.log "shutupify connection lost."
  	  shutupify.socket = null
  	  null

    this.socket.onmessage = (evt) ->
      console.log "shutupify received: #{evt.data}"

      if /^shutupify:.{4,20}/.test evt.data 
        console.log "message is valid"
        message = evt.data.substring(10)
        shutupify.player.control message

      this.socket

chrome.browserAction.onClicked.addListener (tab) ->
  shutupify.open_socket()

chrome.runtime.onMessage.addListener (request, sender) ->
  if !shutupify.player? and shutupify?.player_id != request.player_id
    shutupify.player = new Player sender.tab, request.player_id
  shutupify.player.playing = (request.playback == "started")
  shutupify.open_socket()
  console.log "registered to tab", request, shutupify


class Player
  constructor: (@tab, @player_id) ->
    player_id = @player_id
    console.log "new player!"

  playing: false

  player_id: null

  control: (msg) ->
  	chrome.tabs.sendMessage @tab.id, "#{msg}:#{@player_id}"




