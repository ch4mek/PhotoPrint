import { timesheetModesIds } from '@/pages/TimeSheet/constants';
import { DepartmentType } from '@/types/configuration';

const initialDepartmentsFilterOption: DepartmentType = {
	id: timesheetModesIds.departmentModeId,
	fullName: timesheetModesIds.departmentModeId,
	shortName: timesheetModesIds.departmentModeId,
};

export const expandDepartmentsData = (data?: Array<DepartmentType>) => {
	if (data) {
		const dataCopy = [...data];

		if (
			!dataCopy.find(
				(el: { id: string | number }) =>
					el.id === initialDepartmentsFilterOption.id
			)
		) {
			dataCopy.unshift(initialDepartmentsFilterOption);
		}

		return dataCopy;
	}

	return null;
};
