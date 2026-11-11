import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import { FIRST_COLUMN_SETUP } from '../constants';
import { overrideClaimsGridStyles } from '../styles';
import { convertRows } from '../utils/convertRows';
import { createRolesObjectConfig } from '../utils/createRolesConfig';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { SettingsPageGridWrapper } from '@/pages/Settings/styles';
import { useGetRoleClaims } from '@/queries/useGetAdminRoleClaims';

import { useColumns } from './hooks/useColumns';

export function RoleClaimsTab() {
	const toast = useToast();

	const {
		configurableValues: { roleTypes },
	} = useConfiguration();

	const { data, isFetching } = useGetRoleClaims({
		onError: createOnErrorHandler(toast),
	});

	const columns = useColumns(roleTypes);

	const overrideDepartmentGridStyle = useMemo(() => {
		const styleConfig = {
			name: FIRST_COLUMN_SETUP,
			...createRolesObjectConfig(Object.values(roleTypes)),
		};

		const gridColumnTemplate =
			getGridColumnTemplateFromColumnWidths(styleConfig);

		return overrideClaimsGridStyles(gridColumnTemplate);
	}, [data]);

	const rows = convertRows('roleName', data);

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
