import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import { TEAMBOARD_COLUMNS } from '../constants';
import { UseTeamboardColumnsParams } from '../types';

import {
	FreeTextAutoComplete,
	FreeTextModal,
	FromToProps,
	FromYearsToYearsTimeModal,
	SelectionModal,
	arrayCellRenderer,
	editCellRenderer,
	getArrayColumnFactory,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
	profExperienceCellRender,
	cellWithTooltipRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import {
	areArraysEqualByPrimitiveItems,
	areFiltersEqualByKeys,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { TeamboardGridRow } from '@/types/teamboard';

export const useTeamboardColumns = ({
	filters,
	setFilters,
	options,
	initialFilters,
	teamboardColumnsWidths,
	availablePositions,
	handleUserClick,
}: UseTeamboardColumnsParams): readonly Column<TeamboardGridRow>[] => {
	const {
		position: allPositions,
		location: allLocations,
		hardSkills: allHardSkills,
		englishLevels: allEnglishLevels,
		departments: allDepartments,
		registeredLocation: allRegisteredLocation,
		groups: allGroups,
	} = options;

	const {
		position: initialPositions,
		location: initialLocations,
		registeredLocation: initialRegisteredLocation,
		hardSkills: initialHardSkills,
		comment: initialComment,
		specialization: initialSpecialization,
		currentLocation: initialCurrentLocation,
		user: initialUser,
		professionalExperience: initialProfessionalExperience,
		departments: initialDepartment,
		groups: initialGroup,
	} = initialFilters;

	const positionOptions = useMemo(
		() =>
			allPositions
				? allPositions.map((positionOption) => ({
						...positionOption,
						disabled: !availablePositions.includes(positionOption.id),
				  }))
				: [],
		[initialPositions, allPositions, availablePositions]
	);

	const teamboardFilterKeys = Object.keys(filters) as Array<
		keyof typeof filters
	>;

	const isEditDisabled = areFiltersEqualByKeys(
		filters,
		initialFilters,
		teamboardFilterKeys
	);

	const clearFilters = () => {
		setFilters.reset(initialFilters);
	};

	const columns: Column<TeamboardGridRow>[] = useMemo(
		() => [
			getColumnWithTooltipFactory({
				...TEAMBOARD_COLUMNS.id,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<TeamboardGridRow, string>({
						...props,
						filter: filters.userId,
						setFilter: setFilters.userId as Dispatch<SetStateAction<string>>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.userId,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				width: getColumnWidth(teamboardColumnsWidths.id).width,
			}),
			getColumnWithTooltipFactory({
				...TEAMBOARD_COLUMNS.user,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<TeamboardGridRow, string>({
						...props,
						filter: filters.user,
						isFilterApplied: !!filters.user,
						setFilter: setFilters.user as Dispatch<SetStateAction<string>>,
						areValuesEquals: arePrimitiveValuesEqual,
						defaultValue: initialUser,
						renderModal: FreeTextAutoComplete,
						autocompleteOptions: options.users,
						disableFilterIconActiveStateAsDefault: true,
					}),
				width: getColumnWidth(teamboardColumnsWidths.user).width,
				renderCell: (props) => {
					const clickHandler = handleUserClick?.(props.row.id);

					return cellWithTooltipRenderer(props, {
						onCellClick: clickHandler ? () => clickHandler() : undefined,
					});
				},
			}),
			getColumnWithTooltipFactory({
				...TEAMBOARD_COLUMNS.department,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<TeamboardGridRow, string[]>({
						...props,
						filter: filters.departments || [],
						isFilterApplied: filters.departments.length > 0,
						setFilter: setFilters.department as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: allDepartments,
						defaultValue: initialDepartment,
						disableFilterIconActiveStateAsDefault: true,
					}),
				width: getColumnWidth(teamboardColumnsWidths.department).width,
			}),
			getColumnWithTooltipFactory({
				...TEAMBOARD_COLUMNS.group,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<TeamboardGridRow, string>({
						...props,
						filter: filters.groups,
						isFilterApplied: Boolean(filters.groups),
						setFilter: setFilters.group as Dispatch<SetStateAction<string>>,
						renderModal: FreeTextAutoComplete,
						areValuesEquals: arePrimitiveValuesEqual,
						autocompleteOptions: allGroups,
						defaultValue: initialGroup,
					}),
				width: getColumnWidth(teamboardColumnsWidths.group).width,
			}),
			getColumnWithTooltipFactory({
				...TEAMBOARD_COLUMNS.profExp,
				sortable: true,
				renderCell: profExperienceCellRender(
					(row) => row.profExperienceInMonths
				),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<TeamboardGridRow, FromToProps<string>>({
						...props,
						filter: filters.professionalExperience,
						setFilter: setFilters.professionalExperience as Dispatch<
							SetStateAction<FromToProps<string>>
						>,
						isFilterApplied:
							filters.professionalExperience.from !== null ||
							filters.professionalExperience.to !== null,
						areValuesEquals: areFromToCompatibleObjectsEqual,
						renderModal: FromYearsToYearsTimeModal,
						defaultValue: initialProfessionalExperience,
					}),
				width: getColumnWidth(teamboardColumnsWidths.professionalExperience)
					.width,
			}),
			getColumnWithTooltipFactory({
				...TEAMBOARD_COLUMNS.english,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<TeamboardGridRow, string[]>({
						...props,
						filter: filters.englishLevel || [],
						isFilterApplied: filters.englishLevel.length > 0,
						setFilter: setFilters.englishLevel as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: [...allEnglishLevels, NO_VALUE_FILTER_OPTION],
						defaultValue: initialPositions,
						disableFilterIconActiveStateAsDefault: true,
					}),
				width: getColumnWidth(teamboardColumnsWidths.englishLevel).width,
			}),
			getColumnWithTooltipFactory({
				...TEAMBOARD_COLUMNS.hardSkills,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<TeamboardGridRow, string[]>({
						...props,
						filter: filters.hardSkills || [],
						isFilterApplied: filters.hardSkills.length > 0,
						setFilter: setFilters.hardSkills as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						defaultValue: initialHardSkills,
						disableFilterIconActiveStateAsDefault: true,
						options: allHardSkills,
					}),
				width: getColumnWidth(teamboardColumnsWidths.hardSkills).width,
			}),

			getArrayColumnFactory({
				...TEAMBOARD_COLUMNS.primarySpecialization,
				sortable: true,
				renderCell: (props) => arrayCellRenderer(props, false),
				cellClass: TEAMBOARD_COLUMNS.primarySpecialization.name,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<TeamboardGridRow, string>({
						...props,
						filter: filters.specialization,
						isFilterApplied: !!filters.specialization,
						setFilter: setFilters.specialization as Dispatch<
							SetStateAction<string>
						>,
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: initialSpecialization,
					}),
				width: getColumnWidth(teamboardColumnsWidths.specialization).width,
			}),
			getColumnWithTooltipFactory({
				...TEAMBOARD_COLUMNS.currentLocation,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<TeamboardGridRow, string>({
						...props,
						filter: filters.currentLocation,
						isFilterApplied: !!filters.currentLocation,
						setFilter: setFilters.currentLocation as Dispatch<
							SetStateAction<string>
						>,
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: initialCurrentLocation,
					}),
				width: getColumnWidth(teamboardColumnsWidths.location).width,
			}),
			getColumnWithTooltipFactory({
				...TEAMBOARD_COLUMNS.position,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<TeamboardGridRow, string[]>({
						...props,
						filter: filters.position || [],
						isFilterApplied: filters.position.length > 0,
						setFilter: setFilters.position as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: positionOptions,
						defaultValue: initialPositions,
						disableFilterIconActiveStateAsDefault: true,
					}),
				width: getColumnWidth(teamboardColumnsWidths.position).width,
			}),
			getColumnWithTooltipFactory({
				...TEAMBOARD_COLUMNS.location,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<TeamboardGridRow, string[]>({
						...props,
						filter: filters.location || [],
						isFilterApplied: filters.location.length > 0,
						setFilter: setFilters.location as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: allLocations,
						defaultValue: initialLocations,
						disableFilterIconActiveStateAsDefault: true,
					}),
				width: getColumnWidth(teamboardColumnsWidths.location).width,
			}),
			getColumnWithTooltipFactory({
				...TEAMBOARD_COLUMNS.registeredLocation,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<TeamboardGridRow, string[]>({
						...props,
						filter: filters.registeredLocation || [],
						isFilterApplied: filters.registeredLocation.length > 0,
						setFilter: setFilters.registeredLocation as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: allRegisteredLocation,
						defaultValue: initialRegisteredLocation,
						disableFilterIconActiveStateAsDefault: true,
					}),
				width: getColumnWidth(teamboardColumnsWidths.registeredLocation).width,
			}),
			getColumnWithTooltipFactory({
				...TEAMBOARD_COLUMNS.comments,
				sortable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<TeamboardGridRow, string>({
						...props,
						filter: filters.comment,
						isFilterApplied: !!filters.comment,
						setFilter: setFilters.comment as Dispatch<SetStateAction<string>>,
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: initialComment,
					}),
				width: getColumnWidth(teamboardColumnsWidths.location).width,
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: isEditDisabled,
				renderCell: (props) => editCellRenderer(props),
				clearFilter: clearFilters,
				sortable: false,
				frozen: true,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
			}),
		],

		[
			positionOptions,
			isEditDisabled,
			teamboardColumnsWidths,
			initialFilters,
			options,
			filters,
			handleUserClick,
			clearFilters,
		]
	);

	return useMemo(() => columns.filter(({ width }) => width !== 0), [columns]);
};
