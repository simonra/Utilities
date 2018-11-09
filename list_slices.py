import math

# 0 indexed:
get_nth_slice_of_size_m(n, m, list):
	# Incluse index:
	start = n * m
	# Exclusive index:
	end = start + m
	return list[start:end]

get_k_elements_stating_at_j(k, j):
	# return list[j:k]
	if(j % k == 0):
		slice_number = j / k
		return get_nth_slice_of_size_m(slice_number, k)
	# how_far_out_in_slice_is_start = j % k
	# required_slice_size = k + how_far_out_in_slice_is_start
	# import math
	# recquired_slice = math.floor(j / required_slice_size)
	# slightly_larger_slice = get_nth_slice_of_size_m(recquired_slice, required_slice_size)
	# resulting_slice = slightly_larger_slice[ how_far_out_in_slice_is_start : ]
	# # resulting_slice = slightly_larger_slice[ how_far_out_in_slice_is_start : (how_far_out_in_slice_is_start + k)]
	# return resulting_slice

	if(j % k == 0):
		slice_number = j / k
		return get_nth_slice_of_size_m(slice_number, k)
	m = k
	n = -1
	while(m < j + k):
		n = math.floor(j / m)
		# (n+1)*m should be the first entry in the next slice
		if(n * m <= j and (n+1)*m > j + k):
			break
		m++
	slice_to_look_in = get_nth_slice_of_size_m(n, m)
	# also works out if j < m, because then n = 0
	start_pos = j - (n * m)
	end_pos = start_pos + k
	return slice_to_look_in[start_pos:end_pos]
