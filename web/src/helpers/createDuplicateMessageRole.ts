import { Roles } from '@/constants/roles';
import { DepartmentType } from '@/types/configuration';

export const createDuplicateMessageRole = (
	positionTitle: string,
	departmentTitle: string | null,
	departments: Array<DepartmentType>
): string =>
	positionTitle !== Roles.HD
		? positionTitle
		: `Head of ${
				departments.find(
					(department) => department.shortName === departmentTitle
				)?.fullName
		  } department`;
