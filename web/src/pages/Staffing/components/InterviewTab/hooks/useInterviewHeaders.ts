import { useTheme } from '@emotion/react';
import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	DEFAULT_INTERVIEW_GRID_FILTERS,
	INTERVIEW_GRID_COLUMNS,
	INTERVIEW_STATUS_OPTIONS,
	INTERVIEW_TYPE_OPTIONS,
} from '../constants';
import { InterviewEditRenderCell } from '../renderers/InterviewEditRenderCell/InterviewEditRenderCell';
import { UserCellWithExpand } from '../renderers/UserCellWithExpand/UserCellWithExpand';
import {
	HandleRowClick,
	InterviewFilters,
	InterviewGridRow,
	InterviewOptions,
	InterviewSetFilters,
} from '../types';

import {
	FreeTextAutoComplete,
	FreeTextModal,
	FromHoursToHoursTimeModal,
	FromToProps,
	SelectionModal,
	cellWithTooltipRenderer,
	getColumnFactory,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { CellWithExpandRenderer } from '@/components/Grid/renderers/cell/cellWithExpandRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { GridColumnWidthsConfiguration } from '@/types/grid';

interface UseInterviewColumnsProps {
	filters: InterviewFilters;
	setFilters: InterviewSetFilters;
	options: InterviewOptions;
	columnsWidths: GridColumnWidthsConfiguration;
	onCellClick?: HandleRowClick;
	isResetFiltersDisabled?: boolean;
}

export const useInterviewColumns = ({
	filters,
	setFilters,
	options,
	columnsWidths,
	onCellClick,
	isResetFiltersDisabled,
}: UseInterviewColumnsProps): Column<InterviewGridRow>[] => {
	const theme = useTheme();
	const isUserColumnHidden =
		columnsWidths.user === 0 || columnsWidths.user === undefined;

	const calculatedIsFiltersReset = useMemo(
		() =>
			filters.id === DEFAULT_INTERVIEW_GRID_FILTERS.id &&
			filters.user === DEFAULT_INTERVIEW_GRID_FILTERS.user &&
			filters.specialization ===
				DEFAULT_INTERVIEW_GRID_FILTERS.specialization &&
			filters.client === DEFAULT_INTERVIEW_GRID_FILTERS.client &&
			filters.jiraTask === DEFAULT_INTERVIEW_GRID_FILTERS.jiraTask &&
			filters.type === DEFAULT_INTERVIEW_GRID_FILTERS.type &&
			filters.status === DEFAULT_INTERVIEW_GRID_FILTERS.status &&
			areFromToCompatibleObjectsEqual(
				filters.date,
				DEFAULT_INTERVIEW_GRID_FILTERS.date
			) &&
			areFromToCompatibleObjectsEqual(
				filters.time,
				DEFAULT_INTERVIEW_GRID_FILTERS.time
			) &&
			filters.saleManager === DEFAULT_INTERVIEW_GRID_FILTERS.saleManager &&
			filters.reasonForRefusal ===
				DEFAULT_INTERVIEW_GRID_FILTERS.reasonForRefusal &&
			filters.note === DEFAULT_INTERVIEW_GRID_FILTERS.note,
		[filters]
	);

	const resolvedIsResetFiltersDisabled =
		isResetFiltersDisabled ?? calculatedIsFiltersReset;

	const columns: Column<InterviewGridRow>[] = useMemo(
		() => [
			getColumnWithTooltipFactory({
				key: INTERVIEW_GRID_COLUMNS.id.key,
				name: INTERVIEW_GRID_COLUMNS.id.name,
				sortable: INTERVIEW_GRID_COLUMNS.id.sortable,
				width: getColumnWidth(columnsWidths.id).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<InterviewGridRow, string>({
						...props,
						filter: filters.id,
						setFilter: setFilters.id as Dispatch<SetStateAction<string>>,
						isFilterApplied: filters.id !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: '',
					}),
				renderCell: (props) => {
					if (isUserColumnHidden) {
						return CellWithExpandRenderer(props, {
							theme,
						});
					}

					return cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					});
				},
			}),

			getColumnWithTooltipFactory({
				key: INTERVIEW_GRID_COLUMNS.specialization.key,
				name: INTERVIEW_GRID_COLUMNS.specialization.name,
				sortable: INTERVIEW_GRID_COLUMNS.specialization.sortable,
				width: getColumnWidth(columnsWidths.specialization).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<InterviewGridRow, string>({
						...props,
						filter: filters.specialization,
						setFilter: setFilters.specialization as Dispatch<
							SetStateAction<string>
						>,
						isFilterApplied: filters.specialization !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: '',
					}),
			}),

			getColumnFactory({
				key: INTERVIEW_GRID_COLUMNS.user.key,
				name: INTERVIEW_GRID_COLUMNS.user.name,
				sortable: INTERVIEW_GRID_COLUMNS.user.sortable,
				width: getColumnWidth(columnsWidths.user).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<InterviewGridRow, string>({
						...props,
						filter: filters.user,
						setFilter: setFilters.user as Dispatch<SetStateAction<string>>,
						isFilterApplied: filters.user !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextAutoComplete,
						autocompleteOptions: options.userOptions,
						defaultValue: '',
						disableFilterIconActiveStateAsDefault: true,
					}),

				renderCell: (props) =>
					UserCellWithExpand(props, {
						onContentClick: onCellClick,
						onIconClick: () => {},
					}),
			}),

			getColumnWithTooltipFactory({
				key: INTERVIEW_GRID_COLUMNS.client.key,
				name: INTERVIEW_GRID_COLUMNS.client.name,
				sortable: INTERVIEW_GRID_COLUMNS.client.sortable,
				width: getColumnWidth(columnsWidths.client).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<InterviewGridRow, string>({
						...props,
						filter: filters.client,
						setFilter: setFilters.client as Dispatch<SetStateAction<string>>,
						isFilterApplied: filters.client !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: '',
					}),
			}),

			getColumnFactory({
				key: INTERVIEW_GRID_COLUMNS.type.key,
				name: INTERVIEW_GRID_COLUMNS.type.name,
				sortable: INTERVIEW_GRID_COLUMNS.type.sortable,
				width: getColumnWidth(columnsWidths.type).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<InterviewGridRow, string[]>({
						...props,
						filter: filters.type,
						setFilter: setFilters.type as Dispatch<SetStateAction<string[]>>,
						isFilterApplied: filters.type.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						renderModal: SelectionModal,
						options: INTERVIEW_TYPE_OPTIONS,
						defaultValue: [],
					}),
			}),

			getColumnFactory({
				key: INTERVIEW_GRID_COLUMNS.date.key,
				name: INTERVIEW_GRID_COLUMNS.date.name,
				sortable: INTERVIEW_GRID_COLUMNS.date.sortable,
				width: getColumnWidth(columnsWidths.date).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<InterviewGridRow, FromToProps<Date>>({
						...props,
						filter: filters.date,
						setFilter: setFilters.date as Dispatch<
							SetStateAction<FromToProps<Date>>
						>,
						isFilterApplied: !areFromToCompatibleObjectsEqual(
							filters.date,
							DEFAULT_INTERVIEW_GRID_FILTERS.date
						),
						areValuesEquals: areFromToCompatibleObjectsEqual,
						renderModal: FromToDateModal,
						defaultValue: DEFAULT_INTERVIEW_GRID_FILTERS.date,
					}),
				cellClass: 'interview-date-cell',
			}),

			getColumnFactory({
				key: INTERVIEW_GRID_COLUMNS.time.key,
				name: INTERVIEW_GRID_COLUMNS.time.name,
				sortable: INTERVIEW_GRID_COLUMNS.time.sortable,
				width: getColumnWidth(columnsWidths.time).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<InterviewGridRow, FromToProps<string>>({
						...props,
						filter: filters.time,
						setFilter: setFilters.time as Dispatch<
							SetStateAction<FromToProps<string>>
						>,
						isFilterApplied: !areFromToCompatibleObjectsEqual(
							filters.time,
							DEFAULT_INTERVIEW_GRID_FILTERS.time
						),
						areValuesEquals: areFromToCompatibleObjectsEqual,
						renderModal: FromHoursToHoursTimeModal,
						defaultValue: DEFAULT_INTERVIEW_GRID_FILTERS.time,
					}),
			}),

			getColumnWithTooltipFactory({
				key: INTERVIEW_GRID_COLUMNS.status.key,
				name: INTERVIEW_GRID_COLUMNS.status.name,
				sortable: INTERVIEW_GRID_COLUMNS.status.sortable,
				width: getColumnWidth(columnsWidths.status).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<InterviewGridRow, string[]>({
						...props,
						filter: filters.status,
						setFilter: setFilters.status as Dispatch<SetStateAction<string[]>>,
						isFilterApplied: filters.status.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						renderModal: SelectionModal,
						options: INTERVIEW_STATUS_OPTIONS,
						defaultValue: [],
					}),
			}),

			getColumnWithTooltipFactory({
				key: INTERVIEW_GRID_COLUMNS.saleManager.key,
				name: INTERVIEW_GRID_COLUMNS.saleManager.name,
				sortable: INTERVIEW_GRID_COLUMNS.saleManager.sortable,
				width: getColumnWidth(columnsWidths.saleManager).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<InterviewGridRow, string>({
						...props,
						filter: filters.saleManager,
						setFilter: setFilters.saleManager as Dispatch<
							SetStateAction<string>
						>,
						isFilterApplied: filters.saleManager !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextAutoComplete,
						autocompleteOptions: options.saleManagerOptions,
						defaultValue: '',
					}),
			}),

			getColumnWithTooltipFactory({
				key: INTERVIEW_GRID_COLUMNS.jiraTask.key,
				name: INTERVIEW_GRID_COLUMNS.jiraTask.name,
				sortable: INTERVIEW_GRID_COLUMNS.jiraTask.sortable,
				width: getColumnWidth(columnsWidths.jiraTask).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<InterviewGridRow, string>({
						...props,
						filter: filters.jiraTask,
						setFilter: setFilters.jiraTask as Dispatch<SetStateAction<string>>,
						isFilterApplied: filters.jiraTask !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: '',
					}),
			}),

			getColumnWithTooltipFactory({
				key: INTERVIEW_GRID_COLUMNS.reasonForRefusal.key,
				name: INTERVIEW_GRID_COLUMNS.reasonForRefusal.name,
				sortable: INTERVIEW_GRID_COLUMNS.reasonForRefusal.sortable,
				width: getColumnWidth(columnsWidths.reasonForRefusal).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<InterviewGridRow, string>({
						...props,
						filter: filters.reasonForRefusal,
						setFilter: setFilters.reasonForRefusal as Dispatch<
							SetStateAction<string>
						>,
						isFilterApplied: filters.reasonForRefusal !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: '',
					}),
			}),

			getColumnWithTooltipFactory({
				key: INTERVIEW_GRID_COLUMNS.note.key,
				name: INTERVIEW_GRID_COLUMNS.note.name,
				sortable: INTERVIEW_GRID_COLUMNS.note.sortable,
				width: getColumnWidth(columnsWidths.note).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<InterviewGridRow, string>({
						...props,
						filter: filters.note,
						setFilter: setFilters.note as Dispatch<SetStateAction<string>>,
						isFilterApplied: filters.note !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: '',
					}),
			}),

			getEditColumnFactory({
				key: '',
				name: '',
				width: 36,
				disabled: resolvedIsResetFiltersDisabled,
				renderCell: (props) => InterviewEditRenderCell(props),
				clearFilter: setFilters.reset,
				frozen: true,
				sortable: false,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
			}),
		],
		[filters, options, columnsWidths, resolvedIsResetFiltersDisabled]
	);

	return useMemo(() => columns.filter(({ width }) => width !== 0), [columns]);
};
