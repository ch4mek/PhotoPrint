import { holidaysColumns, holidaysGridColumnWidths } from '../constants';
import { HolidaysGridColumnWidths } from '../types';

export const createHolidaysConfig = (): Record<
	keyof HolidaysGridColumnWidths,
	(typeof holidaysColumns)[keyof HolidaysGridColumnWidths] &
		HolidaysGridColumnWidths[keyof HolidaysGridColumnWidths]
> =>
	Object.fromEntries(
		(Object.keys(holidaysColumns) as Array<keyof HolidaysGridColumnWidths>).map(
			(key) => [
				key,
				{
					...holidaysColumns[key],
					...holidaysGridColumnWidths[key as keyof HolidaysGridColumnWidths],
				},
			]
		)
	) as Record<keyof HolidaysGridColumnWidths, any>;
