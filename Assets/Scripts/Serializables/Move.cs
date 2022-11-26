using System;
using UnityEngine;

namespace Serializables
{
    [Serializable]
    public class Move
    {
        public Vector2 direction;

        public Move(Vector2 direction) => this.direction = direction;
        
        public Move(int[] direction) => this.direction = new Vector2(direction[0], direction[1]);

        public int[] ConvertToArray() => new int[] { (int) direction.x, (int) direction.y };
    }
}