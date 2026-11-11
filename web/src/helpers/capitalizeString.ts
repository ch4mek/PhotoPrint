export const capitalizeString = (str: string) => {
	const trimmedString = str.trim();

	return trimmedString.charAt(0).toUpperCase() + trimmedString.slice(1);
};
