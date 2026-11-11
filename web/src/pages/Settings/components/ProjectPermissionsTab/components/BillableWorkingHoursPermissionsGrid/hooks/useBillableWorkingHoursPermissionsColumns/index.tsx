import { Dispatch, SetStateAction } from 'react';
import { Column } from 'react-data-grid';

import { getRevokePermissionColumnFactory } from '../../../../grids/factories/getRevokePermissionColumnFactory';
import { BillableWorkingHoursPermissionsRow } from '../useBillableWorkingHoursPermissionsRows/types';

import {
	FreeTextAutoComplete,
	FreeTextModal,
	cellWithColorCircleRenderer,
	cellWithTooltipRenderer,
	getColumnWithTooltipFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import { arePrimitiveValuesEqual } from '@/helpers/areValuesEqual';

import { BILLABE_WORKING_HOURS_PERMISSIONS_COLUMNS } from './constants';
import { UseBillableWorkingHoursPermissionsColumnsOptions } from './types';

export const useBillableWorkingHoursPermissionsColumns = ({
	userIdFilter,
	userFullNameFilter,
	projectNameFilter,
	setFilters,
	usersOptions,
}: UseBillableWorkingHoursPermissionsColumnsOptions) => {
	const isResetFilterIconDisabled =
		userIdFilter === '' &&
		userFullNameFilter.length < 1 &&
		projectNameFilter === '';

	const columns: Column<BillableWorkingHoursPermissionsRow, unknown>[] = [
		getColumnWithTooltipFactory({
			...BILLABE_WORKING_HOURS_PERMISSIONS_COLUMNS.userId,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer({
					...props,
					filter: userIdFilter,
					setFilter: setFilters.userIdFilter as Dispatch<
						SetStateAction<string>
					>,
					defaultValue: '',
					renderModal: FreeTextModal,
					isFilterApplied: userIdFilter !== '',
					areValuesEquals: arePrimitiveValuesEqual,
				}),
		}),
		getColumnWithTooltipFactory({
			...BILLABE_WORKING_HOURS_PERMISSIONS_COLUMNS.userFullName,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer({
					...props,
					areValuesEquals: arePrimitiveValuesEqual,
					defaultValue: '',
					filter: userFullNameFilter,
					setFilter: setFilters.userFullNameFilter as Dispatch<
						SetStateAction<string>
					>,
					renderModal: FreeTextAutoComplete,
					isFilterApplied: !!userFullNameFilter,
					disableFilterIconActiveStateAsDefault: true,
					autocompleteOptions: usersOptions,
				}),
			renderCell: (props) => cellWithColorCircleRenderer(props),
		}),
		getColumnWithTooltipFactory({
			...BILLABE_WORKING_HOURS_PERMISSIONS_COLUMNS.projectName,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer({
					...props,
					filter: projectNameFilter,
					setFilter: setFilters.projectNameFilter as Dispatch<
						SetStateAction<string>
					>,
					defaultValue: '',
					renderModal: FreeTextModal,
					isFilterApplied: projectNameFilter !== '',
					areValuesEquals: arePrimitiveValuesEqual,
				}),
			renderCell: (props) =>
				cellWithTooltipRenderer(props, {
					isCopyEnabled: true,
				}),
		}),
		getRevokePermissionColumnFactory({
			...BILLABE_WORKING_HOURS_PERMISSIONS_COLUMNS.revokePermission,
			disabled: isResetFilterIconDisabled,
			clearFilter: setFilters.resetFilters,
		}),
	];

	return { columns };
};
