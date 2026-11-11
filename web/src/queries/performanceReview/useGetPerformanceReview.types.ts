import { ByPageResponse } from '../types';

import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { SortDirection } from '@/constants/sort';
import { PerformanceReviewTableFilters } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/types';
import { PerformanceReviewDate } from '@/types/performanceReview';

export interface UseGetPerformanceReviewParams {
	top: number;
	page: number;
	order: SortDirection;
	orderBy: string;
	filters: PerformanceReviewTableFilters;
	allDepartments: Array<SelectionModalOption>;
	issuedOnly?: boolean;
}

export interface PerformanceReviewItemResponse {
	id: number;
	fullName: string;
	departmentId: string | null;
	dates: PerformanceReviewDate[];
}

export type GetPerformanceReviewResponse =
	ByPageResponse<PerformanceReviewItemResponse>;
