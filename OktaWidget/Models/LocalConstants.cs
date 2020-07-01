using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OktaJS_SDK.Models
{
    public class Messages
    {
        public const string PASSWORD_WARN = "The user’s password was successfully validated but is about to expire and should be changed.";
        public const string PASSWORD_RESET = "The user successfully answered their recovery question and must to set a new password.";
        //public const string PASSWORD_RESET_FAILED = "The user's attempt to update expired password was unsuccessful, please try again.";
        public const string PASSWORD_EXPIRED = "The user’s password was successfully validated but is expired.";
        public const string RECOVERY = "The user has requested a recovery token to reset their password or unlock their account.";
       // public const string RECOVERY_CHECK_EMAIL = "The user has requested a password reset link be sent to their email. Please Check Inbox";
        //public const string RECOVERY_UNLOCK_CHECK_EMAIL = "The user has requested an Unlock Account link be sent to their email. Please Check Inbox";
        public const string RECOVERY_CHALLENGE = "The user must verify the factor-specific recovery challenge.";
       // public const string RECOVERY_CHALLENGE_WAITING = "Waiting for unlock account challenge";
        //public const string UNLOCK_RECOVERY_INCOMPLETE = "Unlock recovery workflow not completed.";
        //public const string UNLOCK_RECOVERY_SUCCEEDED = "Unlock workflow completed successfully.";
        //public const string VERIFICATION_CHALLENGE = "Another verification is required.";
        public const string LOCKED_OUT = "The user account is locked; self-service unlock or admin unlock is required.";
        public const string MFA_ENROLL = "The user must select and enroll an available factor for additional verification.";
        //public const string MFA_ENROLL_AT_LEAST_ONE = "The user must select and enroll in at least one available factor for additional verification.";
        public const string MFA_ENROLL_ACTIVATE = "The user must activate the factor to complete enrollment.";
        public const string MFA_REQUIRED = "The user must provide additional verification with a previously enrolled factor.";
        public const string MFA_CHALLENGE = "The user must verify the factor-specific challenge.";
        public const string WAITING = "Factor verification has started but not yet completed";
        public const string CHALLENGE = "Factor verification Challenge";
        //public const string WAITING_PLEASE_CONTINUE = "Factor verification has started. Please press the Continue button once you have approved the push request.";
        public const string FAILED = "Factor verification failed";
        public const string REJECTED = "Factor verification was denied by user";
        public const string CANCELLED = "Factor verification was canceled by user";
        public const string TIMEOUT = "Unable to verify factor within the allowed time window";
        public const string TIME_WINDOW_EXCEEDED = "Factor was successfully verified but outside of the computed time window. Another verification is required in current time window";
        public const string PASSCODE_REPLAYED = "Factor was previously verified within the same time window. User must wait another time window and retry with a new verification";
        public const string SUCCESS = "The transaction has completed successfully";
        //public const string MFA_VERIFICATION_SUCCEDED = "MFA verification succeeded";
        public const string UNEXPECTED_SERVER_ERROR = "Unexpected server error occurred verifying factor";
        //public const string UNEXPECTED_SERVER_ERROR_ENROLLING_FACTOR = "Unexpected server error occurred enrolling factor";
        //public const string UNRECOGNIZED_RESPONSE = "Factor response not recognized";
        //public const string SECURITY_QUESTION_SET = "Self Serve Security Question Succesfully Set";

    }

    public class LocalResponses
    {

        //some common responses status
        public const string RecoveryChallenge = "RECOVERY_CHALLENGE";
        public const string Challenge = "CHALLENGE";
        public const string Waiting = "WAITING";
        public const string Failed = "FAILED";
        public const string Rejected = "REJECTED";
        public const string Canceled = "CANCELLED";
        public const string Timeout = "TIMEOUT";
        public const string TimeWindowExceeded = "TIME_WINDOW_EXCEEDED";
        public const string PasscodeReplayed = "PASSCODE_REPLAYED";
        public const string Error = "ERROR";


    }


}