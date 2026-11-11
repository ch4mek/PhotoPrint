import { Departments } from '@/constants/departments';
import { isHeadOfDepartment, Roles } from '@/constants/roles';

export const isHeadOfLdDepartment = (
	role: Roles,
	departmentId: string | null | undefined
) => isHeadOfDepartment(role) && departmentId === Departments.LD;
