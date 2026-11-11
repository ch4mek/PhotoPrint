import { Departments } from '@/constants/departments';
import { checkIfUsersWithExtraDepartments } from '@/helpers/checkIfUsersWithExtraDepartments';
import { UserOnProject } from '@/types/userInfo';

describe('checkIfUsersWithExtraDepartments should works correct', () => {
	const usersList: UserOnProject[] = [
		{
			userId: 'user123',
			firstName: 'John',
			lastName: 'Doe',
			departmentId: Departments.Js,
		},
		{
			userId: 'user456',
			firstName: 'Jane',
			lastName: 'Smith',
			departmentId: Departments.Java,
		},
		{
			userId: 'user789',
			firstName: 'Alice',
			lastName: 'Johnson',
			departmentId: Departments.Java,
		},
	];
	const usersSameDeparmentList: UserOnProject[] = [
		{
			userId: 'user123',
			firstName: 'John',
			lastName: 'Doe',
			departmentId: Departments.Java,
		},
		{
			userId: 'user456',
			firstName: 'Jane',
			lastName: 'Smith',
			departmentId: Departments.Java,
		},
		{
			userId: 'user789',
			firstName: 'Alice',
			lastName: 'Johnson',
			departmentId: Departments.Java,
		},
	];

	it('Should return false when usersList is empty', () => {
		const result = checkIfUsersWithExtraDepartments([], Departments.Java);
		expect(result).toBe(false);
	});

	it('Should return false when department is null', () => {
		const result = checkIfUsersWithExtraDepartments(usersList, null);
		expect(result).toBe(false);
	});

	it('Should return false when all users have the same department', () => {
		const result = checkIfUsersWithExtraDepartments(
			usersSameDeparmentList,
			Departments.Java
		);
		expect(result).toBe(false);
	});

	it('Should return true when at least one user has a different department', () => {
		const result = checkIfUsersWithExtraDepartments(usersList, 'Js');
		expect(result).toBe(true);
	});
});
