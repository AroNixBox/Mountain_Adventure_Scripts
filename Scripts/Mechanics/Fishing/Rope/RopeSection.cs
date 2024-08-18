using UnityEngine;

namespace Mechanics.Fishing.Rope
{
    public struct RopeSection
    {
        public Vector3 Pos;
        public Vector3 Vel;
        
        public RopeSection(Vector3 pos)
        {
            Pos = pos;
            Vel = Vector3.zero;
        }
    }
}