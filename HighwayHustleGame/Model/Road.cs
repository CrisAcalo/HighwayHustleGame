using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighwayHustleGame.Model
{

    public class Road
    {
        private float speed;
        private int level;
        private Color roadColor;
        private int laneWidth;
        private Bitmap roadBitmap;
        private Graphics roadGraphics;
        private const int laneCount = 3;
        private const int segmentLength = 30; // Longitud de los segmentos de línea divisoria
        private const int segmentSpacing = 10; // Espaciado entre segmentos
        private float offsetY;

        private float incremento = 1;

        public float Speed { get => speed; set => speed = value; }
        public int Level { get => level; set => level = value; }
        public Color RoadColor { get => roadColor; set => roadColor = value; }
        public int LaneWidth { get => laneWidth; set => laneWidth = value; }
        public Bitmap RoadBitmap { get => roadBitmap; set => roadBitmap = value; }
        public Graphics RoadGraphics { get => roadGraphics; set => roadGraphics = value; }

        public static int LaneCount => laneCount;

        public static int SegmentLength => segmentLength;

        public static int SegmentSpacing => segmentSpacing;

        public float Incremento { get => incremento; set => incremento = value; }

        public Road(int level, float initialSpeed, int width, int height)
        {
            this.level = level;
            this.speed = initialSpeed;
            this.laneWidth = width / 3;
            this.roadBitmap = new Bitmap(width, height);
            this.roadGraphics = Graphics.FromImage(this.roadBitmap);
            UpdateRoadColor();
        }

        private void UpdateRoadColor()
        {
            // Cambia el color de la carretera según el nivel
            switch (level)
            {
                case 1:
                    roadColor = Color.FromArgb(90, 90, 90);
                    break;
                case 2:
                    roadColor = Color.FromArgb(148, 109, 97);
                    break;
                case 3:
                    roadColor = Color.FromArgb(161, 107, 90);
                    break;
                case 4:
                    roadColor = Color.FromArgb(178, 102, 78);
                    break;
                case 5:
                    roadColor = Color.FromArgb(200, 92, 58);
                    break;
                default:
                    roadColor = Color.Gray;
                    break;
            }
        }

        public void Update()
        {
            // Actualiza la carretera
            offsetY += speed * (float)incremento;
            if (offsetY >= roadBitmap.Height)
            {
                offsetY -= roadBitmap.Height;
            }
            DrawRoad();
        }

        public void IncreaseSpeed(float incremento)
        {
            this.incremento += incremento;
        }

        private void DrawRoad()
        {
            roadGraphics.Clear(Color.Black); // Fondo de la carretera
            using (SolidBrush brush = new SolidBrush(roadColor))
            {
                // Dibuja los carriles
                for (int i = 0; i < LaneCount; i++)
                {
                    int x = i * laneWidth;
                    roadGraphics.FillRectangle(brush, x, 0, laneWidth, roadBitmap.Height);
                }

                // Dibuja las líneas divisorias de los carriles con segmentos
                using (Pen pen = new Pen(Color.White, 2))
                {
                    for (int i = 1; i < LaneCount; i++)
                    {
                        int x = i * laneWidth;
                        DrawSegmentedLine(pen, x, -734, x, roadBitmap.Height);
                    }
                }
            }
        }

        private void DrawSegmentedLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            float length = (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            float segmentCount = length / (SegmentLength + SegmentSpacing);
            float dx = (x2 - x1) / length * SegmentLength;
            float dy = (y2 - y1) / length * SegmentLength;

            for (int i = 0; i < segmentCount; i++)
            {
                int startX = (int)(x1 + i * (dx + dx * SegmentSpacing / SegmentLength));
                int startY = (int)(y1 + i * (dy + dy * SegmentSpacing / SegmentLength));
                int endX = (int)(startX + dx);
                int endY = (int)(startY + dy);

                // Dibuja las líneas divisorias en la posición correcta considerando el desplazamiento
                roadGraphics.DrawLine(pen, startX, startY + offsetY, endX, endY + offsetY);
            }
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(roadBitmap, 0, 0);
        }

        public void ChangeLevel(int newLevel)
        {
            this.level = newLevel;
            UpdateRoadColor();
        }
    }

}
