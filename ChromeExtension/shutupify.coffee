
shutupify =
  pingpong_timer: null,
  player: null,
  socket: null,
  set_icon: (icon_name) ->
    if this.socket? and this.socket.readyState == 1
      icon_name += "-connected"
    chrome.browserAction.setIcon "path": "38": "icon-#{icon_name}.png"  

  no_player: ->
    this.player = null
    this.set_icon "stop"

  open_socket: ->
    unless (shutupify.socket?)
      this.connect()
  connect: ->
    this.socket = new WebSocket("ws://localhost:9971/shutupify") 

    this.socket.onopen = ->
      shutupify.pingpong_timer = window.setInterval ->
        shutupify.socket.send "PING" if shutupify.socket? and shutupify.socket.readyState == 1
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
        shutupify.player.control message

      this.socket


class Player
  constructor: (@tab, @player_id) ->
    this.url = @tab.url
    console.log "new player!", this

  playing: false

  url: null

  control: (msg, callback) ->
    console.log "sending"
    chrome.tabs.sendMessage @tab.id, "#{msg}", callback
    this


######### BROWSER EVENTS###########

chrome.browserAction.onClicked.addListener (tab) -> 
  shutupify.player.control "TOGGLE!" if shutupify.player?
  shutupify

chrome.runtime.onMessage.addListener (player_sent, sender) ->
  console.log "tab sent", player_sent, shutupify
  if !shutupify.player or shutupify.player.player_id != player_sent.player_id
    console.log "registered new player"
    shutupify.player = new Player sender.tab, player_sent.player_id
  shutupify.player.playing = (player_sent.playback == "started")
  shutupify.open_socket()
  icon = "stop"

  state = if (player_sent.playback) 
    icon = "pause"
    "PLAYING" 
  else 
    icon = "play"
    "PAUSED"
 
  console.log "sending #{state} to socket."
  shutupify.socket.send state if shutupify.socket? and shutupify.socket.readyState == 1

  shutupify.set_icon icon
  shutupify

chrome.tabs.onRemoved.addListener (tabid) ->
  if shutupify? and shutupify.player? and tabid == shutupify.player.tab.id
    shutupify.no_player()

chrome.tabs.onUpdated.addListener (tabid, changeInfo) ->
  if shutupify? and changeInfo.url? and shutupify.player? and tabid == shutupify.player.tab.id
    console.log changeInfo.url, "!=", shutupify.player.url

    if changeInfo.url.replace(/\#.*/,"") != shutupify.player.url.replace(/\#.*/,"")
      shutupify.no_player() 








