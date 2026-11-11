import { getDepartmentTitle } from '@/helpers/getDepartmentTitle';

import { Configuration } from './mocks/Configuration';

const {
	configurableValues: { departmentTypes },
} = Configuration;

describe('Helper function getDepartmentTitle', () => {
	it('Should return null if department is null', () => {
		expect(getDepartmentTitle('CEO', null)).toBeNull();
		expect(getDepartmentTitle('CTO', null)).toBeNull();
		expect(getDepartmentTitle('HeadOfDepartment', null)).toBeNull();
		expect(getDepartmentTitle('Employee', null)).toBeNull();
		expect(getDepartmentTitle('SystemAdministrator', null)).toBeNull();
	});

	test.each(departmentTypes)(
		'Should return null if positionTitle is from chief roles (CEO/CTO) and department is of type string',
		(department) => {
			expect(getDepartmentTitle('CEO', department.shortName)).toBeNull();
			expect(getDepartmentTitle('CTO', department.shortName)).toBeNull();
		}
	);

	test.each(departmentTypes)(
		'Should return null if positionTitle is from chief roles (CEO/CTO) and department is of Configuration',
		(department) => {
			expect(getDepartmentTitle('CEO', department)).toBeNull();
			expect(getDepartmentTitle('CTO', department)).toBeNull();
		}
	);

	test.each(departmentTypes)(
		'Should return department value if positionTitle is not from chief roles (CEO/CTO) and department is of type string',
		(department) => {
			expect(getDepartmentTitle('HeadOfDepartment', department.shortName)).toBe(
				department.shortName
			);
			expect(getDepartmentTitle('Employee', department.shortName)).toBe(
				department.shortName
			);
			expect(
				getDepartmentTitle('SystemAdministrator', department.shortName)
			).toBe(department.shortName);
		}
	);

	test.each(departmentTypes)(
		'Should return department.shortName if positionTitle is not from chief roles (CEO/CTO) and department is of Configuration',
		(department) => {
			expect(getDepartmentTitle('HeadOfDepartment', department)).toBe(
				department.shortName
			);
			expect(getDepartmentTitle('Employee', department)).toBe(
				department.shortName
			);
			expect(getDepartmentTitle('SystemAdministrator', department)).toBe(
				department.shortName
			);
		}
	);
});
