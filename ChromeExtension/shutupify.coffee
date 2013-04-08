
shutupify =
  playing_tab: null,
  player_id: null

chrome.browserAction.onClicked.addListener (tab) ->
  return unless shutupify.playing_tab
  chrome.tabs.sendMessage shutupify.playing_tab.id, "PLAY:#{shutupify.player_id}"

chrome.runtime.onMessage.addListener (request, sender) ->
  shutupify.playing_tab = sender.tab
  shutupify.player_id = request.player_id
  console.log "registered to tab", request, shutupify

#chrome.runtime.onConnect.addListener (port) ->
#  console.assert port.name is "knockknock"
#  port.onMessage.addListener (msg) ->
#    if msg.joke is "Knock knock"
#      port.postMessage question: "Who's there?"
#    else if msg.answer is "Madame"
#      port.postMessage question: "Madame who?"
#    else port.postMessage question: "I don't get it."  if msg.answer is "Madame... Bovary"

