import { isDateMoreThanCurrentDate } from '@/forms/EquipmentForm/EquipmentUsers/UserForm/utils/isDateMoreThanCurrentDate';
import dayjs from '@/lib/dayjs';

describe('isDateMoreThanCurrentDate function should works correct', () => {
	test('Should return true, if the date more than the current date', () => {
		const futureDate = dayjs().add(1, 'day').toDate();
		expect(isDateMoreThanCurrentDate(futureDate)).toBe(true);
	});

	test('Should return false if the date is less than the current date', () => {
		const pastDate = dayjs().subtract(1, 'day').toDate();
		expect(isDateMoreThanCurrentDate(pastDate)).toBe(false);
	});

	test('Should return false if the date is equal to the current date', () => {
		const currentDate = dayjs().toDate();
		expect(isDateMoreThanCurrentDate(currentDate)).toBe(false);
	});
});
