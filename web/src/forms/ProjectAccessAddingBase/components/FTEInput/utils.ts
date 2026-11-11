export const changeFTEValue = (
	value: string,
	onChange: (value: string) => void
) => {
	if (value === '.') {
		onChange('0.');

		return;
	}

	if (Number.isNaN(Number(value))) {
		return;
	}

	const numberTargetValue = Number(value);

	if (numberTargetValue % 1 === 0 && numberTargetValue > 1) {
		onChange(`0.${Math.round(numberTargetValue) % 100}`);

		return;
	}

	if ((numberTargetValue > 1 && numberTargetValue < 0) || value.length > 4) {
		return;
	}

	if (
		numberTargetValue === 0 &&
		value.length > 1 &&
		Array.from(value).every((char) => char === '0')
	) {
		onChange('0');

		return;
	}

	onChange(value);
};
