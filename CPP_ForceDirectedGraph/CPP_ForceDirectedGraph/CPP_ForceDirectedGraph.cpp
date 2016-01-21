// CPP_ForceDirectedGraph.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>
#include <vector>
#include "Vertex.h"
#include "HC_Forces.h"
#include "Eades_Forces.h"
#include "FR_Forces.h"
#include "qualityChecker.h"


#define V_Amount				20
#define LOCKED_INDEX			1

void coutVertices(std::vector<Vertex> vertices);
std::vector<Vertex> generte_vertices(int amount);

int _tmain(int argc, _TCHAR* argv[])
{
	auto vert = generte_vertices(V_Amount);

	std::vector<Vertex> out_vertices;

	char algo = 'h';

	if (!algo) return 0;

	if (algo == 'h')
		for (float aw = 0; aw < 1; aw += 0.1)
			for (float rw = 0; rw < 1; rw += 0.1)
				{
					out_vertices = HC_Forces::calculate_forces(LOCKED_INDEX, V_Amount, vert, aw, rw, 100);

					auto tested = qualityChecker::test_all(out_vertices);
					std::cout << aw << " " << rw;
					// Crossings/total edges ; edge length ; vertex dispersion
					std::cout << " " << tested[0] << " " << tested[1] << " " << tested[2] << "\n";
				}

	if (algo == 'e')
	for (float aw = 0.1; aw < 1; aw += 0.1)
		for (float rw = 0.1; rw < 1; rw += 0.1)
			for (float c = 0.1; c < 1; c += 0.1)
			{
				out_vertices = Eades_Forces::calculate_forces(LOCKED_INDEX, V_Amount, vert, aw, rw, 100, c);

				auto tested = qualityChecker::test_all(out_vertices);
				std::cout << aw << " " << rw << " " << c;
				// Crossings/total edges ; edge length ; vertex dispersion
				std::cout << " " << tested[0] << " " << tested[1] << " " << tested[2] << "\n";
			}

	if (algo == 'f')
	for (float aw = 0.1; aw < 1; aw += 0.1)
		for (float rw = 0.1; rw < 1; rw += 0.1)
			for (float c = 0.1; c < 1; c += 0.1)
			{
				out_vertices = FR_Forces::calc_forces(LOCKED_INDEX, V_Amount, vert, aw, rw, c, 100);

				auto tested = qualityChecker::test_all(out_vertices);
				std::cout << aw << " " << rw << " " << c;
				// Crossings/total edges ; edge length ; vertex dispersion
				std::cout << " " << tested[0] <<  " " << tested[1] <<  " " << tested[2] << "\n";
			}

	return 0;
}

void coutVertices(std::vector<Vertex> vertices)
{
	for (const auto& elem : vertices)
	{
		std::cout << elem.id << ": " << elem.position_vector[0] << ";" << elem.position_vector[1] << "\n";
		for (const auto& con : elem.connection_set)
			std::cout << " c: " << con;
		std::cout << "\n";
	}

}

std::vector<Vertex> generte_vertices(int amount)
{
	std::vector<Vertex> vertices = std::vector<Vertex>(amount);
	for (int i = 0; i < amount; i++)
	{
		Vertex random_vertex;

		random_vertex.id = i;

		random_vertex.position_vector = std::vector<float>(2);

		random_vertex.position_vector[0] = (rand() % 300) / 10;
		random_vertex.position_vector[1] = (rand() % 300) / 10;

		int maxconnections = 1 + rand() % amount / 10;
		for (int j = 0; j < maxconnections; j++)
		{
			auto add = rand() % amount;
			if (add != i)
				random_vertex.connection_set.insert(add);

		}

		vertices[i] = random_vertex;
	}

	for (int i = 0; i < amount; i++)
	{
		for (const auto& elem : vertices[i].connection_set)
		{
			vertices[elem].connection_set.insert(i);
		}
	}

	bool worker = true;
	// testing graph correctness:
	for (int i = 0; i < amount; i++)
	{

		for (const auto& elem : vertices[i].connection_set)
		{
			if (!worker)
				break;
			worker = vertices[elem].connection_set.count(i);
		}
		if (!worker)
			break;

	}

	if (worker)
		std::cout << "Correct \n";


	return vertices;
}

