#pragma once
#include "Vertex.h"

class HC_Forces
{
public:
	static std::vector<Vertex> calculate_forces(int lockedIndex, int verticesAmt, std::vector<Vertex> vertices, float aWeight, float rWeight, int iterations);
	static std::vector<float> attractive_force(Vertex node1, Vertex node2);
	static std::vector<float> repulsive_force(Vertex node1, Vertex node2);
	static void PrintOutput(std::vector<std::string> to_print_vector);	// IO
};

