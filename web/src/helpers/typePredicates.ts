export function isStringArray(array: string[] | unknown[]): array is string[] {
	return typeof array[0] === 'string';
}
