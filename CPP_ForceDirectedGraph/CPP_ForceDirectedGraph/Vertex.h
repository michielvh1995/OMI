#pragma once
#include <vector>
#include <unordered_map>
#include <unordered_set>

struct Vertex
{
	std::vector<float> position_vector;
	std::unordered_set<int> connection_set;
	int id;
};
