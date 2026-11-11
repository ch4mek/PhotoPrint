import { defaultPerformanceReviewFilterValues } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/constants';
import { PerformanceReviewTableFilters } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/types';

export const isResetDisabled = (filters: PerformanceReviewTableFilters) =>
	JSON.stringify(filters) ===
	JSON.stringify(defaultPerformanceReviewFilterValues);
