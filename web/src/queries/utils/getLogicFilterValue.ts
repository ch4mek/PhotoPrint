export const getLogicFilterValue = (
	value: string[],
	trueValue: string,
	name: string
) => {
	if (!value.length || value.length === 2) {
		return null;
	}

	return `${name} eq ${value[0] === trueValue}`;
};
