DEFAULT_CHUNK_SIZE = 16 # This value is in bytes. The block size has to be smaller than or equal to the key size, which is usually in bits.

def encrypt_list_of_numbers(list_of_numbers, e, n):
	"""Args:
		e is the public key exponent
		n is the modulus for the public key and the private keys, obtained by multiplying the primes chosen when making the certificate
	"""
	encrypted_numbers = []
	for number in list_of_numbers:
		# pow(a,b,c) = (a^b) mod c
		encrypted_numbers.append(pow(number,e,n))
	return encrypted_numbers

def decrypt_list_of_numbers(list_of_numbers, d, n):
	"""Args:
		d is the private key exponent
		n is the modulus for both the public key and the private keys, obtained by multiplying the primes chosen when making the certificate
	"""
	decrypted_numbers = []
	for number in list_of_numbers:
		decrypted_numbers.append(pow(number,d,n))
	return decrypted_numbers

def encrypt_message(message, e, n, chunk_size=DEFAULT_CHUNK_SIZE):
	"""Args:
		message (b''): The binary message you want to encrypt.
		e (int): The public key exponent you want to use to encrypt the message.
		n (int): The public key modulus.
	"""
	chunked_message = chunk_array(message, chunk_size)
	chunks_as_numbers = list_of_byte_chunks_to_list_of_numbers(chunked_message)
	encrypted_chunks = encrypt_list_of_numbers(chunks_as_numbers, e, n)
	encrypted_message = merge_byte_chunks(list_of_numbers_to_list_of_byte_chunks(encrypted_chunks))
	return encrypted_message

def decrypt_message(message, d, n, chunk_size=DEFAULT_CHUNK_SIZE):
	"""Args:
		message (int): The encrypted message you want to decrypt. Should be a number?
		d (int): The private key exponent.
		n (int): The private key modulus.
	"""
	chunked_message = chunk_array(message, chunk_size)
	chunked_message_as_numbers = list_of_byte_chunks_to_list_of_numbers(chunked_message)
	decrypted_chunks = decrypt_list_of_numbers(chunked_message_as_numbers)
	chunks_as_binary_strings = list_of_numbers_to_list_of_byte_chunks(decrypted_chunks, block_size)
	decrypted_message = merge_byte_chunks(chunks_as_strings)
	return decrypted_message

def read_byte_array_from_file(path_to_file):
	with open(path_to_file, "rb") as binary_file:
		return binary_file.read()

def write_byte_array_to_file(byte_array, path_to_file):
	with open(path_to_file, "wb") as out_file:
		out_file.write(byte_array)

def chunk_array(array, chunk_size):
	return [array[i:i+chunk_size] for i in range(0, len(array), chunk_size)]

def merge_byte_chunks(chunks):
	merged_items = b''
	for chunk in chunks:
		merged_items += chunk
	return merged_items

def bytes_to_number(series_of_bytes):
	return int.from_bytes(series_of_bytes, byteorder='big')

def number_to_bytes(number):
	size_of_number_in_bytes = (number.bit_length() + 7) // 8
	return int.to_bytes(number, length=size_of_number_in_bytes, byteorder='big')

def list_of_byte_chunks_to_list_of_numbers(list_of_byte_chunks):
	numbers = []
	for chunk in list_of_byte_chunks:
		numbers.append(bytes_to_number(chunk))
	return numbers

def list_of_numbers_to_list_of_byte_chunks(list_of_numbers):
	byte_chunks = []
	for number in list_of_numbers:
		byte_chunks.append(number_to_bytes(number))
	return byte_chunks

if __name__ == "__main__":
	test_data = b'here is some random data\r\nI guess you could call it a binary blob?\r\nIt contains cool stuff such as "\xc3\xa6", \'\xc3\xb8\' and \\\xc3\xa5/!\r\n'
	test_chunk_size = 4
	if(test_data != number_to_bytes(bytes_to_number(test_data))):
		print('Converting bytes to numbers and back again resulted in different bytes.')
		print('The original bytes were:')
		print(test_data)
		print('The number obtained was:')
		print(bytes_to_number(test_data))
		print('When converting back, got these bytes:')
		print(number_to_bytes(bytes_to_number(test_data)))

	if(test_data != merge_byte_chunks(chunk_array(test_data,test_chunk_size))):
		print('Chunking bytes and stiching them together again failed.')
		print('The original bytes were:')
		print(test_data)
		print('The chunked array was:')
		print(chunk_array(test_data,test_chunk_size))
		print('When merging the chunks, got these bytes:')
		print(merge_byte_chunks(chunk_array(test_data,test_chunk_size)))

	chunked_test_data = chunk_array(test_data,test_chunk_size)
	if(chunked_test_data != list_of_numbers_to_list_of_byte_chunks(list_of_byte_chunks_to_list_of_numbers(chunked_test_data))):
		print('Converting byte chunks to numbers and back again failed.')
		print('The original byte chunks were:')
		print(chunked_test_data)
		print('The number array was:')
		print(list_of_byte_chunks_to_list_of_numbers(chunked_test_data))
		print('When converting back to bytes, got this list of bytes:')
		print(list_of_numbers_to_list_of_byte_chunks(list_of_byte_chunks_to_list_of_numbers(chunked_test_data)))

	number_chunks = list_of_byte_chunks_to_list_of_numbers(chunked_test_data)
	represented_as_bytes = list_of_numbers_to_list_of_byte_chunks(number_chunks)
	as_one_giant_byte_list = merge_byte_chunks(represented_as_bytes)
	rechunked = chunk_array(as_one_giant_byte_list,test_chunk_size)
	back_to_numbers = list_of_byte_chunks_to_list_of_numbers(rechunked)
	if(number_chunks != back_to_numbers):
		print('ToDo: Something is wrong. Express what so that it can be fixed')
