import { getEmployeeOptions } from '@/helpers/getEmployeeOptions';

import { lookupUserList } from './mocks/lookupUsers';

describe('Util function getEmployeeOptions', () => {
	it('Should return empty array if isFetching equals true', () => {
		const result = getEmployeeOptions(lookupUserList, true);
		expect(result).toBeInstanceOf(Array);
		expect(result).toHaveLength(0);
	});

	it('Should return empty array when receives empty array as data argument', () => {
		const result = getEmployeeOptions([], true);
		expect(result).toBeInstanceOf(Array);
		expect(result).toHaveLength(0);
	});

	it('Should return data formatted for employeeOptions if data not empty array and isFetching is false', () => {
		const result = getEmployeeOptions(lookupUserList, false);
		expect(result).toBeInstanceOf(Array);
		expect(result).toHaveLength(lookupUserList.length);

		for (let i = 0; i < lookupUserList.length; i += 1) {
			expect(result[i].id).toEqual(lookupUserList[i].id);
			expect(result[i].name).toEqual(
				`${lookupUserList[i].lastName} ${lookupUserList[i].firstName}`
			);
			expect(result[i].positionDescription).toEqual(
				lookupUserList[i].positionDescription
			);
		}
	});
});
