#pragma once

#include "stdafx.h"
#include <vector>
#include "Vertex.h"
#include <set>
#include "edge.h"

class EdgeCrossingSet
{
	std::vector<std::vector<edge>> crossingSet;
public:
	void start(std::vector<std::vector<edge>> crsSet);
	bool Add(edge edge1, edge edge2);
};