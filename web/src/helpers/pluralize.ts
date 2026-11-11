const DEFAULT_NUMBER = 0;

export const pluralize = (word: string, count = DEFAULT_NUMBER) =>
	count === 1 ? word : `${word}s`;
