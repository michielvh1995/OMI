#include "stdafx.h"
#include "qualityChecker.h"
#include "Vertex.h"
#include <algorithm>
#include "edge.h"
#include <iostream>

int qualityChecker::get_edge_crossings(std::vector<Vertex>& vertices)
{
	int count = 0;

	int edge_count = get_total_connections(vertices);
	auto edge_vector = std::vector<edge>();

	auto edge_2D = std::vector<std::vector<edge>>(edge_count);

	for (int i = 0; i < edge_count; i++)
	{
		edge edge1;
		edge1.pos1 = { vertices[i].position_vector[0], vertices[i].position_vector[1] };

		for (const auto& c : vertices[i].connection_set)
		{
			edge1.pos2 = { vertices[c].position_vector[0], vertices[c].position_vector[1] };

		}
	}

	for (int i = 0; i < vertices.size(); i++)
		for (const auto& c : vertices[i].connection_set)
		{
			edge tmp;
			tmp.pos1 = { vertices[i].position_vector[0], vertices[i].position_vector[1] };
			tmp.pos2 = { vertices[c].position_vector[0], vertices[c].position_vector[1] };
			edge_vector.push_back(tmp);
		}

	std::unordered_set<edge> uniques_set = std::unordered_set<edge>(edge_count);
	for (int i = 0; i < edge_vector.size(); i++)
		uniques_set.insert(edge_vector[i]);

	for (const auto& edge1 : uniques_set)
		for (const auto& edge2 : uniques_set)
			count += check_cross(edge1, edge2);


	return count;
}

// https://www.topcoder.com/community/data-science/data-science-tutorials/geometry-concepts-line-intersection-and-its-applications/
bool qualityChecker::check_cross(edge edge1, edge edge2)
{
	float a1 = edge1.pos2.y - edge1.pos1.y;
	float b1 = edge1.pos2.x - edge1.pos1.x;
	float a2 = edge2.pos2.y - edge2.pos1.y;
	float b2 = edge2.pos2.x - edge2.pos1.x;

	if (edge1 == edge2)
		return 0;

	float d = (a1*b2 - a2*b1);

	if (!d)
		return 0;

	float c1 = a1 *  edge1.pos1.x + b1 * edge1.pos1.y;
	float c2 = a2 *  edge2.pos1.x + b2 * edge2.pos1.y;

	// float t1 = ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34) / denominator;
	float x = (b2*c1 - b1*c2) / d;
	// float t2 = ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
	float y = (a1*c2 - a2*c1) / d;

	return x > std::min(edge1.pos1.x, edge2.pos1.x) && x < std::max(edge1.pos1.x, edge2.pos1.x)
		&& y > std::min(edge1.pos1.y, edge2.pos1.y) && y < std::max(edge1.pos1.y, edge2.pos1.y);
}



int qualityChecker::get_total_connections(std::vector<Vertex>& vertices)
{
	int sum = 0;
	for (int i = 0; i < vertices.size(); ++i)
	{
		sum += vertices[i].connection_set.size();
	}
	return sum / 2;
}


float qualityChecker::std_dev(std::vector<float>& values)
{
	int l = values.size();
	float avg = 0;
	for (int i = 0; i < l; i++)
		avg += values[i];
	avg /= l;

	float sum = 0;

	for (int i = 0; i < l; i++)
		sum += (values[i] - avg)*(values[i] - avg);

	sum /= (l - 1);
	return sum;
}

float qualityChecker::edge_length_deviation(std::vector<Vertex>& vertices)
{
	return 1;
}
