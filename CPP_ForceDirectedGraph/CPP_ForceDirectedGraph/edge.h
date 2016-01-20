#pragma once
#include <utility>

struct point
{
public:
	float x;
	float y;

	bool operator==(const point& r) const;
};

inline bool point::operator==(const point& r) const
{
	return x == r.x && y == r.y;
};


struct edge
{
public:
	point pos1;
	point pos2;

	float get_length() const;
	bool operator==(const edge& r) const;

};

inline float edge::get_length() const
{
	float dx = pos1.x - pos2.x;
	float dy = pos1.y - pos2.y;

	return sqrt(dx*dx + dy*dy);
}

inline bool edge::operator==(const edge& r) const
{
	return pos1 == r.pos1 || pos2 == r.pos2 || pos1 == r.pos2 || pos2 == r.pos1;
};

namespace std
{
	template <>
	struct hash<edge>
	{
		size_t operator()(const edge &k) const
		{
			using std::size_t;
			using std::hash;

			return hash<float>()(k.pos1.x + k.pos1.y + k.pos2.x + k.pos2.y);
		};
	};
};