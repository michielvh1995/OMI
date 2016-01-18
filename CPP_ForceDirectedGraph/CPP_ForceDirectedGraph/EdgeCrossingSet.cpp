#include "stdafx.h"
#include <iostream>
#include <vector>
#include "Vertex.h"
#include <set>
#include <algorithm>
#include "edge.h"
#include "EdgeCrossingSet.h"


void EdgeCrossingSet::start(std::vector<std::vector<edge>> crsSet)
{
	crossingSet = crsSet;
}

bool EdgeCrossingSet::Add(edge edge1, edge edge2)
{
	std::vector<edge> crossing;
	crossing.push_back(edge1);
	crossing.push_back(edge2);

	for (std::vector<edge> checkedCrossing : crossingSet) {
		if (checkedCrossing[0] == crossing[0] && checkedCrossing[1] == crossing[1] ||
			checkedCrossing[1] == crossing[0] && checkedCrossing[0] == crossing[1])
			// Don't add if crossing already exists in list
			return false;
	}
	crossingSet.push_back(crossing);
	return true;
}