import { getAbsenceTrackingUnitTypeValue } from '@/helpers/absences';

describe('Helper function getAbsenceTrackingUnitTypeValue', () => {
	it('Should return days if countOfDays is provided (singular)', () => {
		const result = getAbsenceTrackingUnitTypeValue(1, null);
		expect(result).toEqual({
			quantityType: 'day',
			quantity: 1,
		});
	});

	it('Should return days if countOfDays > 1 (plural)', () => {
		const result = getAbsenceTrackingUnitTypeValue(3, null);
		expect(result).toEqual({
			quantityType: 'days',
			quantity: 3,
		});
	});

	it('Should return hours if countOfDays = null (singular)', () => {
		const result = getAbsenceTrackingUnitTypeValue(null, 1);
		expect(result).toEqual({
			quantityType: 'hour',
			quantity: 1,
		});
	});

	it('Should return hours if countOfDays = null and countOfHours > 1 (plural)', () => {
		const result = getAbsenceTrackingUnitTypeValue(null, 5);
		expect(result).toEqual({
			quantityType: 'hours',
			quantity: 5,
		});
	});

	it('Should return 0 hours if both values are null', () => {
		const result = getAbsenceTrackingUnitTypeValue(null, null);
		expect(result).toEqual({
			quantityType: 'hours',
			quantity: 0,
		});
	});
});
