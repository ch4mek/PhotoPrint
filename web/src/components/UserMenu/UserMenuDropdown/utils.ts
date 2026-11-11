const date = new Date();
const timezoneOffsetInMinutes = date.getTimezoneOffset();
const timezoneOffsetInHours = timezoneOffsetInMinutes / 60;
const timezoneOffsetFormatted =
	timezoneOffsetInHours < 0
		? `+${Math.abs(timezoneOffsetInHours)}`
		: `-${timezoneOffsetInHours}`;

export const localTime = `${timezoneOffsetFormatted}:00`;
export const timezoneName = Intl.DateTimeFormat().resolvedOptions().timeZone;
