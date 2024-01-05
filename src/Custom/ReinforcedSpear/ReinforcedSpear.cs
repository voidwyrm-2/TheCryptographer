using System.Collections.Generic;
using UnityEngine;
using RWCustom;

namespace NuclearPasta.TheCryptographer.Custom.ReinforcedSpear
{
    public class ReinforcedSpear : PhysicalObject, IDrawable
    {

        public ReinforcedSpear(ReinforcedSpearAbstract abstr) : base(abstr)
        {
            float mass = 40f;
            var positions = new List<Vector2>();
            positions.Add(Vector2.zero);

            bodyChunks = new BodyChunk[positions.Count];

            // Create all body chunks
            for (int i = 0; i < bodyChunks.Length; i++)
            {
                bodyChunks[i] = new BodyChunk(this, i, Vector2.zero, 30f, mass / bodyChunks.Length);
            }

            bodyChunks[0].rad = 40f;

            bodyChunkConnections = new BodyChunkConnection[0];
            int connection = 0;

            // Create all chunk connections

            for (int x = 0; x < bodyChunks.Length; x++)
            {
                for (int y = x + 1; y < bodyChunks.Length; y++)
                {
                    bodyChunkConnections[connection] = new BodyChunkConnection(bodyChunks[x], bodyChunks[y], Vector2.Distance(positions[x], positions[y]), BodyChunkConnection.Type.Normal, 0.5f, -1f);
                    connection++;
                }
            }

            airFriction = 0.999f;
            gravity = 0.9f;
            bounce = 0.3f;
            surfaceFriction = 1f;
            collisionLayer = 1;
            waterFriction = 0.92f;
            buoyancy = 0.75f;
            GoThroughFloors = false;
        }

        public override void PlaceInRoom(Room placeRoom)
        {
            base.PlaceInRoom(placeRoom);

            Vector2 center = placeRoom.MiddleOfTile(abstractPhysicalObject.pos);
            bodyChunks[0].HardSetPosition(new Vector2(0, 0) * 20f + center);
        }

        public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
        {
            base.TerrainImpact(chunk, direction, speed, firstContact);

            if (speed > 10)
            {
                room.PlaySound(SoundID.Spear_Fragment_Bounce, bodyChunks[chunk].pos, 0.35f, 2f);
            }
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[bodyChunks.Length];

            for (int i = 0; i < bodyChunks.Length; i++) sLeaser.sprites[i] = new FSprite("Circle20");

            AddToContainer(sLeaser, rCam, null);
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            for (int i = 0; i < bodyChunks.Length; i++)
            {
                var spr = sLeaser.sprites[i];
                spr.SetPosition(Vector2.Lerp(bodyChunks[i].lastPos, bodyChunks[i].pos, timeStacker) - camPos);
                spr.scale = bodyChunks[i].rad / 10f;
            }

            if (slatedForDeletetion || room != rCam.room) sLeaser.CleanSpritesAndRemove();
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            foreach (var sprite in sLeaser.sprites)
                sprite.color = palette.blackColor;
        }

        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContainer)
        {
            newContainer ??= rCam.ReturnFContainer("Items");

            foreach (FSprite fsprite in sLeaser.sprites)
                newContainer.AddChild(fsprite);
        }
    }
}
