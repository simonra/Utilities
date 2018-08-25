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
	import argparse
	parser = argparse.ArgumentParser()
	parser.add_argument(
		'-n',
		'--threshold',
		type=int,
		help='Number denoting the threshold at or above which you want to find a divisor of the other argument.')
	parser.add_argument(
		'-m',
		'--factor',
		type=int,
		help='Number you want to find a multiple of.')
	args = parser.parse_args()
	print(smallest_multiple_of_m_greater_than_or_equal_to_n(args.threshold,args.factor))
