import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStyles,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { SettingsPageGridWrapper } from '@/pages/Settings/styles';
import { useGetPortalSettings } from '@/queries/useGetAdminPortalSettings';

import { COLUMNS_SETUP } from './constants';
import { useColumns } from './hooks/useColumns';
import { useRows } from './hooks/useRows';

export function PortalSettingsTab() {
	const toast = useToast();

	const { data, isFetching } = useGetPortalSettings({
		onError: createOnErrorHandler(toast),
	});

	const columns = useColumns();
	const rows = useRows(data);

	const overrideDepartmentGridStyle = useMemo(() => {
		const gridColumnTemplate =
			getGridColumnTemplateFromColumnWidths(COLUMNS_SETUP);

		return overrideGridStyles(false, gridColumnTemplate);
	}, [data]);

	return (
		<Loader isLoading={isFetching}>
			<SettingsPageGridWrapper>
				<DataGridHoverRowWrapper defineHeight={false}>
					<DataGrid
						rows={rows}
						columns={columns}
						overrideDataGridStyles={overrideDepartmentGridStyle}
						renderers={noRowsGridFallback}
						{...COMMON_GRID_PROPS}
					/>
				</DataGridHoverRowWrapper>
			</SettingsPageGridWrapper>
		</Loader>
	);
}
