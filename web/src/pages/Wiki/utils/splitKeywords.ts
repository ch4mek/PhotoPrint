export const splitKeywords = (keywords: string) => {
	if (keywords.length === 0) {
		return [];
	}

	return keywords.split(' ');
};
