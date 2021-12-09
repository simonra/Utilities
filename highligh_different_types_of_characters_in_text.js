// Small script to highlight different types of characters in a string.
// Intended for confirmation codes with lookalike characters that you have to retype, where you're not sure what exactly to enter in the other place.
// Usage: Run `HighlightString("string you want to highlight");`.

function HighlightString(StringToHighlight){
	// const BackgroundColour = '#fff' // white
	// const LetterColourLowerCase = '#008080' // teal
	// const LetterColourUpperCase = red
	// const NumberColour = '#800080' // purple
	// const OtherColour = '#9a6510' // Golden-brown

	// Colour scheme:
	var BackgroundColour = '#2e2e2e'
	var LetterColourLowerCase = '#b4d273' // Green
	var LetterColourUpperCase = '#9e86c8' // Purple
	var NumberColour = '#6c99bb' // Blue
	var OtherColour = '#e87d3e' // Orange

	// Output size
	var PrintSize = '2em';

	function IsCharacterDigit(character){
		if(0 <= character && character <= 9){
			return true;
		}
		return false;
	}

	function IsCharacterLetter(character){
		// If the character has a different upper and lower case representation it is reasonable for my use to assume that it is a letter.
		// It might even be a bit too inclusive, I guess if it annoys me over time I'll go for something more limmited like "return (/[a-zA-Z]/).test(char)"
		return character.toLowerCase() != character.toUpperCase();
	}

	function IsLetterUppercase(letter){
		return letter == letter.toUpperCase();
	}


	/* Note that ListOfCharactersWithProperties is not used, because I decided
	 it would be simpler and faster to iterate over the string only once, with
	 the tradeof for more memor needed to store the precomputed list of css
	 string and the premade string for the output. Still keeping it around,
	 because it's neat for debugging without having to step through everything
	 for each character.
	*/
	var ListOfCharactersWithProperties = [];
	var ListOfCssStrings = [];
	var ReassembledStringForPrinting = "";

	for(const character of StringToHighlight){
		var colour = OtherColour;
		if(IsCharacterLetter(character)){
			if(IsLetterUppercase(character)){
				colour = LetterColourUpperCase;
			}
			else{
				colour = LetterColourLowerCase;
			}
		}
		else if(IsCharacterDigit(character)){
			colour = NumberColour;
		}

		var CssString = `background: ${BackgroundColour}; color: ${colour}; font-size: ${PrintSize};`;
		var PrintString = `%c${character}`;

		var characterWithMetadata = {
			Character: character,
			StringRepresentationForPrinting: PrintString,
			CssForPrinting: CssString
		};

		ListOfCharactersWithProperties.push(characterWithMetadata);
		ListOfCssStrings.push(CssString);
		ReassembledStringForPrinting += PrintString;

		// console.debug(character);
	}

	// console.debug(ListOfCharactersWithProperties);

	// Print legend so that it's easier to remember what is what if you only have a lot of lookalike characters:
	console.log("Legend: %cABCD%cefgh%c1234%c!@#$",
		`background: ${BackgroundColour}; color: ${LetterColourUpperCase}; font-size: ${PrintSize};`,
		`background: ${BackgroundColour}; color: ${LetterColourLowerCase}; font-size: ${PrintSize};`,
		`background: ${BackgroundColour}; color: ${NumberColour}; font-size: ${PrintSize};`,
		`background: ${BackgroundColour}; color: ${OtherColour}; font-size: ${PrintSize};`);

	// Print the highlighted string:
	console.log(ReassembledStringForPrinting, ...ListOfCssStrings);
}


function TestHighlightString(){
	var testString = "My string has NUMBERS! 1234! ^^";
	HighlightString(testString);
}

// Use this for coloring:
// https://stackoverflow.com/a/13017382
// console.log("%cStr1" + "%cStr2", "CSS-for-str1", "CSS-for-str2")
