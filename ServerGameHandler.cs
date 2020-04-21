using System.Collections;
using System.Collections.Generic;

public class ServerGameHandler 
{
    [SerializeField] private HexGrid hexGrid = null;

    [SerializeField] private int cellCountX = 200;
    [SerializeField] private int cellCountZ = 200;

    HexMapGenerator hexMapGenerator = null;

    Dictionary<int, Player> connectedPlayers = null;

    public ServerHandler serverHandler = null;

    public void StartGame(Dictionary<int, Player> players)
    {
        connectedPlayers = players;
        GenerateMap(players.Values.Count);
    }

    HashSet<HexCell> good = new HashSet<HexCell>();
    HashSet<HexCell> bad = new HashSet<HexCell>();
    private void GenerateMap(int numPlayers)
    {
        hexGrid = Instantiate(hexGrid.gameObject, Vector3.zero, Quaternion.identity).GetComponent<HexGrid>();
        hexMapGenerator = hexGrid.GetComponent<HexMapGenerator>();
        HexMapCamera.instance.grid = hexGrid;

        //Debug.Log("Creating map: " + cellCountX + " by " + cellCountZ);
        List<HexCell> possibleSpawnLocs = new List<HexCell>();
        hexMapGenerator.GenerateMap(cellCountX, cellCountZ, false);

        Debug.Log("Created map of size " + cellCountX + " by " + cellCountZ + " with a seed of " + hexMapGenerator.seed);
        serverHandler.BroadcastHexMapData(cellCountX, cellCountZ, hexMapGenerator.seed);

        HexCell[] cells = hexMapGenerator.grid.GetCells();
        foreach (HexCell cell in cells)
        {
            if (!cell.IsUnderwater && !cell.HasRiver && (good.Contains(cell) || GetNumberOfConnectedNodes(cell) >= 19))
            {
                cell.EnableHighlight(Color.black);
                possibleSpawnLocs.Add(cell);
            }
            else
            {
                cell.TerrainTypeIndex = 0;
            }
        }

        float tempTime = Time.realtimeSinceStartup;

        foreach (Player p in connectedPlayers.Values)
        {
            int rand = Random.Range(0, possibleSpawnLocs.Count - 1);
            possibleSpawnLocs[rand].EnableHighlight(Color.red);
            possibleSpawnLocs[rand].Object = Instantiate(GamePrefabs.instance.buildingPrefabs[0]);

            HexCell current = possibleSpawnLocs[rand];

            possibleSpawnLocs.Remove(current);
            List<HexCell> nearby = GetNearbyCells(current, 4);
            List<HexCell> temp = new List<HexCell>();
            foreach (HexCell cell in nearby)
            {
                if (cell != null)
                {
                    if (possibleSpawnLocs.Contains(cell)) possibleSpawnLocs.Remove(cell);
                }
            }

            nearby = GetNearbyCells(current, 2);
            foreach (HexCell cell in nearby)
            {
                if (cell != null)
                {
                    if (!cell.IsUnderwater && good.Contains(cell))
                    {
                        cell.EnableHighlight(Color.blue);
                        temp.Add(cell);
                    }
                }
            }

            HexUnit unit = Instantiate(GamePrefabs.instance.unitPrefabs[0]).GetComponent<HexUnit>();
            unit.Grid = hexGrid;
            unit.Location = temp[Random.Range(0, temp.Count - 1)];
            serverHandler.SendPlayerStartPosition(p.PlayerCode, current.coordinates);
            Debug.Log("Player position: " + current.coordinates);
            serverHandler.SendNewUnit(p.PlayerCode, unit.Location.coordinates, 0);

            //if (i == 0)
            //{
            //    //Debug.Log(current.Position);
            //    HexMapCamera.instance.transform.position = current.Position;
            //}
        }

        foreach (HexCell cell in hexGrid.GetCells())
        {
            cell.IncreaseVisibility();
        }
    }

    int GetNumberOfConnectedNodes(HexCell cell)
    {
        List<HexCell> openSet = new List<HexCell>();
        HashSet<HexCell> closedSet = new HashSet<HexCell>();

        int connected = 0;

        openSet.Add(cell);

        while (openSet.Count > 0)
        {
            HexCell currentNode = openSet[0];
            openSet.RemoveAt(0);
            closedSet.Add(currentNode);

            connected++;

            HexCell[] neighbors = currentNode.GetNeighbors();
            foreach (HexCell n in neighbors)
            {
                if (n != null && n.IsUnderwater == false && !closedSet.Contains(n) && !openSet.Contains(n))
                {
                    openSet.Add(n);
                }
            }
        }
        if (connected >= 19) foreach (HexCell c in closedSet) good.Add(c);
        return connected;
    }

    List<HexCell> GetNearbyCells(HexCell cell, int range)
    {
        //        Debug.Log("Starting cell: " + cell.coordinates);
        List<HexCell> cells = new List<HexCell>();

        HexCoordinates coords = cell.coordinates;

        for (int z = 0; z <= range; z++)
        {
            for (int x = coords.X - range; x <= coords.X + range - z; x++)
            {
                if (z == 0) cells.Add(hexGrid.GetCell(new HexCoordinates(x, coords.Z)));
                else
                {
                    cells.Add(hexGrid.GetCell(new HexCoordinates(x, coords.Z + z)));
                    cells.Add(hexGrid.GetCell(new HexCoordinates(x + z, coords.Z - z)));
                }
            }
        }

        cells.Remove(cell);
        return cells;
    }
}
