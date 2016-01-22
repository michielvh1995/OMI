#include "stdafx.h"
#include <iostream>
#include "Vertex.h"
#include "FR_Forces.h"
#include <algorithm>

# define M_PI           3.14159265358979323846 /* pi */
# define Begin_Temp		8

std::vector<Vertex> FR_Forces::calc_forces(int lockedIndex, int verticesAmt, std::vector<Vertex> vertices, float aWeight, float rWeight, float c, int iterations)
{
	int area = 30 * 30;
	float k = c * sqrt(area / verticesAmt);

	float temperature = Begin_Temp;

	auto forcesDict = std::vector<std::vector<float>>(verticesAmt);

	for (int i = 0; i < iterations; i++)
	{
		// Calculate Repulsive Forces
		for (int v = 0; v < verticesAmt; v++)
		{
			forcesDict[v] = std::vector<float>(2);

			for (int u = 0; u < verticesAmt; u++)
			{
				if (u == v) continue;

				std::vector<float> delta = std::vector<float>(2);

				delta[0] = vertices[v].position_vector[0] - vertices[u].position_vector[0];
				delta[1] = vertices[v].position_vector[1] - vertices[u].position_vector[1];

				float distance = sqrt(delta[0] * delta[0] + delta[1] * delta[1]);

				if (distance == 0)
					distance = 1;

				float force = repulsive_force(distance, k);

				forcesDict[v][0] += (delta[0] / distance) * force * rWeight;
				forcesDict[v][1] += (delta[1] / distance) * force * rWeight;
			}
		}

		// Calculate Attractive Forces
		for (int v = 0; v < verticesAmt; v++)
		{
			for (const auto& u : vertices[v].connection_set) {

				if (u == v) continue;

				std::vector<float> delta = std::vector<float>(2);

				delta[0] = vertices[v].position_vector[0] - vertices[u].position_vector[0];
				delta[1] = vertices[v].position_vector[1] - vertices[u].position_vector[1];

				float distance = sqrt(delta[0] * delta[0] + delta[1] * delta[1]);
				if (distance == 0)
					distance = 1;

				float force = attractive_force(distance, k);

				forcesDict[v][0] += (delta[0] / distance) * force * aWeight;
				forcesDict[v][1] += (delta[1] / distance) * force * aWeight;
			}
		}

		// Apply the forces, while the maximum change is t
		for (int v = 0; v < verticesAmt; v++)
		{
			if (v == lockedIndex) continue;

			float distance = sqrt(forcesDict[v][0] * forcesDict[v][0] + forcesDict[v][1] * forcesDict[v][1]);
			float displacement = std::min(distance, temperature);

			vertices[v].position_vector[0] += forcesDict[v][0] / distance * displacement;
			vertices[v].position_vector[1] += forcesDict[v][1] / distance * displacement;
		}

		temperature = cool(temperature, i, iterations);
	}
	return vertices;
}

float FR_Forces::attractive_force(float x, float k)
{
	return x*x / k;
}

float FR_Forces::repulsive_force(float x, float k)
{
	return k*k / x;
}

float FR_Forces::cool(float old_temp, float iteration, float max_iterations)
{
	return pow(Begin_Temp, 1 - iteration / max_iterations);
}
