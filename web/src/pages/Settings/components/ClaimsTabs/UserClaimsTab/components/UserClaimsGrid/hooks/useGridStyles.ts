import { useMemo } from 'react';

import {
	firstClaimsColumnSetup,
	getFirstUsersClaimsColumnSetup,
	userDepartmentColumnSetup,
} from '../constants';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { UserClaimsGroup } from '@/pages/Settings/components/ClaimsTabs/types';
import { createUsersObjectConfig } from '@/pages/Settings/components/ClaimsTabs/utils/createUsersConfig';

export function useGridStyles(
	groupClaims: UserClaimsGroup[] | undefined,
	columnsCount: number,
	isDesktopSWidth: boolean
) {
	const overrideUserClaimsGridStyle = useMemo(() => {
		if (!groupClaims) {
			return;
		}

		const styleConfig = {
			id: firstClaimsColumnSetup,
			user: getFirstUsersClaimsColumnSetup(isDesktopSWidth),
			departmentId: userDepartmentColumnSetup,
			...createUsersObjectConfig(groupClaims.map((claim) => claim.name)),
		};

		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			styleConfig,
			true
		);
		const editColumnIndex = columnsCount + 1;

		return overrideGridStylesWithEditColumn(
			Boolean(columnsCount),
			editColumnIndex,
			gridColumnTemplate,
			undefined,
			true
		);
	}, [groupClaims, columnsCount, isDesktopSWidth]);

	return { overrideUserClaimsGridStyle, isDesktopSWidth };
}
