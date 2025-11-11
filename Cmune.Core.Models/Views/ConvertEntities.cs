namespace Cmune.DataCenter.Common.Entities
{
    public class ConvertEntities
    {
        /// <summary>
        /// Converts MemberRegistrationResult to MemberOperationResult
        /// </summary>
        /// <param name="memberRegistration"></param>
        /// <returns></returns>
        public static MemberOperationResult ConvertMemberRegistration(MemberRegistrationResult memberRegistration)
        {
            MemberOperationResult ret = MemberOperationResult.Ok;

            switch (memberRegistration)
            {
                case MemberRegistrationResult.InvalidHandle:
                    ret = MemberOperationResult.InvalidHandle;
                    break;
                case MemberRegistrationResult.InvalidName:
                    ret = MemberOperationResult.InvalidName;
                    break;
                case MemberRegistrationResult.InvalidEsns:
                    ret = MemberOperationResult.InvalidEsns;
                    break;
                case MemberRegistrationResult.DuplicateName:
                    ret = MemberOperationResult.DuplicateName;
                    break;
                case MemberRegistrationResult.DuplicateHandle:
                    ret = MemberOperationResult.DuplicateHandle;
                    break;
                case MemberRegistrationResult.Ok:
                    ret = MemberOperationResult.Ok;
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Converts MemberOperationResult to MemberRegistrationResult
        /// </summary>
        /// <param name="memberOperation"></param>
        /// <returns></returns>
        public static MemberRegistrationResult ConvertMemberOperation(MemberOperationResult memberOperation)
        {
            MemberRegistrationResult ret = MemberRegistrationResult.Ok;

            switch (memberOperation)
            {
                case MemberOperationResult.DuplicateEmail:
                    ret = MemberRegistrationResult.DuplicateEmail;
                    break;
                case MemberOperationResult.DuplicateEmailName:
                    ret = MemberRegistrationResult.DuplicateEmailName;
                    break;
                case MemberOperationResult.DuplicateName:
                    ret = MemberRegistrationResult.DuplicateName;
                    break;
                case MemberOperationResult.Ok:
                    ret = MemberRegistrationResult.Ok;
                    break;
                case MemberOperationResult.InvalidName:
                    ret = MemberRegistrationResult.InvalidName;
                    break;
                case MemberOperationResult.OffensiveName:
                    ret = MemberRegistrationResult.OffensiveName;
                    break;
            }

            return ret;
        }
    }
}
