import { extraFiltersBy, NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { getResourceGroupFilterParam } from '@/helpers/getResourceGroupFilterParam';

const { All } = extraFiltersBy;

describe('Helper function getResourceGroupFilterParam', () => {
	it('should return undefined for null filter', () => {
		expect(getResourceGroupFilterParam(null)).toBeUndefined();
	});

	it('should return undefined for undefined filter', () => {
		expect(getResourceGroupFilterParam(undefined)).toBeUndefined();
	});

	it('should return undefined for All filter', () => {
		const allFilter = { id: All, name: 'All' };
		expect(getResourceGroupFilterParam(allFilter)).toBeUndefined();
	});

	it('should return 0 for NO_VALUE_FILTER_OPTION', () => {
		expect(getResourceGroupFilterParam(NO_VALUE_FILTER_OPTION)).toBe(0);
	});

	it('should return filter id for regular filter', () => {
		const filter = { id: 123, name: 'Test Group' };
		expect(getResourceGroupFilterParam(filter)).toBe(123);
	});

	it('should return string filter id', () => {
		const filter = { id: 'group-abc', name: 'String Group' };
		expect(getResourceGroupFilterParam(filter)).toBe('group-abc');
	});
});
