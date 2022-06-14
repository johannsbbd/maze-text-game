using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace maze_text_game {

    public class Map {
        private BlockType [,] blocks;

        public Size MapSize { get; private set; }

        private List<Point> startPoints;
        private Point flag = new Point();
        private int revealRadius;

        private int initialDistanceToFlag;

        private bool[,] fogMap;

        public Map(int height, int width, int revealRadius = 2, string GameSeed = null) {
            int? CurrentSeed = GameSeed?.GetHashCode();
            this.revealRadius = revealRadius;
            this.MapSize = new Size(width, height);
            this.blocks = new BlockType[height, width];
            this.fogMap = new bool[height, width];
            generateMap(CurrentSeed);

            int [,] distanceMatrix = new int[this.blocks.GetLength(0),this.blocks.GetLength(1)];

            getDistanceMatrix(ref distanceMatrix);

            this.startPoints = getStartPoints(ref distanceMatrix);

            foreach (Point p in this.startPoints)
            {
                this.blocks[p.x,p.y] = BlockType.startPoint;
            }

        }

        public BlockType[,] getMap() {
            BlockType[,] outMap = this.blocks;
            foreach (Point p in this.startPoints)
            {
               outMap[p.x,p.y] = BlockType.startPoint;
            }
            return outMap;
        }
        public List<Point> getStartPoints(){
            return this.startPoints;
        }
        public bool[,] getFogMap(){
            return this.fogMap;
        }
        public int getInitialDistanceToFlag(){
            return this.initialDistanceToFlag;
        }

        private void generateMap(int? CurrentSeed) {
            Random rand = CurrentSeed == null ? new Random() : new Random(CurrentSeed ?? 0);
            for (int k = 0; k < this.blocks.GetLength(0); k++) {
                for (int l = 0; l < this.blocks.GetLength(1); l++) {
                    this.blocks[k, l] = BlockType.wall;
                }
            }
            bool [,] visited = new bool[this.blocks.GetLength(0), this.blocks.GetLength(1)];
            List<Point> choices = new List<Point>();

            int x = (int) rand.NextDouble()*this.blocks.GetLength(0);
            int y = (int) rand.NextDouble()*this.blocks.GetLength(1);

            this.flag.x = x;
            this.flag.y = y;

            visited[this.flag.x,this.flag.y] = true;
            this.blocks[this.flag.x,this.flag.y] = BlockType.flag;
            evalNeighbourhood(this.flag, ref choices, ref visited);
            
            while(choices.Count != 0) {
                int index = (int) (rand.NextDouble() *choices.Count);
                Point p = choices[index];
                visited[p.x,p.y] = true;
                if (reviewPoint(p)) {
                    this.blocks[p.x,p.y] = BlockType.floor;
                    evalNeighbourhood(p, ref choices, ref visited);
                }
                choices.RemoveAt(index);
            }
        }

        private void evalNeighbourhood(Point p, ref List<Point> choices, ref bool[,] visited) {
            
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    if (Math.Abs(i) == Math.Abs(j)) {
                        continue;
                    }
                    Point pn = new Point(p.x+i, p.y+j);

                    if (pn.x < 0 || pn.x >= this.blocks.GetLength(0) || pn.y < 0 || pn.y >= this.blocks.GetLength(1))
                    {
                        continue;
                    }
                    if (!choices.Contains(pn) && !visited[pn.x, pn.y]) {
                        choices.Add(pn);
                    }
                }
            }

        }
    
        private bool reviewPoint(Point p) {
            int count = 0;
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    if (Math.Abs(i) == Math.Abs(j)) {
                        continue;
                    }
                    int xn = p.x+i;
                    xn = Math.Max(xn, 0);
                    xn = Math.Min(xn, this.blocks.GetLength(0) -1);
                    int yn = p.y+j;
                    yn = Math.Max(yn, 0);
                    yn = Math.Min(yn, this.blocks.GetLength(1) -1);
                    if (this.blocks[xn, yn] == BlockType.floor || this.blocks[xn, yn] == BlockType.flag) {
                        if (count == 1) {
                            return false;
                        }
                        count += 1;
                    }
                }
            }
            return true;
        }
    
        private List<Point> getStartPoints(ref int[,] distanceMatrix) {
            int maxVal = 0;

            for (int i = 0; i < distanceMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < distanceMatrix.GetLength(1); j++)
                {
                    if (maxVal<distanceMatrix[i,j])
                    {
                        maxVal = distanceMatrix[i,j];
                    }
                }
            }

            this.initialDistanceToFlag = (int) (maxVal*0.65);

            List<Point> startPoints = new List<Point>();
            
            for (int i = 0; i < distanceMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < distanceMatrix.GetLength(1); j++)
                {
                    if (this.initialDistanceToFlag == distanceMatrix[i,j])
                    {
                        startPoints.Add(new Point(i,j));
                    }
                }
            }

            return startPoints;
        }

        private void getDistanceMatrix(ref int[,] distanceMatrix) {

            getDistanceToflag(this.flag, ref distanceMatrix);

        }

        private void getDistanceToflag(Point p, ref int[,] distanceMatrix) {

            List<Point> nextPoints = new List<Point>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (Math.Abs(i) == Math.Abs(j)) {
                        continue;
                    }
                    Point pn = new Point(p.x+i,p.y+j);

                    if (pn.x < 0 || pn.x >= this.blocks.GetLength(0) || pn.y < 0 || pn.y >= this.blocks.GetLength(1)) {
                        continue;
                    }

                    if (distanceMatrix[pn.x,pn.y] == 0 && this.blocks[pn.x,pn.y] == BlockType.floor) {
                        distanceMatrix[pn.x,pn.y] = distanceMatrix[p.x,p.y]+1;
                        nextPoints.Add(pn);
                    }
                }
            }
            while (nextPoints.Count != 0)
            {
                getDistanceToflag(nextPoints[0], ref distanceMatrix);
                nextPoints.RemoveAt(0);
            }
        }

        public void reveal(Point loc) {

            for (int i = -revealRadius; i <= revealRadius; i++)
            {
                for (int j = -revealRadius; j <= revealRadius; j++)
                {
                    int px = loc.x+i;
                    px = Math.Min(px, this.blocks.GetLength(0)-1);
                    px = Math.Max(px, 0);
                    int py = loc.y+j;
                    py = Math.Min(py, this.blocks.GetLength(1)-1);
                    py = Math.Max(py, 0);

                    this.fogMap[px,py] = true;
                }
            }
        }


    }
}