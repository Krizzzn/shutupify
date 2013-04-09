using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Shutupify.Settings;

namespace Shutupify.Probes
{
    public class LyncCallProbe : IEventProbe
    {
        LyncClient _lyncClient;
        private bool _isActive;

        public LyncCallProbe()
        {}

        public event Action<JukeboxCommand> ReactOnEvent;

        public bool Alive()
        {
            return (_lyncClient != null && _lyncClient.State == ClientState.SignedIn);
        }

        public bool StartObserving()
        {
            if (!Alive()) {
                _lyncClient = LyncClient.GetClient();

                if (!Alive())
                    return false;
            }

            if (!_isActive) {
                _lyncClient.ConversationManager.ConversationAdded += ConversationStarted;
                _lyncClient.ConversationManager.ConversationRemoved += ConversationEnded;
                _lyncClient.ClientDisconnected += ClientDisconnected;
                _isActive = true;
            }
            return true;
        }

        private void ClientDisconnected(object sender, EventArgs e)
        {
            _lyncClient = null;
            _isActive = false;
        }

        private void ConversationEnded(object sender, Microsoft.Lync.Model.Conversation.ConversationManagerEventArgs e)
        {
            if (!ModalityIsNotified(e.Conversation, ModalityTypes.AudioVideo))
                return;
            if (ReactOnEvent != null)
                ReactOnEvent(JukeboxCommand.PlayAfterPaused);
        }

        private void ConversationStarted(object sender, Microsoft.Lync.Model.Conversation.ConversationManagerEventArgs e)
        {
            if (!IsValidCall(e.Conversation))
                return;

            if (ReactOnEvent != null)
                ReactOnEvent(JukeboxCommand.Pause);
        }

        private bool IsValidCall(Conversation conversation)
        {
            if (conversation.State == ConversationState.Inactive)
                return false;

            if (!ModalityIsNotified(conversation, ModalityTypes.AudioVideo))
                return false;

            return true;
        }

        private bool ModalityIsNotified(Conversation conversation, ModalityTypes modalityType)
        {
            return conversation.Modalities.ContainsKey(modalityType) &&
                   conversation.Modalities[modalityType].State == ModalityState.Notified;
        }

        public string Name
        {
            get { return "LyncCall"; }
        }
    }
}
