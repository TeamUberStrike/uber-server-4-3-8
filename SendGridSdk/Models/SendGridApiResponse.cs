// -----------------------------------------------------------------------
// <copyright file="SendGridApiResponse.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SendGridSdk.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class SendGridApiResponse
    {
        #region Properties

        public bool IsSuccess { get; private set; }
        public IList<string> ErrorMessages { get; private set; }

        #endregion

        #region Constructors

        public SendGridApiResponse()
        {
            this.IsSuccess = false;
            this.ErrorMessages = new List<string>();
        }

        public SendGridApiResponse(bool isSuccess, IList<string> errorMessages)
        {
            IsSuccess = isSuccess;
            ErrorMessages = errorMessages;
        }

        #endregion
    }
}
