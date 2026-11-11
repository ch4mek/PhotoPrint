import { Departments } from '@/constants/departments';
import { DepartmentResponse } from '@/types/department';

export const departmentsResponseMock: Array<DepartmentResponse> = [
	{
		departmentName: Departments.Js,
		headOfDepartmentId: '1',
		groups: [
			{
				name: 'React',
				id: 1,
				groupManagerEmail: 'react-gm@modsen-software.com',
			},
			{
				name: 'Angular',
				id: 5,
				groupManagerEmail: 'angular-gm@modsen-software.com',
			},
		],
	},
	{
		departmentName: Departments.Net,
		headOfDepartmentId: '2',
		groups: [
			{
				name: 'Net-Minsk',
				id: 2,
				groupManagerEmail: 'net-minsk-gm@modsen-software.com',
			},
			{
				name: 'Net-Warsaw',
				id: 3,
				groupManagerEmail: 'net-warsaw-gm@modsen-software.com',
			},
		],
	},
	{
		departmentName: Departments.Java,
		headOfDepartmentId: '3',
		groups: [
			{
				name: 'Spring',
				id: 4,
				groupManagerEmail: 'spring-gm@modsen-software.com',
			},
		],
	},
	{
		departmentName: Departments.PHP,
		headOfDepartmentId: '4',
		groups: [],
	},
];
