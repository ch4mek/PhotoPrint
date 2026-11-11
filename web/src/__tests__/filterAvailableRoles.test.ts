import { Roles } from '@/constants/roles';
import { filterAvailableRoles } from '@/helpers/filterAvailableRoles';
import { RoleType } from '@/types/configuration';

const { CEO, CTO, SE } = Roles;
const roles: Array<RoleType> = [
	{
		description: '',
		id: CEO,
		name: CEO,
		location: undefined,
	},
	{
		description: '',
		id: CTO,
		name: CTO,
		location: undefined,
	},
	{
		description: '',
		id: SE,
		name: SE,
		location: undefined,
	},
];

const roleRestrictions: Record<string, string[]> = {
	[CEO]: [CEO],
	[CTO]: [CEO, CTO],
};

describe('Helper function filterAvailableRoles', () => {
	it('should return roles excluding restricted ones for CEO', () => {
		const result = filterAvailableRoles(roles, CEO, roleRestrictions);

		expect(result).toHaveLength(2);
		expect(result).toEqual([
			{ description: '', id: 'CTO', name: 'CTO' },
			{ description: '', id: 'Employee', name: 'Employee' },
		]);
	});

	it('should return roles excluding restricted ones for CTO', () => {
		const result = filterAvailableRoles(roles, CTO, roleRestrictions);

		expect(result).toHaveLength(1);
		expect(result).toEqual([
			{ description: '', id: 'Employee', name: 'Employee' },
		]);
	});

	it('should return all roles if position is not restricted', () => {
		const result = filterAvailableRoles(roles, Roles.SE, roleRestrictions);

		expect(result).toHaveLength(3);
		expect(result).toEqual(roles);
	});
});
