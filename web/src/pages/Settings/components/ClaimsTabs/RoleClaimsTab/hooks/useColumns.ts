import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import { FIRST_COLUMN_SETUP, ROLE_COLUMN_WIDTH } from '../../constants';
import { ClaimsRow } from '../../types';
import { rolesColumnsOrder } from '../constants';

import { getColumnFactory, getLogicColumnFactory } from '@/components/Grid';
import { RoleType } from '@/types/configuration';

export const useColumns = (
	roleTypes: RoleType[]
): readonly Column<ClaimsRow>[] => {
	const rolesColumns = useMemo<readonly Column<ClaimsRow>[]>(() => {
		const columns = roleTypes
			.sort(
				(a, b) =>
					rolesColumnsOrder.indexOf(a.id) - rolesColumnsOrder.indexOf(b.id)
			)
			.map<Column<ClaimsRow>>(({ id, description }) =>
				getLogicColumnFactory({
					name: description,
					width: ROLE_COLUMN_WIDTH,
					key: id,
				})
			);

		return [getColumnFactory(FIRST_COLUMN_SETUP), ...columns];
	}, [roleTypes]);

	return rolesColumns;
};
