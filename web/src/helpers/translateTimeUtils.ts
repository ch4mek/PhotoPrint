export const minsToHrs = (min: number) => Math.floor(min / 60);
export const getRemainingMinutes = (min: number) => min % 60;

export const minsToHrsAndMins = (min: number) => {
	const hours = minsToHrs(min) ? `${minsToHrs(min)} h ` : '';
	const minutes = `${min % 60} min`;

	return `${hours}${minutes}`;
};

export const minsToHrsAndMinsObject = (min: number) => ({
	hours: minsToHrs(min),
	minutes: min % 60,
});

export const minsToMinsRest = (min: number) => (!min ? 0 : min % 60);
