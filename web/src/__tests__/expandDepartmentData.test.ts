import { Departments } from '@/constants/departments';
import { expandDepartmentsData } from '@/helpers/expandDepartmentsData';
import { timesheetModesIds } from '@/pages/TimeSheet/constants';
import { DepartmentType } from '@/types/configuration';

const { departmentModeId } = timesheetModesIds;

const departmentList: Array<DepartmentType> = [
	{
		fullName: '',
		id: '',
		shortName: Departments.Java,
	},
	{
		fullName: '',
		id: '',
		shortName: Departments.Js,
	},
	{
		fullName: '',
		id: '',
		shortName: Departments.Net,
	},
	{
		fullName: '',
		id: '',
		shortName: Departments.Python,
	},
];

describe('Helper function expandDepartmentData', () => {
	const resultOne = expandDepartmentsData(departmentList) ?? [];

	const resultTwo = expandDepartmentsData(resultOne) ?? [];

	it("Should return array of departments with Department mode option if there wasn\\'t it", () => {
		expect(resultOne).toHaveLength(5);

		expect(resultOne[0].fullName).toBe(departmentModeId);

		expect(resultOne[0].id).toBe(departmentModeId);

		expect(resultOne[0].shortName).toBe(departmentModeId);
	});

	it('Should return array of departments if there was option All users', () => {
		expect(resultTwo).toHaveLength(5);

		expect(resultTwo[0].fullName).toBe(departmentModeId);

		expect(resultTwo[0].id).toBe(departmentModeId);

		expect(resultTwo[0].shortName).toBe(departmentModeId);
	});

	it('Should return null if department array is undefined', () => {
		expect(expandDepartmentsData(undefined)).toBeNull();
	});
});
