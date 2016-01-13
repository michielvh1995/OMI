#pragma once
#include <string>
#include <vector>
#include "stdafx.h"

struct Vertex;

class FR_Forces
{
public:
	static std::vector<Vertex> FR_Forces::calc_forces(int verticesAmt, std::vector<Vertex> vertices, float aWeight, float rWeight, float c, int iterations);
	static void PrintOutput(std::vector<std::string> to_print_vector);	// IO
	static float attractive_force(float x, float k);
	static float repulsive_force(float x, float k);
	static float cool(float old_temp, float  iteration, float  max_iterations);
};
