import { useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { EVENTS_WIDGET_COLUMNS, eventsGridColumnWidths } from '../constants';

import {
	cellWithTooltipRenderer,
	getColumnWithTooltipFactory,
} from '@/components/Grid';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { EventsWidgetRow } from '@/types/dashboard';
import { HandleRowClick } from '@/types/grid';

export const useEventsWidgetColumns = (
	onUserCellClick: HandleRowClick<EventsWidgetRow>,
	enableCellClick: (row: EventsWidgetRow) => boolean
): Array<Column<EventsWidgetRow>> => {
	const { t } = useTranslation('dashboard', { keyPrefix: 'events' });

	return useMemo(
		() => [
			getColumnWithTooltipFactory({
				key: EVENTS_WIDGET_COLUMNS.userKey,
				name: t('user'),
				width: getColumnWidth(eventsGridColumnWidths.user).width,
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						onCellClick: enableCellClick(props.row)
							? onUserCellClick
							: undefined,
					}),
			}),
			getColumnWithTooltipFactory({
				key: EVENTS_WIDGET_COLUMNS.typeKey,
				name: t('type'),
				width: getColumnWidth(eventsGridColumnWidths.type).width,
			}),
			getColumnWithTooltipFactory({
				key: EVENTS_WIDGET_COLUMNS.dateKey,
				name: t('date'),
				width: getColumnWidth(eventsGridColumnWidths.date).width,
			}),
		],
		[onUserCellClick, enableCellClick, t]
	);
};
