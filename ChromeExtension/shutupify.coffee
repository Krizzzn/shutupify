
shutupify =
  pingpong_timer: null,
  player: null,
  socket: null,
  open_socket: ->
    unless (shutupify.socket?)
      this.connect()
  connect: ->
    this.socket = new WebSocket("ws://localhost:9971/shutupify") 

    this.socket.onopen = ->
      shutupify.pingpong_timer = window.setInterval ->
        shutupify.socket.send "PING"
      , 1000*60*4
      console.log "shutupify connected"

    this.socket.onclose = ->
      console.log "shutupify connection lost."
      window.clearInterval shutupify.pingpong_timer
      shutupify.socket = null
      null

    this.socket.onmessage = (evt) ->
      console.log "shutupify received: #{evt.data}"

      if /^shutupify:.{4,20}/.test evt.data 
        console.log "message is valid"
        message = evt.data.substring(10)

        if (message == "PLAYING?")
          console.log "THIS DOES NOT WORK :'-("
          #shutupify.player.control "PLAYING?", (answer) ->
          #  state =  (answer.foo) ? "PLAYING" : "PAUSED"
          #  console.log "will send #{state} to", shutupify.socket
          #  shutupify.socket.send state
        else
          shutupify.player.control message

      this.socket

chrome.browserAction.onClicked.addListener (tab) ->
  shutupify.player.control "TOGGLE!"
  #shutupify.open_socket()

chrome.runtime.onMessage.addListener (player_sent, sender) ->
  console.log "tab sent", player_sent, shutupify
  if !shutupify.player or shutupify.player != player_sent
    console.log "registered new player"
    shutupify.player = new Player sender.tab, player_sent.player_id
  shutupify.player.playing = (player_sent.playback == "started")
  shutupify.open_socket()


class Player
  constructor: (@tab, @player_id) ->
    player_id = @player_id
    console.log "new player!"

  playing: false

  player_id: null

  control: (msg, callback) ->
    console.log "sending"
    chrome.tabs.sendMessage @tab.id, "#{msg}", callback
    this






