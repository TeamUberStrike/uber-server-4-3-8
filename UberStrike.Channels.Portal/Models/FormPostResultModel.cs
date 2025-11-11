using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UberStrike.Channels.Portal.Models
{
    public enum FormPostResultStatus
    {
        None,
        Error,
        Success,
        Notification
    }

    public class FormPostResultModel
    {
        public FormPostResultStatus Status { get; set; }
        public string Message { get; set; }

        public FormPostResultModel()
        {
            Status = FormPostResultStatus.None;
        }

        public FormPostResultModel(FormPostResultStatus status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}