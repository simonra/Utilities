BYTE_SIZE = 256 # Number of possible values you can represent with a byte consisting of 8 bits.
DEFAULT_BLOCK_SIZE = 4 # This value is in bytes. The block size has to be smaller than or equal to the key size, which is usually in bits.

# def encrypt(message, product_of_initial_primes, public_key_exponent):
# 	encrypted_message = 

# def decrypt():

# def check_if_prime():

def split_text_into_chunks(long_text, block_size=DEFAULT_BLOCK_SIZE, pad_last_chunk=True):
	# ecquivalent more compact, but perhaps less readable version:
	# blocks = [long_text[i:i+block_size] for i in range(0, len(long_text), block_size)]
	blocks = []
	for i in range(0,len(long_text),block_size):
		blocks.append(long_text[i:i+block_size])

	if(pad_last_chunk):
		if(len(blocks[-1])<block_size):
			blocks[-1] = pad_text(blocks[-1],block_size)

	return blocks

def pad_text(text, desired_length, char_to_fill_with=' ', alignment='<'):
	return '{message:{fill}{align}{width}}'.format(
		message=text,
		fill=char_to_fill_with,
		align=alignment,
		width=desired_length,
	)

def get_number_representing_text(text_as_string):
	return int.from_bytes(text_as_string.encode('utf-8'), byteorder='big')

def get_text_from_number(number_represention_text, block_size=DEFAULT_BLOCK_SIZE):
	return int.to_bytes(number_represention_text, length=block_size, byteorder='big').decode('utf-8')

def convert_list_of_texts_to_list_of_numbers(list_of_strings):
	numbers = []
	for text in list_of_strings:
		numbers.append(get_number_representing_text(text))
	return numbers

def convert_list_of_numbers_to_list_of_texts(list_of_numbers):
	texts = []
	for number in list_of_numbers:
		texts.append(get_text_from_number(number))
	return texts

def encrypt_list_of_numbers(list_of_numbers, e, n):
	"""
	e is the public key exponent
	n is the modulus for the public key and the private keys, obtained by multiplying the primes chosen when making the certificate
	"""
	encrypted_numbers = []
	for number in list_of_numbers:
		# pow(a,b,c) = (a^b) mod c
		encrypted_numbers.append(pow(number,e,n))
	return encrypted_numbers

def decrypt_list_of_numbers(list_of_numbers, d, n):
	"""
	d is the private key exponent
	n is the modulus for both the public key and the private keys, obtained by multiplying the primes chosen when making the certificate
	"""
	decrypted_numbers = []
	for number in list_of_numbers
		decrypted_numbers.append(pow(number,d,n))
	return decrypted_numbers

if __name__ == "__main__":
	text = 'My very long text.'
	print(text)
	print(len(text))
	chunks = split_text_into_chunks(text)
	print(chunks)
	print(len(chunks[0]))

	# reclaimedNumbers = get_text_from_number(88,33)
	# print("reclaimed text = " + reclaimedNumbers)

	numbers = convert_list_of_texts_to_list_of_numbers(chunks)
	print(numbers)
	print((numbers[0].bit_length() + 7) // 8)

	texts = convert_list_of_numbers_to_list_of_texts(numbers)
	print(texts)
