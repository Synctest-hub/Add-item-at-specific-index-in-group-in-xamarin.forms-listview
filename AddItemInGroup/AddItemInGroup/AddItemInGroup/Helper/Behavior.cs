using Syncfusion.DataSource;
using Syncfusion.DataSource.Extensions;
using Syncfusion.ListView.XForms;
using Syncfusion.ListView.XForms.Control.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace Grouping
{
   public class Behavior : Behavior<ContentPage>
    {

        #region Fields
        SfListView ListView; Button AddButton;
        ContactsViewModel VM;
        GroupResult itemGroup;
        VisualContainer visualContainer;

        #endregion

        #region overrides
        protected override void OnAttachedTo(ContentPage bindable)
        {
            ListView = bindable.FindByName<SfListView>("listView");
            VM = new ContactsViewModel();
            ListView.BindingContext =VM;
            AddButton = bindable.FindByName<Button>("addItem");
            AddButton.Clicked += AddItem_Clicked;
            visualContainer = ListView.GetVisualContainer();

            base.OnAttachedTo(bindable);
        }
        protected override void OnDetachingFrom(ContentPage bindable)
        {

            base.OnDetachingFrom(bindable);
        }
        #endregion
        #region CallBacks

        private void AddItem_Clicked(object sender, EventArgs e)
        {
            var contact = new Contacts();
            contact.ContactName = "Adam";
            contact.ContactNumber = "783-457-567";
            contact.DisplayString = "A";
            contact.ContactImage = ImageSource.FromResource("AddItemInGroup.Images.Image" + 25 + ".png");
            VM.ContactItems.Add(contact);

            GetGroupResult(contact);

            if (itemGroup == null)
                return;

            var items = itemGroup.GetType().GetRuntimeProperties().FirstOrDefault(x => x.Name == "ItemList").GetValue(itemGroup) as List<object>;
            InsertItemInGroup(items, contact, 0);
        }

        #endregion

        #region Methods

        internal void GetGroupResult(object ItemData)
        {
            var descriptor = ListView.DataSource.GroupDescriptors[0];
            object key;

            if (descriptor.KeySelector == null)
            {
                var propertyInfoCollection = new PropertyInfoCollection(ItemData.GetType());
                key = propertyInfoCollection.GetValue(ItemData, descriptor.PropertyName);
            }
            else
                key = descriptor.KeySelector(ItemData);

            for (int i = 0; i < this.ListView.DataSource.Groups.Count; i++)
            {
                var group = this.ListView.DataSource.Groups[i];
                if ((group.Key != null && group.Key.Equals(key)) || group.Key == key)
                {
                    itemGroup = group;
                    break;
                }
                group = null;
            }
            descriptor = null;
            key = null;
        }

        internal void InsertItemInGroup(List<object> items, object Item, int InsertAt)
        {
            items.Remove(Item);
            items.Insert(InsertAt, Item);
            visualContainer.ForceLayout();
        }

        #endregion
    }
}
