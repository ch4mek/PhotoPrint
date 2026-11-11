import { Departments } from '@/constants/departments';
import { UserOnProject } from '@/types/userInfo';

export const mockUserOnProject: UserOnProject = {
	userId: 'user123',
	firstName: 'John',
	lastName: 'Doe',
	departmentId: Departments.Js,
};
