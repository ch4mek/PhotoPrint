import { minutesToHours, hoursToMinutes } from '@/helpers/minutesToHours';

describe('Helper function minutesToHours', () => {
	it('Should return correct time string', () => {
		expect(minutesToHours(0)).toBe('');
		expect(minutesToHours(5)).toBe('00:05');
		expect(minutesToHours(60)).toBe('01:00');
		expect(minutesToHours(120)).toBe('02:00');
		expect(minutesToHours(40)).toBe('00:40');
		expect(minutesToHours(100)).toBe('01:40');
		expect(minutesToHours(60000000)).toBe('1000000:00');
	});
});

describe('Helper function hoursToMinutes', () => {
	it('Should return correct amount of minutes with corect time string', () => {
		expect(hoursToMinutes('00:00')).toBe(0);
		expect(hoursToMinutes('01:00')).toBe(60);
		expect(hoursToMinutes('02:00')).toBe(120);
		expect(hoursToMinutes('00:40')).toBe(40);
		expect(hoursToMinutes('01:40')).toBe(100);
		expect(hoursToMinutes('1000000:00')).toBe(60000000);
	});

	it('Should return correct amount of minutes with incorect time string', () => {
		expect(hoursToMinutes('1000000:')).toBe(60000000);
		expect(hoursToMinutes(':10')).toBe(10);
	});

	it('Should return 0 with empty string', () => {
		expect(hoursToMinutes('')).toBe(0);
	});
});
