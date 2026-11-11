const isValidHour = (hour: number) =>
	Number.isInteger(hour) && hour >= 0 && hour < 24;

const isValidMinutes = (minutes: number) => {
	if (!Number.isInteger(minutes) || Number.isNaN(minutes)) {
		return false;
	}

	if (minutes >= 0 && minutes < 60) {
		return true;
	}

	return false;
};

export const isValidTime = (value: string) => {
	const TIME_REG_EXP = /^\d{0,2}?:?\d{0,2}$/;

	if (!value || !TIME_REG_EXP.test(value)) {
		return false;
	}
	const [hoursString, minutesString = '00'] = value.split(':');
	const hours = Number(hoursString);
	const minutes = Number(minutesString);

	if (!(isValidHour(hours) && isValidMinutes(minutes))) {
		return false;
	}

	if (minutes < 10 && Number(minutesString[0]) > 5) {
		return false;
	}

	const valuesArray = value.indexOf(':') !== -1 ? value.split(':') : [value];

	if (
		valuesArray[0] &&
		valuesArray[0].length &&
		(parseInt(valuesArray[0], 10) < 0 || parseInt(valuesArray[0], 10) > 23)
	) {
		return false;
	}

	if (
		valuesArray[1] &&
		valuesArray[1].length &&
		(parseInt(valuesArray[1], 10) < 0 || parseInt(valuesArray[1], 10) > 59)
	) {
		return false;
	}

	return true;
};

export const isValidOnChange = (value: string) =>
	value.length === 2 ? `${value}:00` : value;

export const convertStringDateToValidFormat = (value: string | undefined) => {
	if (!value) {
		return '';
	}

	let correctValue = value;

	if (value.length === 1) {
		if (/\d{1}/.test(correctValue)) {
			correctValue = `0${value}:00`;
		} else {
			return '';
		}
	}

	if (value.length === 2) {
		if (value.includes(':')) {
			if (/\d{1}:/.test(correctValue)) {
				correctValue = `0${correctValue}00`;
			} else if (/:\d{1}/.test(correctValue)) {
				correctValue = `00${correctValue}0`;
			}
		} else {
			correctValue = `${value}:00`;
		}
	}

	if (value.length === 3) {
		if (value.includes(':')) {
			if (/\d{1}:\d{1}/.test(correctValue)) {
				correctValue = `0${correctValue}0`;
			} else if (/\d{2}:/.test(correctValue)) {
				correctValue = `${correctValue}00`;
			} else if (/:\d{2}/.test(correctValue)) {
				correctValue = `00${correctValue}`;
			}
		} else {
			correctValue = `${value}:00`;
		}
	}

	if (value.length === 4) {
		if (value.includes(':')) {
			if (/\d{2}:\d{1}/.test(correctValue)) {
				correctValue = `${correctValue}0`;
			} else if (/\d{1}:\d{2}/.test(correctValue)) {
				correctValue = `0${correctValue}`;
			}
		} else {
			correctValue = `${value}:00`;
		}
	}

	if (/0{2}:0{2}/.test(correctValue)) {
		return '';
	}

	return correctValue;
};
