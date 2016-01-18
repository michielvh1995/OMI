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


#define V_Amount				15
#define LOCKED_INDEX			1

void coutVertices(std::vector<Vertex> vertices);
std::vector<Vertex> generte_vertices(int amount);

int _tmain(int argc, _TCHAR* argv[])
{
	std::vector<Vertex> vertices = std::vector<Vertex>(3);

	vertices[0].position_vector = { 10, 10 };
	vertices[1].position_vector = { 15, 12 };
	vertices[2].position_vector = { 14, 16 };

	vertices[0].connection_set.insert(1);
	vertices[1].connection_set.insert(0);
	vertices[1].connection_set.insert(2);
	vertices[2].connection_set.insert(1);

	auto vert = generte_vertices(V_Amount);

	// Hooke-Coulomb seems to be working fine.

	//										LOCKED_INDEX #vertices vertices aWeight, rWeight, iterations
	// auto out_vertices = HC_Forces::calculate_forces(LOCKED_INDEX, V_Amount, vert, 1, .5, 10);
	// 200: 121,145 || 1250: 120,143

	// Eades is still not 100% perfect.

	// auto out_vertices = Eades_Forces::calculate_forces(LOCKED_INDEX, V_Amount, vert, 1, 1, 100, 1);
	// 100: 943,-296 || 200: 1595, -1246
	// Begin: [0..300],[0..300]
	// log2f:
	// 0.2:  384, 241 || 0.6: 701, -17
	// log10f:
	// 0: 218, 282 || 0.2: 267, 282 || 0.6: 368, 249

	// Fruchterman Reingold seems to be working
	auto out_vertices = FR_Forces::calc_forces(LOCKED_INDEX, V_Amount, vert, 1, 0.3, 0.3, 100);

	std::cout << "\n";
	coutVertices(out_vertices);
	std::cout << "\n";
	std::cout << qualityChecker::get_edge_crossings(out_vertices);
	std::cout << "\n";
	std::cout << RAND_MAX;
	std::cout << "\n";



	int a;
	std::cin >> a;

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

		int maxconnections = 1 + rand() % amount / 4;
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

