import { Departments } from '@/constants/departments';
import { LookupUserResponse } from '@/types/userInfo';

export const lookupUserList: LookupUserResponse[] = [
	{
		id: 1,
		firstName: 'firstName',
		lastName: 'lastName',
		color: 'color',
		avatarFileId: 'fileId',
		positionDescription: 'Employee',
		isActive: true,
		departmentId: Departments.Net,
		resourceGroupId: 0,
		resourceGroupName: 'ASP.NET',
	},
	{
		id: 2,
		firstName: 'firstName',
		lastName: 'lastName',
		color: 'color',
		avatarFileId: 'fileId',
		positionDescription: 'Employee',
		isActive: false,
		departmentId: Departments.Js,
		resourceGroupId: 1,
		resourceGroupName: 'React',
	},
	{
		id: 3,
		firstName: 'firstName',
		lastName: 'lastName',
		color: 'color',
		avatarFileId: 'fileId',
		positionDescription: 'Employee',
		isActive: true,
		departmentId: Departments.Js,
		resourceGroupId: 1,
		resourceGroupName: 'React',
	},
];
