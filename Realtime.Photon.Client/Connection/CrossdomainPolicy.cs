using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Cmune.Realtime.Photon.Client
{
    public static class CrossdomainPolicy
    {
        private static Dictionary<string, bool?> _dict = new Dictionary<string, bool?>(20);

        /// <summary>
        /// Run a routine to check wether there is a Policy server hosted on this IP/Port
        /// The check is performed in seperate thread to avoid the synchronous execution
        /// within the frame loop that is happening when a socket server connection is 
        /// started first.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static IEnumerator CheckDomain(string address)
        {
            //only prefetch the policy in webplayer builds
            if (Application.isWebPlayer)
            {
                RemovePolicyEntry(address);

                try
                {
                    ThreadPool.QueueUserWorkItem(delegate { SetPolicyValue(address, Security.PrefetchSocketPolicy(address, 843)); });
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Failed to queue work item: " + ex.Message);
                }

                while (!HasPolicyEntry(address))
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                SetPolicyValue(address, true);
            }
        }

        /// <summary>
        /// Check if there is a Policy server hosted for this IP/Port
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool HasValidPolicy(string address)
        {
            bool? hasPolicy;
            lock (_dict)
            {
                if (!_dict.TryGetValue(address, out hasPolicy))
                    return false;
            }
            return hasPolicy ?? false;
        }

        /// <summary>
        /// Check if we have a policy entry at all for this IP/Port address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static bool HasPolicyEntry(string address)
        {
            bool? hasPolicy;
            lock (_dict)
            {
                _dict.TryGetValue(address, out hasPolicy);
            }
            return hasPolicy.HasValue;
        }

        private static void RemovePolicyEntry(string address)
        {
            lock (_dict)
            {
                _dict.Remove(address);
            }
        }

        private static void SetPolicyValue(string address, bool b)
        {
            lock (_dict)
            {
                _dict[address] = b;
            }
        }
    }
}
