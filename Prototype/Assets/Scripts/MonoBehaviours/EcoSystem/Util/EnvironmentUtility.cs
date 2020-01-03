﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentUtility : MonoBehaviour
{
    // returns true if unobstructed line of sight to target tile
    public static bool TileIsVisibile (int x, int y, int x2, int y2, bool[,] walkable) {
        // bresenham line algorithm
        int w = x2 - x;
        int h = y2 - y;
        int absW = System.Math.Abs (w);
        int absH = System.Math.Abs (h);

        // Is neighbouring tile
        if (absW <= 1 && absH <= 1) {
            return true;
        }

        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) {
            dx1 = -1;
            dx2 = -1;
        } else if (w > 0) {
            dx1 = 1;
            dx2 = 1;
        }
        if (h < 0) {
            dy1 = -1;
        } else if (h > 0) {
            dy1 = 1;
        }

        int longest = absW;
        int shortest = absH;
        if (longest <= shortest) {
            longest = absH;
            shortest = absW;
            if (h < 0) {
                dy2 = -1;
            } else if (h > 0) {
                dy2 = 1;
            }
            dx2 = 0;
        }

        int numerator = longest >> 1;
        for (int i = 1; i < longest; i++) {
            numerator += shortest;
            if (numerator >= longest) {
                numerator -= longest;
                x += dx1;
                y += dy1;
            } else {
                x += dx2;
                y += dy2;
            }

            if (!walkable[x, y]) {
                return false;
            }
        }
        return true;
    }

    // returns null if path is obstructed
    public static Coord[] GetPath (int x, int y, int x2, int y2, bool[,] walkable) {
        // bresenham line algorithm
        int w = x2 - x;
        int h = y2 - y;
        int absW = System.Math.Abs (w);
        int absH = System.Math.Abs (h);

        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) {
            dx1 = -1;
            dx2 = -1;
        } else if (w > 0) {
            dx1 = 1;
            dx2 = 1;
        }
        if (h < 0) {
            dy1 = -1;
        } else if (h > 0) {
            dy1 = 1;
        }

        int longest = absW;
        int shortest = absH;
        if (longest <= shortest) {
            longest = absH;
            shortest = absW;
            if (h < 0) {
                dy2 = -1;
            } else if (h > 0) {
                dy2 = 1;
            }
            dx2 = 0;
        }

        int numerator = longest >> 1;
        Coord[] path = new Coord[longest];
        for (int i = 1; i <= longest; i++) {
            numerator += shortest;
            if (numerator >= longest) {
                numerator -= longest;
                x += dx1;
                y += dy1;
            } else {
                x += dx2;
                y += dy2;
            }

            if (!walkable[x, y]) {
                return null;
            }
            path[i - 1] = new Coord (x, y);
        }
        return path;
    }

    public static Coord GetNextInPath (int x, int y, int targetX, int targetY) {
        // bresenham line algorithm
        int w = targetX - x;
        int h = targetY - y;
        int absW = System.Math.Abs (w);
        int absH = System.Math.Abs (h);

        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) {
            dx1 = -1;
            dx2 = -1;
        } else if (w > 0) {
            dx1 = 1;
            dx2 = 1;
        }
        if (h < 0) {
            dy1 = -1;
        } else if (h > 0) {
            dy1 = 1;
        }

        int longest = absW;
        int shortest = absH;
        if (longest <= shortest) {
            longest = absH;
            shortest = absW;
            if (h < 0) {
                dy2 = -1;
            } else if (h > 0) {
                dy2 = 1;
            }
            dx2 = 0;
        }

        int numerator = longest >> 1 + shortest;
        if (numerator >= longest) {
            x += dx1;
            y += dy1;
        } else {
            x += dx2;
            y += dy2;
        }

        return new Coord (x, y);
    }
    
}
