using UnityEngine;

namespace UberStrike.WebService.Unity
{
    internal class WebServiceStarter : MonoBehaviour
    {
        private static MonoBehaviour mono;

        public static MonoBehaviour Mono
        {
            get
            {
                if (mono == null)
                {
                    GameObject obj = new GameObject("WebServiceStarter");
                    mono = obj.AddComponent<WebServiceStarter>();
                }
                return mono;
            }
        }
    }
}