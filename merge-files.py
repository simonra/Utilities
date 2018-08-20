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

# with open(destination_path, 'wb') as outfile:
# 	for filename in files:
# 		# if filename == outfilename:
# 		#     # don't want to copy the output into the output
# 		#     continue
# 		path_to_current_chunk = os.path.abspath(os.path.join(directory_with_chunks, filename))
# 		with open(path_to_current_chunk, 'rb') as readfile:
# 			shutil.copyfileobj(readfile, outfile)

# Alternative if can't keep open the connection to the out file for large ammounts of time but we have a lot to merge:
for filename in files:
	path_to_current_chunk = os.path.abspath(os.path.join(directory_with_chunks, filename))
	with open(path_to_current_chunk, 'rb') as readfile:
		with open(destination_path, 'ab+') as outfile:
			shutil.copyfileobj(readfile, outfile)
