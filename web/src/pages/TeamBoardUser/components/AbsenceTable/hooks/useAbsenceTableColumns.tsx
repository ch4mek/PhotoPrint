import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import { ABSENCE_COLUMNS, ABSENCE_TABLE_COLUMNS_WIDTHS } from '../constants';

import {
	cellWithTooltipRenderer,
	getColumnWithTooltipFactory,
} from '@/components/Grid';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { DEFAULT_LOADER_COLUMN_SPAN_AMOUNT } from '@/constants/defaultValues';
import { AbsenceRow } from '@/types/absences';

export const useAbsenceTableColumns = (): Array<Column<AbsenceRow>> =>
	useMemo(
		() => [
			getColumnWithTooltipFactory({
				key: ABSENCE_COLUMNS.name.key,
				name: ABSENCE_COLUMNS.name.label,
				width: getColumnWidth(ABSENCE_TABLE_COLUMNS_WIDTHS.name).width,
				sortable: false,
				colSpan(args) {
					return args.type === 'ROW' && args.row.showSkeleton
						? DEFAULT_LOADER_COLUMN_SPAN_AMOUNT
						: 0;
				},
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
			getColumnWithTooltipFactory({
				key: ABSENCE_COLUMNS.availableDays.key,
				name: ABSENCE_COLUMNS.availableDays.label,
				width: getColumnWidth(ABSENCE_TABLE_COLUMNS_WIDTHS.availableDays).width,
				sortable: false,
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
		],
		[]
	);
