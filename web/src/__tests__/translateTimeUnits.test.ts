import {
	getRemainingMinutes,
	minsToHrs,
	minsToHrsAndMins,
	minsToHrsAndMinsObject,
	minsToMinsRest,
} from '@/helpers/translateTimeUtils';

describe('Helper function translateTimeUnits', () => {
	it('Sould return correct amoun of hours', () => {
		expect(minsToHrs(100)).toBe(1);
	});
});

describe('Helper function getRemainingMinutes', () => {
	it('getRemainingMinutes should return correct amount oh minuts', () => {
		expect(getRemainingMinutes(0)).toBe(0);
		expect(getRemainingMinutes(30)).toBe(30);
		expect(getRemainingMinutes(100)).toBe(40);
	});
});

describe('Helper function minsToHrsAndMins', () => {
	it('Should works string(xx h xx m)', () => {
		expect(minsToHrsAndMins(0)).toBe('0 min');
		expect(minsToHrsAndMins(100)).toBe('1 h 40 min');
		expect(minsToHrsAndMins(60)).toBe('1 h 0 min');
		expect(minsToHrsAndMins(40)).toBe('40 min');
		expect(minsToHrsAndMins(120)).toBe('2 h 0 min');
	});
});

describe('Helper function minsToHrsAndMinsObject', () => {
	const resultWith0 = minsToHrsAndMinsObject(0);
	const resultWith30 = minsToHrsAndMinsObject(30);
	const resultWith60 = minsToHrsAndMinsObject(60);
	const resultWith90 = minsToHrsAndMinsObject(90);

	it('minsToHrsAndMinsObject should correct time object if input 0', () => {
		// 0 min is 0 hour and 00 min
		expect(resultWith0).toHaveProperty('hours');
		expect(resultWith0).toHaveProperty('minutes');
		expect(resultWith0.hours).toBe(0);
		expect(resultWith0.minutes).toBe(0);
	});

	it('minsToHrsAndMinsObject should correct time object if input 30', () => {
		// 30 min is 0 hour and 30 min
		expect(resultWith30).toHaveProperty('hours');
		expect(resultWith30).toHaveProperty('minutes');
		expect(resultWith30.hours).toBe(0);
		expect(resultWith30.minutes).toBe(30);
	});

	it('minsToHrsAndMinsObject should correct time object if input 60', () => {
		// 60 min is 1 hour and 00 min
		expect(resultWith60).toHaveProperty('hours');
		expect(resultWith60).toHaveProperty('minutes');
		expect(resultWith60.hours).toBe(1);
		expect(resultWith60.minutes).toBe(0);
	});

	it('minsToHrsAndMinsObject should correct time object if input 90', () => {
		// 90 min is 1 hour and 30 min
		expect(resultWith90).toHaveProperty('hours');
		expect(resultWith90).toHaveProperty('minutes');
		expect(resultWith90.hours).toBe(1);
		expect(resultWith90.minutes).toBe(30);
	});
});

describe('Helper function minsToMinsRest', () => {
	it('Should return correct amount oh minuts', () => {
		// 100 min is 1 h and  40 min, so result 40
		expect(minsToMinsRest(100)).toBe(40);
		expect(minsToMinsRest(30)).toBe(30);
		expect(minsToMinsRest(0)).toBe(0);
	});
});
