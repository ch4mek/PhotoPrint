import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { GRID_ROW_HEIGHT } from '@/components/Grid/constants';
import { useDynamicRowsHeight } from '@/components/Grid/hooks/useDynamicRowsHeight';
import { overrideGridStylesWithDynamicCell } from '@/components/Grid/renderers/cell/dynamicHeightCellRenderer/styles';
import { Loader } from '@/components/Loader';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { RESIZABLE_ROW_LINE_HEIGHT } from '@/constants/commonStyles';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PopUpSearchParams } from '@/constants/searchParams';
import { HeightsContext } from '@/context/CellHeightsContext';
import { AbsenceTransactionHistoryGridRow } from '@/types/absences';

import { absenceTransactionHistoryWidths, MIN_ROWS_COUNT } from './constants';
import { useAbsenceTransactionHistoryColumns } from './hooks/useAbsenceTransactionHistoryColumns';
import { overrideTransactionsHistoryGridStyles, GridWrapper } from './styles';
import { AbsenceTransactionHistoryProps } from './types';

export function AbsenceTransactionHistory({
	rows,
	sortColumns,
	setRows,
	handleSortColumnChange,
	gridPagination,
	isPending,
}: AbsenceTransactionHistoryProps) {
	const handleOpenPopUp = useOpenPopUpViaUrl(PopUpSearchParams.InquiryIdParam);

	const handleOpen = (row: AbsenceTransactionHistoryGridRow) => {
		handleOpenPopUp(row.inquiryId.toString());
	};

	const columns = useAbsenceTransactionHistoryColumns(handleOpen);

	const { handleUpdateRows, getHeight, heightsMap } = useDynamicRowsHeight({
		setRows,
		defaultRowHeight: GRID_ROW_HEIGHT,
		lineHeight: RESIZABLE_ROW_LINE_HEIGHT,
	});

	const overrideStyles = useMemo(() => {
		const gridColumnsTemplate = getGridColumnTemplateFromColumnWidths(
			absenceTransactionHistoryWidths,
			false
		);

		const additionalStyles = overrideTransactionsHistoryGridStyles(
			!!rows.length,
			undefined,
			gridColumnsTemplate
		);

		return overrideGridStylesWithDynamicCell(additionalStyles);
	}, [rows.length]);

	return (
		<GridWrapper defineHeight={false} flexGrow>
			<HeightsContext.Provider value={heightsMap}>
				<Loader isLoading={isPending} size="small">
					<DataGrid
						{...{ columns, rows, sortColumns }}
						onRowsChange={handleUpdateRows}
						rowHeight={getHeight}
						overrideDataGridStyles={overrideStyles}
						paginationProps={gridPagination}
						renderers={noRowsGridFallback}
						onSortColumnsChange={handleSortColumnChange}
						scrollToTopOnRowsChange={false}
						minRowsCount={MIN_ROWS_COUNT}
					/>
				</Loader>
			</HeightsContext.Provider>
		</GridWrapper>
	);
}
