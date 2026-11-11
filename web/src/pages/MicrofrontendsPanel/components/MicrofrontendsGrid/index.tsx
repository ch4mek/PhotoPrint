import { DataGrid } from '@mms/mms-ui-library/components/Grids';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { Loader } from '@/components/Loader';
import { PageHeader } from '@/components/PageHeader';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';

import {
	MICROFRONTENDS_PAGE_TITLE,
	MICROFRONTENDS_TABLE_COLUMNS_WIDTH,
} from './constants';
import { MicrofrontendsGridProps } from './types';
import { overrideMicrofrontendsGridStyles } from './utils/overrideMicrofrontendsGrid';

export function MicrofrontendsGrid({
	columns,
	rows,
	isFetching,
	sortColumns,
	setRows,
	handleSortColumnChange,
	getRowHeight,
	handleOpen,
}: MicrofrontendsGridProps) {
	const overrideGridStyles = overrideMicrofrontendsGridStyles(
		false,
		MICROFRONTENDS_TABLE_COLUMNS_WIDTH
	);

	return (
		<Loader isLoading={isFetching}>
			<PageHeader
				title={MICROFRONTENDS_PAGE_TITLE}
				addButton={{ onClick: handleOpen }}
			/>
			<DataGridHoverRowWrapper defineHeight={false} flexGrow>
				<DataGrid
					rowHeight={getRowHeight}
					columns={columns}
					rows={rows}
					onRowsChange={setRows}
					sortColumns={sortColumns}
					onSortColumnsChange={handleSortColumnChange}
					renderers={noRowsGridFallback}
					overrideDataGridStyles={overrideGridStyles}
					scrollToTopOnRowsChange={false}
				/>
			</DataGridHoverRowWrapper>
		</Loader>
	);
}
