using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;

namespace Cmune.Channels.Instrumentation.Utils
{
    public static class ContextHelper<T> where T : DataContext, new()
    {
        #region Consts

        private const string ObjectContextKey = "ObjectContext";

        #endregion

        #region Methods

        public static void SetCurrentContext(T objectContext)
        {
            HttpContext httpContext = HttpContext.Current;

            if (httpContext != null)
            {
                string contextTypeKey = ObjectContextKey + typeof(T).Name;
                httpContext.Items[contextTypeKey] = objectContext;
            }
        }

        public static T GetCurrentContext()
        {
            HttpContext httpContext = HttpContext.Current;

            if (httpContext != null)
            {
                string contextTypeKey = ObjectContextKey + typeof(T).Name;

                if (httpContext.Items[contextTypeKey] == null)
                {
                    httpContext.Items.Add(contextTypeKey, new T());
                }

                return httpContext.Items[contextTypeKey] as T;
            }

            throw new ApplicationException("There is no Http Context available");

        }

        #endregion
    }
}