import { SerializedStyles, Theme } from '@emotion/react';
import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { Column, SortColumn } from 'react-data-grid';

import { PerformanceReviewGridRow } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/types';

export interface PerformanceReviewTableProps {
	dataGridColumns: Column<PerformanceReviewGridRow>[];
	sortColumns: SortColumn[];
	dataGridRows: PerformanceReviewGridRow[];
	overrideGridStyles: (theme: Theme) => SerializedStyles;
	handleSortColumnChange:
		| ((sortColumns: SortColumn[]) => void)
		| undefined
		| null;
	gridPagination?: GridPaginationProps;
}
