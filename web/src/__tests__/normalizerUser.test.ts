import { normalizeUser } from '@/helpers/normalizerUser';

describe('Helper function normalizeUser ', () => {
	it('should return empty array', () => {
		expect(normalizeUser(true, [])()).toEqual([]);
	});

	it('should return right User Model', () => {
		expect(
			normalizeUser(false, [
				{
					id: 1,
					firstName: 'FirstNameTest',
					lastName: 'LastNameTest',
				},
			])()
		).toEqual([
			{
				id: 1,
				name: 'LastNameTest FirstNameTest',
				description: 'LastNameTest FirstNameTest',
			},
		]);
	});
});
