import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { memo, useMemo } from 'react';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	COMMON_GRID_PROPS,
	displayMinimalGridRowsProp,
} from '@/components/Grid/constants';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';

import { RATE_HISTORY_MAX_VISIBLE_ROWS_COUNT } from './constants';
import { useRateHistoryColumns } from './hooks/useRateHistoryColumns';
import { RateHistoryGridProps } from './types';
import { getOverrideRateHistoryGridStyles } from './utils/getOverrideRateHistoryGridStyles';

function RateHistoryGridInner({ rows }: RateHistoryGridProps) {
	const columns = useRateHistoryColumns();

	const overrideRateHistoryGridStyles = useMemo(
		() => getOverrideRateHistoryGridStyles(),
		[]
	);

	return (
		<DataGridHoverRowWrapper defineHeight={false}>
			<DataGrid
				{...COMMON_GRID_PROPS}
				{...displayMinimalGridRowsProp}
				enableVirtualization
				columns={columns}
				rows={rows}
				rowKeyGetter={(row) => row.id}
				overrideDataGridStyles={overrideRateHistoryGridStyles}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				maxRowsCount={RATE_HISTORY_MAX_VISIBLE_ROWS_COUNT}
			/>
		</DataGridHoverRowWrapper>
	);
}

export const RateHistoryGrid = memo(RateHistoryGridInner);
