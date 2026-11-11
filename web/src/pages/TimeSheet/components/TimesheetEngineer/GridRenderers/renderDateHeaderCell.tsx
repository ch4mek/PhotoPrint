import { Typography } from '@mms/mms-ui-library/components/Typography';
import i18next from 'i18next';

import { DayHeaderWrapper } from '../../TimeSheeetManager/styles';
import { TIMESHEET_DATE_FORMAT } from '../constants';

import { formatMonthShort, formatWeekdayShort } from '@/helpers/formatRuDays';
import { getDateLocale } from '@/i18n/locale';
import dayjs from '@/lib/dayjs';
import { RenderHeaderCellPropsWithHoverIndex } from '@/types/columnHoverTypes';

const locale = getDateLocale(i18next.language);
export function renderWeekHeaderCell<T>(
	props: RenderHeaderCellPropsWithHoverIndex<T, unknown>
) {
	const {
		column: { key, hoverIndex },
	} = props;

	const date = dayjs(key, TIMESHEET_DATE_FORMAT).toDate();

	const weekDay = formatWeekdayShort(date, locale);
	const month = formatMonthShort(date, locale);
	const day = date.toLocaleDateString(locale, { day: 'numeric' });

	return (
		<DayHeaderWrapper data-hovercolindex={hoverIndex}>
			<Typography align="center" variant="s-400">
				{weekDay}, {month} {day}
			</Typography>
		</DayHeaderWrapper>
	);
}

export function renderMonthHeaderCell<T>(
	props: RenderHeaderCellPropsWithHoverIndex<T, unknown>,
	hideWeekDay: boolean = false
) {
	const {
		column: { key, hoverIndex },
	} = props;

	const date = dayjs(key, TIMESHEET_DATE_FORMAT).toDate();

	const weekDay = formatWeekdayShort(date, locale);
	const day = date.toLocaleDateString(locale, { day: 'numeric' });

	return (
		<DayHeaderWrapper data-hovercolindex={hoverIndex}>
			<Typography align="center" variant="s-600">
				{hideWeekDay ? day : `${weekDay} ${day}`}
			</Typography>
		</DayHeaderWrapper>
	);
}
