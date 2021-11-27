using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Komiwojazer
{
    public static class PointExtensionMethod
    {
        public static bool IsOnRoad(this Point point)
        {
            return (IsOnRow(point) || IsOnColumn(point));
        }

        private static bool IsOnRow(Point point)
        {
            if (point.X < 0 || point.X > 764)
            {
                return false;
            }

            if ((point.Y >= 6 && point.Y <= 12) ||
               (point.Y >= 49 && point.Y <= 57) ||
               (point.Y >= 95 && point.Y <= 101) ||
               (point.Y >= 140 && point.Y <= 145) ||
               (point.Y >= 184 && point.Y <= 189) ||
               (point.Y >= 227 && point.Y <= 234) ||
               (point.Y >= 275 && point.Y <= 284) ||
               (point.Y >= 323 && point.Y <= 330) ||
               (point.Y >= 368 && point.Y <= 373) ||
               (point.Y >= 411 && point.Y <= 417) ||
               (point.Y >= 455 && point.Y <= 462) ||
               (point.Y >= 500 && point.Y <= 505) ||
               (point.Y >= 544 && point.Y <= 550))
            {
                return true;
            }

            return false;
        }

        private static bool IsOnColumn(Point point)
        {
            if (point.Y < 0 || point.Y > 575)
            {
                return false;
            }

            if((point.X >= 0 && point.X <= 11) ||
               (point.X >= 88 && point.X <= 103) ||
               (point.X >= 174 && point.X <= 185) ||
               (point.X > 185 && point.X <= 197) ||
               (point.X >= 275 && point.X <= 286) ||
               (point.X >= 366 && point.X <= 382) ||
               (point.X >= 494 && point.X <= 505) ||
               (point.X >= 630 && point.X <= 642) ||
               (point.X >= 760 && point.X <= 764))
            {
                return true;
            }

            return false;
        }
    }
}
