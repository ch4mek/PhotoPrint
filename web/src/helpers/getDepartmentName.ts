import { findById } from '@/helpers/findById';
import { DepartmentType } from '@/types/configuration';

export const getDepartmentName = (
	departments: Array<DepartmentType>,
	departmentId: string | null | undefined
): string => {
	if (!departmentId) {
		return '';
	}

	return findById(departments, departmentId)?.fullName ?? departmentId;
};
