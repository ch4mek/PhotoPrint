const COMBINED_NAME_REGEX = /^(.+?)\s*\((.+?)\)\s*$/;
const PARTIAL_COMBINED_NAME_REGEX = /^(.+?)\s*\((.+)$/;
const START_WITH_PAREN_REGEX = /^\((.+)$/;

const sanitizeSearchTerm = (term: string): string | null => {
	const sanitized = term.replace(/[()]/g, '').trim();

	return sanitized || null;
};

export const parseUserSearchString = (searchString: string) => {
	const fullMatch = searchString.match(COMBINED_NAME_REGEX);

	if (fullMatch) {
		return {
			russianName: sanitizeSearchTerm(fullMatch[1]),
			englishName: sanitizeSearchTerm(fullMatch[2]),
		};
	}

	const partialMatch = searchString.match(PARTIAL_COMBINED_NAME_REGEX);

	if (partialMatch) {
		return {
			russianName: sanitizeSearchTerm(partialMatch[1]),
			englishName: sanitizeSearchTerm(partialMatch[2]),
		};
	}

	const startWithParenMatch = searchString.match(START_WITH_PAREN_REGEX);

	if (startWithParenMatch) {
		return {
			russianName: null,
			englishName: sanitizeSearchTerm(startWithParenMatch[1]),
		};
	}

	return {
		russianName: null,
		englishName: null,
		fallbackTerm: sanitizeSearchTerm(searchString),
	};
};
