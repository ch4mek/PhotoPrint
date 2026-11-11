import { getDepartmentProfileInfoValue } from '@/helpers/getDepartmentProfileInfoValue';

describe('Helper function getDepartmentProfileInfoValue', () => {
	const departmentName = 'Department';
	const groupName = 'Department';
	const formatString = `${departmentName} (Group ${groupName})`;

	it('Should return string with department and group', () => {
		expect(getDepartmentProfileInfoValue(departmentName, groupName)).toBe(
			formatString
		);
	});

	it('should return only department', () => {
		expect(getDepartmentProfileInfoValue(departmentName, null)).toBe(
			departmentName
		);
	});
});
