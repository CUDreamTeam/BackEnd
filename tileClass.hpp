#include <iostream>
#include <string>
#ifndef tileClass_hpp
#define tileClass_hpp
using namespace std;

class tile {
  private:
    int terrain;
    int tile_id;
    int xloc;
    int yloc;
    int adjacentTiles[6];
    //implement tile height
    //3d coordinate system for tile ids

  public:
    double findAdj(tile *a);
    bool occupied;
    tile();
    bool isOccupied(int id);
    int returnTerrainType(int id);
    int thisId();
    int updateTileId(int id);
    void connectAdjacentTiles(int* id);
    int* returnAdjacentTiles(int id);
};
#endif
