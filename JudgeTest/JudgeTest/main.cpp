#include<fstream>
#include<iostream>
#include<string>
#include<random>
#include<filesystem>

int main(int argc, const char* argv[])
{
	int a, b, c;
	std::string str;
	if (argc == 1 || !std::filesystem::exists(argv[1]))
	{
		std::mt19937 random{ std::random_device()() };
		std::uniform_int_distribution<int> dist(1, 100);
		std::uniform_int_distribution<int> cdis(0, 26);
		a = dist(random);
		b = dist(random);
		c = dist(random);
		str = "01234567";
		for (auto& s : str)
		{
			s = 'a' + cdis(random);
		}
	}
	else
	{
		std::fstream fst{ argv[1] };
		fst >> a;
		fst >> b >> c;
		fst >> str;
	}
	std::cout << a << std::endl;
	std::cout << b << " " << c << std::endl;
	std::cout << str << std::endl;
	int ansi;
	std::string anss;
	std::cin >> ansi >> anss;
	if (ansi != a + b + c || anss != str)
	{
		std::terminate();
	}
}