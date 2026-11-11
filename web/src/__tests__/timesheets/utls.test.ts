import {
	isDateBeforePreviousMonth,
	isDateInPreviousMonth,
} from '@/pages/TimeSheet/components/TimesheetEngineer/utils';

const TODAY = '2023-12-15';

const DAY_IN_MONTH_FROM_PAST = '31.01.2001';
const DAY_IN_MONTH_BEFORE_PREVIOUS = '15.10.2023';
const DAY_IN_PREVIOUS_MONTH = '30.11.2023';

const DAY_IN_CURRENT_MONTH = '11.12.2023';

const DAY_IN_NEXT_MONTH = '01.01.2024';
const DAY_IN_FUTURE_MONTH = '11.12.2025';

describe('TimeSheet utils', () => {
	beforeEach(() => {
		jest.useFakeTimers().setSystemTime(new Date(TODAY));
	});

	afterEach(() => {
		jest.useRealTimers();
	});

	describe('isDateBeforePreviousMonth', () => {
		it('Should return true for a string representation of the date of the day in the past month', () => {
			expect(isDateBeforePreviousMonth(DAY_IN_MONTH_FROM_PAST)).toBe(true);
		});

		it('Should return true for a string representation of a date, the day of the month before the previous one', () => {
			expect(isDateBeforePreviousMonth(DAY_IN_MONTH_BEFORE_PREVIOUS)).toBe(
				true
			);
		});

		it('Should return false for a string representation of the date of the day in the previous month', () => {
			expect(isDateBeforePreviousMonth(DAY_IN_PREVIOUS_MONTH)).toBe(false);
		});

		it('Should return false for a string representation of the date of the day in the current month', () => {
			expect(isDateBeforePreviousMonth(DAY_IN_CURRENT_MONTH)).toBe(false);
		});

		it('Should return false for a string representation of the date of the day in the next month', () => {
			expect(isDateBeforePreviousMonth(DAY_IN_NEXT_MONTH)).toBe(false);
		});

		it('Should return false for a string representation of the date of the day in the future month', () => {
			expect(isDateBeforePreviousMonth(DAY_IN_FUTURE_MONTH)).toBe(false);
		});
	});

	describe('isDateInPreviousMonth', () => {
		it('Should return false for a string representation of the date of the day in the past month', () => {
			expect(isDateInPreviousMonth(DAY_IN_MONTH_FROM_PAST)).toBe(false);
		});

		it('Should return false for a string representation of a date, the day of the month before the previous one', () => {
			expect(isDateInPreviousMonth(DAY_IN_MONTH_BEFORE_PREVIOUS)).toBe(false);
		});

		it('Should return true for a string representation of the date of the day in the previous month', () => {
			expect(isDateInPreviousMonth(DAY_IN_PREVIOUS_MONTH)).toBe(true);
		});

		it('Should return false for a string representation of the date of the day in the current month', () => {
			expect(isDateInPreviousMonth(DAY_IN_CURRENT_MONTH)).toBe(false);
		});

		it('Should return false for a string representation of the date of the day in the next month', () => {
			expect(isDateInPreviousMonth(DAY_IN_NEXT_MONTH)).toBe(false);
		});

		it('Should return false for a string representation of the date of the day in the future month', () => {
			expect(isDateBeforePreviousMonth(DAY_IN_FUTURE_MONTH)).toBeFalsy();
		});
	});
});
