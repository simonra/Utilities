var textToClarify = "My? Or Your? Text. It has numbers, like 1, and 1000! OH~";



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

function PrintStringWithHightlightingForDifferentTypesOfCharacters(textToClarify){
	// const BackgroundColour = '#fff' // white
	// const LetterColour = '#008080' // teal
	// const NumberColour = '#800080' // purple
	// const OtherColour = '#9a6510' // Golden-brown

	var BackgroundColour = '#2e2e2e'
	var LetterColour = '#b4d273'
	var NumberColour = '#6c99bb'
	var OtherColour = '#e87d3e'

	var ListOfCharactersWithProperties = [];
	var ListOfCssStrings = [];
	var ReassembledStringForPrinting = "";
	for(const character of textToClarify){
		var colour = OtherColour;
		if(IsCharacterLetter(character)){
			colour = LetterColour;
		}
		else if(IsCharacterDigit(character)){
			colour = NumberColour;
		}

		var CssString = `background: ${BackgroundColour}; color: ${colour};`;
		var PrintString = `%c${character}`;

		var characterWithMetadata = {
			Character: character,
			StringRepresentationForPrinting: PrintString,
			CssForPrinting: CssString
		};

		ListOfCharactersWithProperties.push(characterWithMetadata);
		ListOfCssStrings.push(CssString);
		ReassembledStringForPrinting += PrintString;

		// console.log(character);
	}

	// Print legend so that it's easier to remember what is what if you only have a lot of lookalike characters:
	console.log("Legend: %cABCD%c1234%c!@#$",
		`background: ${BackgroundColour}; color: ${LetterColour}; font-size: ${PrintSize};`,
		`background: ${BackgroundColour}; color: ${NumberColour}; font-size: ${PrintSize};`,
		`background: ${BackgroundColour}; color: ${OtherColour}; font-size: ${PrintSize};`);

	// Print the highlighted string:
	console.log(ReassembledStringForPrinting, ...ListOfCssStrings);
}

// Use this for coloring:
// https://stackoverflow.com/a/13017382
// console.log("%cStr1" + "%cStr2", "CSS-for-str1", "CSS-for-str2")
