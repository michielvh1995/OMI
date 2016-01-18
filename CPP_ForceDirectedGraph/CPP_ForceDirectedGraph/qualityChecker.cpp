#include "stdafx.h"
#include "qualityChecker.h"
#include "EdgeCrossingSet.h"
#include "Vertex.h"
#include <algorithm>
#include "edge.h"
#include <iostream>

int qualityChecker::get_edge_crossings(std::vector<Vertex>& vertices)
{
	EdgeCrossingSet crossingSet;
	crossingSet.start(std::vector<std::vector<edge>>());

	int edgeCrossings = 0;

	for (auto vert1 : vertices)
	{
		for (auto vert2id : vert1.connection_set)
		{
			auto vert2 = vertices[vert2id];

			edge edge1;
			edge1.pos1 = { vert1.position_vector[0], vert1.position_vector[1] };
			edge1.pos2 = { vert2.position_vector[0], vert2.position_vector[1] };

			for (auto otherEdgeVert1 : vertices)
			{
				for (auto otherEdgeVert2id : otherEdgeVert1.connection_set)
				{
					auto otherEdgeVert2 = vertices[otherEdgeVert2id];

					edge edge2;
					edge2.pos1 = { otherEdgeVert1.position_vector[0], otherEdgeVert1.position_vector[1] };
					edge2.pos2 = { otherEdgeVert2.position_vector[0], otherEdgeVert2.position_vector[1] };

					if (check_cross(edge1, edge2))
					{
						if (crossingSet.Add(edge1, edge2)) edgeCrossings++;
					}
				}
			}
		}
	}

	return edgeCrossings;
}

// https://www.topcoder.com/community/data-science/data-science-tutorials/geometry-concepts-line-intersection-and-its-applications/
bool qualityChecker::check_cross(edge edge1, edge edge2)
{
	point p1 = { edge1.pos1.x, edge1.pos1.y };
	point p2 = { edge1.pos2.x, edge1.pos2.y };
	point p3 = { edge2.pos1.x, edge2.pos1.y };
	point p4 = { edge2.pos2.x, edge2.pos2.y };

	if (p1 == p2 || p2 == p3 || p1 == p4 || p2 == p4 || p1 == p3 || p3 == p4)
		return false;

	// Get the segments' parameters.
	float dx12 = p2.x - p1.x;
	float dy12 = p2.y - p1.y;
	float dx34 = p4.x - p3.x;
	float dy34 = p4.y - p3.y;

	// Solve for t1 and t2
	float denominator = (dy12 * dx34 - dx12 * dy34);
	float t1 = ((p1.x - p3.x) * dy34 + (p3.y - p1.y) * dx34) / denominator;
	/*if (float.IsInfinity(t1))
	{
	return false;
	}*/

	float t2 = ((p3.x - p1.x) * dy12 + (p1.y - p3.y) * dx12) / -denominator;

	// The segments intersect if t1 and t2 are between 0 and 1.
	return ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));

	/* Deze code zorgt voor ander aantal edge crossings, volgens mij fout
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
	*/
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
