#merge-chunks-to-file.py
import sys
import os
import math
import shutil

assert len(sys.argv) > 1, "No path to process specified in arguments."
directory_with_chunks = sys.argv[1]
assert os.path.exists(directory_with_chunks), "Could not find anything at " + directory_with_chunks

destination_path = sys.argv[2]


# import glob

files = [f for f in os.listdir(directory_with_chunks)]

with open(destination_path, 'wb') as outfile:
	for filename in files:
		# if filename == outfilename:
		#     # don't want to copy the output into the output
		#     continue
		path_to_current_chunk = os.path.abspath(os.path.join(directory_with_chunks, filename))
		with open(path_to_current_chunk, 'rb') as readfile:
			shutil.copyfileobj(readfile, outfile)


# # Figure out if we are simply continuing from where we left of
# files = [f for f in os.listdir(directory_with_chunks)]
# # pieces_ending_in_number = [f for f in os.listdir(directory_with_chunks) if str.split(f,".")[-1].isdigit() ]
# print(files)
# if(len(files) > 0):
# 	with open(destination_path,'a+') as destination_file:
# 		for chunk in files:
# 			# print(named_chunk)
# 			# print(int(str.split(named_chunk,".")[-1]))
# 			path_to_current_chunk = os.path.abspath(os.path.join(directory_with_chunks, chunk))
# 			# print(path_to_current_chunk)
# 			with open(path_to_current_chunk,'rb') as current_chunk:
# 				destination_file.write(current_chunk)








	# print('other list:')
	# for named_chunk in pieces_ending_in_number:
	# 	print(named_chunk)
	# 	chunk_number = int(str.split(named_chunk,".")[-1])
	# 	# print('chunk_number')
	# 	# print(chunk_number)
	# 	if(chunk_number > starting_block):
	# 		starting_block = chunk_number
	# if(starting_block == number_of_blocks - 1):
	# 	if(len(pieces) >= number_of_blocks):
	# 		print('Larges part seems to be present in destination, and destination seems to contain all parts. Aborting.')
	# 		raise SystemExit
	# 	else:
	# 		print('Largest chunk seems to already be present. Starting over from scratch to be on the safe side.')
	# 		starting_block = 0
	# else:
		# Start not at last previously written block, but rather at the first missing block.
		# Safer to not enable if you suspect that the last run failed during the writing of the last block.
		# starting_block += 1
