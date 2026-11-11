import { memo, useMemo, useState } from 'react';

import { CollapsibleTable } from '@/components/CollapsibleTable';
import { CollapsibleTableBackground } from '@/components/CollapsibleTable/styles';
import { date } from '@/constants/date';
import { INITIAL_PAGE_NUMBER } from '@/constants/pagination';
import { isRegularEmployee } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useTableState } from '@/pages/hooks/useTableState';
import { UserReportsRequest } from '@/queries/types';
import { useGetUserReports } from '@/queries/useGetUserReports';

import { ReportsUserTableRow } from './ReportsUserTableRow';
import type { ReportsTableProps } from './types';

function ReportsCollapsibleTableComponent({
	queryParams,
	reports,
}: ReportsTableProps) {
	const {
		year,
		userId,
		monthOfYear,
		projectId,
		clientId,
		departmentId,
		pageNo,
		pageSize,
		trackedTimeType,
	} = queryParams;

	const [page, setPage] = useState(INITIAL_PAGE_NUMBER);
	const { rowsPerPage, setRowsPerPage } = useTableState();

	const { role } = useAuth();
	const isEmployeeRole = isRegularEmployee(role);

	const params: UserReportsRequest = useMemo(
		() => ({
			...queryParams,
			year: !year ? date.getUTCFullYear() : year,
			page: pageNo || page + 1,
			pageSize: pageSize || rowsPerPage,
		}),
		[
			queryParams,
			userId,
			year,
			monthOfYear,
			projectId,
			clientId,
			departmentId,
			trackedTimeType,
			page,
			rowsPerPage,
			pageNo,
			pageSize,
		]
	);

	const { data: userReports, isFetching } = useGetUserReports({ ...params });

	if (!reports?.timePerPeriod.length || !userReports?.items?.length) {
		return null;
	}

	return (
		<CollapsibleTableBackground isEmployee={isEmployeeRole}>
			<CollapsibleTable
				setRowsPerPage={setRowsPerPage}
				page={page}
				setPage={setPage}
				rowsPerPage={Number(rowsPerPage)}
				totalCount={userReports.total}
			>
				{!isFetching &&
					userReports?.items.map((row) => (
						<ReportsUserTableRow
							row={row}
							key={row.userId}
							queryParams={queryParams}
						/>
					))}
			</CollapsibleTable>
		</CollapsibleTableBackground>
	);
}

export const ReportsCollapsibleTable = memo(ReportsCollapsibleTableComponent);
