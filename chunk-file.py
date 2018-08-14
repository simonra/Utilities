import sys
import os
import math
import base64

# print("Supplied arguments: ")
# print(sys.argv)

assert len(sys.argv) > 1, "No file to process specified in arguments."
supplied_path_to_file = sys.argv[1]
assert os.path.exists(supplied_path_to_file), "Could not find file at " + supplied_path_to_file

original_filename = os.path.basename(supplied_path_to_file)
file_size_bytes = os.path.getsize(supplied_path_to_file)

destination_directory = os.path.abspath(os.path.join(supplied_path_to_file, os.pardir))
if(len(sys.argv) > 2):
	destination_directory = sys.argv[2]
	os.makedirs(destination_directory, exist_ok=True)

# print('File size:')
# print(file_size_bytes)
# print('File name:')
# print(original_filename)

# Smallest even number larger than 2^30 (1024^3) that is a multiple of 3
block_size_bytes = 1073741826
# block_size_bytes = 642414

# Round up, because we never want to miss that last block
number_of_blocks = math.ceil(file_size_bytes / block_size_bytes)
# print('Number of blocks:')
# print(number_of_blocks)

starting_block = 0
if(len(sys.argv) > 3):
	# Don't care about what might have been before. What if the source is so lage we have to specify that this set of chunks need to go to a separate drive?
	starting_block = int(sys.argv[3])
else:
	# Figure out if we are simply continuing from where we left of
	pre_existing_chunks = [f for f in os.listdir(destination_directory) if f.startswith(original_filename)]
	# print(pre_existing_chunks)
	if(len(sys.argv) == 2 and len(pre_existing_chunks) == 1):
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

if(len(sys.argv) > 4):
	# For when you know that you only want to extract a certain number of chunks at the moment.
	# Usefull for instance when trying to split a large file across several drives.
	number_of_blocks = int(sys.argv[4])

# print('First block:')
# print(starting_block)

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
