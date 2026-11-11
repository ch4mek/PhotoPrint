import { Column } from 'react-data-grid';

import { BillableWorkingHoursPermissionsRow } from '../../components/BillableWorkingHoursPermissionsGrid/hooks/useBillableWorkingHoursPermissionsRows/types';
import { revokePermissionCellRenderer } from '../renderers/revokePermissionCellRenderer';

import { headerCellWithDisabledFilterRenderer } from '@/components/Grid/renderers/header/headerCellWithDisabledFilterRenderer';

interface ExtendedColumn<T> extends Column<T> {
	disabled: boolean;
	clearFilter: VoidFunction;
	displayFilterButton?: boolean;
}

export const getRevokePermissionColumnFactory = <
	T extends BillableWorkingHoursPermissionsRow
>({
	displayFilterButton = true,
	disabled,
	clearFilter = () => null,
	renderHeaderCell = headerCellWithDisabledFilterRenderer({
		disabled,
		onClick: clearFilter,
		displayFilterButton,
	}),
	renderCell = revokePermissionCellRenderer,
	...otherProps
}: ExtendedColumn<T>) => ({
	renderHeaderCell,
	renderCell,
	...otherProps,
});
