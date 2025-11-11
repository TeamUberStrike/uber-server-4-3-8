using System.Collections.Generic;

namespace UberStrike.Realtime.CommServer
{
    class ContactList
    {
        public readonly HashSet<int> ContactIds;

        public ContactList(List<int> cmids)
        {
            ContactIds = new HashSet<int>(cmids);
        }

        public void Update(List<int> cmids)
        {
            ContactIds.Clear();
            ContactIds.UnionWith(cmids);

            //////add friends to the removal list that are not our friends anymore
            //////covers the case of update friend list after friend removal
            ////foreach (int cmid in Versions.Keys)
            ////{
            ////    if (!contacts.Contains(cmid))
            ////    {
            ////        Versions[cmid] = 0;
            ////    }
            ////}

            ////add missing contact to the list
            //foreach (int cmid in contacts)
            //{
            //    if (!ContactIds.ContainsKey(cmid))
            //    {
            //        ContactIds[cmid] = 0;
            //    }
            //}

            //////fill up the contacts to remove, for the next UpdateFriendsState call
            ////foreach (int i in Removals)
            ////{
            ////    Versions.Remove(i);
            ////}
        }
    }
}
