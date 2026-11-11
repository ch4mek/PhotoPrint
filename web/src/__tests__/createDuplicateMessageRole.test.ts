import { createDuplicateMessageRole } from '@/helpers/createDuplicateMessageRole';

import { Configuration } from './mocks/Configuration';

const {
	configurableValues: { departmentTypes },
} = Configuration;

describe('Helper function createDuplicateMessageRole', () => {
	test.each(departmentTypes)(
		'Should return positionTitle value if positionTitle is not HeadOfDepartment',
		(department) => {
			expect(
				createDuplicateMessageRole('CEO', department.shortName, departmentTypes)
			).toBe('CEO');
			expect(
				createDuplicateMessageRole('CTO', department.shortName, departmentTypes)
			).toBe('CTO');
		}
	);

	test.each(departmentTypes)(
		'Should return phrase "Head of <department name> department" if positionTitle is HeadOfDepartment',
		(department) => {
			expect(
				createDuplicateMessageRole(
					'HeadOfDepartment',
					department.shortName,
					departmentTypes
				)
			).toBe(`Head of ${department.fullName} department`);
		}
	);
});
