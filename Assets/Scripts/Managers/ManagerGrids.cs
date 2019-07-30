using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IG.General;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace IG.CGrid {
    public class ManagerGrids : SingletonManager<ManagerGrids> {
        // --- Visual Representation
        //public CellLogic CellPrefab;
        // Grids - in game
        [Space]
        public GridVisuals GridOriginalVisuals;
        public GridVisuals GridMainVisuals;
        public GridVisuals GridPaletteVisuals;
        // --- Visuals end

        [Header("Difficulty")]
        [SerializeField] private Vector2Int MinMaxCellsChanged;
        [SerializeField] private int TilesToRandomize;

        // Matrices - Starting Data
        [Space]
        public int TotalTiles;

        public Vector2Int MatrixLength {
            get { return ManagerCGridGame.I.LevelData.MatrixLength; }
        }

        [TableMatrix(HorizontalTitle = "Loaded Cell Data", SquareCells = true)]
        public CellData[,] MatrixOriginalCellData;
        [Space]
        [TableMatrix(HorizontalTitle = "Randomized Cell Data", SquareCells = true)]
        public CellData[,] MatrixMainCellData;


        //[SerializeField] private GameObject GridOriginalParentObject;
        [TableMatrix(HorizontalTitle = "GridOriginal", SquareCells = true)]
        public CellLogic[,] GridOriginal; // = new CellLogic[DefaultMatrixLength.x, DefaultMatrixLength.y];
        [TableMatrix(HorizontalTitle = "GridMain", SquareCells = true)]
        public CellLogic[,] GridMain;  // = new CellLogic[DefaultMatrixLength.x, DefaultMatrixLength.y];
        public List<CellLogic> GridPalette = new List<CellLogic>(); 

        // Debugging
        //public List<CellLogic> GridMainLogicLowestRow = new List<CellLogic>(); // Only for debugging
        //public List<CellLogic> GridMainLogicFirstColumn = new List<CellLogic>(); // Only for debugging
        //public CellData[] GridLowestRowArray; // Visuals. Only for debugging


        //protected override void Awake() {
        //    base.Awake();
        //}
        private void Start() {
            if(GridOriginalVisuals == null || GridMainVisuals == null || GridPaletteVisuals == null)
                Debug.LogError("Set Grid objects (GridVisuals variables)", this);

            MatrixMainCellData = new CellData[MatrixLength.x, MatrixLength.y];
            MatrixOriginalCellData = new CellData[MatrixLength.x, MatrixLength.y];
        }

        /// <summary>
        /// Active CellBehaviour from ManagerCGridGame, defining CellLogic behaviour, determined by Game Mode
        /// </summary>
        /// <param name="cellBehaviour"></param>
        public void StartGame(CellBehaviour cellBehaviour) {
            SetDifficulty();

            // --- Original grid
            GridOriginal = CreateGrid(GridOriginalVisuals.CellPrefab, MatrixLength.x, MatrixLength.y, GridOriginalVisuals.Distance,
                GridOriginalVisuals.transform, GridOriginalVisuals.HeightDiff, GridOriginalVisuals.WidthDiff);

            CreateMatrixOriginalCellData(ref MatrixOriginalCellData);

            // Randomization: GridMain gets "broken" CellData
            if (ManagerCGridGame.I.FixedDataCellsImage) {
                MatrixOriginalCellData = RandomizeMatrixCellData(MatrixOriginalCellData, TilesToRandomize);
            }
            // TODO: Make game able to use saved images
            else {
                MatrixOriginalCellData = RandomizeMatrixCellData(MatrixOriginalCellData, TilesToRandomize);
            }
            
            SetupNewGrid(GridOriginal, MatrixOriginalCellData);


            // --- Grid for players to interact with
            GridMain = CreateGrid(GridMainVisuals.CellPrefab, MatrixLength.x, MatrixLength.y, GridMainVisuals.Distance,
                GridMainVisuals.transform, GridMainVisuals.HeightDiff, GridMainVisuals.WidthDiff);

            // Cloning MatrixOriginalCellData
            for (int i = 0; i < MatrixLength.x; i++) {
                for (int j = 0; j < MatrixLength.y; j++) {
                    MatrixMainCellData[i, j] = CellData.CreateInstance<CellData>();

                    MatrixMainCellData[i, j] = (CellData) MatrixOriginalCellData[i, j].Clone();
                }
            }

            // Randomization: GridMain gets "broken" CellData
            MatrixMainCellData = RandomizeMatrixCellData(MatrixMainCellData, TilesToRandomize);

            SetupNewGrid(GridMain, MatrixMainCellData, cellBehaviour);


            // --- Palette
            //GridPalette = GridPaletteVisuals.GetComponentsInChildren<CellLogic>().ToList();
            //ManagerAuVisuals.I.UpdateGridPalette();

            TotalTiles = MatrixLength.x*MatrixLength.y;

            // For debugging
            //GridLowestRowArray = new CellData[MatrixRandomizedCellData.GetLength(1)];
            //for (int j = 0; j < MatrixRandomizedCellData.GetLength(1); j++)
            //{
            //    GridLowestRowArray[j] = MatrixRandomizedCellData[0, j];
            //    GridMainLogicLowestRow.Add(GridMain[0, j]);
            //}
            //for (int i = 0; i < MatrixRandomizedCellData.GetLength(0); i++)
            //{
            //    GridMainLogicFirstColumn.Add(GridMain[i, 0]);
            //}
        }

        /// <summary>
        /// Randimizing DATA
        /// </summary>
        [ContextMenu("CreateMatrixOriginalCellData")]
        public void CreateMatrixOriginalCellData(ref CellData[,] matrixCellData) {
            for (int i = 0; i < MatrixLength.x; i++) {
                for (int j = 0; j < MatrixLength.y; j++) {
                    matrixCellData[i, j] = CellData.CreateInstance<CellData>();
                    // Data should now it's initial state state
                    matrixCellData[i, j].Construct(i, j);   //, ManagerCGridGame.I.EnergyMaxInCells
                }
            }
        }

        /// <summary>
        /// Randimizing DATA
        /// </summary>
        [ContextMenu("RandomiseMatrixCellData")]
        public CellData[,] RandomizeMatrixCellData(CellData[,] matrixCellData, int tilesToRandomize) {
            CellData[,] randomMatrixCellData = matrixCellData;
            int RandomNum;
            int tilesLeft = tilesToRandomize;

            int x = randomMatrixCellData.GetLength(0);
            int y = randomMatrixCellData.GetLength(1);

            for (int i = 0; i < x; i++) { //0
                for (int j = 0; j < y; j++) { //1
                    RandomNum = Random.Range(0, 2);

                    if (RandomNum == 1 && GridOriginal[i, j] != null) {
                        randomMatrixCellData[i, j].RandomizeState();

                        tilesLeft--;
                    }
                }
            }
            if (tilesLeft > 0) {
                RandomizeMatrixCellData(matrixCellData, tilesLeft);
            }
            return randomMatrixCellData;
        }

        /// <summary>
        /// Load Difficulty Data
        /// </summary>
        public void SetDifficulty() {
            // 
            TilesToRandomize = Random.Range(MinMaxCellsChanged.x, MinMaxCellsChanged.y);
        }

        public void Clear() {
            if (GridOriginal != null)
                DestroyGrid(GridOriginal);

            if (GridMain != null)
                DestroyGrid(GridMain);
        }

        public void DestroyGrid(CellLogic[,] grid) {
            for (int counter1 = 0; counter1 < grid.GetLength(0); counter1++) {
                for (int counter2 = 0; counter2 < grid.GetLength(1); counter2++) {
                    grid[counter1, counter2].Clear();

                    if (grid[counter1, counter2] != null)
                        Destroy(grid[counter1, counter2].gameObject);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetupNewGrid(CellLogic[,] grid, CellData[,] matrixCellData, CellBehaviour cellBehaviour = null) {
            for (int i = 0; i < MatrixLength.x; i++) {
                for (int j = 0; j < MatrixLength.y; j++) {
                    grid[i, j].InstallCellData(matrixCellData[i, j]);
                    grid[i, j].CreateNeisghborList(grid);

                    if (cellBehaviour != null)
                        grid[i, j].CellBehaviour = cellBehaviour;
                }
            }
        }

        public void ResetGrid(CellLogic[,] grid) {
            for (int counter1 = 0; counter1 < MatrixLength.x; counter1++) {
                for (int counter2 = 0; counter2 < MatrixLength.y; counter2++) {
                    grid[counter1, counter2]?.Clear();
                }
            }
        }

        /// <summary>
        /// функция создания 2D массива на основе шаблона
        /// </summary>
        /// <typeparam name="T">Префаб с каким-то классом. Что за класс - определяется при вызове</typeparam>
        /// <returns></returns>
        private T[,] CreateGrid<T>(T sample, int height, int width, float size, Transform parent, float heightDiff = 0f, float widthDiff = 0f)
            where T : Object {
            T[,] field = new T[height, width];

            float posX = -size*width/2f - size/2f;
            float posY = size*height/2f - size/2f;

            posX += widthDiff;
            posY += heightDiff;

            float Xreset = posX; // Изначальные координаты ячейки по X, для возврата к ним на следующей строке

            int z = 0; // Счётчик итераций, сейчас - только для имени

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    posX += size;
                    field[y, x] = Instantiate(sample, new Vector3(posX, posY, 0), Quaternion.identity, parent) as T;
                    field[y, x].name = "Cell-" + z;
                    z++;
                }
                posY -= size;
                posX = Xreset;
            }

            return field;
        }

    }
}