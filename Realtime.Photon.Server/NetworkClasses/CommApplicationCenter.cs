//using Cmune.Realtime.Common;
//using Cmune.Realtime.Common.IO;
//using Cmune.Realtime.Photon.Server;

//public class CommApplicationCenter : ApplicationCenter
//{
//    [NetworkMethod(CommApplicationRPC.ProfanityCheck)]
//    private void OnProfanityCheck(CmunePeer peer, MessageToApplication op, string message)
//    {
//        if (peer != null && !string.IsNullOrEmpty(message))
//        {
//            try
//            {
//                //CrispWordFilter.FilterMessage("CommSender", "CommReciever", message, true,
//                //    (cleanMessage) =>
//                //    {
//                op.ReturnValue = RealtimeSerialization.ToBytes(message).ToArray();
//                peer.PublishOperationResponse(op.GetOperationResponse(0, string.Empty));
//                //    });
//            }
//            catch
//            {
//                peer.PublishOperationResponse(op.GetOperationResponse(1, string.Format("Exception happend on profanity check of '{0}'", message)));
//            }
//        }
//    }
//}
