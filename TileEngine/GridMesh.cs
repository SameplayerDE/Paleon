using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Paleon
{
    public class GridMesh
    {
        private const int VERTICES_COUNT = 4;
        private const int INDICES_COUNT = 6;

        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }

        public int Columns { get; private set; }
        public int Rows { get; private set; }

        public int CellWidth { get; private set; }
        public int CellHeight { get; private set; }

        private VertexPositionTexture[] vertices;
        private short[] triangles;

        private MyMesh mesh;

        public GridMesh(int offsetX, int offsetY, int columns, int rows, int cellWidth, int cellHeight)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            Columns = columns;
            Rows = rows;
            CellWidth = cellWidth;
            CellHeight = cellHeight;

            GenerateMesh();
        }

        private void GenerateMesh()
        {
            mesh = new MyMesh();

            vertices = new VertexPositionTexture[Columns * Rows * VERTICES_COUNT];
            triangles = new short[Columns * Rows * INDICES_COUNT];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new VertexPositionTexture(Vector3.Zero, Vector2.Zero);
            }

            int indexCount = 0;
            short currIndex = 0;

            for (int gridX = OffsetX, x = 0; gridX < OffsetX + Columns * CellWidth; gridX += CellWidth, x++)
            {
                for (int gridY = OffsetY, y = 0; gridY < OffsetY + Rows * CellHeight; gridY += CellHeight, y++)
                {
                    int vertexCount = (y * Rows + x) * VERTICES_COUNT;

                    vertices[vertexCount].Position = new Vector3(gridX, gridY, 0);
                    vertices[vertexCount + 1].Position = new Vector3(gridX + CellWidth, gridY, 0);
                    vertices[vertexCount + 2].Position = new Vector3(gridX, gridY + CellHeight, 0);
                    vertices[vertexCount + 3].Position = new Vector3(gridX + CellWidth, gridY + CellHeight, 0);

                    // Triangles

                    short tmp = 0;
                    tmp += currIndex;
                    triangles[indexCount] = tmp;

                    tmp = 0;
                    tmp += currIndex;
                    tmp += 1;
                    triangles[indexCount + 1] = tmp;

                    tmp = 0;
                    tmp += currIndex;
                    tmp += 2;
                    triangles[indexCount + 2] = tmp;

                    tmp = 0;
                    tmp += currIndex;
                    tmp += 1;
                    triangles[indexCount + 3] = tmp;

                    tmp = 0;
                    tmp += currIndex;
                    tmp += 3;
                    triangles[indexCount + 4] = tmp;

                    tmp = 0;
                    tmp += currIndex;
                    tmp += 2;
                    triangles[indexCount + 5] = tmp;

                    currIndex += VERTICES_COUNT;
                    indexCount += INDICES_COUNT;
                }
            }

            mesh.Vertices = vertices;
            mesh.Triangles = triangles;
        }

        public void SetCellUV(int x, int y, Vector4 uv)
        {
            int vertexCount = (y * Rows + x) * VERTICES_COUNT;

            vertices[vertexCount].TextureCoordinate = new Vector2(uv.X, uv.Y);
            vertices[vertexCount + 1].TextureCoordinate = new Vector2(uv.Z, uv.Y);
            vertices[vertexCount + 2].TextureCoordinate = new Vector2(uv.X, uv.W);
            vertices[vertexCount + 3].TextureCoordinate = new Vector2(uv.Z, uv.W);
        }

        public void Render()
        {
            mesh.Render();
        }

        public void RefreshUVs()
        {
            mesh.Vertices = vertices;
        }

        public void RefreshTriangles()
        {
            mesh.Triangles = triangles;
        }

    }
}
