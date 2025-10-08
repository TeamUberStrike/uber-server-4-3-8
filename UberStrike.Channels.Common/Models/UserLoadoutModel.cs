using System.Collections.Generic;
using UberStrike.DataCenter.Common.Entities;
using Cmune.DataCenter.Business;

namespace UberStrike.Channels.Common.Models
{
    public class UserLoadoutModel
    {
        public UserLoadoutModel()
        {

        }

        public ItemModel Boots { get; set; }
        public ItemModel Face { get; set; }
        public ItemModel FunctionalItem1 { get; set; }
        public ItemModel FunctionalItem2 { get; set; }
        public ItemModel FunctionalItem3 { get; set; }
        public ItemModel Gloves { get; set; }
        public ItemModel Head { get; set; }
        public ItemModel LowerBody { get; set; }
        public ItemModel MeleeWeapon { get; set; }
        public ItemModel QuickItem1 { get; set; }
        public ItemModel QuickItem2 { get; set; }
        public ItemModel QuickItem3 { get; set; }
        public string SkinColor { get; set; }
        public ItemModel UpperBody { get; set; }
        public ItemModel Weapon1 { get; set; }
        public ItemModel Weapon2 { get; set; }
        public ItemModel Weapon3 { get; set; }
    }

    public static class UserLoadoutModelConvert
    {
        public static UserLoadoutModel ToUserLoadoutModel(this LoadoutView loadoutView)
        {
            UserLoadoutModel userLoadoutModel = new UserLoadoutModel();
            List<int> itemsId = new List<int> { loadoutView.Head, loadoutView.Face, loadoutView.UpperBody, loadoutView.Gloves, loadoutView.LowerBody, loadoutView.Boots, loadoutView.Weapon1, loadoutView.Weapon2, loadoutView.Weapon3 };

            Dictionary<int, string> itemsName = CmuneItem.GetItemNames(itemsId);
            itemsName.Add(0, "-");

            userLoadoutModel.Head = new ItemModel() { Id = loadoutView.Head, Name = itemsName[loadoutView.Head] };
            userLoadoutModel.Face = new ItemModel() { Id = loadoutView.Face, Name = itemsName[loadoutView.Face] };
            userLoadoutModel.UpperBody = new ItemModel() { Id = loadoutView.UpperBody, Name = itemsName[loadoutView.UpperBody] };
            userLoadoutModel.Gloves = new ItemModel() { Id = loadoutView.Gloves, Name = itemsName[loadoutView.Gloves] };
            userLoadoutModel.LowerBody = new ItemModel() { Id = loadoutView.LowerBody, Name = itemsName[loadoutView.LowerBody] };
            userLoadoutModel.Boots = new ItemModel() { Id = loadoutView.Boots, Name = itemsName[loadoutView.Boots] };
            userLoadoutModel.Weapon1 = new ItemModel() { Id = loadoutView.Weapon1, Name = itemsName[loadoutView.Weapon1] };
            userLoadoutModel.Weapon2 = new ItemModel() { Id = loadoutView.Weapon2, Name = itemsName[loadoutView.Weapon2] };
            userLoadoutModel.Weapon3 = new ItemModel() { Id = loadoutView.Weapon3, Name = itemsName[loadoutView.Weapon3] };

            return userLoadoutModel;
        }
    }
}