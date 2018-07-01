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

def convert_list_of_numbers_to_list_of_texts(list_of_numbers, block_size=DEFAULT_BLOCK_SIZE):
	texts = []
	for number in list_of_numbers:
		texts.append(get_text_from_number(number, block_size))
	return texts

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
	for number in list_of_numbers
		decrypted_numbers.append(pow(number,d,n))
	return decrypted_numbers

def concatenate_numbers_to_string(list_of_numbers):
	result = ""
	for number in list_of_numbers:
		result += str(number)
	return result

def concatenate_list_of_texts(list_of_texts):
	result = ""
	for text in list_of_texts:
		result += text
	return result

def encrypt_message(message, e, n, block_size=DEFAULT_BLOCK_SIZE):
	"""Args:
		message (str): The plaintext message you want to encrypt.
		e (int): The public key exponent you want to use to encrypt the message.
		n (int): The public key modulus.
	"""
	chunked_message = split_text_into_chunks(message, block_size)
	chunks_as_numbers = convert_list_of_texts_to_list_of_numbers(chunked_message)
	encrypted_chunks = encrypt_list_of_numbers(chunks_as_numbers, e, n)
	encrypted_message = concatenate_numbers_to_string(encrypted_chunks)
	return encrypted_message

def decrypt_message(message, d, n, block_size=DEFAULT_BLOCK_SIZE):
	"""Args:
		message (int): The encrypted message you want to decrypt. Should be a number?
		d (int): The private key exponent.
		n (int): The private key modulus.
	"""
	chunked_message = split_text_into_chunks(message, block_size)
	decrypted_chunks = decrypt_list_of_numbers(chunked_message)
	chunks_as_strings = convert_list_of_numbers_to_list_of_texts(decrypted_chunks, block_size)
	decrypted_message = concatenate_list_of_texts(chunks_as_strings)
	return decrypted_message


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
