import { normalizeUser } from '@/helpers/normalizeUser';

import { lookupUserList } from './mocks/lookupUsers';

describe('Helper function normalizeUser', () => {
	it('Should return empty array if isFetching is true and Users list is undefined', () => {
		const result = normalizeUser(undefined, true)();
		expect(result).toBeInstanceOf(Array);
		expect(result).toHaveLength(0);
	});

	it('Should return empty array if isFetching is true and Users list is given', () => {
		const result = normalizeUser(lookupUserList, true)();
		expect(result).toBeInstanceOf(Array);
		expect(result).toHaveLength(0);
	});

	it('Should return empty array if isFetching is true and empty Users list is given', () => {
		const result = normalizeUser([], true)();
		expect(result).toBeInstanceOf(Array);
		expect(result).toHaveLength(0);
	});

	it('Should return empty array if isFetching is false and empty Users list is given', () => {
		const result = normalizeUser([], false)();
		expect(result).toBeInstanceOf(Array);
		expect(result).toHaveLength(0);
	});

	it('Should return formatted array of users if isFetching is false and not empty Users list is given', () => {
		const result = normalizeUser(lookupUserList, false)();
		expect(result).toBeInstanceOf(Array);
		expect(result).toHaveLength(lookupUserList.length);

		for (let i = 0; i < lookupUserList.length; i += 1) {
			expect(result[i].id).toBe(lookupUserList[i].id);
			expect(result[i].name).toBe(
				`${lookupUserList[i].lastName} ${lookupUserList[i].firstName}`
			);
		}
	});
});
