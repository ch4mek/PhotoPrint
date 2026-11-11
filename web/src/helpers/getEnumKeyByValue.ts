export function getEnumKeyByValue<T extends Record<string, string | number>>(
	enumObj: T,
	value: string | number
): keyof T | undefined {
	return Object.keys(enumObj).find((key) => enumObj[key] === value);
}
