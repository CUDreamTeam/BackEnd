#ifndef unit_hpp
#define unit_hpp
#include <vector>
#include <stdio.h>
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
    void attackStructure(structure*a);
    int health;
    //location;
    int attack;
    bool alive;
    player * parent;
    unit(int id, int a, int b, player *f);
};
class structure{
public:
    int health;
    //location;
    bool alive;
    player * parent;
    structure(int health, player *a);
};





#endif /* unit_hpp */
