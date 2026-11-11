import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { memo, useCallback, useMemo } from 'react';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { DATA_GRID_ROW_HEIGHT_WITH_TAG } from '@/constants/commonStyles';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PopUpSearchParams } from '@/constants/searchParams';
import { getInfoDateTagContent } from '@/helpers/getInfoDateTagContent';
import { getFullName } from '@/helpers/utils';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import { overrideWidgetGridStyles } from '@/pages/Dashboard/utils/overrideGridStyles';

import { performanceReviewColumnWidth } from './constants';
import { useGetGridColumns } from './hooks/useGetGridColumns';
import {
	PerformanceReviewGridComponentProps,
	PerformanceReviewGridRow,
} from './types';

function PerformanceReviewGridComponent({
	data,
}: PerformanceReviewGridComponentProps) {
	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const handleCrossLinkClick = useCallback(
		(row: PerformanceReviewGridRow) => {
			handlePageOpen(row.userId.toString());
		},
		[handlePageOpen]
	);

	const enableCellClick = useCallback(
		(row: PerformanceReviewGridRow) => canOpenProfile(row.userId),
		[canOpenProfile]
	);

	const infoDateTagContent = getInfoDateTagContent();

	const dataGridColumns = useGetGridColumns(
		performanceReviewColumnWidth,
		handleCrossLinkClick,
		enableCellClick,
		infoDateTagContent
	);

	const dataGridRows = useMemo(
		() =>
			data.map((row) => {
				const {
					id,
					userFirstName,
					userLastName,
					userFirstNameRu,
					userLastNameRu,
					userId,
					date,
					performanceReviewDateType,
				} = row;

				return {
					id,
					userId,
					fullName: getFullName(
						userFirstName,
						userLastName,
						userFirstNameRu,
						userLastNameRu
					),
					dates: [{ date, performanceReviewDateType, performanceReviewId: id }],
				};
			}),
		[data]
	);

	const overrideGridStyles = overrideWidgetGridStyles(
		performanceReviewColumnWidth
	);

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				columns={dataGridColumns}
				rows={dataGridRows}
				overrideDataGridStyles={overrideGridStyles}
				rowHeight={DATA_GRID_ROW_HEIGHT_WITH_TAG}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				enableVirtualization
			/>
		</DataGridHoverRowWrapper>
	);
}

export const PerformanceReviewGrid = memo(PerformanceReviewGridComponent);
