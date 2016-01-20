#pragma once
#include <string>
#include <vector>

struct Vertex;

class Eades_Forces
{
public:
	static std::vector<Vertex> calculate_forces(int lockedIndex, int verticesAmt, std::vector<Vertex> vertices, float aWeight, float rWeight, int iterations, float k);
	static std::vector<float> attractive_force(Vertex node1, Vertex node2, float k);
	static std::vector<float> repulsive_force(Vertex node1, Vertex node2);
};
