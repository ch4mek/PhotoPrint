import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	getLabboardColumnsWidths,
	initialLabboardFilters,
	LABBOARD_PLANNED_END_CELL_CLASS,
	labboardGridColumns,
} from '../constants';
import { LabboardGridRow, UseLabboardColumnsProps } from '../types';

import {
	FreeTextAutoComplete,
	FreeTextModal,
	FromToProps,
	SelectionModal,
	cellWithTooltipRenderer,
	getColumnFactory,
	getEditColumnFactory,
	getLogicalTextColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { FromToNumberModal } from '@/components/Grid/modals/fromToNumberModalRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	activityLogicalOptions,
	statusOptions,
} from '@/constants/activityStatus';
import {
	fromToCurrencyObject,
	fromToDateObject,
} from '@/constants/defaultValues';
import { isGroupManager } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { isDateRangeApplyDisabled } from '@/helpers/isDateRangeApplyDisabled';
import { IS_ACTIVE_DEFAULT_VALUE } from '@/pages/constants';
import { UserClaims } from '@/types/claims';

export const useLabboardColumns = ({
	filters,
	setFilters,
	options,
	isResetFiltersDisabled,
	handleUserClick,
}: UseLabboardColumnsProps) => {
	const { role, claims } = useAuth();

	const showGroup =
		!isGroupManager(role) ||
		checkIsHasClaims([UserClaims.ManageAllInterns], claims);

	const labboardColumnsWidths = useMemo(
		() => getLabboardColumnsWidths(showGroup),
		[showGroup]
	);

	const columns = useMemo((): Column<LabboardGridRow>[] => {
		const {
			id,
			user,
			internshipSpecialization,
			group,
			internshipType,
			internshipEngineerStage,
			englishLevel,
			internshipApprovedPoints,
			internshipCompletedPoints,
			currentLocation,
			internshipStartDate,
			internshipPlannedEndDate,
			status,
			internshipFinalResult,
			comments,
		} = filters;

		return [
			getColumnFactory<LabboardGridRow>({
				...labboardGridColumns.id,
				width: getColumnWidth(labboardColumnsWidths.id).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, string>({
						...props,
						filter: id,
						setFilter: setFilters.id as Dispatch<SetStateAction<string>>,
						defaultValue: initialLabboardFilters.id,
						renderModal: FreeTextModal,
						isFilterApplied: !!id,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getColumnFactory<LabboardGridRow>({
				...labboardGridColumns.user,
				width: getColumnWidth(labboardColumnsWidths.user).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, string>({
						...props,
						filter: user,
						setFilter: setFilters.user as Dispatch<SetStateAction<string>>,
						defaultValue: initialLabboardFilters.user,
						renderModal: FreeTextAutoComplete,
						isFilterApplied: !!user,
						areValuesEquals: arePrimitiveValuesEqual,
						autocompleteOptions: options.users,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) => {
					const clickHandler = handleUserClick?.(props.row.id);

					return cellWithTooltipRenderer(props, {
						onCellClick: clickHandler ? () => clickHandler() : undefined,
					});
				},
			}),
			getColumnFactory<LabboardGridRow>({
				...labboardGridColumns.internshipSpecialization,
				width: getColumnWidth(labboardColumnsWidths.internshipSpecialization)
					.width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, string>({
						...props,
						filter: internshipSpecialization,
						setFilter: setFilters.internshipSpecialization as Dispatch<
							SetStateAction<string>
						>,
						defaultValue: initialLabboardFilters.internshipSpecialization,
						renderModal: FreeTextModal,
						isFilterApplied: !!internshipSpecialization,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			...(showGroup
				? [
						getColumnFactory<LabboardGridRow>({
							...labboardGridColumns.group,
							width: getColumnWidth(
								labboardColumnsWidths.group ?? { width: 0, fixed: true }
							).width,
							renderHeaderCell: (props) =>
								headerCellWithFilterRenderer<LabboardGridRow, string>({
									...props,
									filter: group,
									setFilter: setFilters.group as Dispatch<
										SetStateAction<string>
									>,
									defaultValue: initialLabboardFilters.group,
									renderModal: FreeTextAutoComplete,
									isFilterApplied: !!group,
									areValuesEquals: arePrimitiveValuesEqual,
									autocompleteOptions: options.groups,
									disableFilterIconActiveStateAsDefault: true,
								}),
						}),
				  ]
				: []),
			getColumnFactory<LabboardGridRow>({
				...labboardGridColumns.internshipType,
				width: getColumnWidth(labboardColumnsWidths.internshipType).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, string[]>({
						...props,
						filter: internshipType,
						setFilter: setFilters.internshipType as Dispatch<
							SetStateAction<string[]>
						>,
						defaultValue: initialLabboardFilters.internshipType,
						renderModal: SelectionModal,
						isFilterApplied: internshipType.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: options.internshipTypes,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnFactory<LabboardGridRow>({
				...labboardGridColumns.internshipEngineerStage,
				width: getColumnWidth(labboardColumnsWidths.internshipEngineerStage)
					.width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, string>({
						...props,
						filter: internshipEngineerStage,
						setFilter: setFilters.internshipEngineerStage as Dispatch<
							SetStateAction<string>
						>,
						defaultValue: initialLabboardFilters.internshipEngineerStage,
						renderModal: FreeTextModal,
						isFilterApplied: !!internshipEngineerStage,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getColumnFactory<LabboardGridRow>({
				...labboardGridColumns.englishLevel,
				width: getColumnWidth(labboardColumnsWidths.englishLevel).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, string[]>({
						...props,
						filter: englishLevel,
						setFilter: setFilters.englishLevel as Dispatch<
							SetStateAction<string[]>
						>,
						defaultValue: initialLabboardFilters.englishLevel,
						renderModal: SelectionModal,
						isFilterApplied: englishLevel.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: options.englishLevels,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnFactory<LabboardGridRow>({
				...labboardGridColumns.internshipCompletedPoints,
				width: getColumnWidth(labboardColumnsWidths.internshipCompletedPoints)
					.width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, FromToProps<string>>({
						...props,
						filter: internshipCompletedPoints,
						setFilter: setFilters.internshipCompletedPoints as Dispatch<
							SetStateAction<FromToProps<string>>
						>,
						defaultValue: fromToCurrencyObject,
						renderModal: FromToNumberModal,
						isFilterApplied: !areFromToCompatibleObjectsEqual(
							internshipCompletedPoints,
							fromToCurrencyObject
						),
						areValuesEquals: areFromToCompatibleObjectsEqual,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnFactory<LabboardGridRow>({
				...labboardGridColumns.internshipApprovedPoints,
				width: getColumnWidth(labboardColumnsWidths.internshipApprovedPoints)
					.width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, FromToProps<string>>({
						...props,
						filter: internshipApprovedPoints,
						setFilter: setFilters.internshipApprovedPoints as Dispatch<
							SetStateAction<FromToProps<string>>
						>,
						defaultValue: fromToCurrencyObject,
						renderModal: FromToNumberModal,
						isFilterApplied: !areFromToCompatibleObjectsEqual(
							internshipApprovedPoints,
							fromToCurrencyObject
						),
						areValuesEquals: areFromToCompatibleObjectsEqual,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnFactory<LabboardGridRow>({
				...labboardGridColumns.currentLocation,
				width: getColumnWidth(labboardColumnsWidths.currentLocation).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, string>({
						...props,
						filter: currentLocation,
						setFilter: setFilters.currentLocation as Dispatch<
							SetStateAction<string>
						>,
						defaultValue: initialLabboardFilters.currentLocation,
						renderModal: FreeTextModal,
						isFilterApplied: !!currentLocation,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getColumnFactory<LabboardGridRow>({
				...labboardGridColumns.internshipStartDate,
				width: getColumnWidth(labboardColumnsWidths.internshipStartDate).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, FromToProps<Date>>({
						...props,
						filter: internshipStartDate,
						setFilter: setFilters.internshipStartDate as Dispatch<
							SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied:
							!!internshipStartDate?.from || !!internshipStartDate?.to,
						isApplyButtonDisabled: isDateRangeApplyDisabled,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
			}),
			getColumnFactory<LabboardGridRow>({
				...labboardGridColumns.internshipPlannedEndDate,
				width: getColumnWidth(labboardColumnsWidths.internshipPlannedEndDate)
					.width,
				cellClass: LABBOARD_PLANNED_END_CELL_CLASS,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, FromToProps<Date>>({
						...props,
						filter: internshipPlannedEndDate,
						setFilter: setFilters.internshipPlannedEndDate as Dispatch<
							SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied:
							!!internshipPlannedEndDate?.from ||
							!!internshipPlannedEndDate?.to,
						isApplyButtonDisabled: isDateRangeApplyDisabled,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
			}),
			getLogicalTextColumnFactory<LabboardGridRow>({
				...labboardGridColumns.status,
				logicalTextOptions: activityLogicalOptions,
				width: getColumnWidth(labboardColumnsWidths.status).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, string[]>({
						...props,
						filter: status,
						setFilter: setFilters.status as Dispatch<SetStateAction<string[]>>,
						defaultValue: IS_ACTIVE_DEFAULT_VALUE,
						renderModal: SelectionModal,
						isFilterApplied: status.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: statusOptions,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnFactory<LabboardGridRow>({
				...labboardGridColumns.internshipFinalResult,
				width: getColumnWidth(labboardColumnsWidths.internshipFinalResult)
					.width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, string[]>({
						...props,
						filter: internshipFinalResult,
						setFilter: setFilters.internshipFinalResult as Dispatch<
							SetStateAction<string[]>
						>,
						defaultValue: initialLabboardFilters.internshipFinalResult,
						renderModal: SelectionModal,
						isFilterApplied: internshipFinalResult.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: options.finalResults,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnFactory<LabboardGridRow>({
				...labboardGridColumns.comments,
				width: getColumnWidth(labboardColumnsWidths.comments).width,
				sortable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<LabboardGridRow, string>({
						...props,
						filter: comments,
						setFilter: setFilters.comments as Dispatch<SetStateAction<string>>,
						defaultValue: initialLabboardFilters.comments,
						renderModal: FreeTextModal,
						isFilterApplied: !!comments,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getEditColumnFactory<LabboardGridRow>({
				key: '',
				name: '',
				disabled: isResetFiltersDisabled,
				clearFilter: setFilters.reset,
				frozen: true,
				sortable: false,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
			}),
		];
	}, [
		filters,
		setFilters,
		options,
		labboardColumnsWidths,
		showGroup,
		isResetFiltersDisabled,
		handleUserClick,
	]);

	return { columns, labboardColumnsWidths, showGroup };
};
