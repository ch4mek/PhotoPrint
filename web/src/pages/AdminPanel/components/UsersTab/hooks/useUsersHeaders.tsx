import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import {
	CheckCircleIcon,
	NotificationCrossCircleIcon,
	EditIcon,
} from '@mms/mms-ui-library/icons/base';
import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import { userMenuOptionsValues } from '../constants';
import { UsersColumnsParams } from '../types';

import {
	FreeTextAutoComplete,
	FreeTextModal,
	SelectionModal,
	getLogicalTextColumnFactory,
	getColumnWithColorCircleFactory,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
	cellWithColorCircleRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { CellWithTooltipAlternativeRenderer } from '@/components/Grid/renderers/cell/cellWithTooltipAlternativeRenderer';
import { menuOptionsCellRenderer } from '@/components/Grid/renderers/cell/menuOptionsCellRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	activityLogicalOptions,
	statusOptions,
} from '@/constants/activityStatus';
import { Roles } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import {
	areArraysEqualByPrimitiveItems,
	areFiltersEqualByKeys,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { canToggleUserActive } from '@/helpers/canToggleUserActive';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { IS_ACTIVE_DEFAULT_VALUE } from '@/pages/constants';
import { UserClaims } from '@/types/claims';
import { UsersGridRow } from '@/types/userInfo';

export const useUsersColumns = ({
	filters,
	setFilters,
	options,
	initialFilters,
	positionOptions,
	departmentOptions,
	usersColumnsWidths,
	setTooltipContent,
	onCellClick,
	enableCellClick,
	onToggleUserActive,
}: UsersColumnsParams): Column<UsersGridRow>[] => {
	const { location: allLocations, group: allGroups } = options;
	const {
		departments: initialDepartaments,
		position: initialPositions,
		location: initialLocations,
	} = initialFilters;

	const { role, id: currentUserId, claims, departmentId } = useAuth();

	const hasHrClaim = useMemo(
		() => checkIsHasClaims([UserClaims.ManageHRModule], claims),
		[claims]
	);

	const usersFilterKeys = Object.keys(filters) as Array<keyof typeof filters>;

	const isEditDisabled = areFiltersEqualByKeys(
		filters,
		initialFilters,
		usersFilterKeys
	);

	const clearFilters = () => {
		setFilters.reset();
	};

	const headers: Column<UsersGridRow>[] = useMemo(
		() => [
			getColumnWithTooltipFactory({
				key: 'id',
				name: 'Id',
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<UsersGridRow, string>({
						...props,
						filter: filters.id,
						isFilterApplied: !!filters.id,
						setFilter: setFilters.id as Dispatch<SetStateAction<string>>,
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: '',
					}),
				width: getColumnWidth(usersColumnsWidths.id).width,
			}),
			getColumnWithColorCircleFactory({
				key: 'fullName',
				name: 'User',
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<UsersGridRow, string>({
						...props,
						filter: filters.user,
						isFilterApplied: !!filters.user,
						setFilter: setFilters.user as Dispatch<SetStateAction<string>>,
						areValuesEquals: arePrimitiveValuesEqual,
						defaultValue: '',
						renderModal: FreeTextAutoComplete,
						autocompleteOptions: options.users,
						disableFilterIconActiveStateAsDefault: true,
					}),
				width: getColumnWidth(usersColumnsWidths.user).width,
				renderCell: (props) =>
					cellWithColorCircleRenderer(props, {
						onCellClick: enableCellClick(props.row)
							? onCellClick(props.row.id)
							: undefined,
						enableCellClick,
					}),
			}),
			getColumnWithTooltipFactory({
				key: 'email',
				name: 'Email',
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<UsersGridRow, string>({
						...props,
						filter: filters.email,
						isFilterApplied: !!filters.email,
						setFilter: setFilters.email as Dispatch<SetStateAction<string>>,
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: '',
					}),
				width: getColumnWidth(usersColumnsWidths.email).width,
			}),
			getColumnWithTooltipFactory({
				key: 'position',
				name: 'Position',
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<UsersGridRow, string[]>({
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
				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent),
				width: getColumnWidth(usersColumnsWidths.position).width,
			}),
			getColumnWithTooltipFactory({
				key: 'departmentfullname',
				name: 'Department',
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<UsersGridRow, string[]>({
						...props,
						filter: filters.departments || [],
						isFilterApplied: filters.departments.length > 0,
						setFilter: setFilters.department as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: departmentOptions,
						defaultValue: initialDepartaments,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent),
				width: getColumnWidth(usersColumnsWidths.department).width,
			}),
			getColumnWithTooltipFactory({
				key: 'resourceGroupName',
				name: 'Group',
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<UsersGridRow, string>({
						...props,
						filter: filters.group,
						isFilterApplied: Boolean(filters.group),
						setFilter: setFilters.group as Dispatch<SetStateAction<string>>,
						renderModal: FreeTextAutoComplete,
						areValuesEquals: arePrimitiveValuesEqual,
						autocompleteOptions: allGroups,
						defaultValue: '',
					}),
				width: getColumnWidth(usersColumnsWidths.group).width,
			}),
			getLogicalTextColumnFactory({
				key: 'isActive',
				name: 'Status',
				sortable: true,
				logicalTextOptions: activityLogicalOptions,
				width: getColumnWidth(usersColumnsWidths.status).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<UsersGridRow, string[]>({
						...props,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						filter: filters.status,
						setFilter: setFilters.status as Dispatch<SetStateAction<string[]>>,
						renderModal: SelectionModal,
						isFilterApplied: filters.status.length > 0,
						options: statusOptions,
						defaultValue: IS_ACTIVE_DEFAULT_VALUE,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnWithTooltipFactory({
				key: 'location',
				name: 'Location',
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<UsersGridRow, string[]>({
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
				width: getColumnWidth(usersColumnsWidths.location).width,
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: isEditDisabled,
				renderCell: (props) =>
					menuOptionsCellRenderer(props, {
						options: (row) => {
							const menuOptions: PickerButtonOption<UsersGridRow>[] = [];

							const currentUserDepartmentId = departmentOptions.find(
								({ id }) => id === departmentId
							)?.id;

							const targetUserDepartmentId = departmentOptions.find(
								({ description }) => description === row.departmentfullname
							)?.id;

							const showActivateOption = canToggleUserActive({
								currentUserRole: role,
								targetUserRole: row.positionKey as Roles,
								currentUserId,
								targetUserId: row.id,
								currentUserDepartmentId,
								targetUserDepartmentId,
								hasHrClaim,
							});

							const editClickHandler = row.handleEditClick?.(row.id);

							if (editClickHandler) {
								menuOptions.push({
									value: userMenuOptionsValues.EDIT,
									addon: <EditIcon />,
									onClick: editClickHandler,
								});
							}

							if (showActivateOption) {
								menuOptions.push({
									value: row.isActive
										? userMenuOptionsValues.DEACTIVATE
										: userMenuOptionsValues.ACTIVATE,
									addon: row.isActive ? (
										<NotificationCrossCircleIcon />
									) : (
										<CheckCircleIcon />
									),
									onClick: onToggleUserActive,
								});
							}

							return menuOptions;
						},
					}),
				clearFilter: clearFilters,
				sortable: false,
				frozen: true,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
			}),
		],
		[
			options,
			filters,
			isEditDisabled,
			departmentOptions,
			departmentId,
			role,
			currentUserId,
			hasHrClaim,
			onToggleUserActive,
			onCellClick,
			enableCellClick,
			setTooltipContent,
			usersColumnsWidths,
			positionOptions,
			initialFilters,
		]
	);

	return useMemo(() => headers.filter(({ width }) => width !== 0), [headers]);
};
