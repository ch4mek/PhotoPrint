import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import {
	memo,
	UIEvent,
	useCallback,
	useEffect,
	useLayoutEffect,
	useMemo,
} from 'react';

import { useFocusGridOnCurrentMonth } from '../../hooks/useFocusGridOnCurrentMonth';
import { getNewCellHeight } from '../../utils/getNewCellHeight';
import { getProjectsWithActiveAssigment } from '../../utils/getProjectsWithActiveAssigment';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { Loader } from '@/components/Loader';
import { HttpStatuses } from '@/constants/httpStatuses';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { isManagerRole } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { usePayloadContext } from '@/context/PayloadContext';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { findById } from '@/helpers/findById';
import { getGroupNamesFromDepartments } from '@/helpers/getGroupNamesFromDepartments';
import { isAtScrollBottom } from '@/helpers/isAtScrollBottom';
import { NoPayloadData } from '@/pages/Payload/components/PayloadGrid/components/NoPayloadData';
import { PayloadGridProps } from '@/pages/Payload/components/PayloadGrid/types';
import { SUMMARY_ROW_HEIGHT, PAGE_SIZE } from '@/pages/Payload/contants';
import { useGetGridColumns } from '@/pages/Payload/hooks/useColumns';
import { usePayloadProjectsFilter } from '@/pages/Payload/hooks/usePayloadProjectsFilter';
import { usePayloadUsersFilter } from '@/pages/Payload/hooks/usePayloadUsersFilter';
import { PayloadGridRow } from '@/pages/Payload/types';
import { convertResponseToRow } from '@/pages/Payload/utils/convertResponseToRow';
import { getNumberOfFrozenColumns } from '@/pages/Payload/utils/getNumberOfFrozenColumns';
import { getTotalRow } from '@/pages/Payload/utils/getTotalRow';
import { useGetDepartments } from '@/queries/departments/useGetDepartments';
import { useGetPayloadInfinite } from '@/queries/payload/useGetPayloadInfinite';
import { useGetPayloadTotal } from '@/queries/payload/useGetPayloadTotal';

import { overridePayloadGridStyles } from './styles';
import { getPayloadColumnCount, getPayloadColumnWidth } from './utils';

const getRowKey = (row: PayloadGridRow) => row.id;

function PayloadGridInner({ handleOpen }: PayloadGridProps) {
	const { role, departmentId } = useAuth();

	const handleEdit = useCallback(
		(userId?: string | number) => () => {
			handleOpen(userId);
		},
		[handleOpen]
	);

	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const {
		gridRows,
		setGridRows,
		year,
		specializationTitle,
		gridTotalRow,
		setGridTotalRow,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
		displayDepartmentColumn,
		displayGroupColumn,
		responseBodyFilters,
		gridFilters,
		setters,
		showTime,
		payloadGridRef,
		scrollPositionRef,
		gridWrapperBottomRef,
	} = usePayloadContext();

	const { data: departmentsInfo } = useGetDepartments({
		enabled: isManagerRole(role),
	});
	const department = findById(departmentTypes, departmentId);

	const users = usePayloadUsersFilter();
	const projects = usePayloadProjectsFilter();
	const groups = getGroupNamesFromDepartments(
		departmentsInfo || [],
		role,
		department?.fullName
	);
	const queryParams = {
		year: +year,
		pageSize: PAGE_SIZE,
		order,
		orderBy,
		specializationTitle,
	};

	const {
		data,
		isLoading,
		isFetching,
		isFetchingNextPage,
		fetchNextPage,
		hasNextPage,
		error,
	} = useGetPayloadInfinite(queryParams, responseBodyFilters);

	const {
		data: totalData,
		isLoading: isTotalLoading,
		isFetching: isTotalFetching,
	} = useGetPayloadTotal(+year, responseBodyFilters);

	const columns = useGetGridColumns({
		filters: gridFilters,
		setters,
		options: {
			users: users.map(({ name }) => name),
			groups,
			projects: projects.map(({ name }) => name),
			departments: convertToSelectionModalOptions(departmentTypes, 'fullName'),
			displayDepartmentColumn,
			displayGroupColumn,
		},
	});

	const isTotalSkeletonVisible = isTotalFetching || isTotalLoading;

	const totalRow = useMemo(
		() => getTotalRow(totalData || [], isTotalSkeletonVisible),
		[totalData, isTotalSkeletonVisible]
	);

	useEffect(() => {
		setGridTotalRow(totalRow);
	}, [totalRow]);

	const pages = data?.pages ?? [];
	const lastPage = data?.pageParams.at(-1) ?? 0;

	const fetchedRows = useMemo(
		() =>
			pages.flatMap((pageData) =>
				convertResponseToRow({
					responseData: pageData,
					handleEdit,
					departmentsData: departmentTypes,
					page: lastPage,
				})
			),
		[data, handleEdit, departmentTypes]
	);

	useEffect(() => setGridRows(fetchedRows), [fetchedRows]);

	const isGridLoading = !isFetchingNextPage && (isLoading || isFetching);

	const handleRowsChange = useCallback(
		(newRows: PayloadGridRow[]) => setGridRows(newRows),
		[]
	);

	const columnWidth = getPayloadColumnWidth(
		displayDepartmentColumn,
		displayGroupColumn
	);

	const overrideUsersGridStyles = useMemo(() => {
		const getGridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			columnWidth,
			true
		);

		const freezeColumnsWidth = Object.values(columnWidth)
			.slice(
				0,
				getNumberOfFrozenColumns(displayDepartmentColumn, displayGroupColumn)
			)
			.map(({ width }) => width);

		return overridePayloadGridStyles(
			false,
			getPayloadColumnCount(displayDepartmentColumn, displayGroupColumn),
			getGridColumnTemplate,
			undefined,
			freezeColumnsWidth
		);
	}, [displayDepartmentColumn, displayGroupColumn, columnWidth]);

	const getRowHeight = useCallback(
		(row: PayloadGridRow) =>
			getNewCellHeight(
				row.expanded || false,
				row.projects.length,
				getProjectsWithActiveAssigment(row.projects).length,
				showTime
			),
		[showTime]
	);

	const summaryRow = [gridTotalRow];

	const handleScroll = useCallback(
		(event: UIEvent<HTMLDivElement>) => {
			if (isAtScrollBottom(event) && !isFetchingNextPage && !isGridLoading) {
				fetchNextPage();
			}
		},
		[isFetchingNextPage, isGridLoading, fetchNextPage]
	);

	const { gridElement, handleSetGridElement } = useFocusGridOnCurrentMonth(
		year,
		columnWidth
	);

	useEffect(() => {
		if (gridElement) {
			gridWrapperBottomRef.current = gridElement.getBoundingClientRect().bottom;
		}
	}, [gridElement]);

	useLayoutEffect(() => {
		payloadGridRef.current = gridElement;
		const top = scrollPositionRef.current;

		if (gridElement && top > 0) {
			gridElement.scrollTo({ top });
			scrollPositionRef.current = 0;
		}
	}, [gridElement, scrollPositionRef]);

	useEffect(() => {
		if (!gridElement || !hasNextPage) {
			return;
		}

		const resizeObserver = new ResizeObserver(() => {
			if (
				gridElement.scrollHeight === gridElement.clientHeight &&
				hasNextPage
			) {
				fetchNextPage();
			}
		});

		resizeObserver.observe(gridElement);

		return () => resizeObserver.disconnect();
	}, [gridElement, hasNextPage, isFetchingNextPage, fetchNextPage]);

	if (error?.status === HttpStatuses.NotFound && !isGridLoading) {
		return <NoPayloadData year={year} />;
	}

	return (
		<Loader isLoading={isGridLoading}>
			<DataGridHoverRowWrapper defineHeight={false} flexGrow>
				<DataGrid
					ref={handleSetGridElement}
					columns={columns}
					rows={gridRows}
					overrideDataGridStyles={overrideUsersGridStyles}
					rowHeight={getRowHeight}
					rowKeyGetter={getRowKey}
					onRowsChange={handleRowsChange}
					sortColumns={sortColumns}
					onSortColumnsChange={handleSortColumnChange}
					scrollToTopOnRowsChange={false}
					renderers={noRowsGridFallback}
					bottomSummaryRows={summaryRow}
					summaryRowHeight={gridRows.length === 0 ? 0 : SUMMARY_ROW_HEIGHT}
					enableVirtualization={false}
					onScroll={handleScroll}
				/>
			</DataGridHoverRowWrapper>
		</Loader>
	);
}

export const PayloadGrid = memo(PayloadGridInner);
