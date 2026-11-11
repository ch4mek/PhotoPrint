import { Departments } from '@/constants/departments';
import { Roles } from '@/constants/roles';
import { checkIfProjectBlockedToShow } from '@/helpers/checkIfProjectBlockedToShow';
import { ProjectEntity } from '@/types/project';

import { mockUserOnProject } from './mocks/entities';

describe('Validation function checkIfProjectBlockedToShow', () => {
	const mockUser = {
		role: Roles.HD,
		department: Departments.Js,
	};

	const project: ProjectEntity = {
		id: 1,
		name: 'Project A',
		color: 'blue',
		isBillable: true,
		isActive: true,
		isPublic: true,
		projectUsers: [],
		clientName: 'Client Name',
		trackedTime: 0,
	};

	it('Should return false when user role is HD and project is public', () => {
		const result = checkIfProjectBlockedToShow(
			mockUser.role,
			mockUser.department,
			project
		);
		expect(result).toBe(false);
	});

	it('Should return false when user role is not HD and project is not public', () => {
		const testProject: ProjectEntity = {
			...project,
			isPublic: false,
			projectUsers: [
				mockUserOnProject,
				{
					userId: 'user456',
					firstName: 'Jane',
					lastName: 'Smith',
					departmentId: Departments.QA,
				},
			],
		};
		const result = checkIfProjectBlockedToShow(
			Roles.CEO,
			mockUser.department,
			testProject
		);
		expect(result).toBe(false);
	});

	it('Should return false when user department is in projectUsers', () => {
		const testProject: ProjectEntity = {
			...project,
			isBillable: false,
			isPublic: false,
			projectUsers: [mockUserOnProject],
		};
		const result = checkIfProjectBlockedToShow(
			mockUser.role,
			Departments.Js,
			testProject
		);
		expect(result).toBe(false);
	});

	it('Should return true when user role is HD, project is not public, project is not created by current user, and user department is not in projectUsers', () => {
		const testProject: ProjectEntity = {
			...project,
			isPublic: false,
			projectUsers: [{ ...mockUserOnProject, departmentId: Departments.Net }],
		};
		const result = checkIfProjectBlockedToShow(
			mockUser.role,
			mockUser.department,
			testProject
		);
		expect(result).toBe(true);
	});
});
