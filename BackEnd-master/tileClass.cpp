#include <iostream>
#include <string>
#include "tileClass.hpp"
#include<cmath>

using namespace std;

tile::tile(){
  occupied = false;
  terrain = 0;
  tile_id = 0;
  for (int i=0; i<6; i++){
    adjacentTiles[i] = 0;
  }
}

int tile::thisId(){
  return this->tile_id;
}

bool tile::isOccupied(int id){
  if (id == this->tile_id && this->occupied == true){
    return true;
  }
  else return false;
}

int tile::returnTerrainType(int id){
  if (id == this->tile_id){
    return this->terrain;
  }
    return NULL;
}

int tile:: updateTileId(int id){
  this->tile_id = id;
  return this->tile_id;
}

void tile::connectAdjacentTiles(int* id){
  for (int i=0; i<6; i++){
    this->adjacentTiles[i] = id[i];
  }
}

int* tile::returnAdjacentTiles(int id){
  return this->adjacentTiles;
}
double tile::findAdj(tile*t){
   return sqrt(pow(t -> xloc - xloc,2)+pow(t->yloc-yloc, 2));
}
