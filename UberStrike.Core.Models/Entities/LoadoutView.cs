using System;
using System.Text;
using UberStrike.Core.Types;

namespace UberStrike.DataCenter.Common.Entities
{
    [Serializable]
    public class LoadoutView
    {
        #region Properties

        public int LoadoutId { get; set; }

        public int Backpack { get; set; }

        public int Boots { get; set; }

        public int Cmid { get; set; }

        public int Face { get; set; }

        public int FunctionalItem1 { get; set; }

        public int FunctionalItem2 { get; set; }

        public int FunctionalItem3 { get; set; }

        public int Gloves { get; set; }

        public int Head { get; set; }

        public int LowerBody { get; set; }

        public int MeleeWeapon { get; set; }

        public int QuickItem1 { get; set; }

        public int QuickItem2 { get; set; }

        public int QuickItem3 { get; set; }

        public AvatarType Type { get; set; }

        public int UpperBody { get; set; }

        public int Weapon1 { get; set; }

        public int Weapon1Mod1 { get; set; }

        public int Weapon1Mod2 { get; set; }

        public int Weapon1Mod3 { get; set; }

        public int Weapon2 { get; set; }

        public int Weapon2Mod1 { get; set; }

        public int Weapon2Mod2 { get; set; }

        public int Weapon2Mod3 { get; set; }

        public int Weapon3 { get; set; }

        public int Weapon3Mod1 { get; set; }

        public int Weapon3Mod2 { get; set; }

        public int Weapon3Mod3 { get; set; }

        public int Webbing { get; set; }

        public string SkinColor { get; set; }

        #endregion Properties

        #region Constructors

        public LoadoutView()
        {
            this.Type = AvatarType.LutzRavinoff;
            this.SkinColor = String.Empty;
        }

        public LoadoutView(int loadoutId, int backpack, int boots, int cmid, int face, int functionalItem1, int functionalItem2, int functionalItem3, int gloves, int head, int lowerBody, int meleeWeapon, int quickItem1, int quickItem2, int quickItem3, AvatarType type, int upperBody, int weapon1, int weapon1Mod1, int weapon1Mod2, int weapon1Mod3, int weapon2, int weapon2Mod1, int weapon2Mod2, int weapon2Mod3, int weapon3, int weapon3Mod1, int weapon3Mod2, int weapon3Mod3, int webbing, string skinColor)
        {
            this.Backpack = backpack;
            this.Boots = boots;
            this.Cmid = cmid;
            this.Face = face;
            this.FunctionalItem1 = functionalItem1;
            this.FunctionalItem2 = functionalItem2;
            this.FunctionalItem3 = functionalItem3;
            this.Gloves = gloves;
            this.Head = head;
            this.LoadoutId = loadoutId;
            this.LowerBody = lowerBody;
            this.MeleeWeapon = meleeWeapon;
            this.QuickItem1 = quickItem1;
            this.QuickItem2 = quickItem2;
            this.QuickItem3 = quickItem3;
            this.Type = type;
            this.UpperBody = upperBody;
            this.Weapon1 = weapon1;
            this.Weapon1Mod1 = weapon1Mod1;
            this.Weapon1Mod2 = weapon1Mod2;
            this.Weapon1Mod3 = weapon1Mod3;
            this.Weapon2 = weapon2;
            this.Weapon2Mod1 = weapon2Mod1;
            this.Weapon2Mod2 = weapon2Mod2;
            this.Weapon2Mod3 = weapon2Mod3;
            this.Weapon3 = weapon3;
            this.Weapon3Mod1 = weapon3Mod1;
            this.Weapon3Mod2 = weapon3Mod2;
            this.Weapon3Mod3 = weapon3Mod3;
            this.Webbing = webbing;
            this.SkinColor = skinColor;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder loadoutDisplay = new StringBuilder();
            loadoutDisplay.Append("[LoadoutView: [Backpack: ");
            loadoutDisplay.Append(this.Backpack);
            loadoutDisplay.Append("][Boots: ");
            loadoutDisplay.Append(this.Boots);
            loadoutDisplay.Append("][Cmid: ");
            loadoutDisplay.Append(this.Cmid);
            loadoutDisplay.Append("][Face: ");
            loadoutDisplay.Append(this.Face);
            loadoutDisplay.Append("][FunctionalItem1: ");
            loadoutDisplay.Append(this.FunctionalItem1);
            loadoutDisplay.Append("][FunctionalItem2: ");
            loadoutDisplay.Append(this.FunctionalItem2);
            loadoutDisplay.Append("][FunctionalItem3: ");
            loadoutDisplay.Append(this.FunctionalItem3);
            loadoutDisplay.Append("][Gloves: ");
            loadoutDisplay.Append(this.Gloves);
            loadoutDisplay.Append("][Head: ");
            loadoutDisplay.Append(this.Head);
            loadoutDisplay.Append("][LoadoutId: ");
            loadoutDisplay.Append(this.LoadoutId);
            loadoutDisplay.Append("][LowerBody: ");
            loadoutDisplay.Append(this.LowerBody);
            loadoutDisplay.Append("][MeleeWeapon: ");
            loadoutDisplay.Append(this.MeleeWeapon);
            loadoutDisplay.Append("][QuickItem1: ");
            loadoutDisplay.Append(this.QuickItem1);
            loadoutDisplay.Append("][QuickItem2: ");
            loadoutDisplay.Append(this.QuickItem2);
            loadoutDisplay.Append("][QuickItem3: ");
            loadoutDisplay.Append(this.QuickItem3);
            loadoutDisplay.Append("][Type: ");
            loadoutDisplay.Append(this.Type);
            loadoutDisplay.Append("][UpperBody: ");
            loadoutDisplay.Append(this.UpperBody);
            loadoutDisplay.Append("][Weapon1: ");
            loadoutDisplay.Append(this.Weapon1);
            loadoutDisplay.Append("][Weapon1Mod1: ");
            loadoutDisplay.Append(this.Weapon1Mod1);
            loadoutDisplay.Append("][Weapon1Mod2: ");
            loadoutDisplay.Append(this.Weapon1Mod2);
            loadoutDisplay.Append("][Weapon1Mod3: ");
            loadoutDisplay.Append(this.Weapon1Mod3);
            loadoutDisplay.Append("][Weapon2: ");
            loadoutDisplay.Append(this.Weapon2);
            loadoutDisplay.Append("][Weapon2Mod1: ");
            loadoutDisplay.Append(this.Weapon2Mod1);
            loadoutDisplay.Append("][Weapon2Mod2: ");
            loadoutDisplay.Append(this.Weapon2Mod2);
            loadoutDisplay.Append("][Weapon2Mod3: ");
            loadoutDisplay.Append(this.Weapon2Mod3);
            loadoutDisplay.Append("][Weapon3: ");
            loadoutDisplay.Append(this.Weapon3);
            loadoutDisplay.Append("][Weapon3Mod1: ");
            loadoutDisplay.Append(this.Weapon3Mod1);
            loadoutDisplay.Append("][Weapon3Mod2: ");
            loadoutDisplay.Append(this.Weapon3Mod2);
            loadoutDisplay.Append("][Weapon3Mod3: ");
            loadoutDisplay.Append(this.Weapon3Mod3);
            loadoutDisplay.Append("][Webbing: ");
            loadoutDisplay.Append(this.Webbing);
            loadoutDisplay.Append("][SkinColor: ");
            loadoutDisplay.Append(this.SkinColor);
            loadoutDisplay.Append("]]");

            return loadoutDisplay.ToString(); ;
        }

        #endregion Methods
    }
}