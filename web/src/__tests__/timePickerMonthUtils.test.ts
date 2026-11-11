import dayjs from '@/lib/dayjs';
import {
	getCurrentMonth,
	modifyMonth,
} from '@/pages/TimeSheet/components/TimesheetPageControls/utils';

describe('Helper function modifyMonth', () => {
	it('Should return day from the next month', () => {
		const date = new Date(2023, 1, 1);
		expect(modifyMonth(date, 'add').getMonth()).toBe(date.getMonth() + 1);
	});

	it('Should return day from the next month and next year', () => {
		const date = new Date(2023, 11, 1);
		expect(modifyMonth(date, 'add').getMonth()).toBe(0);
		expect(modifyMonth(date, 'add').getFullYear()).toBe(2024);
	});

	it('Should return day from the previous month', () => {
		const date = new Date(2023, 1, 1);
		expect(modifyMonth(date, 'subtract').getMonth()).toBe(date.getMonth() - 1);
	});

	it('Should return day from the previous month and previous year', () => {
		const date = new Date(2023, 0, 1);
		expect(modifyMonth(date, 'subtract').getMonth()).toBe(11);
		expect(modifyMonth(date, 'subtract').getFullYear()).toBe(2022);
	});
});

describe('Helper function getCurrentMonth', () => {
	it('Should return month days', () => {
		const date = new Date(2023, 0, 1);
		const days = getCurrentMonth(date);
		days.forEach((day, index) => {
			expect(dayjs(day).month()).toBe(0);
			expect(dayjs(day).date()).toBe(1 + index);
		});
	});

	it('Should take current month as default and return month days', () => {
		const currentDate = new Date();
		const days = getCurrentMonth();
		days.forEach((day, index) => {
			expect(dayjs(day).month()).toBe(currentDate.getMonth());
			expect(dayjs(day).date()).toBe(1 + index);
		});
	});
});
