const SAFE_HASH_MOD = 2147483647;

export const hashString = (input: string): string => {
	const hash = Array.from(input).reduce(
		(acc, char) => (acc * 31 + char.charCodeAt(0)) % SAFE_HASH_MOD,
		0
	);

	return Math.abs(hash).toString(36);
};
