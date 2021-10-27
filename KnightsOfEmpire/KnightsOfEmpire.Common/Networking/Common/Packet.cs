using System;
using System.Text;

namespace KnightsOfEmpire.Common.Networking
{
    public abstract class Packet
    {
        /// <summary>
        /// This tag will indicate the end of data in packet. This will help receiver to recognise when they can stop receive single packet.
        /// </summary>
        public static readonly string EOFTag = "<EOF>"; 

        /// <summary>
        /// ID of client who send this packet. This can also be used to send back packet to particular client.
        /// </summary>        
        public int ClientID { get; set; }

        /*
         * Idea of structure of packets send through network:
         * 
         * ####********<EOF>
         * 
         * #### - header, four digits (0-9) to identify what content is in package.
         * For instance, package with header 0125 could mean client's order to move troops. 
         * ***** - actual content of package
         * <EOF> - package end tag. Might subject to change to something more unique, in case content would somehow could have this tag, 
         * ending package unexpectedly.
         * 
         * 
         * We can also use JSON to encode packages, but I'm not sure if the size will be small enough to handle it fast.
         * I leave that to discussion, since messages can be of any shape as long as they are encoded in ASCII.
         */

        public abstract string GetContent();
    }

    public class ReceivedPacket : Packet
    {
        protected string Content;

        public DateTime ReceiveTime { get; protected set; }

        public ReceivedPacket(int clientID, string content)
        {
            ClientID = clientID;
            Content = content;
            ReceiveTime = DateTime.Now;
        }

        public override string GetContent()
        {
            return Content.Substring(0, Content.Length - EOFTag.Length);
        }
    }

    public class SentPacket : Packet
    {
        public StringBuilder stringBuilder { get; protected set; }

        public SentPacket(int clientID)
        {
            ClientID = clientID;
            stringBuilder = new StringBuilder();
        }

        public SentPacket()
        {
            stringBuilder = new StringBuilder();
        }

        public override string GetContent()
        {
            return stringBuilder.ToString() + EOFTag;
        }

    }
}
