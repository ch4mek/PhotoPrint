import { isPMO } from '@/constants/departments';
import { isRegularEmployee, Roles } from '@/constants/roles';

export const isEmployeeOfPMO = (role: Roles, department?: string) =>
	isRegularEmployee(role) && isPMO(department);
