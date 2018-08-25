import sys
import argparse
import os
import math
import base64

parser = argparse.ArgumentParser()
parser.add_argument(
	'-if',
	'--inputFile',
	required=True,
	help='Path to the file you want to split into chunks.')
parser.add_argument(
	'-op',
	'--outputPath',
	help='(Optional) Path to the directory you want the split chunks to be placed into.')
parser.add_argument(
	'-swb',
	'--startWith',
	type=int,
	help='(Optional) Number for chunk you want to start with. Usefull if you have previously aborted the operation and now want to resume.')
parser.add_argument(
	'-nb',
	'--numberOfBlocksToMake',
	type=int,
	help='(Optional) Number of blocks you wish to create this time. Not that the block size is fixed, so this will not help you in the case where you want to split the file into a given number of blocks, but it is usefull if you know that you can only process a given number of blocks at a time. (If there would be created fewer blocks than specified in this argument then setting this argument accomplishes nothing.)')
args = parser.parse_args()

supplied_path_to_file = args.inputFile
assert os.path.exists(supplied_path_to_file), "Could not find file at " + supplied_path_to_file

original_filename = os.path.basename(supplied_path_to_file)
file_size_bytes = os.path.getsize(supplied_path_to_file)

destination_directory = os.path.abspath(os.path.join(supplied_path_to_file, os.pardir))
if(args.outputPath is not None):
	destination_directory = args.outputPath
	os.makedirs(destination_directory, exist_ok=True)

# raise SystemExit

# print('File size:')
# print(file_size_bytes)
# print('File name:')
# print(original_filename)

# Smallest even number larger than 2^30 (1024^3) that is a multiple of 3
block_size_bytes = 1073741826
# block_size_bytes = 642414

# Round up, because we never want to miss that last block
number_of_blocks = math.ceil(file_size_bytes / block_size_bytes)
if(number_of_blocks < 2):
	print('File not large enough to be split into chunks. It\'s meaningless to to split into one chunk. Aborting.')
	raise SystemExit
# print('Number of blocks:')
# print(number_of_blocks)

starting_block = 0
if(args.startWith is not None):
	# Don't care about what might have been before. What if the source is so lage we have to specify that this set of chunks need to go to a separate drive?
	starting_block = args.startWith
else:
	# Figure out if we are simply continuing from where we left of
	pre_existing_chunks = [f for f in os.listdir(destination_directory) if f.startswith(original_filename)]
	# print(pre_existing_chunks)
	if(args.outputPath is not None and len(pre_existing_chunks) == 1):
		# Writing to same directory as source, and have to consider that the original file is not a chunk.
		pass
	elif(len(pre_existing_chunks) > 0):
		# print("there are pre-existing chunks to check")
		for named_chunk in pre_existing_chunks:
			chunk_number = int(str.split(named_chunk,".")[-1])
			# print('chunk_number')
			# print(chunk_number)
			if(chunk_number > starting_block):
				starting_block = chunk_number
		if(starting_block == number_of_blocks - 1):
			if(len(pre_existing_chunks) >= number_of_blocks):
				print('Larges part seems to be present in destination, and destination seems to contain all parts. Aborting.')
				raise SystemExit
			else:
				print('Largest chunk seems to already be present. Starting over from scratch to be on the safe side.')
				starting_block = 0
		# else:
			# Start not at last previously written block, but rather at the first missing block.
			# Safer to not enable if you suspect that the last run failed during the writing of the last block.
			# starting_block += 1

# For when you know that you only want to extract a certain number of chunks at the moment.
# Usefull for instance when trying to split a large file across several drives.
if(args.numberOfBlocksToMake is not None):
	number_of_blocks = args.numberOfBlocksToMake

# print('First block:')
# print(starting_block)

# for slow and messy connections where you want to have files open for as short as possible:
# for i in range(starting_block, number_of_blocks):
# 	print('Starting to chunk ' + supplied_path_to_file + ' into parts in ' + destination_directory)
# 	offset = starting_block * block_size_bytes
# 	source_file = open(supplied_path_to_file,'rb')
# 	source_file.seek(offset)
# 	current_block = source_file.read(block_size_bytes)
# 	source_file.close()

# 	next_output_filename = original_filename + "." + str(i)
# 	destination_path = os.path.join(destination_directory, next_output_filename)
# 	# print('Writing to destination path:')
# 	# print(destination_path)
# 	# Uncomment in case you can't reliably write bytes to the destination:
# 	# current_block = base64.b64encode(current_block)
# 	with open(destination_path,'wb') as destination_file:
# 		destination_file.write(current_block)

# For fast and reliable connections:
with open(supplied_path_to_file,'rb') as source_file:
	print('Starting to chunk ' + supplied_path_to_file + ' into parts in ' + destination_directory)
	offset = starting_block * block_size_bytes
	source_file.seek(offset)
	for i in range(starting_block, number_of_blocks):
		current_block = source_file.read(block_size_bytes)
		next_output_filename = original_filename + "." + str(i)
		destination_path = os.path.join(destination_directory, next_output_filename)
		# print('Writing to destination path:')
		# print(destination_path)
		# Uncomment in case you can't reliably write bytes to the destination:
		# current_block = base64.b64encode(current_block)
		with open(destination_path,'wb') as destination_file:
			destination_file.write(current_block)

print('Finished writing all chunks to directory:')
print(destination_directory)
