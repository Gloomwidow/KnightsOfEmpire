using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Units;
using KnightsOfEmpire.Common.Units;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Server.GameStates.Match
{
    public class UnitUpdateState : UnitState
    {

        private static Stopwatch UnregisterTestTimer;
        public override void HandleTCPPacket(ReceivedPacket packet)
        {
            switch (packet.GetHeader())
            {
                case PacketsHeaders.GameUnitTrainRequest:
                    TrainUnit(packet);
                    break;
               
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            UnregisterTestTimer = new Stopwatch();
            UnregisterTestTimer.Restart();
        }

        public override void Update()
        {
            // this code fragment will first unit in list of each player every 10 seconds to check if unregistering is working 
            // TO-DO: remove this fragment once units will be more 'active'
            if (UnregisterTestTimer.ElapsedMilliseconds/1000.0f>=10.0f)
            {
                for(int i=0;i<MaxPlayerCount;i++)
                {
                    if(GameUnits[i].Count>0)
                    {
                        DeleteUnit(i, 0);
                    }
                }
                UnregisterTestTimer.Restart();
            }
            
        }

        protected void TrainUnit(ReceivedPacket packet)
        {
            TrainUnitRequest request = null;
            try
            {
                request = JsonSerializer.Deserialize<TrainUnitRequest>(packet.GetContent());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            //TO-DO: once we will have gold, check if player has enough of it to train
            //TO-DO: check if player has building to trait this unit (check Building Pos)

            Unit unit = new Unit()
            {
                ID = UnitIdManager.GetNewId(),
                Position = new Vector2f(request.BuildingPosX, request.BuildingPosY),
                TextureId = 0,
                Stats = new UnitStats()
            };

            GameUnits[packet.ClientID].Add(unit);
            RegisterUnitRequest registerRequest = new RegisterUnitRequest
            {
                ID = unit.ID,
                StartPositionX = (float)Math.Round(unit.Position.X, 3, MidpointRounding.AwayFromZero),
                StartPositionY = (float)Math.Round(unit.Position.Y, 3, MidpointRounding.AwayFromZero),
                PlayerId = packet.ClientID,
                UnitTypeId = 0
            };

            SentPacket registerPacket = new SentPacket(PacketsHeaders.GameUnitRegisterRequest, -1);
            registerPacket.stringBuilder.Append(JsonSerializer.Serialize(registerRequest));

            for (int i=0;i<MaxPlayerCount;i++)
            {
                registerPacket.ClientID = i;
                Server.TCPServer.SendToClient(registerPacket);
            }
        }

        protected void DeleteUnit(int playerId, int index)
        {
            Unit unitDeletion = GameUnits[playerId][index];
            UnregisterUnitRequest request = new UnregisterUnitRequest
            {
                PlayerId = playerId,
                ID = unitDeletion.ID
            };

            SentPacket unregisterPacket = new SentPacket(PacketsHeaders.GameUnitUnregisterRequest, -1);
            unregisterPacket.stringBuilder.Append(JsonSerializer.Serialize(request));

            for (int i = 0; i < MaxPlayerCount; i++)
            {
                unregisterPacket.ClientID = i;
                Server.TCPServer.SendToClient(unregisterPacket);
            }

            GameUnits[playerId].RemoveAt(index);
        }
    }
}
