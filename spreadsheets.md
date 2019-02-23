A collection of features and functions I don't use often, but are nice to have a simple summary of how work gathered in one place for when I need them.

# Computations using references

## Match

Takes a value, and finds the index/positon of it in a supplied range.

### Example

Given

| keys |
|---|
| a |
| b |
| c |
| d |

then `MATCH(c, keys) => 3`. Note that this assumes that the header is not part of the range. If it had been included, the result would have been `4`.

## Index

Takes a range and an index within that range, and returns the value at the position of that index.

### Example

Given

| keys |
|---|
| a |
| b |
| c |
| d |

then `INDEX(2, keys) => b`. Note that this assumes that the header is not part of the range. If it had been included, the result would have been `a`.

## Putting it together

To retrieve a value from a dictionary you have defined for further use:

`INDEX(dictionary_values_range, MATCH(key, dictionary_keys_range))`

### Example

Given

| keys | values | colors |
|---|----|--------|
| a | 10 | red    |
| b | 20 | green  |
| c | 30 | blue   |
| d | 40 | yellow |

Then
* `INDEX(values, MATCH(c, keys)) => 30`
* `INDEX(colors, MATCH(d, keys)) => yellow`
* `INDEX(keys, MATCH(red, colors)) => a`
