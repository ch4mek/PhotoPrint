export interface FullName {
	firstName: string;
	lastName: string;
}

export const getFullName = (firstName: string, lastName: string) =>
	`${lastName} ${firstName}`;

export const formatFullName = (lastName: string, firstName: string) =>
	getFullName(firstName || '', lastName || '-');

const splitFullName = (fullName: string) => {
	const lastSpaceIndex = fullName.lastIndexOf(' ');

	if (lastSpaceIndex === -1) {
		return {
			firstName: '',
			lastName: fullName,
		};
	}

	const lastName = fullName.substring(0, lastSpaceIndex);
	const firstName = fullName.substring(lastSpaceIndex + 1);

	return {
		firstName,
		lastName,
	};
};

export const parseFullName = <T extends FullName>(
	fullName: string | T
): FullName => {
	if (typeof fullName === 'string') {
		return splitFullName(fullName);
	}

	const { firstName, lastName } = fullName;

	return {
		firstName,
		lastName,
	};
};
