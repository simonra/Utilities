def smallest_multiple_of_m_greater_than_or_equal_to_n(n, m):
	remainder = n % m
	if(remainder == 0):
		return n
	difference = m - remainder
	return n + difference

# Naive approach:
	# result = n
	# while(result % m != 0):
	# 	result += 1
	# return result

# Recursive naive approach:
	# if(n % m == 0):
	# 	return n
	# return smallest_multiple_of_m_greater_than_or_equal_to_n(n+1, m)

if __name__ == "__main__":
	import sys
	n = int(sys.argv[1])
	m = int(sys.argv[2])
	print(smallest_multiple_of_m_greater_than_or_equal_to_n(n,m))
