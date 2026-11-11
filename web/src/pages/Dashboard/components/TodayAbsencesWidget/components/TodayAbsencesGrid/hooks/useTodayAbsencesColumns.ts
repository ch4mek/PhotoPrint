import { useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import {
	TODAY_ABSENCES_COLUMNS,
	TODAY_ABSENCES_COLUMNS_WIDTHS,
} from '../constants';
import { TodayAbsenceGridRow } from '../types';

import {
	cellWithTooltipRenderer,
	getColumnWithTooltipFactory,
} from '@/components/Grid';
import { absenceNameCellRenderer } from '@/components/Grid/renderers/cell/absenceNameCellRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { HandleRowClick } from '@/types/grid';

export const useTodayAbsencesColumns = ({
	onUserCellClick,
	onAbsenceCellClick,
	enableCellClick,
}: {
	onUserCellClick: HandleRowClick<TodayAbsenceGridRow>;
	onAbsenceCellClick: HandleRowClick<TodayAbsenceGridRow>;
	enableCellClick: (row: TodayAbsenceGridRow) => boolean;
}): Array<Column<TodayAbsenceGridRow>> => {
	const { t } = useTranslation('dashboard', { keyPrefix: 'todaysAbsences' });

	return useMemo(
		() => [
			getColumnWithTooltipFactory({
				key: TODAY_ABSENCES_COLUMNS.userFullName,
				name: t('user'),
				width: getColumnWidth(TODAY_ABSENCES_COLUMNS_WIDTHS.userFullName).width,
				sortable: false,
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						onCellClick: enableCellClick(props.row)
							? onUserCellClick
							: undefined,
					}),
			}),
			getColumnWithTooltipFactory({
				key: TODAY_ABSENCES_COLUMNS.absence,
				name: t('absence'),
				width: getColumnWidth(TODAY_ABSENCES_COLUMNS_WIDTHS.absence).width,
				sortable: false,
				renderCell: (props) =>
					absenceNameCellRenderer(props, {
						onCellClick: onAbsenceCellClick,
						isClickable: true,
					}),
			}),
		],
		[onUserCellClick, onAbsenceCellClick, t]
	);
};
