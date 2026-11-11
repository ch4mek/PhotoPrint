import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import { COLUMNS_SETUP } from '../constants';
import { PortalSettingsGridRow } from '../types';

import { getColumnFactory } from '@/components/Grid';

export const useColumns = (): readonly Column<PortalSettingsGridRow>[] => {
	const headers: Column<PortalSettingsGridRow>[] = useMemo(
		() => [
			getColumnFactory(COLUMNS_SETUP['Settings Name']),
			getColumnFactory(COLUMNS_SETUP.value),
		],
		[]
	);

	return headers;
};
