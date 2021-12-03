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
using System.Net;

namespace KnightsOfEmpire.Server.GameStates.Match
{
    public class UnitUpdateState : UnitState
    {
        private const int unitPacketSize = 10;


        private static Stopwatch UnregisterTestTimer;
        private static Stopwatch UnregisterTestTimer2;

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
            UnregisterTestTimer2 = new Stopwatch();
            UnregisterTestTimer2.Restart();
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

            // this code move first units 1 px per one second
            // TODO: remove this fragment once units will be more 'active'
            if (UnregisterTestTimer2.ElapsedMilliseconds / 1000.0f >= 1.0f)
            {
                for (int i = 0; i < MaxPlayerCount; i++)
                {
                    if (GameUnits[i].Count > 0)
                    {
                        GameUnits[i][0].Position += new Vector2f(1, 1);
                    }
                }
                UnregisterTestTimer2.Restart();
            }


            // Send Uppdate about all units
            // TODO: consider to send only units that was uppdated
            for (int i=0; i<GameUnits.Length; i++)
            {
                if(GameUnits[i].Count > 0)
                {
                    CreateAndSendUnitsUpdate(GameUnits[i], i);
                }
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


        private void CreateAndSendUnitsUpdate(List<Unit> units, int ownerPlayerID)
        {
            UpdateUnitData[] updateUnitDatas = new UpdateUnitData[unitPacketSize];
            int i = 0;
            foreach(Unit unit in units)
            {
                if(i >= unitPacketSize)
                {
                    SendUnitsResponse(updateUnitDatas);
                    updateUnitDatas = new UpdateUnitData[unitPacketSize];
                    i = 0;
                }
                updateUnitDatas[i] = new UpdateUnitData(unit, ownerPlayerID);
                i += 1;
            }
            if(updateUnitDatas[0] != null)
            {
                SendUnitsResponse(updateUnitDatas);
            }

            //Console.WriteLine("Send Units data");
        }

        private void SendUnitsResponse(UpdateUnitData[] updateUnitDatas)
        {
            UpdateUnitsResponse updateUnitsResponse = new UpdateUnitsResponse();
            updateUnitsResponse.TimeStamp = DateTime.Now.Ticks;
            updateUnitsResponse.UnitData = updateUnitDatas;

            SentPacket packet = new SentPacket(PacketsHeaders.GameUnitUpdateRequest);
            packet.stringBuilder.Append(JsonSerializer.Serialize(updateUnitsResponse));

            for(int i=0; i<Server.TCPServer.MaxConnections; i++)
            {
                if(Server.TCPServer.IsClientConnected(i))
                {
                    packet.ClientID = i;
                    Server.UDPServer.SentTo(packet, Server.TCPServer.GetClientAddress(i), i);
                }
            }
        }
    }
}
