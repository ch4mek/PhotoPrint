import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import { getColumnFactory, getLogicColumnFactory } from '@/components/Grid';
import {
	ADMINISTRATION_DEPARTMENT,
	ADMINISTRATION_DEPARTMENT_WIDTH,
	DEPARTMENT_COLUMN_WIDTH,
	FIRST_COLUMN_SETUP,
	SYSTEM_ADMINISTRATION_DEPARTMENT,
	SYSTEM_ADMINISTRATION_DEPARTMENT_SHORT,
} from '@/pages/Settings/components/ClaimsTabs/constants';
import { ClaimsRow } from '@/pages/Settings/components/ClaimsTabs/types';
import { ComplexConfigurableConfigurationItem } from '@/types/configuration';

export const useColumns = (
	departments: ComplexConfigurableConfigurationItem[]
): readonly Column<ClaimsRow>[] => {
	const departmentsColumns = useMemo<readonly Column<ClaimsRow>[]>(() => {
		const columns = departments.map<Column<ClaimsRow>>(({ id, fullName }) =>
			getLogicColumnFactory({
				name:
					fullName === SYSTEM_ADMINISTRATION_DEPARTMENT
						? SYSTEM_ADMINISTRATION_DEPARTMENT_SHORT
						: fullName,
				width:
					fullName === ADMINISTRATION_DEPARTMENT
						? ADMINISTRATION_DEPARTMENT_WIDTH
						: DEPARTMENT_COLUMN_WIDTH,
				key: id,
			})
		);

		return [getColumnFactory(FIRST_COLUMN_SETUP), ...columns];
	}, [departments]);

	return departmentsColumns;
};
