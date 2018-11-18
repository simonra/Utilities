def skip_take_on_top_of_get_by_page_number(skip, take, function_get_page):
	page_size = take
	while((skip // page_size + 1) * page_size < (skip + take)):
		page_size++
	page_number = skip // page_size
	page = function_get_page(page_number, page_size)
	offset_in_result = skip % page_size
	result = page[offset_in_result:(offset_in_result + take)]
	return result
