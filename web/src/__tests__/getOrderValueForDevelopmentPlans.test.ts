import {
	DEFAULT_PLANS_ORDER_BY_FIELD,
	DEADLINE_DATE_FIELD_NAME,
	ID_FIELD_NAME,
} from '@/constants/developmentPlan';
import { SortDirection } from '@/constants/sort';
import { capitalizeString } from '@/helpers/capitalizeString';
import { getOrderValueForDevelopmentPlans } from '@/helpers/getOrderValueForDevelopmentPlans';

jest.mock('@/helpers/capitalizeString', () => ({
	capitalizeString: jest.fn(
		(str) => str.charAt(0).toUpperCase() + str.slice(1)
	),
}));

describe('Helper function getOrderValueForDevelopmentPlans', () => {
	it('should return simple order for non-default fields', () => {
		const result = getOrderValueForDevelopmentPlans('name', SortDirection.ASC);

		expect(result).toBe('Name asc');
	});

	it('should return complex order for default field', () => {
		const result = getOrderValueForDevelopmentPlans(
			DEFAULT_PLANS_ORDER_BY_FIELD,
			SortDirection.DESC
		);

		const expectedResult = `${
			DEFAULT_PLANS_ORDER_BY_FIELD.charAt(0).toUpperCase() +
			DEFAULT_PLANS_ORDER_BY_FIELD.slice(1)
		} desc, ${
			DEADLINE_DATE_FIELD_NAME.charAt(0).toUpperCase() +
			DEADLINE_DATE_FIELD_NAME.slice(1)
		} asc, 
\t\t${ID_FIELD_NAME} desc`;

		expect(result).toBe(expectedResult);
	});

	it('should handle DESC direction for non-default fields', () => {
		const result = getOrderValueForDevelopmentPlans(
			'title',
			SortDirection.DESC
		);

		expect(result).toBe('Title desc');
	});

	it('should handle ASC direction for default field', () => {
		const result = getOrderValueForDevelopmentPlans(
			DEFAULT_PLANS_ORDER_BY_FIELD,
			SortDirection.ASC
		);

		const expectedResult = `${
			DEFAULT_PLANS_ORDER_BY_FIELD.charAt(0).toUpperCase() +
			DEFAULT_PLANS_ORDER_BY_FIELD.slice(1)
		} asc, ${
			DEADLINE_DATE_FIELD_NAME.charAt(0).toUpperCase() +
			DEADLINE_DATE_FIELD_NAME.slice(1)
		} asc, 
\t\t${ID_FIELD_NAME} desc`;

		expect(result).toBe(expectedResult);
	});

	it('should capitalize field names correctly', () => {
		getOrderValueForDevelopmentPlans('createdDate', SortDirection.ASC);

		expect(capitalizeString).toHaveBeenCalledWith('createdDate');
	});
});
