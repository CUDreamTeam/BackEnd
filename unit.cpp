#include <iostream>
#include <stdlib.h>
#include "unit.hpp"
using namespace std;

int a;
int b;
unit::unit(int id, int a, int b, player *f){
    id = id;
    health = a;
    attack = b;
    parent = f;
    alive = true;
    f -> units.push_back(this);
}
structure::structure(int health, player *a){
    health = health;
    parent = a;
    a -> structures.push_back(this);
}
player::player(string username){
    username = username;
}
void unit::attackStructure(structure *a){
    a -> health = a -> health - attack * (.1 * (rand() % 51 + 50));
    if(a -> health <=0){
        a -> alive = false;
    }
}
void battle(unit *a, unit *b){
    if(a -> parent != b -> parent){
        while((a -> health > 0) && (b -> health > 0)){
            a -> health = a -> health - (b -> attack * (.1 * (rand() % 51 + 50)));
            b -> health = b -> health - (a -> attack * (.1 * (rand() % 51 + 50)));
            cout << b -> health << endl;
        }
        if((a -> health <= 0) && (b -> health <= 0)){
            a -> alive = false;
            b -> alive = false;
            cout << "they both dead" << endl;
        }
        else if(a -> health <= 0){
            a -> alive = false;
            cout << "a is dead" << endl;
        }
        else if(b -> health <= 0){
            b -> alive = false;
            cout << "b is dead" << endl;
        }
        else{
            cout << "something aint right" << endl;
        }
    }
    else{
        cout << "cannot battle" << endl;
    }
}
int main(int argc, const char * argv[]){
    int cool = 0;
    player * d = new player("cool");
    player * e = new player("moneyman");
    unit * a = new unit(cool,20000,12,d);
    d -> units.push_back(a);
    cool++;
    unit * b = new unit(cool,10000,2000,e);
    d -> units.push_back(b);
    cool++;
    battle(a, b);
}
