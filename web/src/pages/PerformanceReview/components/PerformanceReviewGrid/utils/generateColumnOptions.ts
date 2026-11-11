import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { PerformanceReviewOptions } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/types';
import { DepartmentType } from '@/types/configuration';
import { PerformanceReviewUserData } from '@/types/performanceReview';

export const generateColumnOptions = (
	departmentTypes: DepartmentType[],
	users: PerformanceReviewUserData[]
): PerformanceReviewOptions => ({
	users: users.map(({ fullName }) => fullName),
	departments: convertToSelectionModalOptions(
		[...departmentTypes, NO_VALUE_FILTER_OPTION],
		'fullName'
	),
});
