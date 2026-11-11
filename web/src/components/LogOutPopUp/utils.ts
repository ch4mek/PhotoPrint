import {
	USER_LAST_ACTIVITY_DATE,
	INACTIVE_LIMIT,
	SHOWING_LOG_OUT_POPUP_DURATION,
} from '@/constants/authConstants';
import { timeUnits } from '@/constants/date';

export const getFormattedSeconds = (seconds: number) =>
	`00:${String(seconds > 0 ? Math.trunc(seconds) : 0).padStart(2, '0')}`;

export const checkTimeActivity = () => {
	const lastUserActivityDate = Number(
		localStorage.getItem(USER_LAST_ACTIVITY_DATE)
	);

	return (
		Date.now() - lastUserActivityDate >
		INACTIVE_LIMIT + SHOWING_LOG_OUT_POPUP_DURATION
	);
};

export const calculateSecondsToLogOut = () =>
	(Number(localStorage.getItem(USER_LAST_ACTIVITY_DATE)) +
		INACTIVE_LIMIT +
		SHOWING_LOG_OUT_POPUP_DURATION -
		Date.now()) /
	timeUnits.MILLISECONDS_IN_SECOND;
