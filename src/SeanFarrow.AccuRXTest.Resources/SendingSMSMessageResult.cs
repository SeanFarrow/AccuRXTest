using System;
using NodaTime;

namespace SeanFarrow.AccuRXTest.Resources
{
    /// <summary>
    /// Represents the result of successfully sending an SMS message through the API.
    /// </summary>
    public class SendingSMSMessageResult
    {
        /// <summary>
        /// The internal ID we have assigned to this message in the system.
        /// </summary>
        public Guid MessageID { get; set; }

        /// <summary>
        /// The FireText message ID passed back to us in the X-message header allowing us to look up delivery reports later.
        /// </summary>
        public string FireTextMessageID { get; set; }

        /// <summary>
        /// The time in UTC the message was queued.
        /// </summary>
        public Instant QueuedTime { get; set; }
        
        /// <summary>
        /// The mobile phone number of the person to which the message was sent.
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// The text of the message.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The NHS number of the messages recipient (should be 10 digits long).
        /// </summary>
        public string NHSNumber { get; set; }
    }
}