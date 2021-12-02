using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.GameStates;
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

namespace KnightsOfEmpire.Server.GameStates.Match
{
    public class UnitUpdateState : UnitState
    {
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
                for(int j=0;j<GameUnits[i].Count;j++)
                {
                    Unit u = GameUnits[i][j];
                    if(u.Stats.HealthPercentage<=0)
                    {
                        DeleteUnit(i, j);
                        j--;
                        continue;
                    }
                    if(u.UnitGroup != null)
                    {
                        Vector2f flowVector = Server.Resources.NavigationManager.GetFlowVector(u.Position, u.UnitGroup.Target);
                        u.Update(flowVector, GetFriendlyUnitsInRange(u, Unit.UnitAvoidanceDistance));
                    }
                    u.Move(Server.DeltaTime);
                    if(u.UnitGroup.Target.Equals(Server.Resources.Map.ToTilePos(u.Position)))
                    {
                        u.UnitGroup.Leave(u);
                    } 
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
