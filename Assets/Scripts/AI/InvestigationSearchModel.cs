using System;

namespace Vanta.AI
{
    public sealed class InvestigationSearchModel
    {
        readonly string[] points;
        int index;

        public InvestigationSearchModel(string[] points)
        {
            if (points == null || points.Length == 0)
                throw new ArgumentException("At least one search point is required.", nameof(points));

            foreach (var point in points)
                if (string.IsNullOrWhiteSpace(point))
                    throw new ArgumentException("Search point ids cannot be empty.", nameof(points));

            this.points = (string[])points.Clone();
        }

        public string NextPoint()
        {
            var point = points[index];
            index = (index + 1) % points.Length;
            return point;
        }

        public void Reset() => index = 0;
    }
}
