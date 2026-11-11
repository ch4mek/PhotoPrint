export const lowercaseFirstSymbol = (str: string) => {
	const trimmedString = str.trim();

	return trimmedString.charAt(0).toLowerCase() + trimmedString.slice(1);
};
