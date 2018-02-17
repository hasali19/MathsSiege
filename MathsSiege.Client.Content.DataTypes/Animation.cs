using Microsoft.Xna.Framework.Content;

namespace MathsSiege.Client.Content.DataTypes
{
    public class Animation
    {
        public string Name { get; set; }

        [ContentSerializer(Optional = true)]
        public bool IsLooping { get; set; } = true;

        [ContentSerializer(Optional = true)]
        public float FrameDuration { get; set; } = 0.1f;

        public int[] FrameIndices { get; set; }
    }
}
