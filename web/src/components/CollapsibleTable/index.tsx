import { Pagination } from '@mms/mms-ui-library/components/Pagination';
import { useCallback } from 'react';

import { ROWS_PER_PAGE_OPTIONS } from '@/constants/pagination';
import { isRegularEmployee } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';

import {
	CollapsibleTableWrapper,
	CollapsibleTableList,
	PaginationWrapper,
} from './styles';
import type { BaseCollapsibleTableProps } from './types';

export function CollapsibleTable({
	totalCount,
	rowsPerPage,
	setRowsPerPage,
	page,
	setPage,
	children,
}: BaseCollapsibleTableProps) {
	const handleChangePage = useCallback(
		(currentPage: number) => {
			setPage(currentPage);
		},
		[page]
	);

	const handleChangeRowsPerPage = useCallback(
		(perPage: number) => {
			setRowsPerPage(perPage);
			setPage(0);
		},
		[rowsPerPage]
	);

	const { role } = useAuth();
	const isEmployeeRole = isRegularEmployee(role);

	return (
		<CollapsibleTableWrapper>
			<CollapsibleTableList
				aria-label="collapsible table"
				aria-labelledby="nested-list-subheader"
				isEmployee={isEmployeeRole}
			>
				{children}
			</CollapsibleTableList>
			{totalCount && !isEmployeeRole && (
				<PaginationWrapper>
					<Pagination
						perPageOptions={ROWS_PER_PAGE_OPTIONS}
						total={totalCount}
						perPage={rowsPerPage}
						currentPage={page}
						onCurrentPageChange={handleChangePage}
						onPerPageChange={handleChangeRowsPerPage}
					/>
				</PaginationWrapper>
			)}
		</CollapsibleTableWrapper>
	);
}
