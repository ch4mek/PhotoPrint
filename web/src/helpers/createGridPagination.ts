import { css, Theme } from '@emotion/react';
import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';

import { ROWS_PER_PAGE_OPTIONS } from '@/constants/pagination';

const overridePaginationContainerStyles = (theme: Theme) => css`
	padding: ${theme.spaces.s}px;

	span {
		font-size: 14px;
	}
`;

export const createGridPagination = (
	rowsLength: number,
	currentPage: number,
	onCurrentPageChange: (newPage: number) => void,
	rowsPerPage: number,
	onPerPageChange: (newPerPage: number) => void,
	total: number = 0,
	perPageOptions: number[] = ROWS_PER_PAGE_OPTIONS
): GridPaginationProps | undefined =>
	rowsLength
		? {
				currentPage,
				perPage: rowsPerPage,
				onPerPageChange,
				onCurrentPageChange,
				total,
				perPageOptions,
				overridePaginationContainerStyles,
		  }
		: undefined;
