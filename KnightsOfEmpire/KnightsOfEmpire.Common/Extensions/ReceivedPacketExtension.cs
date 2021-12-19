using KnightsOfEmpire.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Extensions
{
    public static class ReceivedPacketExtension
    {
        public static T GetDeserializedClassOrDefault<T>(this ReceivedPacket p)
        {
            T request = default(T);
            try
            {
                request = JsonSerializer.Deserialize<T>(p.GetContent());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return request;
        }
    }
}
