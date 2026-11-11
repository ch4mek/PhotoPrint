import { filterByPeriod } from '@/constants/filters';
import { formatMonthLong } from '@/helpers/formatRuDays';
import dayjs from '@/lib/dayjs';
import {
	isCurrentMonth,
	selectValueByPeriod,
} from '@/pages/TimeSheet/components/TimesheetPageControls/utils';

const locale = 'en-US';

describe('selectValueByPeriod', () => {
	const currentWeek = dayjs().week();
	const weekDisplay = 'This Week';
	const currentMonth = dayjs().month();
	const currentYear = dayjs().year();

	it('returns isCurrentMonth result when periodFilter is not filterByPeriod.weekFilter', () => {
		const result = selectValueByPeriod(
			filterByPeriod.monthFilter,
			currentWeek,
			weekDisplay,
			currentMonth,
			currentYear,
			locale
		);

		const expected = isCurrentMonth(currentMonth, currentYear, locale);
		expect(result).toBe(expected);
	});

	it('should return "This Week" when periodFilter is "weekFilter" and current week matches', () => {
		const result = selectValueByPeriod(
			filterByPeriod.weekFilter,
			currentWeek,
			weekDisplay,
			currentMonth,
			currentYear,
			locale
		);
		expect(result).toEqual('This Week');
	});

	it('should return "This Month" when periodFilter is "monthFilter" and current month matches', () => {
		const result = selectValueByPeriod(
			filterByPeriod.monthFilter,
			currentWeek,
			'This Month',
			currentMonth + 1,
			currentYear,
			locale
		);
		expect(result).toEqual('This Month');
	});

	it('should return the month name and date range when periodFilter is "monthFilter" and current month does not match', () => {
		const nextMonthDate = dayjs().add(1, 'M');
		const nextMonth = nextMonthDate.month() + 1;
		const nextYear = nextMonthDate.year();
		const result = selectValueByPeriod(
			filterByPeriod.monthFilter,
			currentWeek,
			weekDisplay,
			nextMonth,
			nextYear,
			locale
		);

		expect(result).toEqual(
			`${formatMonthLong(nextMonth, nextYear, locale)}, ${nextYear}`
		);
	});
});
