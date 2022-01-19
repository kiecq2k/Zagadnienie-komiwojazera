using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Komiwojazer
{

    public enum Version { Demo, Full };

    public static class PointExtensionMethod
    {
        public static bool IsOnRoad(this Point point, Version version)
        {
            switch (version)
            {
                case Version.Demo: return (IsOnRowDemoVersion(point) || IsOnColumnDemoVersion(point));
                case Version.Full: return (IsOnRowFullVersion(point) || IsOnColumnFullVersion(point));
                default: return false;
            }
        }

        private static bool IsOnRowFullVersion(Point point)
        {
            if (point.X < 5 || point.X > 727)
            {
                return false;
            }

            if ((point.X >= 5 && point.X <= 725 && point.Y >= 4 && point.Y <= 12) ||    //y8
               (point.X >= 5 && point.X <= 358 && point.Y >= 48 && point.Y <= 58) ||    //y10
               (point.X >= 467 && point.X <= 725 && point.Y >= 46 && point.Y <= 56) ||  //y10
               (point.X >= 5 && point.X <= 605 && point.Y >= 93 && point.Y <= 103) ||   //y10
               (point.X >= 5 && point.X <= 273 && point.Y >= 137 && point.Y <= 147) ||  //y10
               (point.X >= 345 && point.X <= 725 && point.Y >= 137 && point.Y <= 145) ||//y8
               (point.X >= 5 && point.X <= 725 && point.Y >= 182 && point.Y <= 190) ||  //y8
               (point.X >= 5 && point.X <= 479 && point.Y >= 225 && point.Y <= 235) ||  //y10
               (point.X >= 594 && point.X <= 726 && point.Y >= 225 && point.Y <= 234) ||//y9
               (point.X >= 5 && point.X <= 666 && point.Y >= 275 && point.Y <= 285) ||  //y10
               (point.X >= 5 && point.X <= 726 && point.Y >= 324 && point.Y <= 331) ||  //y7
               (point.X >= 5 && point.X <= 726 && point.Y >= 366 && point.Y <= 375) ||  //y9
               (point.X >= 93 && point.X <= 726 && point.Y >= 412 && point.Y <= 419) || //y7
               (point.X >= 5 && point.X <= 726 && point.Y >= 457 && point.Y <= 464) ||  //y7
               (point.X >= 5 && point.X <= 275 && point.Y >= 502 && point.Y <= 509) ||  //y6
               (point.X >= 351 && point.X <= 726 && point.Y >= 500 && point.Y <= 509) ||//y9
               (point.X >= 5 && point.X <= 608 && point.Y >= 547 && point.Y <= 554) ||  //y8
               (point.X >= 5 && point.X <= 726 && point.Y >= 592 && point.Y <= 600))    //y8
            {
                return true;
            }

            return false;
        }

        private static bool IsOnColumnFullVersion(Point point)
        {
            if (point.Y < 1 || point.Y > 600)
            {
                return false;
            }

            if((point.X >= 5 && point.X <= 15 && point.Y >= 2 && point.Y <= 147) ||
               (point.X >= 5 && point.X <= 15 && point.Y >= 185 && point.Y <= 510) ||
               (point.X >= 5 && point.X <= 15 && point.Y >= 549 && point.Y <= 600) ||
               (point.X >= 62 && point.X <= 69 && point.Y >= 49 && point.Y <= 100) ||
               (point.X >= 91 && point.X <= 100 && point.Y >= 95 && point.Y <= 600) ||
               (point.X >= 171 && point.X <= 180 && point.Y >= 2 && point.Y <= 190) ||
               (point.X >= 171 && point.X <= 180 && point.Y >= 228 && point.Y <= 331) ||
               (point.X >= 171 && point.X <= 180 && point.Y >= 368 && point.Y <= 464) ||
               (point.X >= 171 && point.X <= 180 && point.Y >= 501 && point.Y <= 600) ||
               (point.X >= 181 && point.X <= 191 && point.Y >= 2 && point.Y <= 56) ||
               (point.X >= 181 && point.X <= 191 && point.Y >= 95 && point.Y <= 190) ||
               (point.X >= 181 && point.X <= 191 && point.Y >= 228 && point.Y <= 464) ||
               (point.X >= 181 && point.X <= 191 && point.Y >= 501 && point.Y <= 600) ||
               (point.X >= 264 && point.X <= 273 && point.Y >= 50 && point.Y <= 330) ||
               (point.X >= 264 && point.X <= 273 && point.Y >= 369 && point.Y <= 600) ||
               (point.X >= 345 && point.X <= 358 && point.Y >= 2 && point.Y <= 144) ||
               (point.X >= 345 && point.X <= 358 && point.Y >= 183 && point.Y <= 376) ||
               (point.X >= 345 && point.X <= 358 && point.Y >= 412 && point.Y <= 600) ||
               (point.X >= 405 && point.X <= 412 && point.Y >= 139 && point.Y <= 188) ||
               (point.X >= 408 && point.X <= 415 && point.Y >= 367 && point.Y <= 508) ||
               (point.X >= 467 && point.X <= 478 && point.Y >= 2 && point.Y <= 145) ||
               (point.X >= 467 && point.X <= 478 && point.Y >= 182 && point.Y <= 600) ||
               (point.X >= 540 && point.X <= 547 && point.Y >= 366 && point.Y <= 508) ||
               (point.X >= 593 && point.X <= 604 && point.Y >= 49 && point.Y <= 418) ||
               (point.X >= 593 && point.X <= 604 && point.Y >= 456 && point.Y <= 600) ||
               (point.X >= 660 && point.X <= 666 && point.Y >= 271 && point.Y <= 330) ||
               (point.X >= 712 && point.X <= 724 && point.Y >= 2 && point.Y <= 233) ||
               (point.X >= 712 && point.X <= 724 && point.Y >= 320 && point.Y <= 600))
            {
                return true;
            }

            return false;
        }

        private static bool IsOnRowDemoVersion(Point point)
        {
            if (point.X < 0 || point.X > 695)
            {
                return false;
            }

            if ((point.X >= 176 && point.X <= 678 && point.Y >= 6 && point.Y <= 17) ||
               (point.X >= 5 && point.X <= 678 && point.Y >= 91 && point.Y <= 103) ||
               (point.X >= 5 && point.X <= 678 && point.Y >= 178 && point.Y <= 192) ||
               (point.X >= 5 && point.X <= 678 && point.Y >= 269 && point.Y <= 280) ||
               (point.X >= 5 && point.X <= 678 && point.Y >= 365 && point.Y <= 375))
            {
                return true;
            }

            return false;
        }

        private static bool IsOnColumnDemoVersion(Point point)
        {
            if (point.Y < 0 || point.Y > 378)
            {
                return false;
            }

            if ((point.X >= 6 && point.X <= 24 && point.Y >= 91 && point.Y <= 376) ||
               (point.X >= 177 && point.X <= 195 && point.Y >= 1 && point.Y <= 192) ||
               (point.X >= 177 && point.X <= 195 && point.Y >= 270 && point.Y <= 376) ||
               (point.X >= 416 && point.X <= 434 && point.Y >= 0 && point.Y <= 105) ||
               (point.X >= 416 && point.X <= 434 && point.Y >= 182 && point.Y <= 376) ||
               (point.X >= 308 && point.X <= 319 && point.Y >= 181 && point.Y <= 376) ||
               (point.X >= 81 && point.X <= 92 && point.Y >= 269 && point.Y <= 376) ||
               (point.X >= 522 && point.X <= 532 && point.Y >= 93 && point.Y <= 191) ||
               (point.X >= 669 && point.X <= 678 && point.Y >= 0 && point.Y <= 105) ||
               (point.X >= 669 && point.X <= 678 && point.Y >= 182 && point.Y <= 376))
            {
                return true;
            }

            return false;
        }
    }
}
