import { isCEO, isCTO, Roles } from '@/constants/roles';

const { CEO, CTO, SE, HD, SA } = Roles;

describe('Helper function isCEO', () => {
	it('Should return true if argument is CEO role', () => {
		expect(isCEO(CEO)).toBe(true);
	});

	it('Should return false if argument is role except CEO', () => {
		expect(isCEO(CTO)).toBe(false);
		expect(isCEO(SE)).toBe(false);
		expect(isCEO(HD)).toBe(false);
		expect(isCEO(SA)).toBe(false);
	});

	it('Should return false is argument is undefined', () => {
		expect(isCEO(undefined as unknown as Roles)).toBe(false);
	});
});

describe('Helper function isCTO', () => {
	it('Should return true if argument is CEO role', () => {
		expect(isCTO(CTO)).toBe(true);
	});

	it('hould return false if argument is role except CTO', () => {
		expect(isCTO(CEO)).toBe(false);
		expect(isCTO(SE)).toBe(false);
		expect(isCTO(HD)).toBe(false);
		expect(isCTO(SA)).toBe(false);
	});

	it('Should return false if argument is undefined', () => {
		expect(isCTO(undefined as unknown as Roles)).toBe(false);
	});
});
