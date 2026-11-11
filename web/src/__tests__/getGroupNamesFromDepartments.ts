import { Departments } from '@/constants/departments';
import { getGroupNamesFromDepartments } from '@/helpers/getGroupNamesFromDepartments';
import { DepartmentGroup } from '@/types/department';

import { departmentsResponseMock } from './mocks/departmentsResponse';

const ALL_GROUPS_SORTED: Array<DepartmentGroup> = departmentsResponseMock
	.reduce(
		(allGroups: Array<DepartmentGroup>, { groups }) => [
			...allGroups,
			...(groups ?? []),
		],
		[]
	)
	.sort(({ id: leftId, id: rightId }) => leftId - rightId);

const USER_DEPARTMENT = Departments.Js;

describe('Util function getGroupNamesFromDepartments', () => {
	it('Should return all unique group names', () => {
		const groupNames = getGroupNamesFromDepartments(departmentsResponseMock);
		expect(groupNames).toEqual(ALL_GROUPS_SORTED.map(({ name }) => name));
	});

	it('Should return only department groups', () => {
		const groupNames = getGroupNamesFromDepartments(
			departmentsResponseMock,
			USER_DEPARTMENT
		);
		expect(groupNames).toEqual(departmentsResponseMock[0].groups);
	});
});
