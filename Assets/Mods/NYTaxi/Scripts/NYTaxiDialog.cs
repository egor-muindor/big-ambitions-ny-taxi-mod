using System.Collections.Generic;
using Dialogs;
using Entities;
using Localizor;
using UI;
using UnityEngine;

namespace NYTaxi
{
    public class NYTaxiDialog : Dialog
    {
        public NYTaxiDialog()
        {
            npcNameKey = "nytaxi:dispatcher_name";
            DialogController.current.ShowEntry(
                NYTaxiService.IsPlayerOutside() ? Offer() : Refuse());
        }

        private static DialogEntry Refuse()
        {
            DialogController.current.contact.SendMessage(new TextMessage(
                "nytaxi:dialog_refuse_indoors", null, read: true,
                isNewInteraction: true));
            return new DialogEntry
            {
                messageData = "nytaxi:dialog_refuse_indoors".Localize(),
                Template = DialogEntry.TemplateType.Text,
                OnVisible = DialogController.current.FinishDialog
            };
        }

        private DialogEntry Offer()
        {
            DialogController.current.contact.SendMessage(new TextMessage(
                "nytaxi:dialog_offer", null, read: true, isNewInteraction: true));
            return new DialogEntry
            {
                messageData = "nytaxi:dialog_offer".Localize(),
                Template = DialogEntry.TemplateType.Text,
                ConfirmTextOverride = "nytaxi:dialog_confirm_button".Localize(),
                OnConfirm = OnOrderConfirmed,
                OnCancel = DialogController.current.CancelDialog,
                onCancelMessage = new TextMessage(
                    "ba:messagetype_contacts_message_player_cancel_call")
            };
        }

        private DialogEntry OnOrderConfirmed()
        {
            int waitMinutes = Random.Range(5, 11);
            DialogController.current.contact.ReceivePlayerMessage(new TextMessage(
                "nytaxi:message_player_order", null, read: true));
            DialogController.current.contact.SendMessage(new TextMessage(
                "nytaxi:message_dispatcher_confirm",
                new Dictionary<string, string>
                {
                    { "minutes", waitMinutes.ToString() }
                },
                read: true));
            DialogController.current.FinishDialog();
            InstanceBehavior<UIs>.Instance.fullMenu.Toggle(show: false);
            NYTaxiService service = NYTaxiService.EnsureInstance();
            service.BeginPickup(waitMinutes);
            return null;
        }
    }
}
