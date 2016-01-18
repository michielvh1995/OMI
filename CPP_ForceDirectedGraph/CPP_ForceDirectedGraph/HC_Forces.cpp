#include "stdafx.h"
#include "HC_Forces.h"
#include <iostream>

# define M_PI           3.14159265358979323846 /* pi */

std::vector<Vertex> HC_Forces::calculate_forces(int lockedIndex, int verticesAmt, std::vector<Vertex> vertices, float aWeight, float rWeight, int iterations)
{
	std::vector<std::vector<float>> forcesDict = std::vector<std::vector<float>>(verticesAmt);
	std::vector<bool> visited = std::vector<bool>(verticesAmt);


	for (int j = 0; j < iterations; j++)
	{
		for (int i = 0; i < verticesAmt; i++)
		{
			forcesDict[i] = std::vector<float>(2);
			visited[i] = false;
		}

		// calculate the attractive forces
		for (int i = 0; i < verticesAmt; i++)
		{
			for (const auto& elem : vertices[i].connection_set)
			{
				if (visited[i])
					continue;

				std::vector<float> attractive_vector = attractive_force(vertices[i], vertices[elem]);

				forcesDict[i][0] += attractive_vector[0];
				forcesDict[i][1] += attractive_vector[1];

				visited[i] = true;
			}
		}

		// Calculate the repulsive forces
		for (int i = 0; i < verticesAmt; i++)
		{
			for (int elem = 0; elem < verticesAmt; elem++)
			{
				if (elem == i)
					continue;

				std::vector<float> repulsive_vector = repulsive_force(vertices[i], vertices[elem]);

				forcesDict[i][0] += repulsive_vector[0];
				forcesDict[i][1] += repulsive_vector[1];
			}

		}

		// Apply the forces
		for (int i = 0; i < verticesAmt; i++)
		{
			if (i == lockedIndex) continue;

			vertices[i].position_vector[0] += forcesDict[i][0];
			vertices[i].position_vector[1] += forcesDict[i][1];
		}
	}
	return vertices;
}

// The attractive force is Hooke's spring law
std::vector<float> HC_Forces::attractive_force(Vertex node1, Vertex node2)
{
	std::vector<float> r = std::vector<float>(2);
	r[0] = node1.position_vector[0] - node2.position_vector[0];
	r[1] = node1.position_vector[1] - node2.position_vector[1];

	float distance = sqrt(r[0] * r[0] + r[1] * r[1]);

	r[0] /= distance;
	r[1] /= distance;

	float dL = distance - 10;
	float k = 0.5;

	std::vector<float> output = std::vector<float>(2);
	output[0] = -r[0] * dL * k;
	output[1] = -r[1] * dL * k;

	return output;
}

// The repulsive force is Coulombs law of repulsively charged particles
std::vector<float> HC_Forces::repulsive_force(Vertex node1, Vertex node2)
{// The vector between the two vertices (basically the line connecting them)
	std::vector<float> r = std::vector<float>(2);
	r[0] = node1.position_vector[0] - node2.position_vector[0];
	r[1] = node1.position_vector[1] - node2.position_vector[1];

	float distance = sqrt(r[0] * r[0] + r[1] * r[1]);

	std::vector<float> output = std::vector<float>(2);
	output[0] = 0.1* r[0] / (distance*distance*distance);
	output[1] = 0.1* r[1] / (distance*distance*distance);

	// output[0] = r[0] / (4 * M_PI) * 1 / (distance * distance);
	// output[0] = r[1] / (4 * M_PI) * 1 / (distance * distance);

	return output;
}
