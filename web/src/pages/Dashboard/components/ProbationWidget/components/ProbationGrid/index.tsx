import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { memo, useCallback, useMemo } from 'react';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { DATA_GRID_ROW_HEIGHT_WITH_TAG } from '@/constants/commonStyles';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PopUpSearchParams } from '@/constants/searchParams';
import { formatUtcToLocal } from '@/helpers/date';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import { overrideWidgetGridStyles } from '@/pages/Dashboard/utils/overrideGridStyles';

import { probationColumnWidth } from './constants';
import { useGetGridColumns } from './hooks/useGetGridColumns';
import { ProbationGridComponentProps, ProbationGridRow } from './types';

function ProbationGridComponent({ data }: ProbationGridComponentProps) {
	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const handleCrossLinkClick = useCallback(
		(row: ProbationGridRow) => {
			handlePageOpen(row.userId.toString());
		},
		[handlePageOpen]
	);

	const enableCellClick = useCallback(
		(row: ProbationGridRow) => canOpenProfile(row.userId),
		[canOpenProfile]
	);

	const columns = useGetGridColumns(
		probationColumnWidth,
		handleCrossLinkClick,
		enableCellClick
	);

	const rows = useMemo(
		() =>
			data.map((item) => ({
				...item,
				date: formatUtcToLocal(item.date),
			})),
		[data]
	);

	const overrideDataGridStyles = overrideWidgetGridStyles(probationColumnWidth);

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				{...{ columns, rows, overrideDataGridStyles }}
				rowHeight={DATA_GRID_ROW_HEIGHT_WITH_TAG}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				enableVirtualization
			/>
		</DataGridHoverRowWrapper>
	);
}

export const ProbationGrid = memo(ProbationGridComponent);
