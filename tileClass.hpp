#include <iostream>
#include <string>

using namespace std;

class tile {
  private:
    bool occupied;
    int terrain;
    int tile_id;
    int adjacentTiles[6];
    //implement tile height
    //3d coordinate system for tile ids

  public:
    tile();
    bool isOccupied(int id);
    int returnTerrainType(int id);
    int thisId();
    int updateTileId(int id);
    void connectAdjacentTiles(int* id);
    int* returnAdjacentTiles(int id);
};
