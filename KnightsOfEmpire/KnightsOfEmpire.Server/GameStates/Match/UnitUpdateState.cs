using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Navigation;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Units;
using KnightsOfEmpire.Common.Units;
using KnightsOfEmpire.Common.Units.Groups;
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
        private const int unitPacketSize = 50;

        public List<UnitGroup> UnitGroups;

        public override void HandleTCPPacket(ReceivedPacket packet)
        {
            switch (packet.GetHeader())
            {
                case PacketsHeaders.GameUnitTrainRequest:
                    TrainUnit(packet);
                    break;
                case PacketsHeaders.GameUnitsMoveToTileRequest:
                    CreateMoveUnitsGroup(packet);
                    break;

            }
        }

        public override void Initialize()
        {
            base.Initialize();
            UnitGroups = new List<UnitGroup>();
            Server.Resources.NavigationManager = new FlowFieldManager(Server.Resources.Map);
        }

        public override void Update()
        {
            for (int i = 0; i < UnitGroups.Count; i++)
            {
                UnitGroup ug = UnitGroups[i];
                if (ug.HasGroupBeenCompleted())
                {
                    UnitGroups.RemoveAt(i);
                    i--;
                }
                else ug.Update();
            }
            for(int i=0;i<MaxPlayerCount;i++)
            {
                for (int j = 0; j < GameUnits[i].Count; j++)
                {
                    Unit u = GameUnits[i][j];
                    // delete death units
                    if (u.Stats.HealthPercentage <= 0)
                    {
                        DeleteUnit(i, j);
                        j--;
                        continue;
                    }
                    Vector2f flowVector = new Vector2f(0, 0);

                    List<Unit> enemyInRange = GetEnemyUnitsInRange(u, u.Stats.AttackDistance);


                    // if unit is in moving group, get its movement direction
                    if (u.UnitGroup != null && !u.IsGroupCompleted)
                    {
                        flowVector = Server.Resources.NavigationManager.GetFlowVector(u.Position, u.UnitGroup.Target);
                    }

                    if (enemyInRange.Count > 0)
                    {
                        if (u.UnitGroup != null) u.UnitGroup.Leave(u);
                    }
                    u.Attack(Server.DeltaTime, enemyInRange);
                    u.Update(flowVector, GetFriendlyUnitsInRange(u, Unit.UnitAvoidanceDistance));
                    u.Move(Server.DeltaTime);

                    u.Position = Server.Resources.Map.SnapToWall(u.PreviousPosition, u.Position);
                    u.Position = Server.Resources.Map.SnapToBounds(u.Position);

                    if (u.UnitGroup != null) u.UnitGroup.UpdateUnitComplete(u);
                }
            }



            // Send Uppdate about all units
            for (int i=0; i<GameUnits.Length; i++)
            {
                if(GameUnits[i].Count > 0)
                {
                    CreateAndSendUnitsUpdate(GameUnits[i], i);
                }
            }
        }


        protected void CreateMoveUnitsGroup(ReceivedPacket packet)
        {
            MoveUnitsToPositionRequest request = packet.GetRequestOrDefault<MoveUnitsToPositionRequest>();
            if (request == null) return;

            UnitGroup group = new UnitGroup();
            group.TargetX = request.GoalX;
            group.TargetY = request.GoalY;

            foreach(string ID in request.UnitIDs)
            {
                Unit u = GameUnits[packet.ClientID].Find(x => x.EqualID(ID.ToCharArray()));
                if(u!=null)
                {
                    group.Join(u);
                }
            }

            UnitGroups.Add(group);
        }

        protected void TrainUnit(ReceivedPacket packet)
        {
            TrainUnitRequest request = packet.GetRequestOrDefault<TrainUnitRequest>();
            if (request == null) return;

            //TO-DO: once we will have gold, check if player has enough of it to train
            //TO-DO: check if player has building to trait this unit (check Building Pos)

            Unit unit = new Unit()
            {
                ID = UnitIdManager.GetNewId(),
                PlayerId = packet.ClientID,
                Position = new Vector2f(request.BuildingPosX, request.BuildingPosY),
                MoveDirection = new Vector2f(0, 0),
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
            if(GameUnits[playerId][index].UnitGroup!=null) GameUnits[playerId][index].UnitGroup.Leave(GameUnits[playerId][index]);
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
