using System.Threading.Tasks;
using BAModAPI;
using BAModAPI.Services;
using Dialogs;
using Entities;
using NYTaxi;
using UI.Smartphone.Apps.Contacts;
using UnityEngine;

[assembly: RegisterModClass(typeof(NYTaxiInit))]

namespace NYTaxi
{
    [ModEntryOnCityLoad]
    public class NYTaxiInit : IModBigAmbitions
    {
        private const string BundleKey = "AssetBundles/nytaxi.unity3d";
        private const string ContactId = "nytaxi-contactname";
        private const string ContactDescription = "nytaxi:description";
        private const string ServiceObjectName = "NYTaxiService";

        private ModContext _context;
        private GameObject _serviceObject;

        public string[] RelativeAssetBundlePaths => new[] { BundleKey };

        public Task OnLoadAsync(ModContext context)
        {
            _context = context;
            CreateService();
            RegisterContact();
            return Task.CompletedTask;
        }

        public Task OnUnloadAsync()
        {
            if (_serviceObject != null)
                Object.Destroy(_serviceObject);
            return Task.CompletedTask;
        }

        private void CreateService()
        {
            if (NYTaxiService.Instance != null)
                return;
            _serviceObject = new GameObject(ServiceObjectName);
            _serviceObject.AddComponent<NYTaxiService>();
        }

        private void RegisterContact()
        {
            RegisterContactIcon();
            Contact contact = Contact.GetContact(
                ContactId, ContactCategoryName.General, ContactDescription);
            CallDialogType dialogType =
                (CallDialogType)ModEnumHash.GetSafeHash("nytaxi_calldialogtype");
            contact.callDialogTypeOverride = dialogType;
            CallDialogFactory.RegisterDialog(dialogType, () => new NYTaxiDialog());
            _context.Logger.Info("NY Taxi contact registered");
            if (contact.messagesQueue == null || contact.messagesQueue.Count == 0)
            {
                contact.SendMessage(
                    new TextMessage("nytaxi:textmessage_welcome"),
                    sendNotificationInstantly: true);
            }
        }

        private void RegisterContactIcon()
        {
            Sprite[] oldIcons = GlobalReferences.Instance.contactIcons;
            // Повторная загрузка города не должна плодить дубликаты
            foreach (Sprite existing in oldIcons)
            {
                if (existing != null && existing.name == ContactId)
                    return;
            }
            AssetBundle bundle = AssetService.GetBundle(_context.ModId, BundleKey);
            Sprite contactSprite = bundle.LoadAsset<Sprite>(
                $"Assets/Mods/NYTaxi/{ContactId}.png");
            if (contactSprite == null)
            {
                _context.Logger.Warn("Contact icon not found in bundle");
                return;
            }
            Sprite[] newIcons = new Sprite[oldIcons.Length + 1];
            for (int i = 0; i < oldIcons.Length; i++)
                newIcons[i] = oldIcons[i];
            newIcons[oldIcons.Length] = contactSprite;
            GlobalReferences.Instance.contactIcons = newIcons;
        }
    }
}
