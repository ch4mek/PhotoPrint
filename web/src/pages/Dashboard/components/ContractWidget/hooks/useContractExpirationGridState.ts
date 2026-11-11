import { useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import {
	CONTRACT_EXPIRATION_GRID_COLUMNS,
	contractexpirationGridColumnWidths,
} from '../constants';
import { ContractExpirationGridRow } from '../types';

import { cellWithTooltipRenderer, getColumnFactory } from '@/components/Grid';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { formatUtcToLocal } from '@/helpers/date';
import { overrideWidgetGridStyles } from '@/pages/Dashboard/utils/overrideGridStyles';
import { ContractExpirationItem } from '@/types/dashboard';
import { HandleRowClick } from '@/types/grid';

export function useContractExpirationGridState(
	data: ContractExpirationItem[],
	onUserCellClick: HandleRowClick<ContractExpirationGridRow>,
	enableCellClick: (row: ContractExpirationGridRow) => boolean
) {
	const { t } = useTranslation('dashboard', {
		keyPrefix: 'contractExpiration',
	});
	const columns: Column<ContractExpirationGridRow>[] = useMemo(
		() => [
			getColumnFactory({
				key: CONTRACT_EXPIRATION_GRID_COLUMNS.userKey,
				name: t('user'),
				width: getColumnWidth(contractexpirationGridColumnWidths.user).width,
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						onCellClick: enableCellClick(props.row)
							? onUserCellClick
							: undefined,
					}),
			}),
			getColumnFactory({
				key: CONTRACT_EXPIRATION_GRID_COLUMNS.datesKey,
				name: t('dates'),
				width: getColumnWidth(contractexpirationGridColumnWidths.dates).width,
			}),
		],
		[onUserCellClick, enableCellClick, t]
	);

	const rows = useMemo<ContractExpirationGridRow[]>(
		() =>
			data
				? data.map(({ userFullName, contractTo, ...rest }) => ({
						...rest,
						user: userFullName,
						dates: formatUtcToLocal(contractTo),
				  }))
				: [],
		[data]
	);

	const overrideDataGridStyles = overrideWidgetGridStyles(
		contractexpirationGridColumnWidths
	);

	return { columns, rows, overrideDataGridStyles };
}
