import * as rolesModule from '@/constants/roles';
import { getAvailablePositions } from '@/helpers/getAvailablePosistions';
import { PersistentConfigurationItem } from '@/types/configuration';

const { Roles, chiefRoles } = rolesModule;

describe('Helper function getAvailablePositions', () => {
	const departments: PersistentConfigurationItem[] = [
		{
			id: Roles.HD,
			name: 'Head Department',
			description: '',
			location: undefined,
		},
		{
			id: Roles.SE,
			name: 'Software Engineering',
			description: '',
			location: undefined,
		},
		{
			id: Roles.GM,
			name: 'Group Manager',
			description: '',
			location: undefined,
		},
		{ id: Roles.SA, name: 'Sys Admin', description: '', location: undefined },
	];

	beforeEach(() => {
		jest.restoreAllMocks();
	});

	it('should return empty array if allDepartments is undefined', () => {
		expect(getAvailablePositions(undefined, Roles.HD, '1')).toEqual([]);
	});

	it('should return all departments for chief roles', () => {
		expect(getAvailablePositions(departments, chiefRoles[0], '1')).toEqual(
			departments
		);
	});

	it('should return all departments if user is sys admin', () => {
		jest.spyOn(rolesModule, 'isSysAdmin').mockReturnValue(true);
		expect(getAvailablePositions(departments, Roles.HD, '1')).toEqual(
			departments
		);
	});

	it('should return only HD and SE for regular user', () => {
		jest.spyOn(rolesModule, 'isSysAdmin').mockReturnValue(false);
		jest.spyOn(rolesModule, 'isHeadOrGroupManager').mockReturnValue(false);

		const result = getAvailablePositions(departments, Roles.HD, '1');
		expect(result.map((d) => d.id)).toEqual([Roles.HD, Roles.SE]);
	});

	it('should include GM for head or group manager', () => {
		jest.spyOn(rolesModule, 'isSysAdmin').mockReturnValue(false);
		jest.spyOn(rolesModule, 'isHeadOrGroupManager').mockReturnValue(true);

		const result = getAvailablePositions(departments, Roles.GM, 'Dept1');
		expect(result.map((d) => d.id)).toEqual([Roles.HD, Roles.SE, Roles.GM]);
	});

	it('should include SA if head/group manager and department is SysAdmin', () => {
		jest.spyOn(rolesModule, 'isSysAdmin').mockReturnValue(false);
		jest.spyOn(rolesModule, 'isHeadOrGroupManager').mockReturnValue(true);

		const result = getAvailablePositions(departments, Roles.GM, 'SysAdmin');
		expect(result.map((d) => d.id)).toEqual([
			Roles.HD,
			Roles.SE,
			Roles.GM,
			Roles.SA,
		]);
	});
});
