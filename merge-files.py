import argparse
import os
import shutil

parser = argparse.ArgumentParser()
parser.add_argument(
	'-ip',
	'--inputPath',
	required=True,
	help='Path to the directory with the files you want to merge to a single file.')
parser.add_argument(
	'-op',
	'--outputPath',
	required=True,
	help='Path to the file you want the supplied files to be merged into. For now must be supplied, and has to point to a file that doesn\'t previously exist for the operation to make sense.')
args = parser.parse_args()

directory_with_chunks = args.inputPath
assert os.path.exists(directory_with_chunks), "Could not find anything at " + directory_with_chunks

destination_path = args.outputPath

# If the folder where we wish to place the output-file doesn't exist, create it:
destination_directory = os.path.abspath(os.path.join(destination_path, os.pardir))
os.makedirs(destination_directory, exist_ok=True)

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
