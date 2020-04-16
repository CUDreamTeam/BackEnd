#include <iostream>
#include <stdlib.h>
#include "unit.hpp"
#include "tileClass.hpp"
using namespace std;

int a;
int b;
unit::unit(player *f,tile*t,string type){
    if(type == "Infantry"){
        attack = 20;
        health = 20;
        range = 1.5;
    }
    else if(type == "Archer"){
        attack = 15;
        health = 15;
        range = 3;
    }
    parent = f;
    alive = true;
    location = t;
    type = type;
    t -> occupied = true;
    f -> units.push_back(this);
}
structure::structure(int health, player *a,tile*t){
    health = health;
    parent = a;
    location = t;
    a -> structures.push_back(this);
}
player::player(string username){
    username = username;
}
bool unit::attackStructure(structure *a){
    if(this -> range >= this -> location ->findAdj(a -> location)){
        a -> health = a -> health - attack * (.1 * (rand() % 51 + 50));
        if(a -> health <=0){
            a -> alive = false;
            a -> location -> occupied = false;
        }
            return true;
    }
    else{
        return false;
    }
}
bool unit::attackUnit(unit *a){
    if(this -> range >= this -> location ->findAdj(a -> location)){
        a -> health = a -> health - attack * (.1 * (rand() % 51 + 50));
        if(a -> health <= 0){
            a -> alive = false;
            a -> location -> occupied = false;
        }
        return true;
    }
    else{
        return false;
    }
}
//int main(int argc, const char * argv[]){
//    int cool = 0;
//    player * d = new player("cool");
//    player * e = new player("moneyman");
//    unit * a = new unit(cool,20000,12,d);
//    d -> units.push_back(a);
//    cool++;
//    unit * b = new unit(cool,10000,2000,e);
//    d -> units.push_back(b);
//    cool++;
//    battle(a, b);
//}
//    if(a -> parent != b -> parent){
//        while((a -> health > 0) && (b -> health > 0)){
//            a -> health = a -> health - (b -> attack * (.1 * (rand() % 51 + 50)));
//            b -> health = b -> health - (a -> attack * (.1 * (rand() % 51 + 50)));
//            cout << b -> health << endl;
//        }
//        if((a -> health <= 0) && (b -> health <= 0)){
//            a -> alive = false;
//            b -> alive = false;
//            cout << "they both dead" << endl;
//        }
//        else if(a -> health <= 0){
//            a -> alive = false;
//            cout << "a is dead" << endl;
//        }
//        else if(b -> health <= 0){
//            b -> alive = false;
//            cout << "b is dead" << endl;
//        }
//        else{
//            cout << "something aint right" << endl;
//        }
//    }
//    else{
//        cout << "cannot battle" << endl;
//    }
