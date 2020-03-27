#include <iostream>
#include "tileClass.hpp"
#include <string>

using namespace std;


int main(){

  tile testTile;

  cout << testTile.thisId() << endl;

  if (testTile.isOccupied(0) == false) cout << "Empty" << endl;

  cout << testTile.returnTerrainType(0) << endl;

  cout << testTile.updateTileId(100) << endl;

  cout << testTile.thisId() << endl;

  testTile.updateTileId(0);

  cout << testTile.thisId() << endl;

  int newTiles[6] = {1,2,3,4,5,6};

  testTile.connectAdjacentTiles(newTiles);

  int* connectedTiles = testTile.returnAdjacentTiles(0);

  for (int i=0; i<6; i++){
    cout << connectedTiles[i] << endl;
  }

  return 0;

}
