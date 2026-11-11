import { areFromToCompatibleObjectsEqual } from '@/helpers/areValuesEqual';
import { isDateRangeApplyDisabled } from '@/helpers/isDateRangeApplyDisabled';
import { isDateRangeValid } from '@/helpers/isDateRangeValid';

jest.mock('@/helpers/isDateRangeValid', () => ({
	isDateRangeValid: jest.fn(),
}));

jest.mock('@/helpers/areValuesEqual', () => ({
	areFromToCompatibleObjectsEqual: jest.fn(),
}));

describe('Helper function isDateRangeApplyDisabled', () => {
	const mockIsDateRangeValid = isDateRangeValid as jest.MockedFunction<
		typeof isDateRangeValid
	>;
	const mockAreFromToCompatibleObjectsEqual =
		areFromToCompatibleObjectsEqual as jest.MockedFunction<
			typeof areFromToCompatibleObjectsEqual
		>;

	it('returns true if current range equals previous', () => {
		const selection = {
			from: new Date('2023-01-01'),
			to: new Date('2023-01-02'),
		};
		const prevSelection = {
			from: new Date('2023-01-01'),
			to: new Date('2023-01-02'),
		};

		mockIsDateRangeValid.mockReturnValue(true);
		mockAreFromToCompatibleObjectsEqual.mockReturnValue(true);

		const result = isDateRangeApplyDisabled(prevSelection, selection);

		expect(mockAreFromToCompatibleObjectsEqual).toHaveBeenCalledWith(
			selection,
			prevSelection
		);
		expect(result).toBe(true);
	});

	it('returns true when both dates are set and range is invalid', () => {
		const from = new Date('2023-01-02');
		const to = new Date('2023-01-01');
		const selection = { from, to };
		const prevSelection = { from, to };

		mockAreFromToCompatibleObjectsEqual.mockReturnValue(false);
		mockIsDateRangeValid.mockReturnValue(false);

		const result = isDateRangeApplyDisabled(selection, prevSelection);

		expect(mockIsDateRangeValid).toHaveBeenCalledWith(from, to, true);
		expect(result).toBe(true);
	});

	it('returns false when both dates are set and range is valid', () => {
		const from = new Date('2023-01-01');
		const to = new Date('2023-01-02');
		const selection = { from, to };
		const prevSelection = {
			from: new Date('2023-02-01'),
			to: new Date('2023-02-02'),
		};

		mockAreFromToCompatibleObjectsEqual.mockReturnValue(false);
		mockIsDateRangeValid.mockReturnValue(true);

		const result = isDateRangeApplyDisabled(selection, prevSelection);

		expect(mockIsDateRangeValid).toHaveBeenCalledWith(from, to, true);
		expect(result).toBe(false);
	});

	it('returns true when only from is set', () => {
		const from = new Date('2023-01-01');
		const selection = { from, to: null };
		const prevSelection = { from: null, to: null };

		const result = isDateRangeApplyDisabled(selection, prevSelection);

		expect(result).toBe(true);
	});

	it('returns true when only to is set', () => {
		const to = new Date('2023-01-01');
		const selection = { from: null, to };
		const prevSelection = { from: null, to: null };

		const result = isDateRangeApplyDisabled(selection, prevSelection);

		expect(result).toBe(true);
	});

	it('returns true when both from and to are empty', () => {
		const selection = { from: null, to: null };
		const prevSelection = { from: undefined, to: undefined };

		const result = isDateRangeApplyDisabled(selection, prevSelection);

		expect(result).toBe(true);
	});
});
