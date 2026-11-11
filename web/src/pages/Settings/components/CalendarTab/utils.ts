import { CURRENT_YEAR } from '@/constants/date';
import { SETTINGS_YEAR_FILTER_STORAGE_KEY } from '@/constants/storageKeys';
import { getItemFromStorage } from '@/helpers/storage';
import {
	HOVER_NAME_CELL_CLASS_NAME,
	HOVER_ROW_CELL_CLASS_NAME,
	HOVER_TOTAL_CELL_CLASS_NAME,
} from '@/pages/TimeSheet/components/TimeSheeetManager/constants';
import { getTimesheetColumnClassname } from '@/pages/TimeSheet/utils/getTimesheetColumnClassname';
import {
	CalendarCountries,
	WorkingHoursCalendarRow,
	WorkingHoursCalendarRowType,
	WorkingHoursResponse,
} from '@/types/settings';

import {
	CALENDAR_COUNTRY_COLUMN_CELL_CLASS,
	CALENDAR_TOTAL_COLUMN_CELL_CLASS,
} from './constants';

export const convertWorkingHoursResponseToRows = (
	data: WorkingHoursResponse,
	countries: CalendarCountries
): Array<WorkingHoursCalendarRow> => {
	const countryRows = Object.entries(countries).reduce<
		Array<WorkingHoursCalendarRow>
	>((acc, [countryId, countryName]) => {
		const countryHours = data.hoursPerMonth.filter(
			(month) => String(month.countryId) === countryId
		);

		return [
			...acc,
			{
				disabled: false,
				type: WorkingHoursCalendarRowType.CountryRow,
				countryName,
				countryId: Number(countryId),
				timeMap: new Map(
					countryHours.map(({ monthOfYear, id, hours }) => [
						monthOfYear,
						{ id, hours, isLoading: false, hasError: false },
					])
				),
			},
		];
	}, []);

	return countryRows;
};

export const getInitialYearFilterValue = (userId: string | number) => {
	const yearValue = getItemFromStorage(
		`${SETTINGS_YEAR_FILTER_STORAGE_KEY}/${userId}`
	);

	return yearValue ? +yearValue : CURRENT_YEAR;
};

export const getCountryNameCellClassname = (
	type: WorkingHoursCalendarRowType
) =>
	`${CALENDAR_COUNTRY_COLUMN_CELL_CLASS} ${HOVER_NAME_CELL_CLASS_NAME} ${
		type !== WorkingHoursCalendarRowType.AddCountryButtonRow &&
		HOVER_ROW_CELL_CLASS_NAME
	}`;

export const getCalendarCellClassname = (
	index: number,
	type: WorkingHoursCalendarRowType
) =>
	`${
		type !== WorkingHoursCalendarRowType.AddCountryButtonRow &&
		HOVER_ROW_CELL_CLASS_NAME
	} ${getTimesheetColumnClassname(index)}`;

export const getTotalCellClassname = (type: WorkingHoursCalendarRowType) =>
	`${CALENDAR_TOTAL_COLUMN_CELL_CLASS} ${
		type !== WorkingHoursCalendarRowType.AddCountryButtonRow &&
		HOVER_ROW_CELL_CLASS_NAME
	} ${HOVER_TOTAL_CELL_CLASS_NAME}`;
