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

# Hint or block duplicates in google sheets

To start, right click and select Data validation, or go to Data and then Data validation.
Here, you can select the range to cover, and what criteria should be used for the validation.
To highlight or block duplicate values, you want to select the `Custom formula is` option for the criteria, and then enter a formula following the patterns described in the sections below depending on how simple or complex the range in question is.
It might be a good idea to enter a describing message under `Appearance`, to make it easier to guess why your input all of a sudden fails when you revisit the sheet after some months/years.

## Simple column formula

If you work on a simple sheet and don't want to introduce (your collaborators to) named ranges, you can use an adaptation of this formula:

`=COUNTIF(range, first_cell_in_range_address)=1`

For instance, if you want to validate that the value you enter in the range D5 to D105 does not have any duplicates in the same range, the formula would look like this:

`=COUNTIF(D$5:D105, D5)=1`

Things to note:

* The `D5` actually refers to the cell you just edited/that is being validated itself, not the fist entry in the range. The reason for this is some legacy spreadsheet conventions.
* For the same reason, the start of the range has to be specified with a `$` before the number.

## Named and complex range formula

To extend the rules for checking for duplicates to cover complex ranges or named ranges in a generic way, you can use this formula as a basis:

`=COUNTIF(range, INDIRECT(ADDRESS(ROW(), COLUMN(), 4))) < 2`

Concrete example with named range:

`=COUNTIF(name_of_range, INDIRECT(ADDRESS(ROW(), COLUMN(), 4))) < 2`

Concrete example covering the range D5 to G10:

`=COUNTIF($D$5:$G$10, INDIRECT(ADDRESS(ROW(), COLUMN(), 4))) < 2`

Notes:

* As with the simple formula, if you specify the range directly, you have to use the `$` syntax to not work relative to the element being validated but rather to the absolute boundaries of the range you want to validate against.
* `ADDRESS(ROW(), COLUMN(), 4)` resolves the current address. By passing the magic number `4` it's returned in a format that `INDIRECT` can digest (for instance `D5` if using the sample range above). In turn, `INDIRECT` extracts and yields the content you just entered/of the cell being validated.
* As with the examples in the previous section, you could end the statement with `= 1` rather than `< 2` to validate for duplicates. This shows how you can extend the logic to check for no more than `n` occurrences  in the given range, by replacing `< 2` with `< n + 1`.
* Better/more in-depth explanation: https://stackoverflow.com/a/36065551
