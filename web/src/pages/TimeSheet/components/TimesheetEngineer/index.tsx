import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { Hint } from '@mms/mms-ui-library/components/Hint';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import {
	createContext,
	memo,
	useCallback,
	useContext,
	useEffect,
	useMemo,
	useRef,
	useState,
} from 'react';
import 'react-data-grid/lib/styles.css';
import { useTranslation } from 'react-i18next';

import { PublicProjectContext } from '../../context/projectOptionsContext';
import { TimesheetParametersContext } from '../../context/timesheetParametersContext';
import { useDataGridOffsetWidth } from '../../hooks/useGridWidth';
import { useTotalColumnsStuck } from '../../hooks/useTotalColumnsStuck';
import { getHeaderRowHeight, isMonthPeriod } from '../../utils';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { Roles } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { useMobileView } from '@/hooks/useMobileView';
import { useGetTimesheet } from '@/queries/timesheet';
import { useGetPrivateProjectsWithDisabledDays } from '@/queries/timesheet/useGetPrivateProjectsWIthDisabledDays';

import { EMPTY_ROW_ID, TOTAL_ROW_INDEX } from './constants';
import { useGenerateTimeSheetGridColumns } from './hooks';
import { useDebouncedTooltipContent } from './hooks/useDebouncedTooltipContent';
import { useTimesheetMutations } from './hooks/useTimesheetMutations';
import { useTimesheetRowsOperations } from './hooks/useTimesheetRowsOperations';
import { timeSheetEnginerGridStyles } from './styles';
import { removeTimesheetRows, setTimesheetRows } from './timesheetRowsStorage';
import {
	ProjectOption,
	TimeSheetContextValues,
	TimeSheetRow,
	TimeSheetsEngineerProps,
} from './types';
import {
	convertResponseToTimesheetRows,
	getDynamicRowHeight,
	isStatusEditable,
} from './utils';

const rowKeyGetter = ({ rowId, orderIndex }: TimeSheetRow) =>
	rowId === EMPTY_ROW_ID ? orderIndex || rowId : rowId;

export const TimeSheetContext = createContext<TimeSheetContextValues>(
	{} as unknown as TimeSheetContextValues
);

export function TimesheetEngineerInner({
	isEngineer = true,
	userId,
	isDisabled = false,
	disableMessage,
	overrideGridStyles,
	setParentRowTotalValue,
	onTotalTimeMapChange,
	isLoading: outerIsLoading,
	displayTotalRow = true,
	isActiveUser,
	onGetTotalTimeMapOnFirstRender,
	freezeTaskInfoColumn = true,
	freezeTotalColumn = false,
	displayTooltips = true,
	areColumnsFrozen: outerAreColumnsFrozen,
	status,
	onExpand,
	hideHeaderRow,
}: TimeSheetsEngineerProps) {
	const { tooltipContent, setTooltipContent } = useDebouncedTooltipContent();

	const [dataGridRows, setDataGridRows] = useState<Array<TimeSheetRow>>([]);
	const [blockedDays, setBlockedDays] = useState<string[]>([]);
	const {
		gridRef,
		gridOffsetWidth,
		areColumnsFrozen: innnerAreColumnsFrozen,
	} = useDataGridOffsetWidth();
	const prevStatus = useRef(status);

	const areColumnsFrozen = outerAreColumnsFrozen ?? innnerAreColumnsFrozen;

	const { role } = useAuth();

	const {
		period,
		clientFilter,
		projectFilter,
		isProjectsModeOn,
		isPersonalModeOn,
	} = useContext(TimesheetParametersContext);

	const isPMOEmployee = Boolean(isProjectsModeOn) && role === Roles.SE;

	const startDate = period[0];
	const endDate = period[period.length - 1];
	const isPeriodByMonth = isMonthPeriod(period);
	const isFirstRenderRef = useRef(true);

	const isEditingForbiddenForPeriod = useMemo(
		() => (isEngineer || isPMOEmployee ? !isStatusEditable(status) : false),
		[isEngineer, isPMOEmployee, status]
	);

	const toast = useToast();

	const { updateTaskName } = useTimesheetMutations(toast, userId);

	const {
		addNewRow,
		handleRowChange,
		handleTaskInfoChange,
		handleTaskTimeChange,
		handleUpdateTrackItemAfterSuccessRequest,
		setIsCellLoading,
	} = useTimesheetRowsOperations(setDataGridRows, isEngineer);

	useEffect(() => {
		if (dataGridRows[TOTAL_ROW_INDEX] && onTotalTimeMapChange) {
			onTotalTimeMapChange(dataGridRows[TOTAL_ROW_INDEX].timeMap);
		}
	}, [dataGridRows, onTotalTimeMapChange]);

	useEffect(() => {
		setDataGridRows([]);
	}, [period]);

	const isMobileView = useMobileView();
	const isNarrowViewport = !useHasScreenWidth(breakpoints.desktopXS);
	const handleRowHeight = useCallback(
		(row: TimeSheetRow) =>
			getDynamicRowHeight(
				row,
				isMobileView,
				displayTotalRow,
				isNarrowViewport,
				isPeriodByMonth
			),
		[isMobileView, displayTotalRow, isNarrowViewport]
	);

	useEffect(() => {
		if (!onExpand || !dataGridRows.length) {
			return;
		}

		onExpand(
			dataGridRows.reduce(
				(totalHeight, row) => totalHeight + handleRowHeight(row),
				0
			)
		);
	}, [dataGridRows, onExpand, handleRowHeight]);

	const {
		data,
		isLoading: isTimesheetLoading,
		isFetching: isTimesheetFetching,
		refetch,
	} = useGetTimesheet(
		userId,
		startDate,
		endDate,
		clientFilter?.id,
		projectFilter?.id,
		isProjectsModeOn,
		isPersonalModeOn
	);

	useEffect(() => {
		if (prevStatus.current !== status) {
			refetch();
			prevStatus.current = status;
		}
	}, [status]);

	useEffect(() => {
		if (data?.blockedDays) {
			setBlockedDays(data.blockedDays);
		}
	}, [data]);

	useEffect(() => {
		if (userId) {
			setTimesheetRows(userId, dataGridRows);
		}
	}, [dataGridRows, userId]);

	useEffect(() => {
		if (userId) {
			return () => {
				removeTimesheetRows(userId);
			};
		}
	}, []);

	const {
		data: newPrivateProjects,
		isFetching: areNewPrivateProjectsFetching,
		isLoading: areNewPrivateProjectsLoading,
	} = useGetPrivateProjectsWithDisabledDays(
		userId,
		startDate,
		endDate,
		isProjectsModeOn
	);

	const isLoading =
		isTimesheetLoading ||
		isTimesheetFetching ||
		areNewPrivateProjectsFetching ||
		areNewPrivateProjectsLoading ||
		Boolean(outerIsLoading);

	const userPublicProject = useContext(PublicProjectContext);
	const { t } = useTranslation('common', {
		keyPrefix: 'projectTypes',
	});
	const getUserAvailableProjectsByFilter = (
		availableProjects: ProjectOption[]
	) =>
		availableProjects.filter(({ id, client }) => {
			if (projectFilter?.id && clientFilter?.name) {
				return id === projectFilter.id && client === clientFilter.name;
			}

			if (projectFilter?.id) {
				return id === projectFilter.id;
			}

			if (clientFilter?.name) {
				return client === clientFilter.name;
			}

			return true;
		});

	const userAvailableProjects: ProjectOption[] = useMemo(() => {
		if (!newPrivateProjects || !userPublicProject) {
			return [];
		}

		const availableProjects: ProjectOption[] = [
			...newPrivateProjects.map(
				({ project, disabledDates, billabilityDays }) => ({
					...project,
					type: t('private'),
					client: project.clientName,
					disabledDates,
					billabilityDays,
				})
			),
			...userPublicProject.map((project) => ({
				...project,
				type: t('public'),
				client: project.clientName,
				disabledDates: [],
				billabilityDays: [],
			})),
		];

		if (!projectFilter?.id && !clientFilter?.name) {
			return availableProjects;
		}

		return getUserAvailableProjectsByFilter(availableProjects);
	}, [
		userPublicProject,
		newPrivateProjects,
		projectFilter?.id,
		clientFilter?.name,
	]);

	const displayDoubleTotalRow = useMemo(
		() =>
			isEngineer && displayTotalRow
				? userAvailableProjects.some(
						(project) => project.isCurrentUserAbleToSeeBillableTrackedTime
				  )
				: false,
		[isEngineer, userAvailableProjects, displayTotalRow]
	);

	useEffect(() => {
		setDataGridRows((prevState) =>
			prevState.map((item) => ({
				...item,
				isMobileView,
			}))
		);
	}, [isMobileView]);

	useEffect(() => {
		if (data?.items && !isLoading) {
			setDataGridRows(
				convertResponseToTimesheetRows(
					data.items,
					userAvailableProjects,
					period,
					isEngineer,
					displayTotalRow,
					displayDoubleTotalRow,
					isDisabled,
					isEditingForbiddenForPeriod,
					disableMessage,
					isMobileView
				)
			);
		}
	}, [
		isLoading,
		data,
		userAvailableProjects,
		displayTotalRow,
		period,
		isEngineer,
		displayDoubleTotalRow,
		isEditingForbiddenForPeriod,
	]);

	const columns = useGenerateTimeSheetGridColumns(
		period,
		gridOffsetWidth,
		isPeriodByMonth,
		freezeTaskInfoColumn && areColumnsFrozen,
		areColumnsFrozen || freezeTotalColumn,
		isMobileView,
		hideHeaderRow
	);

	const isTimeSheetIsLoading = isLoading || dataGridRows.length === 0;

	useEffect(
		() => () => {
			if (isFirstRenderRef.current && data && dataGridRows.length) {
				isFirstRenderRef.current = false;

				if (onGetTotalTimeMapOnFirstRender) {
					onGetTotalTimeMapOnFirstRender(dataGridRows[TOTAL_ROW_INDEX].timeMap);
				}
			}
		},
		[]
	);

	const timeSheetContextInitialValues: TimeSheetContextValues = useMemo(
		() => ({
			columns,
			projectOptions: userAvailableProjects,
			toast,
			onTaskNameUpdate: updateTaskName,
			isEngineer,
			isPMOEmployee,
			userId,
			isActiveUser,
			isDisabledTimesheet: isDisabled,
			disableMessage,
			displayTooltips,
			period,
			updateTotalTimeMap: setParentRowTotalValue,
			onTotalTimeMapChange,
			onTaskTimeChange: handleTaskTimeChange,
			onParentRowChange: handleRowChange,
			onAddNewRow: addNewRow,
			onUpdateTrackItemAfterSuccessRequest:
				handleUpdateTrackItemAfterSuccessRequest,
			onTaskInfoChange: handleTaskInfoChange,
			setTooltipContent,
			setIsCellLoading,
			blockedDays,
		}),
		[
			userAvailableProjects,
			period,
			userId,
			isDisabled,
			disableMessage,
			isActiveUser,
			isEngineer,
			setIsCellLoading,
			blockedDays,
		]
	);

	const headerHeight = useMemo(
		() => getHeaderRowHeight(isEngineer),
		[isEngineer]
	);

	const overrideDataGridStyles = useMemo(
		() =>
			overrideGridStyles
				? overrideGridStyles(period.length, isMobileView, areColumnsFrozen)
				: timeSheetEnginerGridStyles(
						period.length,
						isMobileView,
						areColumnsFrozen,
						headerHeight
				  ),
		[
			isMobileView,
			areColumnsFrozen,
			period.length,
			headerHeight,
			overrideGridStyles,
		]
	);

	useTotalColumnsStuck(isTimeSheetIsLoading);

	const content = (
		<>
			<TimeSheetContext.Provider value={timeSheetContextInitialValues}>
				<DataGrid
					ref={gridRef}
					rows={dataGridRows}
					columns={columns}
					enableVirtualization={false}
					rowHeight={handleRowHeight}
					rowKeyGetter={rowKeyGetter}
					hideVerticalScrollbar
					headerRowHeight={headerHeight}
					scrollToTopOnRowsChange={false}
					overrideDataGridStyles={overrideDataGridStyles}
				/>
			</TimeSheetContext.Provider>
			{tooltipContent?.text && (
				<Hint
					open
					anchorElement={tooltipContent.anchorElement}
					text={tooltipContent.text}
					position="bottom"
					fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
				/>
			)}
		</>
	);

	if (isEngineer) {
		return <Loader isLoading={isTimeSheetIsLoading}>{content}</Loader>;
	}

	return content;
}

export const TimesheetEngineer = memo(TimesheetEngineerInner);
