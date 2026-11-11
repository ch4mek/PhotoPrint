import { ROLE_COLUMN_WIDTH } from '../constants';
import { ClaimsRowConfig } from '../types';

import { RoleType } from '@/types/configuration';

export const createRolesObjectConfig = (
	roles: RoleType[]
): Record<string, ClaimsRowConfig> =>
	roles.reduce((acc, { name }) => {
		acc[name] = {
			name,
			width: ROLE_COLUMN_WIDTH,
			key: name,
		};

		return acc;
	}, {} as Record<string, ClaimsRowConfig>);
