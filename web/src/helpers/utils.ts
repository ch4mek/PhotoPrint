const getRussianFullName = (
	firstNameRu?: string | null,
	lastNameRu?: string | null
) => {
	if (firstNameRu && lastNameRu) {
		return `${lastNameRu} ${firstNameRu}`;
	}

	if (firstNameRu) {
		return firstNameRu;
	}

	if (lastNameRu) {
		return lastNameRu;
	}

	return null;
};

export const mergeUserFullNames = (
	fullNameEn: string,
	fullNameRu?: string | null
) => {
	if (fullNameRu) {
		return `${fullNameRu} (${fullNameEn})`;
	}

	return fullNameEn;
};

export const getFullName = (
	firstName: string,
	lastName: string,
	firstNameRu?: string | null,
	lastNameRu?: string | null
) => {
	const russianFullName = getRussianFullName(firstNameRu, lastNameRu);
	const englishFullName = `${lastName} ${firstName}`;

	return mergeUserFullNames(englishFullName, russianFullName);
};
