import { EMPTY_GRID_CELL_VALUE } from '@/components/Grid/constants';
import { PerformanceReviewItemResponse } from '@/queries/performanceReview/useGetPerformanceReview.types';
import { DepartmentType } from '@/types/configuration';

export const generateDataGridRows = (
	departmentTypes: DepartmentType[],
	items?: PerformanceReviewItemResponse[]
) => {
	if (!items?.length) {
		return [];
	}

	return items.map((performanceData) => ({
		...performanceData,
		departmentfullname:
			departmentTypes.find(
				({ id: departmentId }) => performanceData.departmentId === departmentId
			)?.fullName ?? EMPTY_GRID_CELL_VALUE,
	}));
};
