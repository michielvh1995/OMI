#pragma once
#include <vector>

struct edge;
struct Vertex;

class qualityChecker
{
public:
	static int get_edge_crossings(std::vector<Vertex>& vertices);
	static int get_total_connections(std::vector<Vertex>& vertices);
	static std::vector<float> test_all(std::vector<Vertex>& vertices);
private:
	static bool check_cross(std::vector<float> edge1, std::vector<float> edge2);
	static bool qualityChecker::check_cross(edge edge1, edge edge2);
	static float std_dev(std::vector<float>& values);
	static float edge_length_deviation(std::vector<Vertex>& vertices);
	static int vertex_density(std::vector<Vertex>& vertices);
	
};

