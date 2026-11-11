import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';
import {
	UIEvent,
	useCallback,
	useEffect,
	useMemo,
	useRef,
	useState,
} from 'react';
import { DataGridHandle } from 'react-data-grid';

import { usePersonalInfoPopupGuard } from '../../hooks/usePersonalInfoPopupGuard';
import { GRID_FROZEN_EDIT_COLUMN_DIFF } from '../constants';
import { useTableState } from '../hooks/useTableState';
import { PageWrapper } from '../styles';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import {
	COMMON_GRID_PROPS,
	DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT,
} from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { PageHeader } from '@/components/PageHeader';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PageTitles } from '@/constants/pageTitles';
import { PopUpSearchParams } from '@/constants/searchParams';
import { useAuth, useConfiguration, useToast } from '@/context/index';
import { useListScrollObserver } from '@/context/ListScrollObserver';
import { createGridPagination } from '@/helpers/createGridPagination';
import { getAvailablePositions } from '@/helpers/getAvailablePosistions';
import { mapNamesFromArray } from '@/helpers/mapNamesFromArray';
import { useInitialFiltersWithDepartmentByDefault } from '@/hooks/useInitialFiltersWithDepartmentByDefault';
import { useTeamboard } from '@/queries/teamboard/useTeamboard';

import {
	DEFAULT_TEAMBOARD_FILTERS,
	getTeamboardColumnsAmount,
	getTeamboardColumnsWidths,
} from './constants';
import { useTeamboardColumns } from './hooks/useTeamboardColumns';
import { useTeamboardFilters } from './hooks/useTeamboardFilters';
import { useTeamboardOptions } from './hooks/useTeamboardOptions';
import { useTeamBoardRows } from './hooks/useTeambordRows';
import { TeamboardDataGridHoverRowWrapper } from './styles';
import 'react-data-grid/lib/styles.css';
import { getRowKey } from './utils';

export function TeamboardPage() {
	const { role, departmentId } = useAuth();
	const toast = useToast();
	const {
		persistentValues: { locationTypes },
		configurableValues: {
			seniorityLevels,
			englishLevels,
			roleTypes,
			countries,
		},
		isLoading: loading,
	} = useConfiguration();
	const listScrollObserver = useListScrollObserver();

	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const [page, setPage] = useState(0);
	const gridTemplateRef = useRef<DataGridHandle>(null);
	const {
		rowsPerPage,
		setRowsPerPage,
		order,
		sortColumns,
		handleSortColumnChange,
		orderBy,
	} = useTableState();

	const initialFilters = useInitialFiltersWithDepartmentByDefault(
		DEFAULT_TEAMBOARD_FILTERS,
		role,
		departmentId
	);
	const { filters, setFilters } = useTeamboardFilters(initialFilters);

	const teamboardColumnsWidth = useMemo(
		() => getTeamboardColumnsWidths(role),
		[role]
	);
	const { departmentOptions, options } = useTeamboardOptions();
	const avaliablePositions = useMemo(
		() =>
			mapNamesFromArray(getAvailablePositions(roleTypes, role, departmentId)),
		[role, roleTypes, departmentId]
	);

	const { isLoading, data, dataUpdatedAt, isFetching } = useTeamboard(
		toast,
		page + 1,
		Number(rowsPerPage),
		order,
		orderBy,
		filters,
		locationTypes,
		roleTypes,
		seniorityLevels,
		englishLevels,
		countries,
		departmentOptions
	);

	const handleEdit = useCallback(
		(clickedUserId?: string | number) => {
			if (!clickedUserId || !canOpenProfile(Number(clickedUserId))) {
				return undefined;
			}

			return () => handlePageOpen(clickedUserId.toString());
		},
		[canOpenProfile, handlePageOpen]
	);

	const handleGridScroll = (event: UIEvent<HTMLDivElement>) => {
		const { scrollTop, clientHeight, scrollHeight } =
			event.target as HTMLElement;
		const bottomRowsOffsetCount = 2;

		const startIdx = Math.floor(scrollTop / DEFAULT_GRID_ROW_HEIGHT);
		const visibleRowCount = Math.ceil(clientHeight / DEFAULT_GRID_ROW_HEIGHT);
		const endIdx =
			Math.min(
				startIdx + visibleRowCount,
				scrollHeight / DEFAULT_GRID_ROW_HEIGHT
			) - bottomRowsOffsetCount;
		listScrollObserver.notifyScrollListeners(startIdx, endIdx);
	};

	const handleOnPerPageChange = useCallback((rowsPerPageCounter: number) => {
		setRowsPerPage(rowsPerPageCounter);
		setPage(0);
	}, []);

	const teamBoardRows = useTeamBoardRows({
		items: data?.teamBoardUsers.items,
		dataUpdatedAt,
		page,
		rowsPerPage,
		showSkeleton: isLoading || isFetching,
		role,
		roleTypes,
		handleEdit,
	});

	const teamboardColumns = useTeamboardColumns({
		filters,
		setFilters,
		options,
		initialFilters,
		teamboardColumnsWidths: teamboardColumnsWidth,
		availablePositions: avaliablePositions,
		handleUserClick: handleEdit,
	});

	const projectGridPagination = useMemo(
		() =>
			createGridPagination(
				teamBoardRows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.teamBoardUsers.total
			),
		[teamBoardRows.length, page, rowsPerPage, data?.teamBoardUsers.total]
	);

	const overrideTeamboardGridStyles = useMemo(() => {
		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			teamboardColumnsWidth,
			true
		);

		return overrideGridStylesWithEditColumn(
			!!teamBoardRows.length,
			getTeamboardColumnsAmount(role) + GRID_FROZEN_EDIT_COLUMN_DIFF,
			gridColumnTemplate
		);
	}, [teamBoardRows.length, role]);

	useEffect(() => setPage(0), [filters, sortColumns]);

	const isLoaderShown = loading || isLoading || isFetching;

	return (
		<PageWrapper>
			<PageHeader title={PageTitles.Teamboard} />
			<Loader isLoading={isLoaderShown}>
				<TeamboardDataGridHoverRowWrapper defineHeight={false} flexGrow>
					<DataGrid
						enableVirtualization
						columns={teamboardColumns}
						rows={teamBoardRows}
						ref={gridTemplateRef}
						defaultColumnOptions={{
							sortable: true,
						}}
						rowKeyGetter={getRowKey}
						sortColumns={sortColumns}
						onSortColumnsChange={handleSortColumnChange}
						overrideDataGridStyles={overrideTeamboardGridStyles}
						paginationProps={projectGridPagination}
						renderers={noRowsGridFallback}
						hideVerticalScrollbar={
							teamBoardRows.length < DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT &&
							(isLoading || isFetching)
						}
						onScroll={handleGridScroll}
						{...COMMON_GRID_PROPS}
						scrollToTopOnRowsChange={false}
					/>
				</TeamboardDataGridHoverRowWrapper>
			</Loader>
		</PageWrapper>
	);
}
