#include "Eades_Forces.h"
#include <iostream>
#include "Vertex.h"

# define M_PI           3.14159265358979323846 /* pi */
# define K_e			8.9875517873681764E9 /* Coulombs Constant */

std::vector<Vertex> Eades_Forces::calculate_forces(int lockedIndex, int verticesAmt, std::vector<Vertex> vertices, float aWeight, float rWeight, int iterations, float k)
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

				std::vector<float> attractive_vector = attractive_force(vertices[i], vertices[elem], k);

				forcesDict[i][0] += aWeight * attractive_vector[0];
				forcesDict[i][1] += aWeight * attractive_vector[1];

				/*
				if (forcesDict[i][0] != forcesDict[i][0])
				std::cout << "err \n";
				*/
				//std::cout << "\n a:  " << attractive_vector[0] << ";" << attractive_vector[1];
				
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

				forcesDict[i][0] += rWeight*repulsive_vector[0];
				forcesDict[i][1] += rWeight*repulsive_vector[1];

				// std::cout << "\n r:  " << repulsive_vector[0] << ";" << repulsive_vector[1];
				// This is way to small
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

// The attractive force according to the eades algorithm
std::vector<float> Eades_Forces::attractive_force(Vertex node1, Vertex node2, float k)
{
	// Output graph
	std::vector<float> output = std::vector<float>(2);

	// The vector between two nodes
	std::vector<float> r = std::vector<float>(2);
	r[0] = node1.position_vector[0] - node2.position_vector[0];
	r[1] = node1.position_vector[1] - node2.position_vector[1];

	float distance = sqrt(r[0] * r[0] + r[1] * r[1]);

	r[0] /= distance;
	r[1] /= distance;

	float change = log10f(distance / k);

	output[0] = r[0] * change;
	output[1] = r[1] * change;

	return output;
}

// The repulsive force is Coulombs law of repulsively charged particles
// I think...
std::vector<float> Eades_Forces::repulsive_force(Vertex node1, Vertex node2)
{
	// The vector between the two vertices (basically the line connecting them)
	std::vector<float> r = std::vector<float>(2);
	r[0] = node1.position_vector[0] - node2.position_vector[0];
	r[1] = node1.position_vector[1] - node2.position_vector[1];

	float distance = sqrt(r[0] * r[0] + r[1] * r[1]);

	std::vector<float> output = std::vector<float>(2);
	output[0] = K_e * (r[0] / (distance*distance*distance));
	output[1] = K_e * (r[1] / (distance*distance*distance));

	if (distance == 0)
		output = { 1, 1 };

	// output[0] = r[0] / (4 * M_PI) * 1 / (distance * distance);
	// output[0] = r[1] / (4 * M_PI) * 1 / (distance * distance);

	return output;
}

