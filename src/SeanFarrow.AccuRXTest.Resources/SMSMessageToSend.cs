namespace SeanFarrow.AccuRXTest.Resources
{
    /// <summary>
    /// Represents the details of an SMS message we wish to send through the API.
    /// </summary>
    public class SMSMessageToSend
    {
        /// <summary>
        /// The mobile phone number of the person to which the message should be sent.
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// The text we wish to include in the message (up to 160 characters.)
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The NHS number of the messages recipient (should be 10 digits long).
        /// </summary>
        public string NHSNumber { get; set; }
    }
}