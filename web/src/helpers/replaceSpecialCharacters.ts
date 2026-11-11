export const replaceSpecialCharacters = (string: string) => {
	let stringCopy = string;

	stringCopy = stringCopy.replace(/'/g, "''");
	stringCopy = stringCopy.replace(/%/g, '%25');
	stringCopy = stringCopy.replace(/\+/g, '%2B');
	stringCopy = stringCopy.replace(/\//g, '%2F');
	stringCopy = stringCopy.replace(/\?/g, '%3F');
	stringCopy = stringCopy.replace(/#/g, '%23');
	stringCopy = stringCopy.replace(/&/g, '%26');

	return stringCopy;
};
