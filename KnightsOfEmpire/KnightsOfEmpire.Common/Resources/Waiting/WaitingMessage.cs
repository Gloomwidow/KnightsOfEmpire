using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Waiting
{
    public enum WaitingMessage
    {
        ServerOk = 0, // will be sent when server will send updates from waiting state
        ServerRefuse = 1, // will be sent when server will refuse to join player (for example, when message is malformed or if client can't join server because the game has already been started)
        ServerFull = 2, // will be sent when server has maximum connections
        ServerInGame = 3, //will acknowlegde player that server is in game, so the client should jump straight into proper gamestate. This will be used when client is reconnecting to already started game 
    }
}
