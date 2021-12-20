using KnightsOfEmpire.Common.Buildings;
using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Buildings;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Server.GameStates.Match
{
    public class BuildingUpdateState : BuildingState
    {

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void HandleTCPPacket(ReceivedPacket packet)
        {
            switch (packet.GetHeader())
            {
                case PacketsHeaders.BuildingCreateRequest:
                    CreateBuilding(packet);
                    break;
            }
        }
        protected void CreateBuilding(ReceivedPacket packet)
        {
            CreateBuildingRequest request = packet.GetDeserializedClassOrDefault<CreateBuildingRequest>();
            if (request == null) return;

            //TO-DO: once we will have gold, check if player has enough of it to train
            //TO-DO: check if player has building to train this unit (check Building Pos)

            Building building = new Building()
            {
                //ID = UnitIdManager.GetNewId(),
                PlayerId = packet.ClientID,
                Position = new Vector2i(request.BuildingPosX, request.BuildingPosY),
                Health = 100,
                MaxHealth = 100,
                TextureId = 0
            };

            // TO-DO Send packet with building
        }
    }
}
