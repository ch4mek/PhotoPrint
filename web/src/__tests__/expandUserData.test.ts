import { Departments } from '@/constants/departments';
import { expandUsersData } from '@/helpers/expandUsersData';
import { LookupUserResponse } from '@/types/userInfo';

import { lookupUserList } from './mocks/lookupUsers';

describe('Helper function expandUserData', () => {
	const resultOne = expandUsersData(lookupUserList, null) ?? [];
	const resultTwo = expandUsersData(lookupUserList, Departments.Java) ?? [];

	it('Should same users without departments if specificDepartment is null', () => {
		expect(resultOne).toBeDefined();
		expect(resultOne).toHaveLength(3);
		expect(resultOne[0]).toHaveProperty('id');
		expect(resultOne[0]).toHaveProperty('firstName');
		expect(resultOne[0]).toHaveProperty('lastName');
		expect(resultOne[0]).toHaveProperty('color');
		expect(resultOne[0]).toHaveProperty('avatarFileId');
	});

	it('Should same users with departments if specificDepartment is Departments', () => {
		expect(resultTwo).toBeDefined();
		expect(resultTwo).toHaveLength(3);
		expect(resultTwo[0]).toHaveProperty('id');
		expect(resultTwo[0]).toHaveProperty('firstName');
		expect(resultTwo[0]).toHaveProperty('lastName');
		expect(resultTwo[0]).toHaveProperty('color');
		expect(resultTwo[0]).toHaveProperty('avatarFileId');
		expect(resultTwo[0]).toHaveProperty('departmentId');
		expect(
			(resultTwo[0] as LookupUserResponse & { departmentId: string })
				.departmentId
		).toBe(Departments.Java);
	});

	it('Should return null if all params undefined or null', () => {
		expect(expandUsersData(undefined, null)).toBeNull();
	});
});
