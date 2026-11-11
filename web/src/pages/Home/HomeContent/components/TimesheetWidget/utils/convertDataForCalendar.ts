import { Theme } from '@emotion/react';
import { DayContentDataItem } from '@mms/mms-ui-library/components/ContentCalendar';
import { ThemeMode } from '@mms/mms-ui-library/theme';

import {
	darkBackgrounds,
	DEFAULT_BACKGROUND,
	DEFAULT_TRACKED_CONTENT,
	MAX_HOURS_PER_DAY,
	MINUTES_IN_HOUR,
	OPACITY_BACKGROUND_DARK,
	OPACITY_BACKGROUND_LIGHT,
} from '../constants';

import { addOpacityToColor } from '@/helpers/addOpacityToColor';
import { minutesToHours } from '@/helpers/minutesToHours';
import { TimesheetCalendarResponse } from '@/queries/timesheet/types';

const getBackgroundByMinutes = (
	factTrackedSum: number,
	theme: Theme
): string => {
	const hours = factTrackedSum / MINUTES_IN_HOUR;
	const isDarkTheme = theme.mode === ThemeMode.DARK;
	const backgroudOpacity = isDarkTheme
		? OPACITY_BACKGROUND_DARK
		: OPACITY_BACKGROUND_LIGHT;

	const backgroundStatuses = {
		high: isDarkTheme
			? darkBackgrounds.hight
			: addOpacityToColor(theme.palette['text-violet'], backgroudOpacity),
		low: isDarkTheme
			? darkBackgrounds.low
			: addOpacityToColor(theme.palette['graphics-error'], backgroudOpacity),
		equal: isDarkTheme
			? darkBackgrounds.equal
			: addOpacityToColor(theme.palette['feedback-success'], backgroudOpacity),
	};

	if (hours > MAX_HOURS_PER_DAY) {
		return backgroundStatuses.high;
	}

	if (hours < MAX_HOURS_PER_DAY) {
		return backgroundStatuses.low;
	}

	return backgroundStatuses.equal;
};

export const convertDataForCalendar = (
	data: TimesheetCalendarResponse[],
	theme: Theme
): DayContentDataItem[] => {
	const today = new Date().toISOString().split('T')[0];

	const hasTodayInData = data.some((item) => item.date === today);

	const convertedData = data.map(({ date, factTrackedSum }) => {
		const background = getBackgroundByMinutes(factTrackedSum, theme);

		return {
			date,
			content:
				factTrackedSum > 0
					? minutesToHours(factTrackedSum, true)
					: DEFAULT_TRACKED_CONTENT,
			background,
		};
	});

	if (!hasTodayInData) {
		convertedData.push({
			date: today,
			content: DEFAULT_TRACKED_CONTENT,
			background: DEFAULT_BACKGROUND,
		});
	}

	return convertedData;
};
