#ifndef unit_hpp
#define unit_hpp
#include <vector>
#include <stdio.h>
#include "tileClass.hpp"
using namespace std;
class unit;
class structure;

class player{
public:
    string username;
    int id;
    vector<unit*>units;
    vector<structure*>structures;
    player(string username);
    void addStructure(structure*s);
    void addUnit(unit*u);
};
class unit{
public:
    
    bool attackStructure(structure*a);
    bool attackUnit(unit*a);
    int health;
    int range;
    tile * location;
    
    int attack;
    bool alive;
    player * parent;
    unit(player *f,tile*t,string type);
};
class structure{
public:
    int health;
    tile * location;
    bool alive;
    player * parent;
    structure(int health, player *a,tile*t);
};





#endif /* unit_hpp */
